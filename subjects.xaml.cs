using diplom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
namespace diplom
{

    public partial class subjects : Page
    {
        public ObservableCollection<subjectsshow> SubjectItems { get; set; } = new ObservableCollection<subjectsshow>();
        public DiplomSchoolContext db = new DiplomSchoolContext();
        public List<subjectsshow> subjectsFromView;

        public subjects()
        {
            InitializeComponent();
            this.ShowsNavigationUI = true;

            LoadSubjects();
            DataContext = this;
            LoadTypes();
            LoadGroups();
        }

        private void LoadTypes()
        {
            try
            {
                List<diplom.Models.Type> types = db.Types.ToList();
                TypeComboBox.ItemsSource = types;
                TypeComboBox.DisplayMemberPath = "Type1";

                TypeComboBox.SelectedIndex = 3;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "Не получилось загрузить типы",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error
                   );

                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void LoadGroups()
        {
            try
            {
                List<Group> groups = db.Groups.ToList();
                tableGroups.ItemsSource = groups;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "Не получилось загрузить группы",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error
                   );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void LoadSubjects()
        {
            try
            {
                SubjectItems.Clear();
                subjectsFromView = db.SubjectShowItems.FromSqlRaw("select * from subjectsshow").ToList();

                foreach (var subject in subjectsFromView)
                {
                    SubjectItems.Add(subject);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "Не получилось загрузить занятия",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error
                   );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                subjectsshow path = table.SelectedItem as subjectsshow;

                subject subj = new subject(path.subject_id);
                bool? dialogResult = subj.ShowDialog();
                if (dialogResult == true)
                {
                    LoadSubjects();
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
                add_subject add_subj = new add_subject();
                bool? dialogResult = add_subj.ShowDialog();
                if (dialogResult == true)
                {
                    LoadSubjects();
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

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            try
            {
                if (TypeComboBox.SelectedItem != null)
                {

                    Models.Type selType = TypeComboBox.SelectedItem as Models.Type;

                    if (selType.Id == 0)
                    {
                        table.ItemsSource = subjectsFromView;
                    }
                    else
                    {
                        table.ItemsSource = subjectsFromView.Where(s => s.type_id == selType.Id).ToList();
                    }

                    table.Items.Refresh();
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

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"Предметы  {((ComboBoxItem)TypeComboBox.SelectedItem)?.Content ?? "все предметы"} {DateTime.Now:yyyy-MM-dd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        // Экспорт предметов
                        var subjectsSheet = package.Workbook.Worksheets.Add("Предметы");
                        subjectsSheet.Cells[1, 1].Value = "ID";
                        subjectsSheet.Cells[1, 2].Value = "Название";
                        subjectsSheet.Cells[1, 3].Value = "Описание";
                        subjectsSheet.Cells[1, 4].Value = "ID типа";
                        subjectsSheet.Cells[1, 5].Value = "Тип";

                        for (int i = 0; i < SubjectItems.Count; i++)
                        {
                            subjectsSheet.Cells[i + 2, 1].Value = SubjectItems[i].subject_id;
                            subjectsSheet.Cells[i + 2, 2].Value = SubjectItems[i].subject_name;
                            subjectsSheet.Cells[i + 2, 3].Value = SubjectItems[i].description;
                            subjectsSheet.Cells[i + 2, 4].Value = SubjectItems[i].type_id;
                            subjectsSheet.Cells[i + 2, 5].Value = SubjectItems[i].type_name;
                        }

                        // Экспорт групп
                        var groupsSheet = package.Workbook.Worksheets.Add("Группы");
                        groupsSheet.Cells[1, 1].Value = "ID";
                        groupsSheet.Cells[1, 2].Value = "Название";

                        var groups = tableGroups.ItemsSource as IEnumerable<Group>;
                        if (groups != null)
                        {
                            int row = 2;
                            foreach (var group in groups)
                            {
                                groupsSheet.Cells[row, 1].Value = group.Idgroups;
                                groupsSheet.Cells[row, 2].Value = group.Name;
                                row++;
                            }
                        }

                        // Автонастройка ширины столбцов
                        subjectsSheet.Cells[subjectsSheet.Dimension.Address].AutoFitColumns();
                        groupsSheet.Cells[groupsSheet.Dimension.Address].AutoFitColumns();

                        package.SaveAs(new FileInfo(saveFileDialog.FileName));
                        App.ShowToast($"Файл успешно сохранен: {saveFileDialog.FileName}");
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
