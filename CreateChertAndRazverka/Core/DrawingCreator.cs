using System;
using System.IO;
using CreateChertAndRazverka.Helpers;
using CreateChertAndRazverka.Models;

namespace CreateChertAndRazverka.Core
{
    /// <summary>
    /// Creates a SolidWorks drawing (.slddrw) for a part document,
    /// adds standard views, applies auto-dimensions, fills the title block,
    /// and optionally exports to PDF.
    /// </summary>
    public class DrawingCreator
    {
        private readonly SolidWorksConnector _connector;
        private readonly DimensionManager    _dimManager;
        private readonly TitleBlockManager   _titleManager;

        public DrawingCreator(SolidWorksConnector connector)
        {
            _connector    = connector ?? throw new ArgumentNullException("connector");
            _dimManager   = new DimensionManager(connector);
            _titleManager = new TitleBlockManager(connector);
        }

        /// <summary>
        /// Creates a drawing for the given part file.
        /// </summary>
        public SingleResult CreateDrawing(string partFilePath, DrawingSettings settings)
        {
            if (!File.Exists(partFilePath))
                return Error(partFilePath, "Файл не найден: " + partFilePath);

            try
            {
                LogHelper.Log($"Создание чертежа для: {Path.GetFileName(partFilePath)}", LogLevel.Info);

                dynamic swApp = _connector.SwApp;
                if (swApp == null)
                    return Error(partFilePath, "SolidWorks не подключён.");

                // Open part document (silently)
                int docErr = 0, docWarn = 0;
                dynamic partDoc = swApp.OpenDoc6(
                    partFilePath,
                    1,                  // swDocPART
                    1,                  // swOpenDocOptions_Silent
                    "",
                    ref docErr,
                    ref docWarn);

                if (partDoc == null)
                    return Error(partFilePath, $"Не удалось открыть документ (err={docErr}).");

                // Create new drawing
                // swDwgPaperSizes_e value from settings
                int paperSize = settings.GetSwPaperSize();
                dynamic drawingDoc = swApp.NewDocument(
                    GetDrawingTemplatePath(swApp, settings.DrawingTemplatePath),
                    paperSize,
                    0.297, 0.210); // default A4 sheet dimensions (overridden by template)

                if (drawingDoc == null)
                    return Error(partFilePath, "Не удалось создать документ чертежа.");

                // Insert standard views: front, top, right, isometric
                dynamic sheet = drawingDoc.GetCurrentSheet();
                InsertStandardViews(drawingDoc, partDoc, settings);
                double sheetW = GetPaperWidth(settings.SheetFormat);
                double sheetH = GetPaperHeight(settings.SheetFormat);
                AutoScaleViews(drawingDoc, sheetW, sheetH);

                // Auto-dimensions
                if (settings.AutoDimensions)
                    _dimManager.AddDimensions(drawingDoc);

                // Fill title block
                _titleManager.FillTitleBlock(drawingDoc, settings.Author,
                    Path.GetFileNameWithoutExtension(partFilePath));

                // Save drawing
                string outputPath = Path.Combine(
                    settings.OutputFolder,
                    FileHelper.SanitizeFileName(Path.GetFileNameWithoutExtension(partFilePath)) + ".slddrw");
                outputPath = FileHelper.GetUniqueFilePath(
                    settings.OutputFolder,
                    FileHelper.SanitizeFileName(Path.GetFileNameWithoutExtension(partFilePath)),
                    ".slddrw");

                int saveErr = 0, saveWarn = 0;
                bool saved = drawingDoc.Extension.SaveAs(
                    outputPath, 0, 0, null, ref saveErr, ref saveWarn);

                if (!saved)
                    return Error(partFilePath, $"Не удалось сохранить чертёж (err={saveErr}).");

                // PDF export
                if (settings.ExportToPdf)
                {
                    string pdfPath = Path.ChangeExtension(outputPath, ".pdf");
                    PdfExporter.ExportToPdf(swApp, drawingDoc, pdfPath);
                }

                drawingDoc.Quit();

                LogHelper.Log($"Чертёж создан: {Path.GetFileName(outputPath)}", LogLevel.Success);

                return new SingleResult
                {
                    ComponentName = Path.GetFileName(partFilePath),
                    Status        = ResultStatus.Success,
                    Message       = "Чертёж создан",
                    OutputPath    = outputPath
                };
            }
            catch (Exception ex)
            {
                return Error(partFilePath, ex.Message);
            }
        }

