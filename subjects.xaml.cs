using diplom.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
            catch
            {
                MessageBox.Show("Не получилось загрузить типы");
            }
        }

        private void LoadSubjects()
        {
            try
            {
                SubjectItems.Clear();
                subjectsFromView = db.SubjectShowItems.FromSqlRaw("select * from subjectsshow").ToList();

                foreach (var subject in subjectsFromView)
                {
                    SubjectItems.Add(subject);
                }
            }
            catch
            {
                MessageBox.Show("Не получилось загрузить занятия");
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                subjectsshow path = table.SelectedItem as subjectsshow;
                subject Subject = new subject(path.subject_id);
                Subject.Show();
            }
            catch
            {
                MessageBox.Show("Возникла неизвестная проблема. Пожалуйста, попробуйте позднее");
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                add_subject add_Subject = new add_subject();
                add_Subject.Closed += (s, args) => LoadSubjects();
                add_Subject.Show();
            }
            catch
            {
                MessageBox.Show("Возникла неизвестная проблема. Пожалуйста, попробуйте позднее");
            }

        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            try
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
            catch
            {
                MessageBox.Show("Возникла неизвестная проблема. Пожалуйста, попробуйте позднее");
            }

        }
    }
}
