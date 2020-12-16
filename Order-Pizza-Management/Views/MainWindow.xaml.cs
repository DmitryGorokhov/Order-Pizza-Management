using System.Windows;
using System.Windows.Input;
using Order_Pizza_Management.ViewModels;

namespace Order_Pizza_Management
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                if (Window.WindowState == WindowState.Maximized)
                    Window.WindowState = WindowState.Normal;
                else Window.WindowState = WindowState.Maximized; 
            }
        }
    }
}
