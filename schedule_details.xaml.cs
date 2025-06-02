using Microsoft.EntityFrameworkCore;
using System.Windows;


namespace diplom
{
    public partial class schedule_details : Window
    {
        public DiplomSchoolContext db = new DiplomSchoolContext();
        public List<AttendanceViewModel> attendanceData;
        public class AttendanceViewModel
        {
            public int IdUser { get; set; }
            public string FIO { get; set; }
            public string Status { get; set; }
        }

        public schedule_details(int id)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            attendanceData = GetAttendanceData(id);
            table.ItemsSource = attendanceData;
            LoadData(id);
        }

        public void LoadData(int id)
        {
            try
            {
                var attendanceInfo = db.Attendances
                    .Include(a => a.IdscheduleNavigation)
                    .ThenInclude(s => s.GroupsIdgroupNavigation)
                    .FirstOrDefault(a => a.Idattendance == id);

                if (attendanceInfo != null)
                {
                    Attend.Text = "Дата: " + attendanceInfo.Date.ToString("dd.MM.yyyy");

                    Group.Text = "Группа: " +
                        (attendanceInfo.IdscheduleNavigation.GroupsIdgroupNavigation?.Name ?? "не указана");
                }
                else
                {
                    MessageBox.Show("Занятие не найдено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private List<AttendanceViewModel> GetAttendanceData(int attendanceId)
        {
            try
            {
                // Получаем id расписания из attendance
                var scheduleId = db.Attendances
                    .Where(a => a.Idattendance == attendanceId)
                    .Select(a => a.Idschedule)
                    .FirstOrDefault();

                if (scheduleId == 0)
                {
                    MessageBox.Show("Не найдено расписание для данного занятия");
                    return new List<AttendanceViewModel>();
                }

                // Получаем данные о студентах и их посещаемости
                var query = from u in db.Users
                            join gu in db.GroupsUsers on u.Idusers equals gu.UsersIdusers
                            join s in db.Schedules on gu.GroupsIdgroups equals s.GroupsIdgroup
                            join bn in db.BilNebils on new { Idattendance = attendanceId, Iduser = u.Idusers }
                                equals new { bn.Idattendance, bn.Iduser } into bns
                            from bn in bns.DefaultIfEmpty()
                            where s.Idschedule == scheduleId
                            select new AttendanceViewModel
                            {
                                IdUser = u.Idusers,
                                FIO = $"{u.Surname} {u.Name} {u.Paternity}",
                                Status = bn != null ? (bn.Status == true ? "Присутствовал" : "Отсутствовал") : "Не отмечен"
                            };

                return query.ToList();
            }
            catch
            {
                MessageBox.Show("Возникла неизвестная проблема. Пожалуйста, попробуйте позднее");
                return null;
            }
        }

    }
}