        private void InsertStandardViews(dynamic drawingDoc, dynamic partDoc, DrawingSettings settings)
        {
            try
            {
                string partPath = (string)partDoc.GetPathName();
                double W = GetPaperWidth(settings.SheetFormat);
                double H = GetPaperHeight(settings.SheetFormat);

                double frontX = W * 0.28, frontY = H * 0.38;
                double topX   = W * 0.28, topY   = H * 0.78;
                double rightX = W * 0.65, rightY = H * 0.38;
                double isoX   = W * 0.72, isoY   = H * 0.75;

                dynamic frontView = drawingDoc.CreateDrawViewFromModelView3(
                    partPath, "*Front", frontX, frontY, 0);

                if (frontView != null)
                {
                    // Project top view (above front)
                    drawingDoc.CreateUnfoldedViewFromModelView(frontView, 2, topX, topY, 0);
                    // Activate front view and project right view
                    drawingDoc.ActivateView(frontView.Name);
                    drawingDoc.CreateUnfoldedViewAt3(rightX, rightY, 0, false);
                    // Isometric view
                    drawingDoc.CreateDrawViewFromModelView3(partPath, "*Isometric", isoX, isoY, 0);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("Предупреждение при вставке видов: " + ex.Message, LogLevel.Warning);
            }
        }

        private void AutoScaleViews(dynamic drawingDoc, double sheetW, double sheetH)
        {
            // Allow each view to use at most 35 % of the sheet dimension
            const double maxFraction = 0.35;
            try
            {
                object[] sheets = (object[])drawingDoc.GetViews();
                if (sheets == null) return;

                foreach (object sheetObj in sheets)
                {
                    object[] sheetViews = (object[])sheetObj;
                    if (sheetViews == null) continue;
                    for (int i = 1; i < sheetViews.Length; i++)
                    {
                        dynamic view = sheetViews[i];
                        if (view == null) continue;
                        try
                        {
                            double curScale = (double)view.ScaleDecimal;
                            if (curScale <= 0) curScale = 1.0;

                            double[] outline = (double[])view.GetOutline();
                            if (outline == null || outline.Length < 4) continue;
                            double vw = Math.Abs(outline[2] - outline[0]);
                            double vh = Math.Abs(outline[3] - outline[1]);
                            if (vw <= 0 || vh <= 0) continue;

                            double sx = (sheetW * maxFraction) / vw * curScale;
                            double sy = (sheetH * maxFraction) / vh * curScale;
                            double newScale = FloorToStandardScale(Math.Min(sx, sy));
                            if (newScale > 0 && Math.Abs(newScale - curScale) > 1e-4)
                                view.ScaleDecimal = newScale;
                        }
                        catch { }
                    }
                }
            }
            catch { }
            try { drawingDoc.ForceRebuild3(false); } catch { }
        }

        private static double GetPaperWidth(string sheetFormat)
        {
            switch (sheetFormat)
            {
                case "A0": return 1.189;
                case "A1": return 0.841;
                case "A2": return 0.594;
                case "A3": return 0.420;
                default:   return 0.297; // A4
            }
        }

        private static double GetPaperHeight(string sheetFormat)
        {
            switch (sheetFormat)
            {
                case "A0": return 0.841;
                case "A1": return 0.594;
                case "A2": return 0.420;
                case "A3": return 0.297;
                default:   return 0.210; // A4
            }
        }

        private static double FloorToStandardScale(double s)
        {
            double[] standards = { 0.05, 0.1, 0.2, 0.25, 0.5, 0.75, 1.0, 1.5, 2.0, 2.5, 5.0, 10.0 };
            double best = standards[0];
            foreach (double std in standards)
                if (std <= s) best = std;
            return best;
        }

        private string GetDrawingTemplatePath(dynamic swApp, string userTemplate)
        {
            if (!string.IsNullOrEmpty(userTemplate) && File.Exists(userTemplate))
                return userTemplate;

            try
            {
                // swUserPreferenceStringValue_e.swDefaultTemplateDrawing = 13
                string templatePath = swApp.GetUserPreferenceStringValue(13);
                if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
                    return templatePath;
            }
            catch { /* fall through */ }

            return "";
        }

        private static SingleResult Error(string file, string message)
        {
            LogHelper.Log($"Ошибка (чертёж {Path.GetFileName(file)}): {message}", LogLevel.Error);
            return new SingleResult
            {
                ComponentName = Path.GetFileName(file),
                Status        = ResultStatus.Error,
                Message       = message
            };
        }
    }
}
