// SolidWorks API Stub Interfaces — CreateChert And Razverka
//
// PURPOSE:
//   These are minimal stub definitions of the SolidWorks 2022 COM API types.
//   They serve as documentation for the interfaces used by this application
//   and allow IDE intellisense when working with the codebase.
//
//   The application uses late-binding (dynamic) to call SolidWorks COM API,
//   so these stubs are NOT compiled into the project and NOT required at
//   build time.
//
// REAL DLLs:
//   Copy from your SolidWorks 2022 installation:
//     C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist\
//   or from the path specified during installation, e.g.:
//     D:\Solid\SolidWorks1\SOLIDWORKS\api\redist\
//
//   Required DLLs:
//     SolidWorks.Interop.sldworks.dll
//     SolidWorks.Interop.swconst.dll
//     SolidWorks.Interop.swpublished.dll
//
//   After copying, uncomment the <Reference> entries in CreateChertAndRazverka.csproj
//   to enable early-binding instead of dynamic dispatch.

using System.Runtime.InteropServices;

// ReSharper disable All
#pragma warning disable CS1591 // Missing XML comment

namespace SolidWorks.Interop.sldworks
{
    // ─── Application ─────────────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ISldWorks
    {
        int RevisionNumber();
        bool Visible { get; set; }
        object IActiveDoc2 { get; }
        object OpenDoc6(string fileName, int type, int options, string configuration, ref int errors, ref int warnings);
        object NewDocument(string templatePath, int paperSize, double width, double height);
        void CloseDoc(string name);
        string GetUserPreferenceStringValue(int preference);
        int GetUserPreferenceIntegerValue(int preference);
        bool SetUserPreferenceIntegerValue(int preference, int value);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IModelDoc2
    {
        string GetPathName();
        int GetType();
        object GetTitle();
        object Extension { get; }
        object FeatureManager { get; }
        object GetCurrentSheet();
        object FirstFeature();
        object SelectionManager { get; }
        void EditRebuild3();
        bool ForceRebuild3(bool topOnly);
        void GraphicsRedraw2();
        bool Save3(int saveOptions, ref int errors, ref int warnings);
        object GetComponents(bool topLevelOnly);
        object CreateDrawViewFromModelView3(string modelPath, string viewName, double x, double y, double z);
        object CreateUnfoldedViewFromModelView(object baseView, int type, double x, double y, double z);
        object InsertBOMTable4(bool followAssemblyOrder, double x, double y, int bomType, string config, string template, object anchor, bool mirroredAxis);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDrawingDoc : IModelDoc2
    {
        object GetCurrentSheet();
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IPartDoc : IModelDoc2
    {
        object FirstFeature();
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IAssemblyDoc : IModelDoc2
    {
        object GetComponents(bool topLevelOnly);
    }

    // ─── Views ────────────────────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IView
    {
        double ScaleDecimal { get; set; }
        void ActivateView();
        void SelectAllAnnotations();
        string GetPathName();
        string Name { get; set; }
    }

    // ─── Features ────────────────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IFeature
    {
        string GetTypeName2();
        object GetNextFeature();
        int SetSuppression2(int state, int configurationOption, object configurationNames);
        string Name { get; set; }
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IFeatureManager
    {
    }

    // ─── Component ───────────────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IComponent2
    {
        string GetPathName();
        string Name2 { get; }
        object GetModelDoc2();
        int GetSuppression();
    }

    // ─── Custom Properties ───────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ICustomPropertyManager
    {
        int Add3(string fieldName, int fieldType, string fieldValue, int overwrite);
        bool Set2(string fieldName, string fieldValue);
        string Get5(string fieldName, bool useCache, out string resolvedValue, out bool wasResolved, out bool linkToProperty);
        bool Delete2(string fieldName);
    }

    // ─── Model Extension ─────────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IModelDocExtension
    {
        object CustomPropertyManager { get; }
        bool SaveAs(string fileName, int version, int options, object exportData, ref int errors, ref int warnings);
        int InsertModelAnnotations3(int src, int type, bool config, bool eliminate, bool combineView);
        bool InsertBalloon(int style, int fit, string text, bool ignoreMultiple, object textValue, int textUpperValue, object lowerTextValue, int lowerTextUpperValue);
    }

    // ─── BOM ─────────────────────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IBomTableAnnotation
    {
        int RowCount { get; }
        int ColumnCount { get; }
        string GetCellText(int row, int col);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IBomFeature
    {
        object GetTableAnnotation();
        string Name { get; set; }
    }

    // ─── Notes and Balloons ──────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface INote
    {
        string Text { get; set; }
        bool Angle { get; set; }
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IBalloonOptions
    {
        int Style { get; set; }
        int Fit { get; set; }
        string Text { get; set; }
        string LowerText { get; set; }
    }

    // ─── Sheet ───────────────────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ISheet
    {
        string GetName();
        object GetViews();
        object GetBOMTableAnchor(int type);
        object CustomPropertyManager { get; }
    }

    // ─── PDF Export ──────────────────────────────────────────────────────────

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IExportPdfData
    {
        bool ExportAs3D { get; set; }
        bool ViewPdfAfterSaving { get; set; }
    }
}

namespace SolidWorks.Interop.swconst
{
    // ─── Document Types ──────────────────────────────────────────────────────

    public enum swDocumentTypes_e
    {
        swDocNONE     = 0,
        swDocPART     = 1,
        swDocASSEMBLY = 2,
        swDocDRAWING  = 3
    }

    // ─── Open Options ────────────────────────────────────────────────────────

    public enum swOpenDocOptions_e
    {
        swOpenDocOptions_Silent           = 1,
        swOpenDocOptions_ReadOnly         = 2,
        swOpenDocOptions_ViewOnly         = 4,
        swOpenDocOptions_AutoMissingConfig = 64
    }

    // ─── Custom Property ─────────────────────────────────────────────────────

    public enum swCustomInfoType_e
    {
        swCustomInfoUnknown = 0,
        swCustomInfoText    = 30,
        swCustomInfoDate    = 31,
        swCustomInfoNumber  = 32,
        swCustomInfoDouble  = 33
    }

    public enum swCustomPropertyAddOption_e
    {
        swCustomPropertyDeleteAndAdd   = 0,
        swCustomPropertyOnlyIfNew      = 1,
        swCustomPropertyReplaceValue   = 2
    }

    // ─── Drawing Sheet Sizes ─────────────────────────────────────────────────

    public enum swDrawingStandardSheetSize_e
    {
        swDwgPaperA0size          = 5,  // ISO A0
        swDwgPaperA1size          = 6,  // ISO A1
        swDwgPaperA2size          = 7,  // ISO A2
        swDwgPaperA3size          = 8,  // ISO A3
        swDwgPaperA4size          = 9,  // ISO A4
        swDwgPapersUserDefined    = 12  // custom
    }

    // ─── Import Model Items ──────────────────────────────────────────────────

    public enum swImportModelItemsSource_e
    {
        swImportModelItemsFromSelectedFeature = 0,
        swImportModelItemsFromSelectedView    = 4,
        swImportModelItemsFromEntireModel     = 8
    }

    // ─── Insert Annotations ──────────────────────────────────────────────────

    public enum swInsertAnnotation_e
    {
        swInsertAnnotationDimensions          = 2,
        swInsertAnnotationNotes               = 4,
        swInsertAnnotationReferenceGeometry   = 256
    }

    // ─── BOM ─────────────────────────────────────────────────────────────────

    public enum swBOMConfigurationType_e
    {
        swBOMConfigurationType_Indented        = 0,
        swBOMConfigurationType_TopLevelOnly    = 1,
        swBOMConfigurationType_PartsOnly       = 2,
        swBOMConfigurationType_IndentedNumeric = 3
    }

    public enum swNumberingType_e
    {
        swNumberingType_None       = 0,
        swNumberingType_Flat       = 1,
        swNumberingType_Detailed   = 2
    }

    // ─── Save As ─────────────────────────────────────────────────────────────

    public enum swSaveAsVersion_e
    {
        swSaveAsCurrentVersion = 0
    }

    public enum swSaveAsOptions_e
    {
        swSaveAsOptions_Silent                  = 1,
        swSaveAsOptions_Copy                    = 2,
        swSaveAsOptions_UpdateInactiveViews     = 4,
        swSaveAsOptions_OverrideSaveEmodel      = 8,
        swSaveAsOptions_AvoidRebuildOnSave      = 16,
        swSaveAsOptions_IgnoreBiographicData    = 32
    }

    // ─── Feature Suppression ────────────────────────────────────────────────

    public enum swFeatureSuppressionAction_e
    {
        swSuppressFeature   = 0,
        swUnSuppressFeature = 1
    }

    // ─── Document Template Types ─────────────────────────────────────────────

    public enum swDocTemplateTypes_e
    {
        swDocTemplateTypeDRAWING   = 2,
        swDocTemplateTypePART      = 0,
        swDocTemplateTypeASSEMBLY  = 1
    }

    // ─── User Preference Strings ─────────────────────────────────────────────

    public enum swUserPreferenceStringValue_e
    {
        swDefaultTemplatePart     = 11,
        swDefaultTemplateAssembly = 12,
        swDefaultTemplateDrawing  = 13
    }

    // ─── PDF Export ──────────────────────────────────────────────────────────

    public enum swSaveAsPDFExportData_e
    {
        swSaveAsPDF = 33
    }

    // ─── View Type ───────────────────────────────────────────────────────────

    public enum swStandardViews_e
    {
        swFrontView     = 1,
        swBackView      = 2,
        swLeftView      = 3,
        swRightView     = 4,
        swTopView       = 5,
        swBottomView    = 6,
        swIsometricView = 7,
        swTrimetricView = 8,
        swDimetricView  = 9
    }

    // ─── Balloon Style ───────────────────────────────────────────────────────

    public enum swBalloonStyle_e
    {
        swBS_Circular    = 0,
        swBS_Triangle    = 1,
        swBS_Hexagonal   = 2,
        swBS_Box         = 3,
        swBS_Diamond     = 4,
        swBS_Pentagon    = 5
    }

    public enum swBalloonFit_e
    {
        swBF_Tightest    = 0,
        swBF_2           = 1,
        swBF_3           = 2,
        swBF_4           = 3,
        swBF_5           = 4,
        swBF_6           = 5
    }
}
