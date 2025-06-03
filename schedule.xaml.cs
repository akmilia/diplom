using diplom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
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
                MessageBox.Show($"Ошибка загрузки расписания: {ex.Message}");
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
            catch
            {
                MessageBox.Show($"error ");
                return null;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is DateTime selectedDate))
                return;

            var scheduleItem = button.Tag as ScheduleAttendanceItem;
            if (scheduleItem == null)
            {
                MessageBox.Show("Ошибка: информация о занятии не найдена");
                return;
            }

            try
            {
                var searchDate = selectedDate.Date;

                var attendance = db.Attendances
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Idschedule == scheduleItem.IdSchedule &&
                                        a.Date.Date == searchDate);

                if (attendance == null)
                {
                    MessageBox.Show($"Занятие '{scheduleItem.Subject}' на {searchDate:dd.MM.yyyy} не найдено");
                    return;
                }

                var detailsWindow = new schedule_details(attendance.Idattendance);
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии занятия:\n{ex.Message}");
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                var selectedGroup = GroupComboBox.SelectedItem as Models.Group;
                if (selectedGroup == null)
                {
                    MessageBox.Show("Выберите группу для экспорта");
                    return;
                }
                var groupName = selectedGroup.Name;

                // Получаем все занятия для этой группы
                var groupSchedules = db.Schedules
                    .Include(s => s.GroupsIdgroupNavigation)
                    .Where(s => s.GroupsIdgroupNavigation.Name == groupName)
                    .ToList();

                if (!groupSchedules.Any())
                {
                    MessageBox.Show("Не найдены занятия для выбранной группы");
                    return;
                }

                // Получаем всех студентов группы
                var groupStudents = db.GroupsUsers
                    .Include(gu => gu.UsersIdusersNavigation)
                    .Where(gu => gu.GroupsIdgroups == groupSchedules.First().GroupsIdgroup)
                    .Select(gu => gu.UsersIdusersNavigation)
                    .ToList();

                // Получаем все даты занятий (первые 30)
                var allDates = db.Attendances
                    .Where(a => groupSchedules.Select(s => s.Idschedule).Contains(a.Idschedule))
                    .Select(a => a.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .Take(30)
                    .ToList();

                // Создаем диалог сохранения файла (WPF версия)
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"Посещаемость {groupName} {DateTime.Now:yyyy-MM-dd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Посещаемость");

                        // Заголовки
                        worksheet.Cells[1, 1].Value = "ID";
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
                            worksheet.Cells[row + 2, 1].Value = student.Idusers;
                            worksheet.Cells[row + 2, 2].Value = student.Surname;
                            worksheet.Cells[row + 2, 3].Value = student.Name;
                            worksheet.Cells[row + 2, 4].Value = student.Paternity;

                            // Статусы посещения
                            for (int col = 0; col < allDates.Count; col++)
                            {
                                var date = allDates[col];
                                var attendance = db.BilNebils
                                    .Any(bn => bn.Iduser == student.Idusers &&
                                              db.Attendances.Any(a => a.Idattendance == bn.Idattendance &&
                                                                     a.Date.Date == date.Date &&
                                                                     groupSchedules.Select(s => s.Idschedule).Contains(a.Idschedule)));
                                if (attendance == null)
                                    worksheet.Cells[row + 2, col + 5].Value = "Не указано";
                                else
                                    worksheet.Cells[row + 2, col + 5].Value = attendance ? "Присутствовал" : "Отсутствовал";
                            }
                        }

                        // Автонастройка ширины столбцов
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        package.SaveAs(new FileInfo(saveFileDialog.FileName)); 

                        MessageBox.Show($"Файл успешно сохранен: {saveFileDialog.FileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}");
            }
        }

        private void NewSchedule_Click(object sender, RoutedEventArgs e)
        {
            add_schedule add_sch = new add_schedule();
            bool? dialogResult = add_sch.ShowDialog();
            if (dialogResult == true)
            {
                LoadSchedule();
            }
        }
    }
}
