using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Order_Pizza_Management
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Clear();
            ContentGrid.Children.Add(new UserControlNewOrder());
        }

        private void ContolButton_Click(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Clear();
            // ContentGrid.Children.Add(new UserControlManage());
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Clear();
            //ContentGrid.Children.Add(new UserControlReport());
        }
    }
}
