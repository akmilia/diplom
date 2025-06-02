//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

//namespace diplom.Components
//{
    
//    public partial class CustomMessageBox : Window
//    {
       
//        public enum MessageBoxType
//        {
//            Ошибка,
//            Предупреждение,
//            Информация
//        }

//        public CustomMessageBox(string message, string title, MessageBoxType type, MessageBoxButton buttons)
//        {
//            InitializeComponent();

//            txtTitle.Text = title;
//            txtMessage.Text = message;

//            // Настройка внешнего вида и иконки в зависимости от типа
//            switch (type)
//            {
//                case MessageBoxType.Ошибка:
//                    this.Title = "Ошибка";
//                    // Можно добавить красный цвет, иконку и т.п.
//                    txtTitle.Foreground = System.Windows.Media.Brushes.DarkRed;
//                    break;
//                case MessageBoxType.Предупреждение:
//                    this.Title = "Предупреждение";
//                    txtTitle.Foreground = System.Windows.Media.Brushes.DarkOrange;
//                    break;
//                case MessageBoxType.Информация:
//                    this.Title = "Информация";
//                    txtTitle.Foreground = System.Windows.Media.Brushes.DarkGreen;
//                    break;
//            }

//            // Настройка кнопок
//            switch (buttons)
//            {
//                case MessageBoxButton.OK:
//                    btnOk.Visibility = Visibility.Visible;
//                    btnCancel.Visibility = Visibility.Collapsed;
//                    btnClose.Visibility = Visibility.Collapsed;
//                    break;
//                case MessageBoxButton.OKCancel:
//                    btnOk.Visibility = Visibility.Visible;
//                    btnCancel.Visibility = Visibility.Visible;
//                    btnClose.Visibility = Visibility.Collapsed;
//                    break;
//                case MessageBoxButton.YesNoCancel:
//                    btnOk.Content = "Да";
//                    btnCancel.Content = "Нет";
//                    btnOk.Visibility = Visibility.Visible;
//                    btnCancel.Visibility = Visibility.Visible;
//                    btnClose.Visibility = Visibility.Visible;
//                    break;
//                default:
//                    btnOk.Visibility = Visibility.Visible;
//                    btnCancel.Visibility = Visibility.Collapsed;
//                    btnClose.Visibility = Visibility.Visible;
//                    break;
//            }
//        }

//        private void btnOk_Click(object sender, RoutedEventArgs e)
//        {
//            this.DialogResult = true;
//            this.Close();
//        }

//        private void btnCancel_Click(object sender, RoutedEventArgs e)
//        {
//            this.DialogResult = false;
//            this.Close();
//        }

//        private void btnClose_Click(object sender, RoutedEventArgs e)
//        {
//            this.DialogResult = null;
//            this.Close();
//        }
//    }
//}

