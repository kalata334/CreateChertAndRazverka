using System;
using System.IO;
using CreateChertAndRazverka.Helpers;
using CreateChertAndRazverka.Models;

namespace CreateChertAndRazverka.Core
{
    /// <summary>
    /// Creates a flat-pattern (развёртка) drawing for a sheet-metal part.
    /// </summary>
    public class FlatPatternCreator
    {
        private readonly SolidWorksConnector _connector;
        private readonly DimensionManager    _dimManager;
        private readonly TitleBlockManager   _titleManager;

        public FlatPatternCreator(SolidWorksConnector connector)
        {
            _connector    = connector ?? throw new ArgumentNullException("connector");
            _dimManager   = new DimensionManager(connector);
            _titleManager = new TitleBlockManager(connector);
        }

        public SingleResult CreateFlatPattern(string partFilePath, DrawingSettings settings)
        {
            if (!File.Exists(partFilePath))
                return Error(partFilePath, "Файл не найден: " + partFilePath);

            try
            {
                LogHelper.Log($"Создание развёртки для: {Path.GetFileName(partFilePath)}", LogLevel.Info);

                dynamic swApp = _connector.SwApp;
                if (swApp == null)
                    return Error(partFilePath, "SolidWorks не подключён.");

                int docErr = 0, docWarn = 0;
                dynamic partDoc = swApp.OpenDoc6(
                    partFilePath, 1, 1, "", ref docErr, ref docWarn);

                if (partDoc == null)
                    return Error(partFilePath, $"Не удалось открыть деталь (err={docErr}).");

                if (!SolidWorksConnector.IsSheetMetal(partDoc))
                {
                    swApp.CloseDoc(partFilePath);
                    return new SingleResult
                    {
                        ComponentName = Path.GetFileName(partFilePath),
                        Status        = ResultStatus.Skipped,
                        Message       = "Деталь не является листовой"
                    };
                }

                // Unsuppress the flat-pattern feature
                UnsuppressFlatPattern(partDoc);

                // Create drawing
                dynamic drawingDoc = swApp.NewDocument(
                    GetDrawingTemplatePath(swApp, settings.FlatPatternTemplatePath),
                    settings.GetSwPaperSize(),
                    0.297, 0.210);

                if (drawingDoc == null)
                    return Error(partFilePath, "Не удалось создать документ развёртки.");

                // Insert flat-pattern view using the dedicated SolidWorks API
                string partPath = (string)partDoc.GetPathName();
                dynamic fpView = null;
                try
                {
                    // CreateFlatPatternView: available in SW 2012+ for sheet-metal flat patterns
                    fpView = drawingDoc.CreateFlatPatternView(partPath, 0, 0, false, null);
                }
                catch { /* older API version — use fallback below */ }

                if (fpView == null)
                {
                    // Fallback: use the standard SM-FLAT-PATTERN configuration name
                    string flatConfig = FindFlatPatternConfig(partDoc);
                    fpView = drawingDoc.CreateDrawViewFromModelView3(
                        partPath, "SM-FLAT-PATTERN", 0.12, 0.18, 0);
                    if (fpView != null && !string.IsNullOrEmpty(flatConfig))
                        fpView.ReferencedConfiguration = flatConfig;
                }

                if (fpView != null)
                    ScaleFlatPatternView(fpView, settings.SheetFormat);

                // Auto-dimensions on flat pattern
                if (settings.AutoDimensions)
                    _dimManager.AddDimensions(drawingDoc);

                // Fill title block
                _titleManager.FillTitleBlock(drawingDoc, settings.Author,
                    Path.GetFileNameWithoutExtension(partFilePath) + "_Развёртка");

                // Save
                string outputPath = FileHelper.GetUniqueFilePath(
                    settings.OutputFolder,
                    FileHelper.SanitizeFileName(
                        Path.GetFileNameWithoutExtension(partFilePath) + "_Развёртка"),
                    ".slddrw");

                int saveErr = 0, saveWarn = 0;
                bool saved = drawingDoc.Extension.SaveAs(
                    outputPath, 0, 0, null, ref saveErr, ref saveWarn);

                if (!saved)
                    return Error(partFilePath, $"Не удалось сохранить развёртку (err={saveErr}).");

                if (settings.ExportToPdf)
                {
                    string pdfPath = Path.ChangeExtension(outputPath, ".pdf");
                    PdfExporter.ExportToPdf(swApp, drawingDoc, pdfPath);
                }

                drawingDoc.Quit();

                LogHelper.Log($"Развёртка создана: {Path.GetFileName(outputPath)}", LogLevel.Success);

                return new SingleResult
                {
                    ComponentName = Path.GetFileName(partFilePath),
                    Status        = ResultStatus.Success,
                    Message       = "Развёртка создана",
                    OutputPath    = outputPath
                };
            }
            catch (Exception ex)
            {
                return Error(partFilePath, ex.Message);
            }
        }

