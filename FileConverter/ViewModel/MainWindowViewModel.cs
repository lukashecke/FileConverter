using FileConverter.Commands;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FileConverter.ViewModel
{

    public class MainWindowViewModel : ViewModelBase
    {
        private string filePath;
        public ICommand BrowseCommand
        {
            get; set;
        }

        public MainWindowViewModel()
        {
            this.BrowseCommand = new RelayCommand(CommandBrowse, CanExecuteBrowse);
        }
        public void CommandBrowse(object param)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // file = File.ReadAllText(openFileDialog.FileName);
                filePath = openFileDialog.FileName;
            }
            Model.Converter.Convert(filePath, "jpg");
        }


        public bool CanExecuteBrowse(object param)
        {
            return true;
        }
    }
}
