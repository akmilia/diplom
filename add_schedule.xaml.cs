using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using diplom.Models;
using Microsoft.EntityFrameworkCore;
namespace diplom
{
    public partial class add_schedule : Window
    {
        DiplomSchoolContext db = new();
        public Dictionary<string, int> dayMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"понедельник", 1},
            {"вторник", 2},
            {"среда", 3},
            {"четверг", 4},
            {"пятница", 5},
            {"суббота", 6},
            {"воскресенье", 7}
        };
        public add_schedule()
        {
            InitializeComponent();
            DataContext = this;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LoadCabinets();
            LoadTeachers();
            LoadSubject();
            LoadGroups();
        }

        private void LoadCabinets()
        {
            CabinetComboBox.ItemsSource = db.Cabinets.ToList();
            CabinetComboBox.DisplayMemberPath = "Description";
            CabinetComboBox.SelectedValuePath = "Idcabinet";
        }

        private void LoadTeachers()
        {
            TeacherComboBox.ItemsSource = db.Users
               .Where(u => u.RolesIdroles == 2) 
               .ToList();
            TeacherComboBox.DisplayMemberPath = "Surname";
            TeacherComboBox.SelectedValuePath = "Idusers";
        }

        private void LoadSubject()
        {
           
            SubjectComboBox.ItemsSource = db.Subjects.ToList();
            SubjectComboBox.DisplayMemberPath = "Name";
            SubjectComboBox.SelectedValuePath = "Idsubjects";
        }

        private void LoadGroups()
        {
          
            GroupComboBox.ItemsSource = db.Groups.ToList();
            GroupComboBox.DisplayMemberPath = "Name";
            GroupComboBox.SelectedValuePath = "Idgroups";
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения всех полей
                if (CabinetComboBox.SelectedItem == null ||
                    TeacherComboBox.SelectedItem == null ||
                    SubjectComboBox.SelectedItem == null ||
                    GroupComboBox.SelectedItem == null ||
                    DayOfWeekComboBox.SelectedItem == null ||
                    StartTimePicker.Value == null ||
                    StartDatePicker.SelectedDate == null ||
                    EndDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на конфликты расписания
                var startTime = StartTimePicker.Value.Value.TimeOfDay;
                int dayOfWeek = DayOfWeektoNumber(DayOfWeekComboBox.SelectedItem.ToString());
                var cabinetId = (int)CabinetComboBox.SelectedValue;
                var teacherId = (int)TeacherComboBox.SelectedValue; 


                bool hasConflict = db.Schedules.Any(s => 
                    s.DayOfWeek == dayOfWeek &&
                    s.CabinetsIdcabinet == cabinetId &&
                    s.Time == startTime);

                if (hasConflict)
                {
                    MessageBox.Show("Выбранное время занято для данного кабинета или преподавателя!", "Конфликт расписания", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создание новой записи расписания
                var newSchedule = new Schedule
                {
                    UsersIdusers = teacherId,
                    Time = startTime,
                     SubjectsIdsubjects = (int)SubjectComboBox.SelectedValue,
                     CabinetsIdcabinet = cabinetId,
                     GroupsIdgroup = (int)GroupComboBox.SelectedValue,
                    DayOfWeek = dayOfWeek
                };
                db.Schedules.Add(newSchedule);
                db.SaveChanges();

                // Автоматическое создание записей посещений
                CreateAttendanceRecords(newSchedule, DayOfWeekComboBox.SelectedItem.ToString());

                MessageBox.Show("Расписание успешно добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateAttendanceRecords(Schedule schedule, string dayOfWeek)
        {
            DateTime currentDate = StartDatePicker.SelectedDate.Value; 
            var endDate = EndDatePicker.SelectedDate.Value;
            var groupId = schedule.GroupsIdgroup;

            var studentIds = db.GroupsUsers
                .Where(gu => gu.GroupsIdgroups == groupId)
                .Select(gu => gu.UsersIdusers)
                .ToList();

            while (currentDate <= endDate)
            {
                if (currentDate.DayOfWeek.ToString() == dayOfWeek)
                {
                    // Создаем запись посещения
                    var attendance = new Attendance
                    {
                        Idschedule = schedule.Idschedule,
                        Date = currentDate,
                    };
                    db.Attendances.Add(attendance);
                    db.SaveChanges(); 
                    foreach (var studentId in studentIds)
                    {
                        var bilNebil = new BilNebil
                        {
                            Idattendance = attendance.Idattendance,
                            Iduser = studentId,
                            Status = null
                        };
                        db.BilNebils.Add(bilNebil);
                    }
                }
                currentDate = currentDate.AddDays(1);
            }

            db.SaveChanges();

        }
        public int DayOfWeektoNumber(string dayofweek)
        {
                if (dayMap.TryGetValue(dayofweek, out int dayNumber))
                {
                    return dayNumber;
                }
                else
                {
                    MessageBox.Show("Возникла ошибка с конвертацией дней недели");
                    return -1;
                }
         
        }
    }
}
