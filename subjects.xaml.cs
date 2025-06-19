using diplom.Models;
using Microsoft.Win32;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace diplom
{

    public partial class subjects : Page, INotifyPropertyChanged
    {
        public ObservableCollection<subjectsshow> SubjectItems { get; set; } = new ObservableCollection<subjectsshow>();
        public ObservableCollection<Group> GroupsItems { get; set; } = new ObservableCollection<Group>();

        public ICollectionView SubjectView { get; private set; }
        public ICollectionView GroupView { get; private set; }

        public DiplomSchoolContext db = new DiplomSchoolContext();

        private int? _selectedTypeId;

        public event PropertyChangedEventHandler PropertyChanged;
        public subjects()
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
                    LoadTypes(db);
                    LoadGroups(db);
                    LoadSubjects(db);
                }
                InitializeFiltering();
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

        private void LoadTypes(DiplomSchoolContext db)
        {
            try
            {
                List<diplom.Models.Type> types = db.Types.ToList();
                TypeComboBox.ItemsSource = types;
                TypeComboBox.DisplayMemberPath = "Type1";
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

        private void LoadGroups(DiplomSchoolContext db)
        {
            try
            {
                List<Group> groupList = db.Groups.ToList();
                GroupsItems.Clear();
                foreach (var gr in groupList)
                {
                    GroupsItems.Add(gr);
                }
                GroupView = CollectionViewSource.GetDefaultView(GroupsItems);
                tableGroups.ItemsSource = GroupView;

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

        private void LoadSubjects(DiplomSchoolContext db)
        {
            try
            {
                var subjectList = db.SubjectShowItems.ToList();
                SubjectItems.Clear();

                foreach (var sb in subjectList)
                {
                    SubjectItems.Add(sb);
                }
                SubjectView?.Refresh();
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

        private void InitializeFiltering()
        {
            SubjectView = CollectionViewSource.GetDefaultView(SubjectItems);
            SubjectView.Filter = SubjectFilter;
            table.ItemsSource = SubjectView;


            _selectedTypeId = null;
            SubjectView.Refresh();
        }

        private bool SubjectFilter(object item)
        {
            if (item is not subjectsshow subject) return false;
            bool typeMatches = _selectedTypeId == null || subject.type_id == _selectedTypeId;

            return typeMatches;
        }


        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            try
            {
                if (TypeComboBox.SelectedItem is Models.Type selectedType)
                {
                    _selectedTypeId = selectedType.Id;
                    if (_selectedTypeId == 0)
                        _selectedTypeId = null;
                }
                else
                {
                    _selectedTypeId = null;
                }

                SubjectView?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                      "Не получилось загрузить типы.",
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
                if (table.SelectedItem is subjectsshow path)
                {
                    subject subj = new subject(path.subject_id);
                    subj.Style = (Style)Application.Current.Resources["ModalWindowStyle"];

                    if (subj.ShowDialog() == true)
                    {
                        using (var db = new DiplomSchoolContext())
                        {
                            LoadSubjects(db);
                        }
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
                add_subject add_subj = new add_subject();
                add_subj.Style = (Style)Application.Current.Resources["ModalWindowStyle"];

                if (add_subj.ShowDialog() == true)
                {
                    using (var db = new DiplomSchoolContext())
                    {
                        LoadSubjects(db);
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

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string tName = $"Предметы_{(TypeComboBox.SelectedItem as Models.Type)?.Type1 ?? "Все"}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = tName
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    Export(saveFileDialog.FileName);
                    Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                    App.ShowToast($"Файл успешно сохранен: {tName}");
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

        private void Export(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                // Get filtered subjects from the current view
                var filteredSubjects = SubjectView.Cast<subjectsshow>().ToList();

                // Экспорт отфильтрованных предметов
                var subjectsSheet = package.Workbook.Worksheets.Add("Занятия");
                subjectsSheet.Cells[1, 1].Value = "ID";
                subjectsSheet.Cells[1, 2].Value = "Название";
                subjectsSheet.Cells[1, 3].Value = "Описание";
                subjectsSheet.Cells[1, 4].Value = "ID типа";
                subjectsSheet.Cells[1, 5].Value = "Тип";

                if (filteredSubjects.Count > 0)
                {
                    for (int i = 0; i < filteredSubjects.Count; i++)
                    {
                        subjectsSheet.Cells[i + 2, 1].Value = filteredSubjects[i].subject_id;
                        subjectsSheet.Cells[i + 2, 2].Value = filteredSubjects[i].subject_name;
                        subjectsSheet.Cells[i + 2, 3].Value = filteredSubjects[i].description;
                        subjectsSheet.Cells[i + 2, 4].Value = filteredSubjects[i].type_id;
                        subjectsSheet.Cells[i + 2, 5].Value = filteredSubjects[i].type_name;
                    }
                }
                else
                {
                    subjectsSheet.Cells[2, 1].Value = "Нет данных о занятиях";
                }

                // Экспорт групп (всех, как было)
                var groupsSheet = package.Workbook.Worksheets.Add("Группы");
                groupsSheet.Cells[1, 1].Value = "ID";
                groupsSheet.Cells[1, 2].Value = "Название";

                if (GroupsItems.Count > 0)
                {
                    int row = 2;
                    foreach (var group in GroupsItems)
                    {
                        groupsSheet.Cells[row, 1].Value = group.Idgroups;
                        groupsSheet.Cells[row, 2].Value = group.Name;
                        row++;
                    }
                }
                else
                {
                    groupsSheet.Cells[2, 1].Value = "Нет данных о группах";
                }

                // Автонастройка ширины столбцов
                subjectsSheet.Cells[subjectsSheet.Dimension.Address].AutoFitColumns();
                groupsSheet.Cells[groupsSheet.Dimension.Address].AutoFitColumns();

                // Добавим форматирование для заголовков
                using (var headerCells = subjectsSheet.Cells["A1:E1"])
                {
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                using (var headerCells = groupsSheet.Cells["A1:B1"])
                {
                    headerCells.Style.Font.Bold = true;
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }

    }
}
