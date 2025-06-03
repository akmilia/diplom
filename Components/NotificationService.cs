using MaterialDesignThemes.Wpf;
using System.Windows;

namespace diplom.Components
{
    public static class NotificationService
    {
        private static Snackbar _snackbar;

        public static void Initialize(Snackbar snackbar)
        {
            _snackbar = snackbar;
        }

        public static void ShowSnackbar(string message, bool isError = false)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _snackbar?.MessageQueue?.Enqueue(
                    message,
                    null,
                    null,
                    null,
                    false,
                    true,
                    TimeSpan.FromSeconds(3));

                // Для отладки
                System.Diagnostics.Debug.WriteLine($"Snackbar shown: {message}");
            });
        }
    }
}