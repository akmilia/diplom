using diplom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace diplom
{
    public partial class users : Page
    {
        public ObservableCollection<usersshow> UsersItems { get; set; } = new ObservableCollection<usersshow>();
        public DiplomSchoolContext db = new DiplomSchoolContext();
        public List<usersshow> userFromView;

        public users()
        {
            InitializeComponent();
            LoadTypes();
            LoadUsers();
        }

        private void LoadTypes()
        {
            try
            {
                List<Role> types = [.. db.Roles];
                RoleComboBox.ItemsSource = types;
                RoleComboBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла проблема с загрузкой ролей");
            }
        }
        private void RoleComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (RoleComboBox.SelectedItem != null)
                {

                    Role selType = RoleComboBox.SelectedItem as Role;

                    if (selType.Idroles == 0)
                    {
                        table.ItemsSource = userFromView;
                    }
                    else
                    {
                        table.ItemsSource = userFromView.Where(s => s.idroles == selType.Idroles).ToList();
                    }

                    table.Items.Refresh();
                }
            }
            catch
            {
                MessageBox.Show("Возникла неизвестная проблема. Пожалуйста, попробуйте позднее");
            }

        }

        private void LoadUsers()
        {
            try
            {
                table.Items.Clear();
                userFromView = [.. db.userShowItems.FromSqlRaw("select * from usersshow")];
                table.ItemsSource = userFromView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла неизвестная проблема. Пожалуйста, попробуйте позднее");
            }
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
                    MessageBox.Show("Пользователь не найден.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла ошибка: {ex.Message}. Попробуйте позднее");
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
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("UsersReport");

                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Логин";
                    worksheet.Cells[1, 3].Value = "Пароль";
                    worksheet.Cells[1, 4].Value = "ФИО";
                    worksheet.Cells[1, 5].Value = "Роль";

                    List<usersshow> list = (table.ItemsSource as IEnumerable<object>).Cast<usersshow>().ToList();

                    for (int i = 0; i < list.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = list[i].idusers;
                        worksheet.Cells[i + 2, 2].Value = list[i].login;
                        worksheet.Cells[i + 2, 3].Value = list[i].password;
                        worksheet.Cells[i + 2, 4].Value = list[i].full_name;
                        worksheet.Cells[i + 2, 5].Value = list[i].user_role;
                    }

                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        FileName = "UsersReport.xlsx",
                        DefaultExt = ".xlsx",
                        Filter = "Excel Files|*.xlsx"
                    };


                    if (saveFileDialog.ShowDialog() == true)
                    {
                        FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
                        excelPackage.SaveAs(fileInfo);
                        MessageBox.Show("Данные успешно выгружены в Excel файл.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выгрузке данных в Excel: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchText = SearchTextBox.Text.Trim().ToLower();

                if (string.IsNullOrEmpty(searchText))
                {
                    table.ItemsSource = UsersItems;
                }
                else
                {
                    var filteredUsers = userFromView
                        .Where(u => u.full_name.ToLower().Contains(searchText))
                        .ToList();

                    table.ItemsSource = filteredUsers;
                }
            }
            catch
            {
                MessageBox.Show("Возникла неизвестная проблема. Пожалуйста, попробуйте позднее");
            }

        }
    }
}
