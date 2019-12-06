using System;
using System.Windows.Forms;
using log4net;

namespace cach
{
    static class Program
    {
        private static ILog _logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(typeof(Program).Name);

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception: {ex.Message}", ex);
            }
        }
    }
}
