using System.Windows;


namespace Order_Pizza_Management.Views
{
    public partial class EnterOrderDataWindow : Window
    {
        public EnterOrderDataWindow()
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

        public string Address
        {
            get { return addressBox.Text; }
        }
        public string PhoneNumber
        {
            get { return numberBox.Text; }
        }
    }
}
