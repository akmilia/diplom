using diplom.Components;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MaterialDesignThemes.Wpf;
using System.Windows.Documents;
using System.Windows.Media;

namespace diplom;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var navWindow = new NavigationWindow
        {
            Source = new Uri("authorization.xaml", UriKind.Relative),
            WindowState = WindowState.Maximized,
            MinHeight = 700,
            MinWidth = 1200,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        // 2. Создаем Snackbar
        var snackbar = new Snackbar
        {
            Name = "GlobalSnackbar",
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(20)
        };

        // 3. Создаем контейнер для контента
        var contentContainer = new Grid();
        contentContainer.Children.Add(snackbar);

        // 4. Настраиваем визуальное дерево
        navWindow.Loaded += (sender, args) =>
        {
            if (navWindow.Content is FrameworkElement content)
            {
                contentContainer.Children.Insert(0, content);
                navWindow.Content = contentContainer;
            }
        };

        // 5. Инициализация сервиса
        NotificationService.Initialize(snackbar);

        navWindow.Show();
    }
}

