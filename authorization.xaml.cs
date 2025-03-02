using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace diplom
{

    public partial class authorization : Page
    {
        public authorization()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxButton btn = MessageBoxButton.OK;
            MessageBoxImage ico = MessageBoxImage.Information;
            string caption = "Дата сохранения";
            string LoginA = log.Text;
            string PasswordA = pas.Password;

            if (string.IsNullOrWhiteSpace(LoginA) || string.IsNullOrWhiteSpace(PasswordA))
            {
                MessageBox.Show("Все поля обязательны для ввода.");
                log.Text = "";
                pas.Password = "";
                return;
            }
            if (!Regex.IsMatch(LoginA, "^[A-za-z]{5,15}$"))
            {
                MessageBox.Show("Пожалуйста,введите логин повторно!", caption, btn, ico);
                log.Text = "";
                return;
            }

            if (!Regex.IsMatch(PasswordA, "^(?=.*[a-z])(?=.*\\d)[a-zA-Z\\d]{5,15}$"))
            {
                MessageBox.Show("Пожалуйста, введите пароль правильно!", caption, btn, ico);
                pas.Password = "";
                return;
            }


            bool isAuthenticated = AuthenticateUser(LoginA, PasswordA);

            if (isAuthenticated)
            {
                NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
                window.Navigate(new Uri("admin.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Неверные логин или пароль.");
            }
        }

        private bool AuthenticateUser(string login, string password)
        {
            using (DiplomSchoolContext db = new DiplomSchoolContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                return user != null;
            }
        }
    }
}
