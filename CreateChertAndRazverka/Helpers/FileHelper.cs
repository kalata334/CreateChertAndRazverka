using System;
using System.IO;
using System.Windows.Forms;

namespace CreateChertAndRazverka.Helpers
{
    public static class FileHelper
    {
        public static string EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string GetUniqueFilePath(string directory, string baseName, string extension)
        {
            string candidate = Path.Combine(directory, baseName + extension);
            int counter = 1;
            while (File.Exists(candidate))
            {
                candidate = Path.Combine(directory, baseName + "_" + counter + extension);
                counter++;
            }
            return candidate;
        }

        public static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        public static string SelectFolder(string description, string initialPath = "")
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = description;
                dlg.ShowNewFolderButton = true;
                if (!string.IsNullOrEmpty(initialPath) && Directory.Exists(initialPath))
                    dlg.SelectedPath = initialPath;

                if (dlg.ShowDialog() == DialogResult.OK)
                    return dlg.SelectedPath;
            }
            return null;
        }

        public static string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public static bool IsValidDirectory(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
        }
    }
}
