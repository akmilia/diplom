using System.Windows.Controls;

namespace diplom
{
    public partial class admin : Page
    {
        public admin()
        {
            InitializeComponent();
            SetGreeting();
        }

        private void SetGreeting()
        {
            DateTime currentTime = DateTime.Now;
            string greeting;

            if (currentTime.Hour >= 5 && currentTime.Hour < 12)
            {
                greeting = "Доброе утро, Администратор!";
            }
            else if (currentTime.Hour >= 12 && currentTime.Hour < 17)
            {
                greeting = "Добрый день, Администратор!";
            }
            else if (currentTime.Hour >= 17 && currentTime.Hour < 22)
            {
                greeting = "Добрый вечер, Администратор!";
            }
            else
            {
                greeting = "Доброй ночи, Администратор!";
            }

            GreetingTextBlock.Text = greeting;
        }
    }
}
