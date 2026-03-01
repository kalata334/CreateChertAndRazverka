namespace CreateChertAndRazverka.Models
{
    public enum ComponentType
    {
        Part,
        Assembly,
        AssemblyDrawing
    }

    public enum ComponentStatus
    {
        Pending,
        Processing,
        Done,
        Skipped,
        Error
    }

    public class ComponentInfo
    {
        public string FilePath       { get; set; } = "";
        public string FileName       { get; set; } = "";
        public ComponentType Type    { get; set; } = ComponentType.Part;
        public bool IsSheetMetal     { get; set; } = false;
        public bool IsSelected       { get; set; } = true;
        public ComponentStatus Status { get; set; } = ComponentStatus.Pending;
        public string StatusMessage  { get; set; } = "Ожидание";

        public string TypeIcon
        {
            get
            {
                switch (Type)
                {
                    case ComponentType.Part:            return "📄";
                    case ComponentType.Assembly:        return "📦";
                    case ComponentType.AssemblyDrawing: return "📋";
                    default:                            return "📄";
                }
            }
        }

        public string SheetMetalDisplay
        {
            get
            {
                if (Type == ComponentType.Assembly || Type == ComponentType.AssemblyDrawing)
                    return "—";
                return IsSheetMetal ? "✅ листовая" : "❌ не листовая";
            }
        }
    }
}
