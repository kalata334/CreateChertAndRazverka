using System;
using System.IO;
using CreateChertAndRazverka.Helpers;
using CreateChertAndRazverka.Models;

namespace CreateChertAndRazverka.Core
{
    /// <summary>
    /// Creates an assembly drawing (.slddrw) for a SolidWorks assembly,
    /// including a BOM table and balloon annotations.
    /// </summary>
    public class AssemblyDrawingCreator
    {
        private readonly SolidWorksConnector _connector;
        private readonly TitleBlockManager   _titleManager;

        public AssemblyDrawingCreator(SolidWorksConnector connector)
        {
            _connector    = connector ?? throw new ArgumentNullException("connector");
            _titleManager = new TitleBlockManager(connector);
        }

        public SingleResult CreateAssemblyDrawing(string assemblyFilePath, DrawingSettings settings)
        {
            if (!File.Exists(assemblyFilePath))
                return Error(assemblyFilePath, "Файл не найден: " + assemblyFilePath);

            try
            {
                LogHelper.Log($"Создание сборочного чертежа для: {Path.GetFileName(assemblyFilePath)}", LogLevel.Info);

                dynamic swApp = _connector.SwApp;
                if (swApp == null)
                    return Error(assemblyFilePath, "SolidWorks не подключён.");

                int docErr = 0, docWarn = 0;
                dynamic asmDoc = swApp.OpenDoc6(
                    assemblyFilePath, 2, 1, "", ref docErr, ref docWarn);

                if (asmDoc == null)
                    return Error(assemblyFilePath, $"Не удалось открыть сборку (err={docErr}).");

                dynamic drawingDoc = swApp.NewDocument(
                    GetDrawingTemplatePath(swApp, settings.AssemblyDrawingTemplatePath),
                    settings.GetSwPaperSize(),
                    0.297, 0.210);

                if (drawingDoc == null)
                    return Error(assemblyFilePath, "Не удалось создать документ чертежа.");

                string asmPath = (string)asmDoc.GetPathName();

                // Insert isometric view of the whole assembly
                dynamic isoView = drawingDoc.CreateDrawViewFromModelView3(
                    asmPath, "*Isometric", 0.15, 0.18, 0);
                if (isoView != null)
                    isoView.ScaleDecimal = 1.0;

                // Insert front view
                dynamic frontView = drawingDoc.CreateDrawViewFromModelView3(
                    asmPath, "*Front", 0.08, 0.10, 0);

                // Insert BOM table
                if (isoView != null)
                    InsertBomTable(drawingDoc, isoView, asmPath);

                // Add balloons to front view
                if (frontView != null)
                    AddBalloons(drawingDoc, frontView);

                // Fill title block
                _titleManager.FillTitleBlock(drawingDoc, settings.Author,
                    Path.GetFileNameWithoutExtension(assemblyFilePath) + "_СБ");

                // Save
                string outputPath = FileHelper.GetUniqueFilePath(
                    settings.OutputFolder,
                    FileHelper.SanitizeFileName(
                        Path.GetFileNameWithoutExtension(assemblyFilePath) + "_СБ"),
                    ".slddrw");

                int saveErr = 0, saveWarn = 0;
                bool saved = drawingDoc.Extension.SaveAs(
                    outputPath, 0, 0, null, ref saveErr, ref saveWarn);

                if (!saved)
                    return Error(assemblyFilePath, $"Не удалось сохранить сборочный чертёж (err={saveErr}).");

                if (settings.ExportToPdf)
                {
                    string pdfPath = Path.ChangeExtension(outputPath, ".pdf");
                    PdfExporter.ExportToPdf(swApp, drawingDoc, pdfPath);
                }

                drawingDoc.Quit();

                LogHelper.Log($"Сборочный чертёж создан: {Path.GetFileName(outputPath)}", LogLevel.Success);

                return new SingleResult
                {
                    ComponentName = Path.GetFileName(assemblyFilePath),
                    Status        = ResultStatus.Success,
                    Message       = "Сборочный чертёж создан",
                    OutputPath    = outputPath
                };
            }
            catch (Exception ex)
            {
                return Error(assemblyFilePath, ex.Message);
            }
        }

        private void InsertBomTable(dynamic drawingDoc, dynamic view, string modelPath)
        {
            try
            {
                // swBOMConfigurationAnchorType_e.swBOMConfigurationAnchor_TopRight = 1
                // swBomType_e.swBomType_PartsOnly = 2
                dynamic sheet   = drawingDoc.GetCurrentSheet();
                dynamic bomAnchor = sheet.GetBOMTableAnchor(1);

                dynamic bomTable = drawingDoc.InsertBOMTable4(
                    true,       // followAssemblyOrder
                    0,          // x (ignored when anchor used)
                    0,          // y (ignored when anchor used)
                    1,          // swBomType_e.swBomType_TopLevelOnly
                    "",         // configuration name (empty = active)
                    "",         // template path (empty = default)
                    bomAnchor,
                    false);     // tableAnchorMirroredAxis

                if (bomTable != null)
                    LogHelper.Log("BOM таблица вставлена.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                LogHelper.Log("Предупреждение при вставке BOM: " + ex.Message, LogLevel.Warning);
            }
        }

        private void AddBalloons(dynamic drawingDoc, dynamic view)
        {
            try
            {
                // Activate the drawing view before inserting balloons
                view.ActivateView();

                // Insert auto-balloon for all components in the view
                // swBalloonStyle_e.swBS_Circular = 0
                // swBalloonFit_e.swBF_5 = 4 (fits text)
                dynamic selMgr = drawingDoc.SelectionManager;
                view.SelectAllAnnotations();
                drawingDoc.Extension.InsertBalloon(
                    0,      // swBalloonStyle_e.swBS_Circular
                    4,      // swBalloonFit_e
                    "",     // text (empty = item number)
                    false,  // ignoreMultiple
                    null,   // balloonTextValue
                    0,      // balloonTextUpperValue
                    null,   // balloonLowerTextValue
                    0);     // balloonLowerTextUpperValue
            }
            catch (Exception ex)
            {
                LogHelper.Log("Предупреждение при вставке выносок: " + ex.Message, LogLevel.Warning);
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
            LogHelper.Log($"Ошибка (сборочный чертёж {Path.GetFileName(file)}): {message}", LogLevel.Error);
            return new SingleResult
            {
                ComponentName = Path.GetFileName(file),
                Status        = ResultStatus.Error,
                Message       = message
            };
        }
    }
}
