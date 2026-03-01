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
                    GetDrawingTemplatePath(swApp),
                    paperSize,
                    0.297, 0.210); // default A4 sheet dimensions (overridden by template)

                if (drawingDoc == null)
                    return Error(partFilePath, "Не удалось создать документ чертежа.");

                // Insert standard views: front, top, right, isometric
                dynamic sheet = drawingDoc.GetCurrentSheet();
                InsertStandardViews(drawingDoc, partDoc);

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
                    OutputFile    = outputPath
                };
            }
            catch (Exception ex)
            {
                return Error(partFilePath, ex.Message);
            }
        }

        private void InsertStandardViews(dynamic drawingDoc, dynamic partDoc)
        {
            try
            {
                string partPath = partDoc.GetPathName();
                // Standard front view at position (0.10, 0.15)
                dynamic view = drawingDoc.CreateDrawViewFromModelView3(
                    partPath, "*Front", 0.10, 0.15, 0);

                if (view != null)
                {
                    view.ScaleDecimal = 1.0;
                    // Project top view
                    drawingDoc.CreateUnfoldedViewFromModelView(view, 2, 0.10, 0.27, 0);
                    // Project right view
                    drawingDoc.CreateUnfoldedViewFromModelView(view, 3, 0.22, 0.15, 0);
                    // Isometric view
                    drawingDoc.CreateDrawViewFromModelView3(
                        partPath, "*Isometric", 0.22, 0.27, 0);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log("Предупреждение при вставке видов: " + ex.Message, LogLevel.Warning);
            }
        }

        private string GetDrawingTemplatePath(dynamic swApp)
        {
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
