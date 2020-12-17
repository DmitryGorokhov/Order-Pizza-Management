using Microsoft.Win32;
using System.Windows;
using Order_Pizza_Management.Views;
using System;

namespace Order_Pizza_Management.Utils
{
    class DialogService
    {
        public string FilePath { get; set; }
        public string PhoneNubmber { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }

        public bool SaveFileDialog(string name)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files(*.csv)|*.csv";
            saveFileDialog.FileName = name;
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

        public bool EnterOrderDataDialog()
        {
            EnterOrderDataWindow window = new EnterOrderDataWindow();
            if (window.ShowDialog() == true)
            {
                PhoneNubmber = window.PhoneNumber;
                Address = window.Address;
                return true;
            }
            else return false;
        }

        public bool LogIn()
        {
            LogInWindow window = new LogInWindow();
            if (window.ShowDialog() == true)
            {
                Password = window.Password;
                return true;
            }
            else return false;
        }
    }
}
