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
            // Регистрируем резолвер ПЕРВЫМ — до любого использования SolidWorks типов
            InteropResolver.Register();

            // НЕ проверяем права администратора — это мешает подключению к SolidWorks
            // (COM GetActiveObject не работает если программа запущена с повышенными правами,
            //  а SolidWorks — без них, из-за Session 0 Isolation / UIPI)

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
