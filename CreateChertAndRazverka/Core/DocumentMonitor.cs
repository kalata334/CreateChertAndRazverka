using System;
using System.Windows.Forms;
using CreateChertAndRazverka.Models;

namespace CreateChertAndRazverka.Core
{
    /// <summary>
    /// Polls SolidWorks every 1500 ms for the active document and fires events on change.
    /// </summary>
    public class DocumentMonitor : IDisposable
    {
        private readonly SolidWorksConnector _connector;
        private readonly Timer               _timer;
        private string                       _lastDocPath = "__UNINITIALIZED__";
        private bool                         _disposed;

        // ── Events ──────────────────────────────────────────────────────────────
        public event EventHandler<DocumentState> DocumentChanged;
        public event EventHandler                SwDisconnected;
        public event EventHandler                SwReconnected;

        public DocumentMonitor(SolidWorksConnector connector)
        {
            _connector = connector ?? throw new ArgumentNullException("connector");

            _timer          = new Timer { Interval = 1500 };
            _timer.Tick    += OnTick;
        }

        public void Start() => _timer.Start();
        public void Stop()  => _timer.Stop();

        private void OnTick(object sender, EventArgs e)
        {
            // Try to connect if not connected
            if (!_connector.IsConnected)
            {
                if (!_connector.TryConnect())
                {
                    if (_lastDocPath != null) // was connected before
                    {
                        _lastDocPath = null;
                        SwDisconnected?.Invoke(this, EventArgs.Empty);
                    }
                    return;
                }
                SwReconnected?.Invoke(this, EventArgs.Empty);
            }

            dynamic doc = _connector.GetActiveDocument();

            // ИСПРАВЛЕНИЕ: НЕ использовать ?. с dynamic — это не работает в .NET Framework 4.8!
            string currentPath = null;
            if (doc != null)
            {
                try { currentPath = (string)doc.GetPathName(); }
                catch { /* SW may throw when switching docs */ }
            }

            if (currentPath == _lastDocPath) return;

            _lastDocPath = currentPath;

            if (doc == null || string.IsNullOrEmpty(currentPath))
            {
                DocumentChanged?.Invoke(this, DocumentState.Empty);
                return;
            }

            DocumentType type = SolidWorksConnector.GetDocumentType(doc);

            bool isSheetMetal = false;
            if (type == DocumentType.Part)
                isSheetMetal = SolidWorksConnector.IsSheetMetal(doc);

            var state = new DocumentState
            {
                Type        = type,
                FilePath    = currentPath,
                FileName    = System.IO.Path.GetFileName(currentPath),
                IsSheetMetal = isSheetMetal
            };

            DocumentChanged?.Invoke(this, state);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _timer.Stop();
                _timer.Dispose();
                _disposed = true;
            }
        }
    }
}
