using System;
using System.Runtime.InteropServices;
using CreateChertAndRazverka.Helpers;
using CreateChertAndRazverka.Models;

namespace CreateChertAndRazverka.Core
{
    /// <summary>
    /// Manages the connection to a running SolidWorks 2022 instance via COM.
    /// Uses dynamic typing to avoid a compile-time dependency on the SolidWorks Interop DLLs.
    /// When the DLLs are present in the References folder the same code works with early binding.
    /// </summary>
    public class SolidWorksConnector : IDisposable
    {
        private const string SwProgId = "SldWorks.Application.30"; // SolidWorks 2022

        private dynamic _swApp;
        private bool    _disposed;

        public bool IsConnected => _swApp != null;

        /// <summary>
        /// Returns the raw ISldWorks / SldWorks.Application COM object.
        /// </summary>
        public dynamic SwApp => _swApp;

        /// <summary>
        /// Attempts to attach to an already running SolidWorks instance.
        /// Does NOT start SolidWorks if it is not running.
        /// Tries versioned ProgID first, then falls back to the generic ProgID.
        /// </summary>
        public bool TryConnect()
        {
            try
            {
                _swApp = Marshal.GetActiveObject(SwProgId);
                LogHelper.Log("Подключено к SolidWorks 2022.", LogLevel.Success);
                return true;
            }
            catch (COMException) { /* try fallback */ }
            catch (Exception ex)
            {
                LogHelper.Log("Ошибка подключения к SolidWorks (versioned): " + ex.Message, LogLevel.Warning);
            }

            // Fallback: try generic ProgID (works when program runs without elevation)
            try
            {
                _swApp = Marshal.GetActiveObject("SldWorks.Application");
                LogHelper.Log("Подключено к SolidWorks (generic ProgID).", LogLevel.Success);
                return true;
            }
            catch (COMException) { /* not running */ }
            catch (Exception ex)
            {
                LogHelper.Log("Ошибка подключения к SolidWorks: " + ex.Message, LogLevel.Error);
            }

            // Last fallback: check if SLDWORKS process is running and try again
            try
            {
                var swProcesses = System.Diagnostics.Process.GetProcessesByName("SLDWORKS");
                bool isRunning = swProcesses.Length > 0;
                foreach (var p in swProcesses) p.Dispose();

                if (isRunning)
                {
                    _swApp = Marshal.GetActiveObject("SldWorks.Application");
                    LogHelper.Log("Подключено к SolidWorks (через процесс).", LogLevel.Success);
                    return true;
                }
            }
            catch { /* ignore */ }

            _swApp = null;
            return false;
        }

        /// <summary>
        /// Disconnects from SolidWorks and releases the COM reference.
        /// </summary>
        public void Disconnect()
        {
            if (_swApp != null)
            {
                try { Marshal.ReleaseComObject(_swApp); }
                catch { /* ignore */ }
                _swApp = null;
                LogHelper.Log("Отключено от SolidWorks.", LogLevel.Info);
            }
        }

        /// <summary>
        /// Returns the currently active document or null if none is open.
        /// </summary>
        public dynamic GetActiveDocument()
        {
            if (_swApp == null) return null;
            try
            {
                dynamic doc = null;
                try { doc = _swApp.ActiveDoc; }
                catch { /* ignore */ }

                if (doc == null)
                {
                    try { doc = _swApp.IActiveDoc2; }
                    catch { /* ignore */ }
                }

                return doc;
            }
            catch { return null; }
        }

        /// <summary>
        /// Determines the DocumentType by inspecting the file extension of the active document path.
        /// Falls back to probing SolidWorks-specific methods when the document has not yet been saved.
        /// swDocPART=1, swDocASSEMBLY=2, swDocDRAWING=3
        /// </summary>
        public static DocumentType GetDocumentType(dynamic doc)
        {
            if (doc == null) return DocumentType.None;
            try
            {
                // Primary: use file extension — most reliable with dynamic COM objects
                string path = null;
                try { path = (string)doc.GetPathName(); } catch { }

                if (!string.IsNullOrEmpty(path))
                {
                    string ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
                    switch (ext)
                    {
                        case ".sldprt": return DocumentType.Part;
                        case ".sldasm": return DocumentType.Assembly;
                        case ".slddrw": return DocumentType.Drawing;
                    }
                }

                // Fallback for unsaved documents: probe for assembly/drawing-specific members
                try
                {
                    doc.GetComponents(true);
                    return DocumentType.Assembly;
                }
                catch { /* not an assembly */ }

                try
                {
                    doc.GetFirstView();
                    return DocumentType.Drawing;
                }
                catch { /* not a drawing */ }

                // Default to Part when nothing else matches
                string title = null;
                try { title = (string)doc.GetTitle(); } catch { }
                return string.IsNullOrEmpty(title) ? DocumentType.None : DocumentType.Part;
            }
            catch { return DocumentType.None; }
        }

        /// <summary>
        /// Returns true when the part document has a Sheet-Metal feature.
        /// </summary>
        public static bool IsSheetMetal(dynamic partDoc)
        {
            if (partDoc == null) return false;
            try
            {
                dynamic feat = partDoc.FirstFeature();
                while (feat != null)
                {
                    try
                    {
                        string typeName = (string)feat.GetTypeName2();
                        if (typeName == "SheetMetal"    || typeName == "SMFlatPattern"  ||
                            typeName == "SMBaseFlange"  || typeName == "BaseFlange"     ||
                            typeName == "EdgeFlange"    || typeName == "FlatPattern"    ||
                            typeName == "SMMiteredFlange")
                            return true;
                    }
                    catch { /* skip feature that cannot be inspected */ }

                    try { feat = feat.GetNextFeature(); }
                    catch { break; }
                }
                return false;
            }
            catch { return false; }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Disconnect();
                _disposed = true;
            }
        }
    }
}
