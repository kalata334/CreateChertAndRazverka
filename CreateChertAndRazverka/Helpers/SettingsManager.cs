using System;
using System.IO;
using System.Text;

namespace CreateChertAndRazverka.Helpers
{
    public class AppSettings
    {
        public string DrawingTemplatePath        { get; set; } = "";
        public string FlatPatternTemplatePath    { get; set; } = "";
        public string AssemblyDrawingTemplatePath { get; set; } = "";
        public string AuthorName                 { get; set; } = "";
        public string OutputFolder               { get; set; } = "";
        public string SheetFormat                { get; set; } = "A4";
        public bool   CreatePartDrawings         { get; set; } = true;
        public bool   CreateFlatPatterns         { get; set; } = true;
        public bool   ExportToPdf                { get; set; } = true;
        public bool   AutoDimensions             { get; set; } = true;
    }

    public static class SettingsManager
    {
        /// <summary>Default SolidWorks 2022 templates folder.</summary>
        public const string DefaultTemplatesPath =
            @"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2022\templates";

        private static string SettingsFilePath => Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "",
            "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath, Encoding.UTF8);
                    return ParseJson(json);
                }
            }
            catch { /* return defaults on any error */ }
            return new AppSettings();
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                string json = ToJson(settings);
                File.WriteAllText(SettingsFilePath, json, Encoding.UTF8);
            }
            catch { /* ignore save errors */ }
        }

        // ── Simple manual JSON serialisation (no external dependencies) ────────

        private static string ToJson(AppSettings s)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"DrawingTemplatePath\": {JsonString(s.DrawingTemplatePath)},");
            sb.AppendLine($"  \"FlatPatternTemplatePath\": {JsonString(s.FlatPatternTemplatePath)},");
            sb.AppendLine($"  \"AssemblyDrawingTemplatePath\": {JsonString(s.AssemblyDrawingTemplatePath)},");
            sb.AppendLine($"  \"AuthorName\": {JsonString(s.AuthorName)},");
            sb.AppendLine($"  \"OutputFolder\": {JsonString(s.OutputFolder)},");
            sb.AppendLine($"  \"SheetFormat\": {JsonString(s.SheetFormat)},");
            sb.AppendLine($"  \"CreatePartDrawings\": {JsonBool(s.CreatePartDrawings)},");
            sb.AppendLine($"  \"CreateFlatPatterns\": {JsonBool(s.CreateFlatPatterns)},");
            sb.AppendLine($"  \"ExportToPdf\": {JsonBool(s.ExportToPdf)},");
            sb.AppendLine($"  \"AutoDimensions\": {JsonBool(s.AutoDimensions)}");
            sb.Append("}");
            return sb.ToString();
        }

        private static AppSettings ParseJson(string json)
        {
            var s = new AppSettings();
            s.DrawingTemplatePath        = ReadStringField(json, "DrawingTemplatePath")        ?? s.DrawingTemplatePath;
            s.FlatPatternTemplatePath    = ReadStringField(json, "FlatPatternTemplatePath")    ?? s.FlatPatternTemplatePath;
            s.AssemblyDrawingTemplatePath = ReadStringField(json, "AssemblyDrawingTemplatePath") ?? s.AssemblyDrawingTemplatePath;
            s.AuthorName                 = ReadStringField(json, "AuthorName")                 ?? s.AuthorName;
            s.OutputFolder               = ReadStringField(json, "OutputFolder")               ?? s.OutputFolder;
            s.SheetFormat                = ReadStringField(json, "SheetFormat")                ?? s.SheetFormat;

            bool? cp = ReadBoolField(json, "CreatePartDrawings");
            if (cp.HasValue) s.CreatePartDrawings = cp.Value;

            bool? cfp = ReadBoolField(json, "CreateFlatPatterns");
            if (cfp.HasValue) s.CreateFlatPatterns = cfp.Value;

            bool? ep = ReadBoolField(json, "ExportToPdf");
            if (ep.HasValue) s.ExportToPdf = ep.Value;

            bool? ad = ReadBoolField(json, "AutoDimensions");
            if (ad.HasValue) s.AutoDimensions = ad.Value;

            return s;
        }

        private static string ReadStringField(string json, string key)
        {
            string pattern = "\"" + key + "\"";
            int keyIdx = json.IndexOf(pattern, StringComparison.Ordinal);
            if (keyIdx < 0) return null;

            int colonIdx = json.IndexOf(':', keyIdx + pattern.Length);
            if (colonIdx < 0) return null;

            int quoteOpen = json.IndexOf('"', colonIdx + 1);
            if (quoteOpen < 0) return null;

            int quoteClose = quoteOpen + 1;
            var result = new StringBuilder();
            while (quoteClose < json.Length)
            {
                char c = json[quoteClose];
                if (c == '\\' && quoteClose + 1 < json.Length)
                {
                    char next = json[quoteClose + 1];
                    switch (next)
                    {
                        case '"':  result.Append('"');  break;
                        case '\\': result.Append('\\'); break;
                        case '/':  result.Append('/');  break;
                        case 'n':  result.Append('\n'); break;
                        case 'r':  result.Append('\r'); break;
                        case 't':  result.Append('\t'); break;
                        default:   result.Append(next); break;
                    }
                    quoteClose += 2;
                }
                else if (c == '"')
                {
                    break;
                }
                else
                {
                    result.Append(c);
                    quoteClose++;
                }
            }
            return result.ToString();
        }

        private static bool? ReadBoolField(string json, string key)
        {
            string pattern = "\"" + key + "\"";
            int keyIdx = json.IndexOf(pattern, StringComparison.Ordinal);
            if (keyIdx < 0) return null;

            int colonIdx = json.IndexOf(':', keyIdx + pattern.Length);
            if (colonIdx < 0) return null;

            // skip whitespace
            int i = colonIdx + 1;
            while (i < json.Length && (json[i] == ' ' || json[i] == '\t' || json[i] == '\r' || json[i] == '\n'))
                i++;

            if (i + 4 <= json.Length && json.Substring(i, 4) == "true")  return true;
            if (i + 5 <= json.Length && json.Substring(i, 5) == "false") return false;
            return null;
        }

        private static string JsonString(string value)
        {
            if (value == null) return "null";
            var sb = new StringBuilder("\"");
            foreach (char c in value)
            {
                switch (c)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n");  break;
                    case '\r': sb.Append("\\r");  break;
                    case '\t': sb.Append("\\t");  break;
                    default:   sb.Append(c);      break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        private static string JsonBool(bool value) => value ? "true" : "false";
    }
}
