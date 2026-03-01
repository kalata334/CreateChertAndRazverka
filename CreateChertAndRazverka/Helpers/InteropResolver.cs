using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace CreateChertAndRazverka.Helpers
{
    public static class InteropResolver
    {
        private static readonly string[] BuiltInSearchPaths = new[]
        {
            @"D:\Solid\SolidWorks1\SOLIDWORKS\api\redist",
            @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist",
            @"C:\Program Files\SolidWorks Corp\SOLIDWORKS\api\redist",
        };

        public static void Register()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = new AssemblyName(args.Name).Name;

            if (!assemblyName.StartsWith("SolidWorks.Interop"))
                return null;

            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // 1. Search in user-configurable paths from InteropPaths.txt
            string pathsFile = Path.Combine(exeDir, "InteropPaths.txt");
            if (File.Exists(pathsFile))
            {
                foreach (string line in File.ReadAllLines(pathsFile))
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                        continue;
                    string dllPath = Path.Combine(trimmed, assemblyName + ".dll");
                    if (File.Exists(dllPath))
                    {
                        LogHelper.Log($"Найдена DLL (InteropPaths.txt): {dllPath}", LogLevel.Success);
                        return Assembly.LoadFrom(dllPath);
                    }
                }
            }

            // 2. Search in built-in known paths
            foreach (string basePath in BuiltInSearchPaths)
            {
                string dllPath = Path.Combine(basePath, assemblyName + ".dll");
                if (File.Exists(dllPath))
                {
                    LogHelper.Log($"Найдена DLL: {dllPath}", LogLevel.Success);
                    return Assembly.LoadFrom(dllPath);
                }
            }

            // 3. Search via Windows Registry
            string swFolder = GetSolidWorksFolder();
            if (!string.IsNullOrEmpty(swFolder))
            {
                string dllPath = Path.Combine(swFolder, "api", "redist", assemblyName + ".dll");
                if (File.Exists(dllPath))
                {
                    LogHelper.Log($"Найдена DLL через реестр: {dllPath}", LogLevel.Success);
                    return Assembly.LoadFrom(dllPath);
                }
            }

            // 4. Search next to exe
            string localPath = Path.Combine(exeDir, assemblyName + ".dll");
            if (File.Exists(localPath))
            {
                LogHelper.Log($"Найдена DLL рядом с exe: {localPath}", LogLevel.Success);
                return Assembly.LoadFrom(localPath);
            }

            LogHelper.Log($"Не найдена DLL: {assemblyName}. Установите SolidWorks 2022 или скопируйте Interop DLL вручную.", LogLevel.Warning);
            return null;
        }

        private static string GetSolidWorksFolder()
        {
            try
            {
                string[] versions = { "SOLIDWORKS 2025", "SOLIDWORKS 2024", "SOLIDWORKS 2023", "SOLIDWORKS 2022", "SOLIDWORKS 2021" };
                foreach (string version in versions)
                {
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\SolidWorks\" + version + @"\Setup"))
                    {
                        string folder = key?.GetValue("SolidWorks Folder") as string;
                        if (!string.IsNullOrEmpty(folder))
                            return folder;
                    }
                }
                return null;
            }
            catch { return null; }
        }
    }
}
