using System;
using System.Windows.Forms;
using log4net;
using cachCore.controllers;
using cachCore.enums;

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

            MainForm form = null;
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                form = new MainForm();
                Application.Run(form);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception: {ex.Message}", ex);
                if (form != null && form.Game != null)
                {
                    var fen = new GameController().GetFEN(form.Game, ItemColor.White);
                    _logger.Info($"Main: FEN at exception: {fen}");
                }
            }
        }
    }
}
