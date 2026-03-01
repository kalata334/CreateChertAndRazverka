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

                // Insert flat-pattern view
                string partPath = (string)partDoc.GetPathName();
                dynamic fpView = drawingDoc.CreateDrawViewFromModelView3(
                    partPath, "Flat-Pattern1", 0.12, 0.18, 0);

                if (fpView == null)
                {
                    // Try alternate flat-pattern feature name
                    fpView = drawingDoc.CreateDrawViewFromModelView3(
                        partPath, "*Flat-Pattern", 0.12, 0.18, 0);
                }

                if (fpView != null)
                    fpView.ScaleDecimal = 1.0;

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
                    OutputFile    = outputPath
                };
            }
            catch (Exception ex)
            {
                return Error(partFilePath, ex.Message);
            }
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
