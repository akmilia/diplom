using diplom.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace diplom
{
    public partial class add_user : Page
    {
        DiplomSchoolContext db = new();
        public add_user()
        {
            InitializeComponent();

            LoadRoles();
            Loaded += Add_subject_Loaded;
        }
        private void LoadRoles()
        {
            try
            {
                List<Role> types = [.. db.Roles.Where(u => u.Idroles != 0)];

                RoleComboBox.ItemsSource = types;
                RoleComboBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Проблема загрузки типов: {ex.Message}");
            }
        }
        private void Add_subject_Loaded(object sender, RoutedEventArgs e)
        {
            BirthDatePicker.DisplayDateStart = DateTime.Now.AddYears(-100);
            BirthDatePicker.DisplayDateEnd = DateTime.Now;
            BirthDatePicker.SelectedDate = DateTime.Now.AddYears(-20);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Role role = RoleComboBox.SelectedItem as Role;

                if (string.IsNullOrWhiteSpace(name.Text))
                {
                    MessageBox.Show("Please enter a subject name.");
                    return;
                }

                var test = db.Users.OrderByDescending(s => s.Idusers).FirstOrDefault();



                User newUser = new User
                {
                    Idusers = test.Idusers + 1,
                    Surname = surname.Text,
                    Name = name.Text,
                    Paternity = paternity.Text,
                    Birthdate = BirthDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value) : null,
                    Login = login.Text,
                    Password = Password.Text,
                    RolesIdroles = role.Idroles
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                MessageBox.Show("Пользователь добавлен успешно!");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не получилось создать новый аккаунт {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
            window.Navigate(new Uri("users .xaml", UriKind.Relative));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
