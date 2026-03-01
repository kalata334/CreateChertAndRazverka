using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;

namespace CreateChertAndRazverka.Helpers
{
    public static class AdminHelper
    {
        public static bool IsRunningAsAdmin()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static void RestartAsAdmin()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName        = Assembly.GetExecutingAssembly().Location,
                    UseShellExecute = true,
                    Verb            = "runas",
                    Arguments       = "--restarted"
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "CreateChert And Razverka требует прав администратора.\n\n" +
                    "Запустите программу вручную от имени администратора.\n\n" +
                    "Ошибка: " + ex.Message,
                    "CreateChert And Razverka — Требуются права администратора",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}