        private static string FindFlatPatternConfig(dynamic partDoc)
        {
            try
            {
                object[] configs = (object[])partDoc.GetConfigurationNames();
                if (configs == null) return null;
                foreach (object cfg in configs)
                {
                    string name = cfg as string;
                    if (name != null && name.ToUpperInvariant().Contains("FLAT"))
                        return name;
                }
            }
            catch { }
            return null;
        }

        private static void ScaleFlatPatternView(dynamic fpView, string sheetFormat)
        {
            try
            {
                double W = GetPaperWidth(sheetFormat);
                double H = GetPaperHeight(sheetFormat);
                double curScale = 1.0;
                try { curScale = (double)fpView.ScaleDecimal; } catch { }
                if (curScale <= 0) curScale = 1.0;

                double[] outline = null;
                try { outline = (double[])fpView.GetOutline(); } catch { }
                if (outline != null && outline.Length >= 4)
                {
                    double vw = Math.Abs(outline[2] - outline[0]);
                    double vh = Math.Abs(outline[3] - outline[1]);
                    if (vw > 0 && vh > 0)
                    {
                        // Target: flat pattern fills 70 % of the sheet (single large view)
                        const double flatFraction = 0.70;
                        double sx = (W * flatFraction) / vw * curScale;
                        double sy = (H * flatFraction) / vh * curScale;
                        double newScale = FloorToStandardScale(Math.Min(sx, sy));
                        if (newScale > 0)
                            fpView.ScaleDecimal = newScale;
                        return;
                    }
                }
                fpView.ScaleDecimal = 1.0;
            }
            catch { try { fpView.ScaleDecimal = 1.0; } catch { } }
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

        private void UnsuppressFlatPattern(dynamic partDoc)
        {
            try
            {
                dynamic feat = partDoc.FirstFeature();
                while (feat != null)
                {
                    string typeName = (string)feat.GetTypeName2();
                    if (typeName == "SMFlatPattern")
                    {
                        feat.SetSuppression2(1, 2, null); // unsuppress
                        break;
                    }
                    feat = feat.GetNextFeature();
                }
                partDoc.EditRebuild3();
            }
            catch (Exception ex)
            {
                LogHelper.Log("Предупреждение: не удалось разворачить развёртку: " + ex.Message, LogLevel.Warning);
            }
        }

        private string GetDrawingTemplatePath(dynamic swApp, string userTemplate)
        {
            if (!string.IsNullOrEmpty(userTemplate) && File.Exists(userTemplate))
                return userTemplate;

            try
            {
                string path = swApp.GetUserPreferenceStringValue(13);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    return path;
            }
            catch { /* fall through */ }
            return "";
        }

        private static SingleResult Error(string file, string message)
        {
            LogHelper.Log($"Ошибка (развёртка {Path.GetFileName(file)}): {message}", LogLevel.Error);
            return new SingleResult
            {
                ComponentName = Path.GetFileName(file),
                Status        = ResultStatus.Error,
                Message       = message
            };
        }
    }
}
