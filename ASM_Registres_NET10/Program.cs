using ASM_Registres;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ASM_Registres_NET10
{
    internal static class Program
    {
        private static readonly string LogFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ASM_Registres",
            "crash.log");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => LogException("AppDomain.UnhandledException", e.ExceptionObject as Exception);
            Application.ThreadException += (s, e) => LogException("Application.ThreadException", e.Exception);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new FormLogin());
            }
            catch (Exception ex)
            {
                LogException("Main", ex);
                throw;
            }
        }

        private static void LogException(string source, Exception ex)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFile));
                string entry = $"=== {DateTime.Now:yyyy-MM-dd HH:mm:ss} [{source}] ==={Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}";
                File.AppendAllText(LogFile, entry);
                MessageBox.Show("Error fatal: " + (ex?.Message ?? "desconocido") + Environment.NewLine + Environment.NewLine + "Detalles guardados en: " + LogFile, "ASM Registres", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { /* ignore logging failures */ }
        }
    }
}