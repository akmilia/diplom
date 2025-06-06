using Notification.Wpf;
using System.Windows;
using System.Windows.Navigation;

namespace diplom
{
    public partial class App : Application
    {
        private static readonly NotificationManager _notifier = new();

        public static void ShowToast(string message, bool isError = false)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.Show(
                    new NotificationContent
                    {
                        Title = "Уведомление",
                        Message = message,
                        Type = isError ? NotificationType.Error : NotificationType.Information
                    },
                    expirationTime: TimeSpan.FromSeconds(3));
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new NavigationWindow
            {
                Source = new Uri("authorization.xaml", UriKind.Relative),
                WindowState = WindowState.Maximized,
                MinHeight = 700,
                MinWidth = 1200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowsNavigationUI = true
            };
            mainWindow.Show();
        }
    }
}