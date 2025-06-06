using diplom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
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
    public partial class users : Page
    {
        public ObservableCollection<usersshow> UsersItems { get; set; } = new ObservableCollection<usersshow>();
        public DiplomSchoolContext db = new DiplomSchoolContext();
        private int? _selectedRoleId;
        private ICollectionView _usersView;

        public users()
        {
            InitializeComponent();
            this.ShowsNavigationUI = true;

            LoadTypes();
            LoadUsers();
        }

        private void LoadTypes()
        {
            try
            {
                List<Role> types = db.Roles.ToList();
                RoleComboBox.ItemsSource = types;
                RoleComboBox.DisplayMemberPath = "Name";

                RoleComboBox.SelectedIndex = 3;
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

        private void LoadUsers()
        {
            try
            {
                var usersList = db.userShowItems.FromSqlRaw("select * from usersshow").ToList();

                UsersItems.Clear();
                foreach (var user in usersList)
                {
                    UsersItems.Add(user);
                }

                if (_usersView == null)
                {
                    _usersView = CollectionViewSource.GetDefaultView(UsersItems);
                    table.ItemsSource = _usersView;
                }
                else
                {
                    _usersView.Refresh();
                }
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

        private void ApplyFilters()
        {
            string searchText = SearchTextBox.Text.Trim().ToLower();

            if (_usersView == null)
                return;

            _usersView.Filter = obj =>
            {
                if (obj is usersshow user)
                {
                    bool matchesRole = !_selectedRoleId.HasValue || user.idroles == _selectedRoleId.Value;
                    bool matchesSearch = string.IsNullOrEmpty(searchText) || (user.full_name != null && user.full_name.ToLower().Contains(searchText));
                    return matchesRole && matchesSearch;
                }
                return false;
            };

            _usersView.Refresh();
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleComboBox.SelectedItem is Role selType)
            {
                _selectedRoleId = selType.Idroles == 0 ? null : selType.Idroles;
                ApplyFilters();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }


        private void table_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            var usersh = table.SelectedItem as usersshow;

            try
            {

                if (usersh != null)
                {
                    user userPage = new user(usersh);
                    bool? dialogResult = userPage.ShowDialog();
                    if (dialogResult == true)
                    {
                        LoadUsers();
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
            add_user add_user = new add_user();
            bool? dialogResult = add_user.ShowDialog();
            if (dialogResult == true)
            {
                LoadUsers();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Подготовка данных для экспорта
                var exportData = new List<dynamic>();
                foreach (usersshow user in _usersView)
                {
                    exportData.Add(new
                    {
                        ID = user.idusers,
                        Логин = user.login,
                        ФИО = user.full_name,
                        Роль = user.user_role
                    });
                }

                if (!exportData.Any())
                {
                    App.ShowToast("Нет данных для экспорта");
                    return;
                }

                // Настройка диалога сохранения
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = $"Пользователи_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx",
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    DefaultExt = ".xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
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
                        foreach (var item in exportData)
                        {
                            worksheet.Cells[row, 1].Value = item.ID;
                            worksheet.Cells[row, 2].Value = item.Логин;
                            worksheet.Cells[row, 3].Value = item.ФИО;
                            worksheet.Cells[row, 4].Value = item.Роль;
                            row++;
                        }

                        // Автоподбор ширины столбцов
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Сохранение файла
                        package.SaveAs(new FileInfo(saveFileDialog.FileName));
                        App.ShowToast($"Файл успешно сохранен: {saveFileDialog.FileName}");

                        // Открытие файла после сохранения
                        Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
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

    }
}

