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
        /// Step 2: If SLDWORKS process is running but ROT entry not found yet, wait 500 ms and retry.
        /// Step 3: Never creates a new SolidWorks instance — returns false if connection fails.
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
            catch { /* Reflection not available */ }

            if (mi != null)
            {
                // First try versioned ProgID (SolidWorks 2022)
                try
                {
                    swObj = mi.Invoke(null, new object[] { SwProgIdVersioned });
                }
                catch (TargetInvocationException) { swObj = null; }
                catch { swObj = null; }

                // Then try generic ProgID
                if (swObj == null)
                {
                    try
                    {
                        swObj = mi.Invoke(null, new object[] { SwProgIdGeneric });
                    }
                    catch (TargetInvocationException) { swObj = null; }
                    catch { swObj = null; }
                }
            }

            // Step 2: If SLDWORKS process is running but GetActiveObject failed,
            // retry after a short delay (SW may not have registered in ROT yet).
            if (swObj == null && mi != null)
            {
                try
                {
                    var procs    = System.Diagnostics.Process.GetProcessesByName("SLDWORKS");
                    bool running = procs.Length > 0;
                    foreach (var p in procs) p.Dispose();

                    if (running)
                    {
                        System.Threading.Thread.Sleep(500);
                        try
                        {
                            swObj = mi.Invoke(null, new object[] { SwProgIdGeneric });
                        }
                        catch (TargetInvocationException) { swObj = null; }
                        catch { swObj = null; }
                    }
                }
                catch { /* ignore process-enumeration errors */ }
            }

            // Step 3: SW is running but COM object is still unavailable — do NOT create a new instance.
            if (swObj == null)
            {
                try
                {
                    var procs    = System.Diagnostics.Process.GetProcessesByName("SLDWORKS");
                    bool running = procs.Length > 0;
                    foreach (var p in procs) p.Dispose();

                    if (running)
                        LogHelper.Log("SolidWorks запущен, но COM-объект недоступен. Проверьте права администратора.", LogLevel.Warning);
                }
                catch { /* ignore */ }

                _isConnected = false;
                return false;
            }

            _swApp       = swObj;
            _isConnected = true;

            // Ensure the existing SW instance is visible so ActiveDoc is accessible.
            try
            {
                dynamic sw = (dynamic)_swApp;
                sw.Visible = true;
            }
            catch { /* ignore — Visible may not be settable in all configurations */ }

            LogHelper.Log("Подключено к SolidWorks 2022.", LogLevel.Success);
            return true;
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

                // Verify the doc object is still alive before returning it.
                if (doc != null)
                {
                    try { _ = (string)((dynamic)doc).GetTitle(); }
                    catch (COMException) { return null; }              // doc is stale
                    catch (InvalidComObjectException) { return null; } // doc was released
                    catch { /* GetTitle may throw for unsaved/unnamed docs — still valid */ }
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
