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

            try
            {
                MessageBoxButton btn = MessageBoxButton.OK;
                MessageBoxImage ico = MessageBoxImage.Information;

                string LoginA = log.Text;
                string PasswordA = pas.Password;

                if (string.IsNullOrWhiteSpace(log.Text) || string.IsNullOrWhiteSpace(pas.Password))
                {
                    MessageBox.Show("Все поля обязательны для ввода.");
                    log.Text = "";
                    pas.Password = "";
                    return;
                }
                if (!Regex.IsMatch(LoginA, "^[A-za-z]{5,15}$"))
                {
                    MessageBox.Show("Пожалуйста,введите логин повторно!", "Уведомление", btn, ico);
                    log.Text = "";
                    return;
                }

                if (!Regex.IsMatch(PasswordA, "^(?=.*[a-z])(?=.*\\d)[a-zA-Z\\d]{5,15}$"))
                {
                    MessageBox.Show("Пожалуйста, введите пароль правильно!", "Уведомление", btn, ico);
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
            catch
            {
                MessageBox.Show("Возникла неизвестная проблема. Пожалуйста, попробуйте позднее");
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
