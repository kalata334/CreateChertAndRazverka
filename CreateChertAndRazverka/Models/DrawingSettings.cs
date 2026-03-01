namespace CreateChertAndRazverka.Models
{
    public enum SheetFormat
    {
        A4,
        A3,
        A2,
        A1,
        A0
    }

    public class DrawingSettings
    {
        public string OutputFolder    { get; set; } = "";
        public string Author          { get; set; } = "";
        public SheetFormat SheetFormat { get; set; } = SheetFormat.A4;
        public bool CreateDrawings    { get; set; } = true;
        public bool CreateFlatPatterns { get; set; } = true;
        public bool ExportToPdf       { get; set; } = true;
        public bool AutoDimensions    { get; set; } = true;

        // User-selected drawing template paths (empty = use SolidWorks default)
        public string DrawingTemplatePath         { get; set; } = "";
        public string FlatPatternTemplatePath     { get; set; } = "";
        public string AssemblyDrawingTemplatePath { get; set; } = "";

        /// <summary>
        /// Returns the SolidWorks paper size enum value for the selected sheet format.
        /// swDwgPaperSizes_e: A4 = 9, A3 = 8, A2 = 7, A1 = 6, A0 = 5
        /// </summary>
        public int GetSwPaperSize()
        {
            switch (SheetFormat)
            {
                case SheetFormat.A0: return 5;
                case SheetFormat.A1: return 6;
                case SheetFormat.A2: return 7;
                case SheetFormat.A3: return 8;
                case SheetFormat.A4: return 9;
                default:             return 9;
            }
        }
    }
}
