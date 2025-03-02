using diplom.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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


            LoadSubjects();
            DataContext = this;
            LoadTypes();
        }

        private void LoadTypes()
        {
            try
            {
                List<diplom.Models.Type> types = db.Types.ToList();
                TypeComboBox.ItemsSource = types;
                TypeComboBox.DisplayMemberPath = "Type1";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading types: {ex.Message}");
            }
        }

        private void LoadSubjects()
        {
            try
            {
                SubjectItems.Clear();
                subjectsFromView = db.SubjectShowItems.FromSqlRaw("select * from  subjectsshow").ToList();

                foreach (var subject in subjectsFromView)
                {
                    SubjectItems.Add(subject);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subjects: {ex.Message}");
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            subjectsshow path = table.SelectedItem as subjectsshow;
            if (path != null)
            {
                subject subjectPage = new subject(path.subject_id);
                NavigationService.Navigate(subjectPage);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
            window.Navigate(new Uri("add_subject.xaml", UriKind.Relative));

        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
    }
}
