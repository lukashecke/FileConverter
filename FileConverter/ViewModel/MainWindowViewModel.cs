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
        private string[] filePaths;
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
        private ObservableCollection<string> fileNames = new ObservableCollection<string>() { "Bitte wähle eine Datei aus." };
        public ObservableCollection<string> FileNames
        {
            get
            {
                if (this.fileNames == null)
                {
                    this.fileNames = new ObservableCollection<string>();
                }
                return this.fileNames;
            }
            set
            {
                this.fileNames = value;
                this.OnPropertyChanged("FileNames");
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
            Model.Converter.Convert(filePaths, Formats.Current);
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
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                // Array mit der richtigen Länge deklarieren
                filePaths = new string[openFileDialog.FileNames.Length];
                int j = 0;
                foreach (string file in openFileDialog.FileNames)
                {
                    filePaths[j] = file;
                    j++;

                }
                // file = File.ReadAllText(openFileDialog.FileName);
            }
            // Default Wert löschen
            FileNames.Clear();
            try // Falls Auswahl der Datei abgebrochen wird,... 
            {

                foreach (var file in filePaths)
                {

                    // Namen der Datei anzeigen
                    FileNames.Add(Path.GetFileName(file));

                    // TODO mehr Info an den Benutzer nach Prüfung, zb die triggernden Dateien oder so, oder "png und jpg", oder so

                    // Prüfen, ob alle Dateien das selbe Format besitzen
                    if (!Path.GetExtension(file).ToLower().Trim('.').Equals(Path.GetExtension(FileNames.First()).ToLower().Trim('.')))
                    {
                        FileNames.Clear();
                        // TODO Wenn kein Inhalt FileNames automatisch den Default
                        FileNames.Add("Bitte wähle eine Datei aus.");
                        MessageBox.Show("Deine ausgewählten Dateien haben unterschiedliche Formate.", "Unterschiedliche Formate");
                        Visibility = "Hidden";
                        break;
                    }
                    // Zielformatwahl erscheinen lassen, soweit konvertierungsgeeignete Datei ausgewählt
                    if (!Formats.Contains(Path.GetExtension(file).ToLower().Trim('.')))
                    {
                        FileNames.Clear();
                        // TODO Wenn kein Inhalt FileNames automatisch den Default
                        FileNames.Add("Bitte wähle eine Datei aus.");
                        MessageBox.Show("Die Konvertierung eines der ausgewählten Dateienformate wird leider noch nicht unterstützt.", "Konvertierung nicht möglich");
                        Visibility = "Hidden";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // ... ist das Programm automatisch auf der Startseite
            }

            Visibility = "Visible";
        }
        public bool CanExecuteBrowse(object param)
        {
            return true;
        }
    }
}
