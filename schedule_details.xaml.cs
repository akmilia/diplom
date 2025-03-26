using diplom.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;


namespace diplom
{   

    public partial class schedule_details : Window
    {


        public DiplomSchoolContext db = new DiplomSchoolContext();
        public BilNebil curDate;

        public schedule_details(int id)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            curDate = db.BilNebils.FirstOrDefault(b=> b.Idattendance == id);
            DataContext = curDate;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

    }
}
