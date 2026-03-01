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
        private readonly SwConnectionManager _connectionMgr = new SwConnectionManager();
        private bool                         _disposed;

        /// <summary>True when we have a live COM reference to SolidWorks.</summary>
        public bool IsConnected => _connectionMgr.IsConnected;

        /// <summary>
        /// Returns the raw ISldWorks / SldWorks.Application COM object.
        /// </summary>
        public dynamic SwApp => _connectionMgr.SwApp;

        /// <summary>
        /// Attempts to attach to an already running SolidWorks instance.
        /// Does NOT start SolidWorks if it is not running.
        /// Tries versioned ProgID first, then falls back to the generic ProgID.
        /// </summary>
        public bool TryConnect() => _connectionMgr.TryConnect();

        /// <summary>
        /// Disconnects from SolidWorks and releases the COM reference.
        /// </summary>
        public void Disconnect() => _connectionMgr.Disconnect();

        /// <summary>
        /// Returns the currently active document or null if none is open.
        /// </summary>
        public dynamic GetActiveDocument() => (dynamic)_connectionMgr.GetActiveDocument();

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
