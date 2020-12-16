using System.Windows;


namespace Order_Pizza_Management.Views
{
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Password
        {
            get { return passwordBox.Password; }
        }
    }
}
