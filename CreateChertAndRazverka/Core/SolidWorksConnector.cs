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
        /// </summary>
        public bool TryConnect()
        {
            try
            {
                Type swType = Type.GetTypeFromProgID(SwProgId);
                if (swType == null)
                {
                    LogHelper.Log("SolidWorks 2022 не зарегистрирован (ProgID не найден).", LogLevel.Warning);
                    return false;
                }

                _swApp = Marshal.GetActiveObject(SwProgId);
                LogHelper.Log("Подключено к SolidWorks 2022.", LogLevel.Success);
                return true;
            }
            catch (COMException)
            {
                _swApp = null;
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Log("Ошибка подключения к SolidWorks: " + ex.Message, LogLevel.Error);
                _swApp = null;
                return false;
            }
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
            try { return _swApp.IActiveDoc2; }
            catch { return null; }
        }

        /// <summary>
        /// Determines the DocumentType from the SolidWorks GetType() return value.
        /// swDocPART=1, swDocASSEMBLY=2, swDocDRAWING=3
        /// </summary>
        public static DocumentType GetDocumentType(dynamic doc)
        {
            if (doc == null) return DocumentType.None;
            try
            {
                int t = (int)doc.GetType();
                switch (t)
                {
                    case 1: return DocumentType.Part;
                    case 2: return DocumentType.Assembly;
                    case 3: return DocumentType.Drawing;
                    default: return DocumentType.None;
                }
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
                    string typeName = feat.GetTypeName2();
                    if (typeName == "SheetMetal" || typeName == "SMFlatPattern")
                        return true;
                    feat = feat.GetNextFeature();
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
