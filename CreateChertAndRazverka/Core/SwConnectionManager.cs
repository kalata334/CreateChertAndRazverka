using System;
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
        /// Uses the same three-tier fallback as the original connector.
        /// </summary>
        public bool TryConnect()
        {
            // Tier 1: versioned ProgID (SolidWorks 2022)
            try
            {
                _swApp       = Marshal.GetActiveObject(SwProgIdVersioned);
                _isConnected = true;
                LogHelper.Log("Подключено к SolidWorks 2022.", LogLevel.Success);
                return true;
            }
            catch (COMException) { /* try fallback */ }
            catch (Exception ex)
            {
                LogHelper.Log("Ошибка подключения к SolidWorks (versioned): " + ex.Message, LogLevel.Warning);
            }

            // Tier 2: generic ProgID
            try
            {
                _swApp       = Marshal.GetActiveObject(SwProgIdGeneric);
                _isConnected = true;
                LogHelper.Log("Подключено к SolidWorks (generic ProgID).", LogLevel.Success);
                return true;
            }
            catch (COMException) { /* not running */ }
            catch (Exception ex)
            {
                LogHelper.Log("Ошибка подключения к SolidWorks: " + ex.Message, LogLevel.Error);
            }

            // Tier 3: verify process is running, then try generic ProgID again
            try
            {
                var  procs     = System.Diagnostics.Process.GetProcessesByName("SLDWORKS");
                bool isRunning = procs.Length > 0;
                foreach (var p in procs) p.Dispose();

                if (isRunning)
                {
                    _swApp       = Marshal.GetActiveObject(SwProgIdGeneric);
                    _isConnected = true;
                    LogHelper.Log("Подключено к SolidWorks (через процесс).", LogLevel.Success);
                    return true;
                }
            }
            catch { /* ignore */ }

            _swApp       = null;
            _isConnected = false;
            return false;
        }

        /// <summary>
        /// Probes the COM object by reading the <c>Visible</c> property.
        /// Resets the connection state when the object is stale.
        /// </summary>
        public bool VerifyConnection()
        {
            if (!_isConnected || _swApp == null)
            {
                _isConnected = false;
                return false;
            }

            try
            {
                // Reading Visible is a lightweight probe that will throw if SW went away.
                ((dynamic)_swApp).Visible.ToString();
                return true;
            }
            catch (COMException)
            {
                ResetState();
                return false;
            }
            catch (InvalidComObjectException)
            {
                ResetState();
                return false;
            }
            catch
            {
                ResetState();
                return false;
            }
        }

        /// <summary>
        /// Returns the active document as <c>object</c>, or <c>null</c>.
        /// Attempts automatic reconnection when the COM reference is stale.
        /// </summary>
        public object GetActiveDocument()
        {
            if (!_isConnected || _swApp == null) return null;

            try
            {
                dynamic sw  = (dynamic)_swApp;
                object  doc = null;

                try { doc = sw.ActiveDoc; }
                catch (COMException) { ResetState(); return null; }
                catch (InvalidComObjectException) { ResetState(); return null; }
                catch { /* ignore other errors */ }

                if (doc == null)
                {
                    try { doc = sw.IActiveDoc2; }
                    catch (COMException) { ResetState(); return null; }
                    catch (InvalidComObjectException) { ResetState(); return null; }
                    catch { /* ignore */ }
                }

                if (doc == null)
                {
                    try { doc = sw.GetFirstDocument(); }
                    catch { /* ignore */ }
                }

                return doc;
            }
            catch (COMException) { ResetState(); return null; }
            catch (InvalidComObjectException) { ResetState(); return null; }
            catch { return null; }
        }

        /// <summary>
        /// Releases the COM reference and resets connection state.
        /// </summary>
        public void Disconnect()
        {
            if (_swApp != null)
            {
                try { Marshal.FinalReleaseComObject(_swApp); }
                catch { /* ignore */ }
                LogHelper.Log("Отключено от SolidWorks.", LogLevel.Info);
            }
            ResetState();
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

        private void ResetState()
        {
            _swApp       = null;
            _isConnected = false;
        }
    }
}
