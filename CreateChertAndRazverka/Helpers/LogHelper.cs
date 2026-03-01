using System;
using System.Drawing;
using System.Windows.Forms;

namespace CreateChertAndRazverka.Helpers
{
    public enum LogLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

    public static class LogHelper
    {
        private static RichTextBox _logBox;

        public static void Initialize(RichTextBox logBox)
        {
            _logBox = logBox;
        }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (_logBox == null) return;

            string icon;
            Color color;

            switch (level)
            {
                case LogLevel.Success:
                    icon  = "✅ ";
                    color = Color.Green;
                    break;
                case LogLevel.Warning:
                    icon  = "⚠️ ";
                    color = Color.Goldenrod;
                    break;
                case LogLevel.Error:
                    icon  = "❌ ";
                    color = Color.Red;
                    break;
                default:
                    icon  = "ℹ️ ";
                    color = Color.Gray;
                    break;
            }

            string timestamp = DateTime.Now.ToString("[HH:mm:ss]");
            string line = timestamp + " " + icon + message + Environment.NewLine;

            if (_logBox.InvokeRequired)
            {
                _logBox.Invoke(new Action(() => AppendLine(line, color)));
            }
            else
            {
                AppendLine(line, color);
            }
        }

        private static void AppendLine(string line, Color color)
        {
            if (_logBox == null) return;
            _logBox.SelectionStart  = _logBox.TextLength;
            _logBox.SelectionLength = 0;
            _logBox.SelectionColor  = color;
            _logBox.AppendText(line);
            _logBox.SelectionColor  = _logBox.ForeColor;
            _logBox.ScrollToCaret();
        }

        public static void Clear()
        {
            if (_logBox == null) return;
            if (_logBox.InvokeRequired)
                _logBox.Invoke(new Action(() => _logBox.Clear()));
            else
                _logBox.Clear();
        }
    }
}
