namespace CreateChertAndRazverka.Models
{
    public enum DocumentType
    {
        None,
        Part,
        Assembly,
        Drawing
    }

    public class DocumentState
    {
        public DocumentType Type  { get; set; } = DocumentType.None;
        public string FilePath    { get; set; } = "";
        public string FileName    { get; set; } = "";
        public bool IsSheetMetal  { get; set; } = false;

        public string TypeLabel
        {
            get
            {
                switch (Type)
                {
                    case DocumentType.Part:     return "📄 Деталь";
                    case DocumentType.Assembly: return "📦 Сборка";
                    case DocumentType.Drawing:  return "🗒️ Чертёж";
                    default:                    return "🔴 Нет документа";
                }
            }
        }

        public static DocumentState Empty => new DocumentState();
    }
}
