using System.Windows;

namespace Order_Pizza_Management.Views
{
    public partial class AcceptWindow : Window
    {
        public AcceptWindow()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
