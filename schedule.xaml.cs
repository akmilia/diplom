using diplom.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace diplom
{
    public partial class schedule : Page
    {
        DiplomSchoolContext db = new DiplomSchoolContext();
        public schedule()
        {
            InitializeComponent();
            Loaded += Schedule_Loaded;
        }

        private void Schedule_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSchedule();
        }

        public class ScheduleAttendanceItem
        {   
            public int IdSchedule { get; set; }
            //public int IdAttendance { get; set; }
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
                //var schedules = db.attendanceShow.ToList();

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
            //try
            //{
            //    var schedules = db.schedulesShow
            //                    .AsEnumerable()
            //                    .ToList();

            //    var monday = ConvertToScheduleItems(schedules.Where(s => s.day_of_week == "Понедельник").ToList());
            //    var tuesday = ConvertToScheduleItems(schedules.Where(s => s.day_of_week == "Вторник").ToList());
            //    var wednesday = ConvertToScheduleItems(schedules.Where(s => s.day_of_week == "Среда").ToList());
            //    var thursday = ConvertToScheduleItems(schedules.Where(s => s.day_of_week == "Четверг").ToList());
            //    var friday = ConvertToScheduleItems(schedules.Where(s => s.day_of_week == "Пятница").ToList());
            //    var saturday = ConvertToScheduleItems(schedules.Where(s => s.day_of_week == "Суббота").ToList());

            //    MondayListBox.ItemsSource = monday;
            //    TuesdayListBox.ItemsSource = tuesday;
            //    WednesdayListBox.ItemsSource = wednesday;
            //    ThursdayListBox.ItemsSource = thursday;
            //    FridayListBox.ItemsSource = friday;
            //    SaturdayListBox.ItemsSource = saturday;
            //}
            //catch
            //{
            //    MessageBox.Show("Произошла ошибка при загрузке расписания. Пожалуйста, попробуйте позднее.");
            //}
            //try
            //{
            //    var schedules = db.schedulesShow
            //                    .AsEnumerable()
            //                    .ToList();

            //    var monday = schedules.Where(s => s.day_of_week == "Понедельник").ToList();
            //    var tuesday = schedules.Where(s => s.day_of_week == "Вторник").ToList();
            //    var wednesday = schedules.Where(s => s.day_of_week == "Среда").ToList();
            //    var thursday = schedules.Where(s => s.day_of_week == "Четверг").ToList();
            //    var friday = schedules.Where(s => s.day_of_week == "Пятница").ToList();
            //    var saturday = schedules.Where(s => s.day_of_week == "Суббота").ToList();

            //    MondayListBox.ItemsSource = monday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})"); 
            //    TuesdayListBox.ItemsSource = tuesday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
            //    WednesdayListBox.ItemsSource = wednesday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
            //    ThursdayListBox.ItemsSource = thursday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
            //    FridayListBox.ItemsSource = friday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
            //    SaturdayListBox.ItemsSource = saturday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");


            //    //var Dates = db.Attendances.Select(u=> u.Idschedule == )
            //}
            //catch
            //{
            //    MessageBox.Show("Произошла ошибка при загрузке расписания. Пожалуйста, попробуйте позднее.");
            //}

        }

        private List<ScheduleAttendanceItem> GetDaySchedule(List<scheduleshow> schedules, string dayOfWeek)
        {
            return schedules
                .Where(s => s.day_of_week == dayOfWeek)
                .Select(s => new ScheduleAttendanceItem
                {
                    IdSchedule = s.idschedule,
                    //IdAttendance = s.idattendance,
                    Time = s.time.ToString("HH:mm"),
                    Subject = s.subject_name,
                    Teacher = s.teacher,
                    Cabinet = s.cabinet.ToString(),
                    Dates = db.Attendances
                        .Where(a => a.Idschedule == s.idschedule)
                        .Select(a => a.Date) 
                        .ToList()
                })
                .ToList();
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
                // Нормализуем дату (убираем время)
                var searchDate = selectedDate.Date;

                // Для PostgreSQL используем прямое сравнение дат
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
                Debug.WriteLine($"Error opening attendance: {ex}");
            } 
        }
    
    }
}
