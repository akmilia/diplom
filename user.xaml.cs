using diplom.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace diplom
{
    public partial class user : Window, INotifyPropertyChanged, IDataErrorInfo
    {
        private string _password;
        private string _surname;
        private string _name;
        private string _paternity;
        private DateOnly? _birthdate;

        public DiplomSchoolContext db = new DiplomSchoolContext();
        public User curUser;
        public List<Group> UserGroups { get; set; }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string Surname
        {
            get => _surname;
            set { _surname = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Paternity
        {
            get => _paternity;
            set { _paternity = value; OnPropertyChanged(); }
        }

        public DateOnly? Birthdate
        {
            get => _birthdate;
            set { _birthdate = value; OnPropertyChanged(); }
        }

        public user(usersshow usr)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            curUser = db.Users.FirstOrDefault(u => u.Idusers == usr.idusers);
            Password = curUser.Password;
            Surname = curUser.Surname;
            Name = curUser.Name;
            Paternity = curUser.Paternity;
            Birthdate = curUser.Birthdate;

            LoadUserGroups();

            DataContext = this;
        }

        private void LoadUserGroups()
        {
            UserGroups = db.GroupsUsers
                .Where(gu => gu.UsersIdusers == curUser.Idusers)
                .Select(gu => gu.GroupsIdgroupsNavigation)
                .ToList();

            SubjectList.ItemsSource = UserGroups;
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
                    case "Password":
                        if (string.IsNullOrEmpty(Password))
                            error = "Пароль обязателен для заполнения";
                        else if (Password.Length > 25)
                            error = "Пароль не должен превышать 25 символов";
                        break;
                    case "Surname":
                        if (string.IsNullOrEmpty(Surname))
                            error = "Фамилия обязательна для заполнения";
                        else if (Surname.Length > 45)
                            error = "Фамилия не должна превышать 45 символов";
                        break;
                    case "Name":
                        if (string.IsNullOrEmpty(Name))
                            error = "Имя обязательно для заполнения";
                        else if (Name.Length > 45)
                            error = "Имя не должно превышать 45 символов";
                        break;
                    case "Paternity":
                        if (Paternity?.Length > 45)
                            error = "Отчество не должно превышать 45 символов";
                        break;
                }
                return error;
            }
        }

        public string Error => null;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?",
                "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (MessageBox.Show("Вы уверены, что хотите удалить пользователя? Это может привести к необратимым потерям данных. ", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
                {
                    var userToDelete = db.Users.FirstOrDefault(u => u.Idusers == curUser.Idusers);
                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();

                        App.ShowToast("Пользователь успешно удален.");
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                      "Не получилось найти данного пользователя.",
                      "Уведомление",
                      MessageBoxButton.OK,
                      MessageBoxImage.Warning
                         );
                    }
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this["Password"]) &&
                string.IsNullOrEmpty(this["Surname"]) &&
                string.IsNullOrEmpty(this["Name"]) &&
                string.IsNullOrEmpty(this["Paternity"]))
            {
                try
                {
                    var userToUpdate = db.Users.FirstOrDefault(u => u.Idusers == curUser.Idusers);
                    if (userToUpdate != null)
                    {
                        userToUpdate.Password = Password;
                        userToUpdate.Surname = Surname;
                        userToUpdate.Name = Name;
                        userToUpdate.Paternity = Paternity;
                        userToUpdate.Birthdate = Birthdate;

                        db.SaveChanges();


                        App.ShowToast("Данные пользователя успешно обновлены.");
                        this.DialogResult = true;
                        this.Close();
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
            else
            {
                MessageBox.Show(
                      "Пожалуйста, заполните все данные корректно!.",
                      "Уведомление",
                      MessageBoxButton.OK,
                      MessageBoxImage.Warning
                  );
            }
        }
    }
}