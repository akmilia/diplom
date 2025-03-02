using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace diplom.Components
{

    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }
        private void CloseWindow()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
            window.Navigate(new Uri("schedule.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
            window.Navigate(new Uri("subjects.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
            window.Navigate(new Uri("users.xaml", UriKind.Relative));
        }

        private void ImageClick(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
            window.Navigate(new Uri("adin.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.CloseWindow();
        }
    }
}
