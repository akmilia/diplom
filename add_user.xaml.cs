using diplom.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
namespace diplom
{
    public partial class add_user : Window, INotifyPropertyChanged, IDataErrorInfo
    {
        private string _surname;
        private string _nameN;
        private string _paternity;
     
        private string _login;
        private string _password;
        private Role _selectedRole;
        private List<Role> _roles;

        public string Surname
        {
            get { return _surname; }
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NameN
        {
            get { return _nameN; }
            set
            {
                if (_nameN != value)
                {
                    _nameN = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Paternity
        {
            get { return _paternity; }
            set
            {
                if (_paternity != value)
                {
                    _paternity = value;
                    OnPropertyChanged();
                }
            }
        }


        public string Login
        {
            get { return _login; }
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        public Role SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                if (_selectedRole != value)
                {
                    _selectedRole = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Role> Roles
        {
            get { return _roles; }
            set
            {
                _roles = value;
                OnPropertyChanged();
            }
        }

        DiplomSchoolContext db = new();
        public add_user()
        {
            InitializeComponent();
            DataContext = this;
            WindowStartupLocation = WindowStartupLocation.CenterScreen; 

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

         
                surname.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                NameU.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            if (string.IsNullOrEmpty(this["Surname"]) && string.IsNullOrEmpty(this["NameN"]) && string.IsNullOrEmpty(this["Login"]) && string.IsNullOrEmpty(this["Password"]) && SelectedRole != null)
            {

                try
                {
                    var test = db.Users.OrderByDescending(s => s.Idusers).FirstOrDefault();

                    User newUser = new User
                    {
                        Idusers = test != null ? test.Idusers + 1 : 1,
                        Surname = Surname,
                        Name = NameN,
                        Paternity = Paternity,
                        Birthdate = BirthDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value) : null,
                        Login = Login,
                        Password = Password,
                        RolesIdroles = SelectedRole.Idroles
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Пользователь добавлен успешно!");

                    this.Close();

                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Не получилось создать новый аккаунт {ex.Message}");
                }
            }
            else MessageBox.Show("Вся обязательные поля должны быть заполнены");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "Surname":
                    case "NameN":
                    case "Login":
                    case "Password":
                        if (string.IsNullOrEmpty((string)GetValue(columnName)))
                        {
                            error = "Поле обязательно для заполнения.";
                        }
                        else if (((string)GetValue(columnName)).Length > 150)
                        {
                            error = "Поле не должно превышать 150 символов.";
                        }
                        break;
                    case "Paternity":
                        if (!string.IsNullOrEmpty((string)GetValue(columnName)) && ((string)GetValue(columnName)).Length > 150)
                        {
                            error = "Поле не должно превышать 150 символов.";
                        }
                        break;
                  
                    case "SelectedRole":
                        if (SelectedRole == null)
                        {
                            error = "Роль обязательна для выбора.";
                        }
                        break;
                }
                return error;
            }
        }

        public string Error
        {
            get { return null; } 
        }

        private object GetValue(string propertyName)
        {
            switch (propertyName)
            {
                case "Surname": return Surname;
                case "NameN": return NameN;
                case "Paternity": return Paternity;
                case "Login": return Login;
                case "Password": return Password;
             
                default: return null;
            }
        }

        private DateOnly? ConvertToDateOnly(DateTime? dateTime)
        {
            return dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null;
        }
    }
}