using NLog;
using System.Windows;
using System.Windows.Threading;

namespace Bifald
{
    public partial class App : Application
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error(string.Format("{0} \n{1}", e.Exception.Message, e.Exception.StackTrace));
            MessageBox.Show("Fejl");
            e.Handled = true;
        }
    }
}
