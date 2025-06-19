using diplom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace diplom
{

    public partial class schedule : Page
    {
        private DiplomSchoolContext db = new DiplomSchoolContext();
        public schedule()
        {
            InitializeComponent();
            this.ShowsNavigationUI = true;
            Loaded += Schedule_Loaded;
        }

        private void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSchedule();

            GroupComboBox.ItemsSource = db.Groups.ToList();
        }

        public class ScheduleAttendanceItem
        {
            public int IdSchedule { get; set; }
            public string Time { get; set; }
            public string Subject { get; set; }
            public string Teacher { get; set; }
            public string Cabinet { get; set; }
            public List<DateTime> Dates { get; set; }
            public string DayOfWeek { get; set; }
        }
        private void LoadSchedule()
        {
            try
            {
                var schedules = db.schedulesShow.ToList();
                var monday = GetDaySchedule(schedules, "Понедельник");
                var tuesday = GetDaySchedule(schedules, "Вторник");
                var wednesday = GetDaySchedule(schedules, "Среда");
                var thursday = GetDaySchedule(schedules, "Четверг");
                var friday = GetDaySchedule(schedules, "Пятница");
                var saturday = GetDaySchedule(schedules, "Суббота");

                MondayListBox.ItemsSource = monday;
                TuesdayListBox.ItemsSource = tuesday;
                WednesdayListBox.ItemsSource = wednesday;
                ThursdayListBox.ItemsSource = thursday;
                FridayListBox.ItemsSource = friday;
                SaturdayListBox.ItemsSource = saturday;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        "Не получилось загрузить занятия.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private List<ScheduleAttendanceItem> GetDaySchedule(List<scheduleshow> schedules, string dayOfWeek)
        {

            try
            {
                return schedules
               .Where(s => s.day_of_week.Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase))
               .OrderBy(s => s.time)
               .Select(s => new ScheduleAttendanceItem
               {
                   IdSchedule = s.idschedule,
                   Time = s.time.ToString(@"hh\:mm"),
                   Subject = s.subject_name,
                   Teacher = s.teacher,
                   Cabinet = s.cabinet.ToString(),
                   Dates = db.Attendances
                       .Where(a => a.Idschedule == s.idschedule)
                       .OrderBy(a => a.Date)
                       .Select(a => a.Date)
                       .ToList()
               })
               .ToList();
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
                return null;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is DateTime selectedDate))
                return;

            try
            {
                var scheduleItem = button.Tag as ScheduleAttendanceItem;
                var searchDate = selectedDate.Date;

                var attendance = db.Attendances
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Idschedule == scheduleItem.IdSchedule &&
                                        a.Date.Date == searchDate);

                if (attendance == null)
                {
                    MessageBox.Show(
                        $"Занятие '{scheduleItem.Subject}' на {searchDate:dd.MM.yyyy} не найдено",
                        "Уведомление",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
                var detailsWindow = new schedule_details(attendance.Idattendance);
                detailsWindow.Style = (Style)Application.Current.Resources["ModalWindowStyle"];
                detailsWindow.ShowDialog();
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
                var selectedGroup = GroupComboBox.SelectedItem as Group;
                if (selectedGroup == null)
                {
                    MessageBox.Show(
                         "Пожалуйста, вначале выберите группу для экспорта.",
                         "Уведомление",
                         MessageBoxButton.OK,
                         MessageBoxImage.Warning
                     );
                    return;
                }

                var groupName = selectedGroup.Name;

                // Получаем все занятия для этой группы
                var groupSchedules = db.Schedules
                    .Where(s => s.GroupsIdgroup == selectedGroup.Idgroups)
                    .ToList();

                if (groupSchedules.Count == 0)
                {
                    MessageBox.Show(
                        $"Не найдены занятия для группы '{groupName}'",
                        "Уведомление",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // Получаем всех студентов группы
                var groupStudents = db.GroupsUsers
                    .Include(gu => gu.UsersIdusersNavigation)
                    .Where(gu => gu.GroupsIdgroups == selectedGroup.Idgroups)
                    .Select(gu => gu.UsersIdusersNavigation)
                    .OrderBy(s => s.Surname)
                    .ThenBy(s => s.Name)
                    .ToList();

                if (groupStudents.Count == 0)
                {
                    MessageBox.Show(
                        $"В группе '{groupName}' нет студентов",
                        "Уведомление",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // Получаем все даты занятий (первые 30)
                var allDates = db.Attendances
                    .Where(a => groupSchedules.Select(s => s.Idschedule).Contains(a.Idschedule))
                    .Select(a => a.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .Take(30)
                    .ToList();

                if (allDates.Count == 0)
                {
                    MessageBox.Show(
                        $"Для группы '{groupName}' нет запланированных занятий",
                        "Уведомление",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }


                string sName = $"Посещаемость {(GroupComboBox.SelectedItem as Group)?.Name} {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = sName
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Посещаемость");

                        // Заголовки
                        worksheet.Cells[1, 1].Value = "№";
                        worksheet.Cells[1, 2].Value = "Фамилия";
                        worksheet.Cells[1, 3].Value = "Имя";
                        worksheet.Cells[1, 4].Value = "Отчество";

                        // Даты занятий
                        for (int i = 0; i < allDates.Count; i++)
                        {
                            worksheet.Cells[1, i + 5].Value = allDates[i].ToString("dd.MM.yyyy");
                        }

                        // Данные студентов
                        for (int row = 0; row < groupStudents.Count; row++)
                        {
                            var student = groupStudents[row];
                            worksheet.Cells[row + 2, 1].Value = row + 1;
                            worksheet.Cells[row + 2, 2].Value = student.Surname;
                            worksheet.Cells[row + 2, 3].Value = student.Name;
                            worksheet.Cells[row + 2, 4].Value = student.Paternity ?? string.Empty;

                            // Статусы посещения
                            for (int col = 0; col < allDates.Count; col++)
                            {
                                var date = allDates[col];
                                var attendance = db.BilNebils
                                    .FirstOrDefault(bn => bn.Iduser == student.Idusers &&
                                              db.Attendances.Any(a => a.Idattendance == bn.Idattendance &&
                                                                     a.Date.Date == date.Date &&
                                                                     groupSchedules.Select(s => s.Idschedule).Contains(a.Idschedule)));

                                worksheet.Cells[row + 2, col + 5].Value = attendance != null ?
                                    ((bool)attendance.Status ? "Присутствовал" : "Отсутствовал") :
                                    "Не отмечено";
                            }
                        }

                        // Форматирование заголовков
                        using (var range = worksheet.Cells[1, 1, 1, 4 + allDates.Count])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        }

                        // Автонастройка ширины столбцов
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        package.SaveAs(new FileInfo(saveFileDialog.FileName));
                        App.ShowToast($"Файл успешно сохранен: {sName}");
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

        private void NewSchedule_Click(object sender, RoutedEventArgs e)
        {
            add_schedule add_sch = new add_schedule();
            add_sch.Style = (Style)Application.Current.Resources["ModalWindowStyle"];
            bool? dialogResult = add_sch.ShowDialog();
            if (dialogResult == true)
            {
                LoadSchedule();
            }
        }
    }
}
