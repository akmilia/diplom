using System.Windows;
using System.Windows.Navigation;

namespace diplom;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        NavigationWindow window = new NavigationWindow
        {
            Source = new Uri("authorization.xaml", UriKind.Relative),
            WindowState = WindowState.Maximized,
            MinHeight = 700,
            MinWidth = 1200
        };
        window.Show();
    }
}

