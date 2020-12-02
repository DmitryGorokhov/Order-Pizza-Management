using Microsoft.Win32;
using System.Windows;
using Order_Pizza_Management.Views;

namespace Order_Pizza_Management.Utils
{
    class DialogService
    {
        public string FilePath { get; set; }

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public bool AcceptionDialog()
        {
            AcceptWindow window = new AcceptWindow();
            return (window.ShowDialog() == true) ? true : false;
        }
    }
}
