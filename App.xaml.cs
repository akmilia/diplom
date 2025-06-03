using System;
using System.Windows;
using System.Windows.Navigation;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace diplom
{
    public partial class App : Application
    {
        private static Notifier _notifier;
        private static NavigationWindow _mainWindow;

        public static void ShowToast(string message, NotificationType type = NotificationType.Information)
        {
            if (_notifier == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Явная проверка типа
                if (type == NotificationType.Success)
                {
                    _notifier.ShowSuccess(message);
                }
                else if (type == NotificationType.Error)
                {
                    _notifier.ShowError(message);
                }
                else if (type == NotificationType.Warning)
                {
                    _notifier.ShowWarning(message);
                }
                else
                {
                    _notifier.ShowInformation(message);
                }
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _mainWindow = new NavigationWindow
            {
                Source = new Uri("authorization.xaml", UriKind.Relative),
                WindowState = WindowState.Maximized,
                MinHeight = 700,
                MinWidth = 1200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowsNavigationUI = false
            };

            // Инициализация Notifier с явным указанием типа
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: _mainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            _mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifier?.Dispose();
            base.OnExit(e);
        }
    }

    public enum NotificationType
    {
        Success,
        Error,
        Warning,
        Information
    }
}