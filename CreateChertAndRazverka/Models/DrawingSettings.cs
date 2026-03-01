namespace CreateChertAndRazverka.Models
{
    public class DrawingSettings
    {
        public string OutputFolder    { get; set; } = "";
        public string Author          { get; set; } = "";
        public string SheetFormat     { get; set; } = "A4";
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
        /// swDwgPaperSizes_e: A4 = 9, A3 = 5, A2 = 6, A1 = 7, A0 = 8
        /// </summary>
        public int GetSwPaperSize()
        {
            switch (SheetFormat)
            {
                case "A0": return 8;  // swDwgPaperA0size
                case "A1": return 7;  // swDwgPaperA1size
                case "A2": return 6;  // swDwgPaperA2size
                case "A3": return 5;  // swDwgPaperA3size
                case "A4": return 9;  // swDwgPaperA4size
                default:   return 9;  // swDwgPaperA4size (fallback)
            }
        }
    }
}
