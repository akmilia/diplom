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

        private void LoadSchedule()
        {
            try
            {
                var schedules = db.schedulesShow
                                .AsEnumerable() // This executes the SQL query
                                .ToList();


                var monday = schedules.Where(s => s.day_of_week == "Понедельник").ToList();
                var tuesday = schedules.Where(s => s.day_of_week == "Вторник").ToList();
                var wednesday = schedules.Where(s => s.day_of_week == "Среда").ToList();
                var thursday = schedules.Where(s => s.day_of_week == "Четверг").ToList();
                var friday = schedules.Where(s => s.day_of_week == "Пятница").ToList();
                var saturday = schedules.Where(s => s.day_of_week == "Суббота").ToList();

                MondayListBox.ItemsSource = monday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
                TuesdayListBox.ItemsSource = tuesday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
                WednesdayListBox.ItemsSource = wednesday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
                ThursdayListBox.ItemsSource = thursday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
                FridayListBox.ItemsSource = friday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");
                SaturdayListBox.ItemsSource = saturday.Select(s => $"{s.time.ToString("HH:mm")} - {s.subject_name} ({s.teacher}) ({s.cabinet})");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
    }
}
