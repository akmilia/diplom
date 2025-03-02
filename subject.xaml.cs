using diplom.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace diplom
{

    public partial class subject : Page
    {
        public DiplomSchoolContext db = new DiplomSchoolContext();
        subjectsshow constS = new subjectsshow();
        public int constID;
        public subject(int ID)
        {
            InitializeComponent();

            constID = ID;
            NewData();
        }

        public void NewData()
        {

            try
            {
                if (constID != 0)
                {
                    constS = db.SubjectShowItems
                       .FromSqlRaw($"SELECT * FROM subjectsshow WHERE subject_id = {constID}")
                       .FirstOrDefault();

                    if (constS != null)
                    {
                        Id.Text = constS.subject_id.ToString();
                        name.Text = constS.subject_name;
                        description.Text = constS.description;
                        IdType.Text = constS.type_id.ToString();
                        type.Text = constS.type_name;
                    }
                }
                else
                {
                    MessageBox.Show("Идентификатор предмета не задан.");
                }

            }
            catch
            {
                MessageBox.Show("Не передалось");
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
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить занятие?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
                {
                    var subjectToDelete = db.Subjects.Find(constS.subject_id);
                    if (subjectToDelete != null)
                    {
                        db.Subjects.Remove(subjectToDelete);
                        db.SaveChanges();
                        MessageBox.Show("Занятие успешно удалено.");

                        NavigationWindow window = (NavigationWindow)Application.Current.MainWindow;
                        window.Navigate(new Uri("subjects.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Занятие не найдено.");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Возникла ошибка. Попробуйте позднее");
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var subjectToUpdate = db.Subjects.Find(constS.subject_id);
                if (subjectToUpdate != null)
                {
                    subjectToUpdate.Name = name.Text;
                    subjectToUpdate.Description = description.Text;

                    db.SaveChanges();
                    MessageBox.Show("Изменения успешно сохранены.");
                }
                else
                {
                    MessageBox.Show("Занятие не найдено.");
                }
            }
            catch
            {
                MessageBox.Show("Возникла ошибка. Попробуйте позднее");
            }
        }
    }
}
