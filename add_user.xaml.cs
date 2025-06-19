using diplom.Models;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
namespace diplom
{
    public partial class add_user : Window, INotifyPropertyChanged
    {
        private string _surname;
        private string _nameN;
        private string _paternity;
        private string _login;
        private string _password;
        private Role _selectedRole;
        private bool _isSaving;
        private bool _isSaved = false; 
        public string Surname
        {
            get => _surname;
            set { _surname = value; OnPropertyChanged(); }
        }

        public string NameN
        {
            get => _nameN;
            set { _nameN = value; OnPropertyChanged(); }
        }

        public string Paternity
        {
            get => _paternity;
            set { _paternity = value; OnPropertyChanged(); }
        }

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        public bool IsSaving { get => _isSaving; set { _isSaving = value; OnPropertyChanged(); } }

        private DiplomSchoolContext db = new DiplomSchoolContext();

        public event PropertyChangedEventHandler PropertyChanged;

        public add_user()
        {
            InitializeComponent();
            DataContext = this;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += OnPageLoaded;
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new DiplomSchoolContext())
                {
                    List<Role> rolesList = await db.Roles
                        .Where(r => r.Idroles != 0)
                        .OrderBy(r => r.Name)
                        .ToListAsync();

                    RoleComboBox.ItemsSource = rolesList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "Не получилось загрузить роли.",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error
                   );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            } 
            BirthDatePicker.DisplayDateStart = DateTime.Now.AddYears(-100);
            BirthDatePicker.DisplayDateEnd = DateTime.Now;
            BirthDatePicker.SelectedDate = DateTime.Now.AddYears(-6);
        } 

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaving) return;
        
            try
            {
                IsSaving = true;
                UpdateBindings();
                
                if (HasValidationErrors())
                {
                    MessageBox.Show(
                        "Пожалуйста, заполните все обязательные поля правильно!",
                        "Ошибка валидации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }
                await SaveUserAsync();
                _isSaved = true;

                App.ShowToast("Пользователь добавлен успешно");
                    DialogResult = true;
                    Close();
                //}
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

        private bool HasValidationErrors()
        {
            return !string.IsNullOrEmpty(this["Surname"]) ||
                   !string.IsNullOrEmpty(this["NameN"]) ||
                   !string.IsNullOrEmpty(this["Login"]) ||
                   !string.IsNullOrEmpty(this["Password"]) ||
                   !string.IsNullOrEmpty(this["SelectedRole"]);
        }


        private async System.Threading.Tasks.Task SaveUserAsync()
        {
            using (var db = new DiplomSchoolContext())
            {
                bool loginExists = await db.Users.AnyAsync(u => u.Login == Login);
                if (loginExists)
                {
                    MessageBox.Show("Этот логин уже занят");
                    return;
                }

                int maxId = await db.Users.MaxAsync(u => (int?)u.Idusers) ?? 0; 

                var newUser = new User
                {
                    Idusers = maxId + 1,
                    Surname = Surname,
                    Name = NameN,
                    Paternity = string.IsNullOrWhiteSpace(Paternity) ? null : Paternity,
                    Birthdate = BirthDatePicker.SelectedDate.HasValue
                        ? DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value)
                        : null,
                    Login = Login,
                    Password = Password,
                    RolesIdroles = SelectedRole.Idroles
                };

                await db.Users.AddAsync(newUser);
                await db.SaveChangesAsync();
            }
        } 

        private void UpdateBindings()
        {
            surname.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            NameU.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            paternity.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            login.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            password.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            RoleComboBox.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get
            {
                string value = GetValue(columnName);

                if (string.IsNullOrWhiteSpace(value) &&
                   (columnName == "Surname" || columnName == "NameN" ||
                    columnName == "Login" || columnName == "Password"))
                    return "Обязательное поле";

                if (!string.IsNullOrWhiteSpace(value) && value.Length > 50)
                    return "Максимум 50 символов";

                if (columnName == "SelectedRole" && SelectedRole == null)
                    return "Выберите роль"; 



                return null;
            }
        }
        private string GetValue(string propertyName) => propertyName switch
        {
            "Surname" => Surname,
            "NameN" => NameN,
            "Paternity" => Paternity,
            "Login" => Login,
            "Password" => Password,
            _ => null
        }; 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isSaved)
                return;
            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

       
    }
}