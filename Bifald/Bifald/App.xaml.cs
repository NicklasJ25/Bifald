using Bifald.Dialog;
using MaterialDesignThemes.Wpf;
using NLog;
using System.Windows;
using System.Windows.Threading;

namespace Bifald
{
    public partial class App : Application
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        async void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error(string.Format("{0} \n{1}", e.Exception.Message, e.Exception.StackTrace));
            var view = new StandardDialog();
            view.label.Content = "Der skete en uventet fejl!\nNoter hvad du gjorde for at skabe fejlen og hvad tid det skete.\nKontakt efterfølgende Nicklas :D";
            view.cancelButton.Visibility = Visibility.Hidden;
            view.acceptButton.Content = "Ok";
            await DialogHost.Show(view, "RootDialog");
            e.Handled = true;
        }
    }
}
