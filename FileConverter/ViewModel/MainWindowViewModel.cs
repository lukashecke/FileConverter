using FileConverter.Commands;
using FileConverter.Model;
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
        public ICommand ConvertCommand
        {
            get; set;
        }
        #region Entitäten
        private ObservableCollectionEx<string> formats = new ObservableCollectionEx<string>();
        public ObservableCollectionEx<string> Formats
        {
            get
            {
                if (this.formats == null)
                {
                    this.formats = new ObservableCollectionEx<string>();
                }
                return this.formats;
            }
            set
            {
                this.formats = value;
                this.OnPropertyChanged("Formats");
            }
        }
        private string visibility = "Hidden";
        public string Visibility
        {
            get
            {
                if (this.visibility == null)
                {
                    this.visibility = "Visible";
                }
                return this.visibility;
            }
            set
            {
                this.visibility = value;
                this.OnPropertyChanged("Visibility");
            }
        }
        private string fileName = "Bitte wähle eine Datei aus.";
        public string FileName
        {
            get
            {
                if (this.fileName == null)
                {
                    this.fileName = string.Empty;
                }
                return this.fileName;
            }
            set
            {
                this.fileName = value;
                this.OnPropertyChanged("FileName");
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            this.BrowseCommand = new RelayCommand(CommandBrowse, CanExecuteBrowse);
            this.ConvertCommand = new RelayCommand(CommandConvert, CanExecuteConvert);
            formats.Add("png");
            formats.Add("jpg");
            formats.Add("bmp");
            formats.Add("gif");
            formats.Add("tiff");
        }

        private void CommandConvert(object obj)
        {
            Model.Converter.Convert(filePath, Formats.Current);
        }

        private bool CanExecuteConvert(object arg)
        {
            if (Formats.Current != null)
            {
                return true;
            }
            return false;
        }

        // TODO using wenn datei zum konvertieren benutzen sonst blockiert!!!!!
        public void CommandBrowse(object param)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // file = File.ReadAllText(openFileDialog.FileName);
                filePath = openFileDialog.FileName;
            }

            // Zielformatwahl erscheinen lassen, soweit konvertierungsgeeignete Datei ausgewählt wurde
            if (!Formats.Contains(Path.GetExtension(filePath).ToLower().Trim('.')))
            {
                MessageBox.Show("Die Konvertierung des ausgewählten Dateienformats wird leider noch nicht unterstützt.", "Konvertierung nicht möglich");
            }
            else
            {
                Visibility = "Visible";
            }

            // Namen der Datei anzeigen
            FileName = Path.GetFileName(filePath);
        }
        public bool CanExecuteBrowse(object param)
        {
            return true;
        }
    }
}
