using diplom.Models;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace diplom
{
    public partial class users : Page, INotifyPropertyChanged
    {
        public ObservableCollection<usersshow> UsersItems { get; set; } = new ObservableCollection<usersshow>();
        public ICollectionView UsersView { get; private set; }

        private readonly DiplomSchoolContext db = new DiplomSchoolContext();
        private int? _selectedRoleId;
        private string _searchText = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public users()
        {
            InitializeComponent();
            this.ShowsNavigationUI = true;

            DataContext = this;
            Loaded += OnPageLoaded;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new DiplomSchoolContext())
                {
                    LoadRoles(db);
                    LoadUsers(db);
                }
                InitializeFiltering();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        "Не получилось загрузить пользователей",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void LoadRoles(DiplomSchoolContext db)
        {
            try
            {
                var roles = db.Roles.ToList();
                RoleComboBox.ItemsSource = roles;
                RoleComboBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "Не получилось загрузить роли",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void LoadUsers(DiplomSchoolContext db)
        {
            try
            {
                var usersList = db.UserShowItems.ToList();
                UsersItems.Clear();
                foreach (var user in usersList)
                {
                    UsersItems.Add(user);
                }
                UsersView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не получилось загрузить пользователей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void InitializeFiltering()
        {
            UsersView = CollectionViewSource.GetDefaultView(UsersItems);
            UsersView.Filter = UserFilter;
            table.ItemsSource = UsersView;

            // Принудительно обновляем фильтрацию при инициализации
            _searchText = string.Empty;
            _selectedRoleId = null;
            UsersView.Refresh();
        }

        private bool UserFilter(object item)
        {
            if (item is not usersshow user) return false;

            // Всегда показывать, если не выбрана роль
            bool roleMatches = _selectedRoleId == null || user.idroles == _selectedRoleId;

            // Всегда показывать, если строка поиска пустая
            bool searchMatches = string.IsNullOrEmpty(_searchText) ||
                               (user.full_name?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false);

            return roleMatches && searchMatches;
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (RoleComboBox.SelectedItem is Role selectedRole)
                {
                    _selectedRoleId = selectedRole.Idroles;
                    if (_selectedRoleId == 0)
                        _selectedRoleId = null;
                }
                else
                {
                    _selectedRoleId = null;
                }

                UsersView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        "Не получилось загрузить пользователей",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }

        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = SearchTextBox.Text.Trim();
            UsersView?.Refresh();
        }

        private void table_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (table.SelectedItem is usersshow selectedUser)
                {
                    var userWindow = new user(selectedUser);
                    userWindow.Style = (Style)Application.Current.Resources["ModalWindowStyle"];
                    if (userWindow.ShowDialog() == true)
                    {
                        using (var db = new DiplomSchoolContext())
                        {
                            LoadUsers(db);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Не найден данный пользователь.",
                        "Уведомление",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
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

        private void toAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addUserWindow = new add_user();
                addUserWindow.Style = (Style)Application.Current.Resources["ModalWindowStyle"];
                if (addUserWindow.ShowDialog() == true)
                {
                    using (var db = new DiplomSchoolContext())
                    {
                        LoadUsers(db);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rName = $"Пользователи_{(RoleComboBox.SelectedItem as Role)?.Name ?? "Все"}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

                var saveFileDialog = new SaveFileDialog
                {
                    FileName = rName,
                    Filter = "Excel файлы (*.xlsx)|*.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportToExcel(saveFileDialog.FileName);
                    Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                    App.ShowToast($"Файл успешно сохранен: {rName}");
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


        private void ExportToExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Пользователи");

                // Заголовки
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Логин";
                worksheet.Cells[1, 3].Value = "ФИО";
                worksheet.Cells[1, 4].Value = "Роль";

                // Данные
                int row = 2;
                foreach (usersshow user in UsersView)
                {
                    worksheet.Cells[row, 1].Value = user.idusers;
                    worksheet.Cells[row, 2].Value = user.login;
                    worksheet.Cells[row, 3].Value = user.full_name;
                    worksheet.Cells[row, 4].Value = user.user_role;
                    row++;
                }

                // Форматирование
                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.SaveAs(new FileInfo(filePath));

            }
        }

    }
}

