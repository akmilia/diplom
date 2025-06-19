using diplom.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace diplom
{
    public partial class user : Window, INotifyPropertyChanged, IDataErrorInfo
    {
        private string _login;
        private string _password;
        private string _surname;
        private string _name1;
        private string _paternity;
        private DateTime? _birthdate;

        public DiplomSchoolContext db = new DiplomSchoolContext();
        public User curUser { get; set; }
        public List<string> UserSubjectsOrGroups { get; set; }

        public string UserRole
        {
            get
            {
                if (curUser == null) return "Не определена";
                Role rl = db.Roles.FirstOrDefault(r => r.Idroles == curUser.RolesIdroles);
                return rl.Name;
            }
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

        public string Surname
        {
            get => _surname;
            set { _surname = value; OnPropertyChanged(); }
        }

        public string Name1
        {
            get => _name1;
            set { _name1 = value; OnPropertyChanged(); }
        }

        public string Paternity
        {
            get => _paternity;
            set { _paternity = value; OnPropertyChanged(); }
        }

        public DateTime? Birthdate
        {
            get => _birthdate;
            set { _birthdate = value; OnPropertyChanged(); }
        }

        public user(usersshow usr)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = this;


            curUser = db.Users.FirstOrDefault(u => u.Idusers == usr.idusers);

            if (curUser == null)
            {
                MessageBox.Show("Пользователь не найден в базе данных", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            Login = curUser.Login;
            Password = curUser.Password;
            Surname = curUser.Surname;
            Name1 = curUser.Name;
            Paternity = curUser.Paternity;

            if (curUser.Birthdate.HasValue)
            {
                Birthdate = new DateTime(
                    curUser.Birthdate.Value.Year,
                    curUser.Birthdate.Value.Month,
                    curUser.Birthdate.Value.Day);
            }

            LoadUserData();

        }
        private void LoadUserData()
        {
            try
            {
                // Загрузка данных в зависимости от роли
                if (curUser.RolesIdroles == 1) // Администратор
                {
                    UserSubjectsOrGroups = new List<string> { "Администратор не имеет занятий" };
                    DeleteSubject.Visibility = Visibility.Collapsed;
                }
                else if (curUser.RolesIdroles == 2) // Преподаватель
                {
                    UserSubjectsOrGroups = db.Schedules
                        .Where(s => s.UsersIdusers == curUser.Idusers)
                        .Select(s => s.SubjectsIdsubjectsNavigation.Name)
                        .Distinct()
                        .ToList();

                    DeleteSubject.Visibility = Visibility.Visible;
                }
                else if (curUser.RolesIdroles == 3) // Ученик
                {
                    UserSubjectsOrGroups = db.GroupsUsers
                       .Where(gu => gu.UsersIdusers == curUser.Idusers)
                       .Select(gu => gu.GroupsIdgroupsNavigation.Name)
                       .ToList();

                    DeleteSubject.Visibility = Visibility.Visible;
                }

                SubjectList.ItemsSource = UserSubjectsOrGroups;
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

        private void DeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            if (SubjectList.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите занятие для удаления", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedItem = SubjectList.SelectedItem.ToString();

            try
            {
                if (curUser.RolesIdroles == 2) // Преподаватель
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить предмет '{selectedItem}' из расписания преподавателя?",
                        "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var subject = db.Subjects.FirstOrDefault(s => s.Name == selectedItem);
                        if (subject != null)
                        {
                            // Удаляем все занятия преподавателя по этому предмету
                            var schedulesToDelete = db.Schedules
                                .Where(s => s.UsersIdusers == curUser.Idusers &&
                                            s.SubjectsIdsubjects == subject.Idsubjects)
                                .ToList();

                            db.Schedules.RemoveRange(schedulesToDelete);
                            db.SaveChanges();

                            LoadUserData();
                            App.ShowToast($"Предмет '{selectedItem}' успешно удалён из расписания");
                        }
                    }
                }
                else if (curUser.RolesIdroles == 3) // Ученик
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите отписаться от группы '{selectedItem}'?",
                        "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Находим группу по имени
                        var group = db.Groups.FirstOrDefault(g => g.Name == selectedItem);
                        if (group != null)
                        {
                            // Удаляем связь ученика с группой
                            var groupUser = db.GroupsUsers
                                .FirstOrDefault(gu => gu.UsersIdusers == curUser.Idusers &&
                                                    gu.GroupsIdgroups == group.Idgroups);

                            if (groupUser != null)
                            {
                                db.GroupsUsers.Remove(groupUser);
                                db.SaveChanges();

                                LoadUserData();
                                App.ShowToast($"Вы успешно отписались от группы '{selectedItem}'");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось выполнить операцию. Пожалуйста, попробуйте позднее.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Debug.WriteLine($"Ошибка: {ex.Message}");
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
                    case "Login":
                        if (string.IsNullOrEmpty(Login))
                            error = "Логин обязателен для заполнения";
                        else if (Login.Length > 25)
                            error = "Логин не должен превышать 25 символов";
                        break;
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
                    case "Name1":
                        if (string.IsNullOrEmpty(Name1))
                            error = "Имя обязательно для заполнения";
                        else if (Name1.Length > 45)
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
                string.IsNullOrEmpty(this["Name1"]) &&
                string.IsNullOrEmpty(this["Paternity"]) &&
                string.IsNullOrEmpty(this["Login"]))
            {
                try
                {
                    var userToUpdate = db.Users.FirstOrDefault(u => u.Idusers == curUser.Idusers);
                    if (userToUpdate != null)
                    {
                        // Обновляем основные поля
                        userToUpdate.Login = Login;
                        userToUpdate.Password = Password;
                        userToUpdate.Surname = Surname;
                        userToUpdate.Name = Name1;
                        userToUpdate.Paternity = Paternity;

                        // Конвертируем DateTime? обратно в DateOnly? для сохранения в базу
                        if (Birthdate.HasValue)
                        {
                            userToUpdate.Birthdate = DateOnly.FromDateTime(Birthdate.Value);
                        }
                        else
                        {
                            userToUpdate.Birthdate = null;
                        }

                        db.SaveChanges();

                        App.ShowToast("Данные пользователя успешно обновлены.");
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Пользователь не найден в базе данных",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Не удалось обновить данные пользователя: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    Debug.WriteLine($"Ошибка при обновлении пользователя: {ex.Message}");
                }
            }
        }

    }
}