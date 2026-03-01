using System;
using System.Windows.Forms;
using CreateChertAndRazverka.Helpers;

namespace CreateChertAndRazverka
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (!AdminHelper.IsRunningAsAdmin())
            {
                if (!Array.Exists(args, a => a == "--restarted"))
                {
                    AdminHelper.RestartAsAdmin();
                    Environment.Exit(0);
                }
                else
                {
                    MessageBox.Show(
                        "CreateChert And Razverka не может быть запущен без прав администратора.\n" +
                        "Взаимодействие с SolidWorks 2022 через COM API требует повышенных привилегий.",
                        "CreateChert And Razverka — Ошибка прав доступа",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
