using FileConverter.Commands;
using FileConverter.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        //DEBUG
        private bool exceptionWasThrown = false;
        private string buttonVisibility = "Visible";

        public string ButtonVisibility
        {
            get
            {
                if (this.buttonVisibility == null)
                {
                    this.buttonVisibility = "Visible";
                }
                return this.buttonVisibility;
            }
            set
            {
                this.buttonVisibility = value;
                this.OnPropertyChanged("ButtonVisibility");
            }
        }
        private string zielformatVisibility = "Hidden";

        public string ZielformatVisibility
        {
            get
            {
                if (this.zielformatVisibility == null)
                {
                    this.zielformatVisibility = "Visible";
                }
                return this.zielformatVisibility;
            }
            set
            {
                this.zielformatVisibility = value;
                this.OnPropertyChanged("ZielformatVisibility");
            }
        }
        private string infoTextVisibility;

        public string InfoTextVisibility
        {
            get
            {
                if (this.infoTextVisibility == null)
                {
                    this.infoTextVisibility = "Visible";
                }
                return this.infoTextVisibility;
            }
            set
            {
                this.zielformatVisibility = value;
                this.OnPropertyChanged("InfoTextVisibility");
            }
        }
        private string infoText;

        public string InfoText
        {
            get
            {
                if (this.infoText == null)
                {
                    this.infoText = string.Empty;
                }
                return this.infoText;
            }
            set
            {
                this.infoText = value;
                this.OnPropertyChanged("InfoText");
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
        // TODO Statusleiste einbausen?
        private void CommandConvert(object obj)
        {
            // TODO Grafik richtig dynamisieren
            ButtonVisibility = "Hidden";
            ZielformatVisibility = "Hidden";
            InfoTextVisibility = "Visible";
            InfoText = "Konvertierung läuft...";
            Model.Converter.Convert(filePaths, Formats.Current);
            InfoText = "Konvertierung abgeschlossen!";
        }

        private bool CanExecuteConvert(object arg)
        {
            if (Formats.Current != null)
            {
                return true;
            }
            return false;
        }
        
        public void CommandBrowse(object param)
        {
            // TODO Ausgabe konvertierungstart und -ende
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
            }
            // Default Wert löschen
            FileNames.Clear();
            try // Falls Auswahl der Datei abgebrochen wurde,... 
            {
                AddFiles(filePaths);
            }
            catch (Exception ex)
            {
                exceptionWasThrown = true;
                ZielformatVisibility = "Hidden";
                FileNames.Clear();
                // TODO Wenn kein Inhalt FileNames automatisch den Default
                FileNames.Add("Bitte wähle eine Datei aus.");
            }
            

        }

        public void AddFiles(string[] filePaths)
        {
            foreach (var file in filePaths)
            {
                FileNames.Add(Path.GetFileName(file));
                // TODO Erklärung?
                // Die erste Datei MUSS UMBEDINGT separat in eine Variable abgespeichert werden, damit diese wieder freigegeben wird. Wird im if durch FileNames.First() abgefragt, bleibt die erste datei zur Laufszeit des Programms gesperrt.
                string firstFile = FileNames.First();
                if (!Path.GetExtension(file).ToLower().Trim('.').Equals(Path.GetExtension(firstFile).ToLower().Trim('.'))) // Prüfen, ob alle Dateien das selbe Format besitzen
                {
                    // TODO mehr Info an den Benutzer nach Prüfung, zb die triggernden Dateien oder so, oder "png und jpg", oder so
                    FileNames.Clear();
                    // TODO Wenn kein Inhalt FileNames automatisch den Default
                    FileNames.Add("Bitte wähle eine Datei aus.");
                    MessageBox.Show("Deine ausgewählten Dateien haben unterschiedliche Formate.", "Unterschiedliche Formate");
                    ZielformatVisibility = "Hidden";
                    break;
                }
                // Zielformatwahl erscheinen lassen, soweit konvertierungsgeeignete Datei ausgewählt
                if (!Formats.Contains(Path.GetExtension(file).ToLower().Trim('.')))
                {
                    FileNames.Clear();
                    // TODO Wenn kein Inhalt FileNames automatisch den Default
                    FileNames.Add("Bitte wähle eine Datei aus.");
                    MessageBox.Show("Die Konvertierung eines der ausgewählten Dateienformate wird leider noch nicht unterstützt.", "Konvertierung nicht möglich");
                    ZielformatVisibility = "Hidden";
                    break;
                }
                if (!exceptionWasThrown)
                {
                    this.filePaths = filePaths;
                    ZielformatVisibility = "Visible";
                }
               
                
            }
        }

        public bool CanExecuteBrowse(object param)
        {
            return true;
        }
    }
}
