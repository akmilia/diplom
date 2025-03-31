using diplom.Models;
using System.Windows;


namespace diplom
{

    public partial class user : Window
    {

        public DiplomSchoolContext db = new DiplomSchoolContext();
        public usersshow curUser;

        public user(usersshow u)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            curUser = u;
            DataContext = curUser;
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
                var userToDelete = db.Users.FirstOrDefault(u => u.Idusers == curUser.idusers);
                if (userToDelete != null)
                {
                    db.Users.Remove(userToDelete);
                    db.SaveChanges();
                    MessageBox.Show("Пользователь успешно удален.");
                    this.Close();
                }
            }
            catch
            {
                MessageBox.Show("Возникла неизвестная ошибка. Пожалуйста, попробуйте позднее");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    var userToUpdate = db.Users.FirstOrDefault(u => u.Idusers == curUser.idusers);
            //    if (userToUpdate != null)
            //    {
            //        userToUpdate.Login = curUser.login;
            //        userToUpdate.Password = curUser.password;
            //        userToUpdate.Surname =
            //        userToUpdate.Name =
            //        userToUpdate.Paternity =
            //        userToUpdate.RolesIdroles; 
            //        userToUpdate.Birthdate = curUser.datebirth;

            //        db.SaveChanges();
            //        MessageBox.Show("Пользователь успешно обновлен.");
            //        this.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка при обновлении пользователя: {ex.Message}");
            //}
        }
    }
}
