using System;
using System.Reflection;
using System.Runtime.InteropServices;
using CreateChertAndRazverka.Helpers;

namespace CreateChertAndRazverka.Core
{
    /// <summary>
    /// Manages the lifetime of the COM reference to a running SolidWorks instance.
    /// Stores the reference as <c>object</c> (not <c>dynamic</c>) so that null checks
    /// never touch the DLR and cannot throw <see cref="COMException"/>.
    /// Connection state is tracked with a separate boolean flag.
    /// </summary>
    public class SwConnectionManager : IDisposable
    {
        private const string SwProgIdVersioned = "SldWorks.Application.30"; // SolidWorks 2022
        private const string SwProgIdGeneric   = "SldWorks.Application";

        // Store as object – NOT dynamic – so that null checks are always safe.
        private object _swApp;
        private bool   _isConnected;
        private bool   _disposed;

        /// <summary>True when we have a live COM reference to SolidWorks.</summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Returns the COM object cast to <c>dynamic</c> for API calls.
        /// Only call this when <see cref="IsConnected"/> is true.
        /// </summary>
        public dynamic SwApp => (dynamic)_swApp;

        // ── Connection ────────────────────────────────────────────────────────────

        /// <summary>
        /// Attempts to attach to an already-running SolidWorks instance.
        /// Step 1: Marshal.GetActiveObject via Reflection (versioned ProgID, then generic).
        /// Step 2: Fallback to Type.GetTypeFromProgID + Activator.CreateInstance.
        /// </summary>
        public bool TryConnect()
        {
            object swObj = null;

            // Step 1: Marshal.GetActiveObject via Reflection
            MethodInfo mi = null;
            try
            {
                mi = typeof(Marshal).GetMethod("GetActiveObject", new Type[] { typeof(string) });
            }
            catch { /* ignore */ }

            if (mi != null)
            {
                // First try versioned ProgID (SolidWorks 2022)
                try
                {
                    swObj = mi.Invoke(null, new object[] { SwProgIdVersioned });
                }
                catch { swObj = null; }

                // Then try generic ProgID
                if (swObj == null)
                {
                    try
                    {
                        swObj = mi.Invoke(null, new object[] { SwProgIdGeneric });
                    }
                    catch { swObj = null; }
                }
            }

            // Step 2: Fallback — Type.GetTypeFromProgID + Activator.CreateInstance
            // NOTE: This creates a NEW SolidWorks instance if none is running.
            if (swObj == null)
            {
                try
                {
                    Type t = Type.GetTypeFromProgID(SwProgIdGeneric);
                    if (t != null)
                    {
                        swObj = Activator.CreateInstance(t);
                        LogHelper.Log("SolidWorks запущен через Activator (новый экземпляр).", LogLevel.Warning);
                    }
                }
                catch { swObj = null; }
            }

            if (swObj != null)
            {
                _swApp       = swObj;
                _isConnected = true;
                LogHelper.Log("Подключено к SolidWorks 2022.", LogLevel.Success);
                return true;
            }

            _isConnected = false;
            return false;
        }

        /// <summary>
        /// Probes the COM object by calling <c>GetProcessID()</c>.
        /// Resets the connection state when the object is stale.
        /// </summary>
        public bool VerifyConnection()
        {
            if (_swApp == null || !_isConnected) return false;
            try
            {
                // Probe the COM object — if it's dead this throws
                dynamic sw = (dynamic)_swApp;
                _ = (int)sw.GetProcessID();
                return true;
            }
            catch
            {
                // COM object is dead — clean up
                ReleaseCom();
                return false;
            }
        }

        /// <summary>
        /// Returns the active document as <c>object</c>, or <c>null</c>.
        /// Calls <see cref="ReleaseCom"/> when the COM reference is stale.
        /// </summary>
        public object GetActiveDocument()
        {
            if (!_isConnected || _swApp == null) return null;

            try
            {
                dynamic sw  = (dynamic)_swApp;
                object  doc = null;

                try { doc = sw.ActiveDoc; }
                catch (COMException) { ReleaseCom(); return null; }
                catch (InvalidComObjectException) { ReleaseCom(); return null; }
                catch { /* ignore other errors */ }

                if (doc == null)
                {
                    try { doc = sw.IActiveDoc2; }
                    catch (COMException) { ReleaseCom(); return null; }
                    catch (InvalidComObjectException) { ReleaseCom(); return null; }
                    catch { /* ignore */ }
                }

                return doc;
            }
            catch (COMException) { ReleaseCom(); return null; }
            catch (InvalidComObjectException) { ReleaseCom(); return null; }
            catch { return null; }
        }

        /// <summary>
        /// Releases the COM reference and resets connection state.
        /// </summary>
        public void Disconnect()
        {
            if (_swApp != null)
            {
                LogHelper.Log("Отключено от SolidWorks.", LogLevel.Info);
                ReleaseCom();
            }
            else
            {
                _isConnected = false;
            }
        }

        // ── IDisposable ───────────────────────────────────────────────────────────

        public void Dispose()
        {
            if (!_disposed)
            {
                Disconnect();
                _disposed = true;
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void ReleaseCom()
        {
            if (_swApp != null)
            {
                try { Marshal.FinalReleaseComObject(_swApp); }
                catch { /* ignore */ }
                _swApp = null;
            }
            _isConnected = false;
        }
    }
}
