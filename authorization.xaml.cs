using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace diplom
{

    public partial class authorization : Page
    {

        public string LoginA, PasswordA;
        public authorization()
        {
            InitializeComponent();

            this.ShowsNavigationUI = false;

            log.Text = Properties.Settings.Default.SavedLogin;
            pas.Password = Properties.Settings.Default.SavedPassword;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                LoginA = log.Text;
                PasswordA = pas.Password;

                Validate();


                if (AuthenticateUser(LoginA, PasswordA))
                {

                    PropertiesSave();
                    App.ShowToast("Вход выполнен успешно!");


                    NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
                    window.Navigate(new Uri("admin.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Пользователя с таким логином и паролем не существует.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        "Возникла неизвестная проблема. Пожалуйста, попробуйте позднее.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void PropertiesSave()
        {
            if (rememberMe.IsChecked == true)
            {
                Properties.Settings.Default.SavedLogin = LoginA;
                Properties.Settings.Default.SavedPassword = PasswordA;
                Properties.Settings.Default.Save();

                App.ShowToast("Данные для входа сохранены.");
            }
            else
            {
                Properties.Settings.Default.SavedLogin = null;
                Properties.Settings.Default.SavedPassword = null;
                Properties.Settings.Default.Save();
            }
        }

        private void Validate()
        {
            MessageBoxButton btn = MessageBoxButton.OK;
            MessageBoxImage ico = MessageBoxImage.Information;


            if (string.IsNullOrWhiteSpace(LoginA) || string.IsNullOrWhiteSpace(PasswordA))
            {
                MessageBox.Show("Все поля обязательны для ввода.", "Уведомление", btn, ico);
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
        }

        private bool AuthenticateUser(string login, string password)
        {

            try
            {
                using (DiplomSchoolContext db = new DiplomSchoolContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

                    return user != null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        "Возникла неизвестная проблема. Пожалуйста, попробуйте позднее.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                Debug.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
        }
    }
}
