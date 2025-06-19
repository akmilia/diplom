using diplom.Models;
using System.Diagnostics;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace diplom
{
    public partial class add_schedule : System.Windows.Window
    {
        private bool _isSaved = false; 
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
                Debug.WriteLine($"Ошибка: {ex.Message}");
                MessageBox.Show(
                         "Не получилось загрузить данные.",
                         "Ошибка",
                         MessageBoxButton.OK,
                         MessageBoxImage.Error
                     );
                return;
               
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!ValidateInputs())
                    return;

                var startTime = StartTimePicker.Value.Value.TimeOfDay;
                var dayOfWeekNumber = (int)DayOfWeekComboBox.SelectedValue;
                var cabinetId = (int)CabinetComboBox.SelectedValue;
                var teacherId = (int)TeacherComboBox.SelectedValue;

                if (CheckScheduleConflict(dayOfWeekNumber, cabinetId, startTime))
                {
                    MessageBox.Show("Выбранное время занято для данного кабинета или преподавателя!",
                                  "Уведомление",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var newSchedule = CreateNewSchedule(teacherId, startTime, dayOfWeekNumber);
                        db.Schedules.Add(newSchedule);
                        db.SaveChanges();

                        CreateAttendanceRecords(newSchedule, dayOfWeekNumber);
                        transaction.Commit();

                        App.ShowToast("Расписание успешно добавлено!");
                        _isSaved = true; 
                        this.DialogResult = true;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при сохранении: {ex.InnerException?.Message ?? ex.Message}",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                        Debug.WriteLine($"Ошибка: {ex.ToString()}");
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
                MessageBox.Show(
                         "Заполните все обязательные поля!",
                         "Уведомление",
                         MessageBoxButton.OK,
                         MessageBoxImage.Warning
                     );
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
            int nextId = db.Schedules.Any() ? db.Schedules.Max(s => s.Idschedule) + 1 : 1;
            return new Schedule
            {
                Idschedule = nextId,
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

            try
            {
                var currentDate = StartDatePicker.SelectedDate.Value;
                var endDate = EndDatePicker.SelectedDate.Value;
                var groupId = schedule.GroupsIdgroup;

                // Adjust for .NET DayOfWeek (0=Sunday) vs your mapping (1=Monday)
                int targetDayOfWeek = dayOfWeekNumber % 7;

                var studentIds = db.GroupsUsers
                    .Where(gu => gu.GroupsIdgroups == groupId)
                    .Select(gu => gu.UsersIdusers)
                    .ToList();

                while (currentDate <= endDate)
                {
                    if ((int)currentDate.DayOfWeek == targetDayOfWeek - 1)
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
                        db.SaveChanges();
                    }
                    currentDate = currentDate.AddDays(1);
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isSaved)
                return; 

            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

    }
}
