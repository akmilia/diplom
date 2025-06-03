using System.Windows.Controls;

namespace diplom.Components
{
    public partial class AlertDialog : UserControl
    {
        public AlertDialog(string message, string title)
        {
            InitializeComponent();
            DataContext = new { Message = message, Title = title };
        }
    }
}
