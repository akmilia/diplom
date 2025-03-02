using diplom.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace diplom
{

    public partial class add_subject : Page
    {
        public DiplomSchoolContext db = new DiplomSchoolContext();
        public add_subject()
        {
            InitializeComponent();
            LoadTypes();
        }
        private void LoadTypes()
        {
            try
            {
                List<diplom.Models.Type> types = db.Types.Where(u => u.Id != 0).ToList();


                TypeComboBox.ItemsSource = types;
                TypeComboBox.DisplayMemberPath = "Type1";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading types: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(SubjectNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a subject name.");
                    return;
                }

                var test = db.Subjects.OrderByDescending(s => s.Idsubjects).FirstOrDefault();

                if (TypeComboBox.SelectedItem is diplom.Models.Type selectedType)
                {

                    Subject newSubject = new Subject
                    {
                        Idsubjects = test.Idsubjects + 1,
                        Name = SubjectNameTextBox.Text,
                        Description = DescriptionTextBox.Text,

                    };

                    db.Subjects.Add(newSubject);
                    db.SaveChanges();
                    TypesSubject typesSubject = new TypesSubject
                    {
                        TypesId = selectedType.Id,
                        SubjectsIdsubjects = test.Idsubjects + 1,
                    };

                    db.TypesSubjects.Add(typesSubject);
                    db.SaveChanges();

                    MessageBox.Show("Subject added successfully!");

                }
                else
                {
                    MessageBox.Show("Please select a subject type.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving subject: {ex.Message}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
            window.Navigate(new Uri("subjects.xaml", UriKind.Relative));
        }
    }
}
