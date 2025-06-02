using diplom.Models;
using System.Windows;
namespace diplom
{
    public partial class add_schedule : Window
    {
        private DiplomSchoolContext db = new();
        private static readonly Dictionary<string, int> DayOfWeekMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"понедельник", 1},
            {"вторник", 2},
            {"среда", 3},
            {"четверг", 4},
            {"пятница", 5},
            {"суббота", 6}
        };
        public class DayOfWeekItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public add_schedule()
        {
            InitializeComponent();
            DataContext = this;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                CabinetComboBox.ItemsSource = db.Cabinets.ToList();
                CabinetComboBox.DisplayMemberPath = "Description";
                CabinetComboBox.SelectedValuePath = "Idcabinet";

                TeacherComboBox.ItemsSource = db.Users
                    .Where(u => u.RolesIdroles == 2)
                    .ToList();
                TeacherComboBox.DisplayMemberPath = "Surname";
                TeacherComboBox.SelectedValuePath = "Idusers";

                SubjectComboBox.ItemsSource = db.Subjects.ToList();
                SubjectComboBox.DisplayMemberPath = "Name";
                SubjectComboBox.SelectedValuePath = "Idsubjects";

                GroupComboBox.ItemsSource = db.Groups.ToList();
                GroupComboBox.DisplayMemberPath = "Name";
                GroupComboBox.SelectedValuePath = "Idgroups";

                var dayOfWeekItems = DayOfWeekMap
               .Select(kvp => new DayOfWeekItem { Id = kvp.Value, Name = kvp.Key })
               .ToList();

                DayOfWeekComboBox.ItemsSource = dayOfWeekItems;
                DayOfWeekComboBox.DisplayMemberPath = "Name";
                DayOfWeekComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!ValidateInputs())
                    return;
                var startTime = StartTimePicker.Value.Value.TimeOfDay;
                //var dayOfWeekNumber = DayOfWeekToNumber(DayOfWeekComboBox.SelectedItem.ToString()); 
                var dayOfWeekNumber = (int)DayOfWeekComboBox.SelectedValue;
                var cabinetId = (int)CabinetComboBox.SelectedValue;
                var teacherId = (int)TeacherComboBox.SelectedValue;

                if (CheckScheduleConflict(dayOfWeekNumber, cabinetId, startTime))
                {
                    MessageBox.Show("Выбранное время занято для данного кабинета или преподавателя!",
                        "Конфликт расписания", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newSchedule = CreateNewSchedule(teacherId, startTime, dayOfWeekNumber);
                db.Schedules.Add(newSchedule);
                db.SaveChanges();

                CreateAttendanceRecords(newSchedule, dayOfWeekNumber);

                MessageBox.Show("Расписание успешно добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool ValidateInputs()
        {
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
                return false;
            }
            return true;
        }


        private bool CheckScheduleConflict(int dayOfWeek, int cabinetId, TimeSpan startTime)
        {
            return db.Schedules.Any(s =>
                s.DayOfWeek == dayOfWeek &&
                s.CabinetsIdcabinet == cabinetId &&
                s.Time == startTime);
        }

        private Schedule CreateNewSchedule(int teacherId, TimeSpan startTime, int dayOfWeekNumber)
        {
            return new Schedule
            {
                UsersIdusers = teacherId,
                Time = startTime,
                SubjectsIdsubjects = (int)SubjectComboBox.SelectedValue,
                CabinetsIdcabinet = (int)CabinetComboBox.SelectedValue,
                GroupsIdgroup = (int)GroupComboBox.SelectedValue,
                DayOfWeek = dayOfWeekNumber
            };
        }
        private void CreateAttendanceRecords(Schedule schedule, int dayOfWeekNumber)
        {
            var currentDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value;
            var groupId = schedule.GroupsIdgroup;

            var studentIds = db.GroupsUsers
                .Where(gu => gu.GroupsIdgroups == groupId)
                .Select(gu => gu.UsersIdusers)
                .ToList();

            while (currentDate <= endDate)
            {
                if ((int)currentDate.DayOfWeek + 1 == dayOfWeekNumber) // +1 для соответствия вашей нумерации
                {
                    var attendance = new Attendance
                    {
                        Idschedule = schedule.Idschedule,
                        Date = currentDate
                    };

                    db.Attendances.Add(attendance);
                    db.SaveChanges();

                    foreach (var studentId in studentIds)
                    {
                        db.BilNebils.Add(new BilNebil
                        {
                            Idattendance = attendance.Idattendance,
                            Iduser = studentId,
                            Status = null
                        });
                    }
                }
                currentDate = currentDate.AddDays(1);
            }
        }
        private static int DayOfWeekToNumber(string dayOfWeek)
        {
            if (DayOfWeekMap.TryGetValue(dayOfWeek, out int dayNumber))
            {
                return dayNumber;
            }

            throw new ArgumentException($"Некорректный день недели: {dayOfWeek}");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

    }
}
