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
using System.Windows.Threading;

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
        private string convertingFile;
        public string ConvertingFile
        {
            get
            {
                if (this.convertingFile == null)
                {
                    this.convertingFile = string.Empty;
                }
                return this.convertingFile;
            }
            set
            {
                this.convertingFile = value;
                this.OnPropertyChanged("ConvertingFile");
            }
        }
        private int convertingProgress;
        public int ConvertingProgress
        {
            get
            {
                if (this.convertingProgress == null)
                {
                    this.convertingProgress = 0;
                }
                return this.convertingProgress;
            }
            set
            {
                this.convertingProgress = value;
                this.OnPropertyChanged("ConvertingProgress");
            }
        }
        private int comboBoxSelectedIndex = -1;

        public int ComboBoxSelectedIndex
        {
            get
            {
                if (this.comboBoxSelectedIndex == null)
                {
                    this.comboBoxSelectedIndex = -1;
                }
                return this.comboBoxSelectedIndex;
            }
            set
            {
                this.comboBoxSelectedIndex = value;
                this.OnPropertyChanged("ComboBoxSelectedIndex");
            }
        }
        private ObservableCollection<string> fileNames = new ObservableCollection<string>() { "Bitte wähle eine Datei aus." };
        public ObservableCollection<string> FileNames
        {
            get
            {
                if (this.fileNames == null)
                {
                    this.fileNames = new ObservableCollection<string>() { "Bitte wähle eine Datei aus." };
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
            // TODO Infotext richtig dynamisieren (Kovertierungstart und -ende dynamisch für Benutzer ausgeben), bzw. Statusleiste einbauen?
            ButtonVisibility = "Hidden";
            ZielformatVisibility = "Hidden";
            InfoText = "Konvertierung läuft...";
            int amountConvertedFiles = 0;
            int amountOfFiles = filePaths.Length;
            foreach (var file in filePaths)
            {
                ConvertingFile = file;
                Model.Converter.Convert(file, Formats.Current);
                amountConvertedFiles++;
                //  ConvertingProgress muss Zahl zwischen 0 und 100 zurückgeben
                ConvertingProgress = (int)((Convert.ToDouble(amountConvertedFiles) / amountOfFiles) * 100);
                // Manuelles UI-Refresh
                EnforceUIUpdate();
            }
            InfoText = "Konvertierung abgeschlossen!";
            // Um die Auswahl in der Kombobox für/ vor die nächste Auführung zu leeren
            ComboBoxSelectedIndex = -1;
        }

        void EnforceUIUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
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
            string[] gettedFilePaths = new string[0];
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                // Array mit der richtigen Länge deklarieren
                gettedFilePaths = new string[openFileDialog.FileNames.Length];
                int j = 0;
                foreach (string file in openFileDialog.FileNames)
                {
                    gettedFilePaths[j] = file;
                    j++;
                }
            }
            // FileNames auf Default setzen lassen
            FileNames = null;
            try
            {
                if (gettedFilePaths.Count() > 0)
                {
                    // Wenn Dateien ausgewählt wurden soll der Defaultwert gelöscht werden
                    FileNames.Clear();
                }
                AddFiles(gettedFilePaths);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Programmfehler");
                exceptionWasThrown = true;
                ZielformatVisibility = "Hidden";
            }
            finally
            {
                // Statusleiste mit zurücksetzen
                ConvertingFile = "";
                ConvertingProgress = 0;
            }
        }
        /// <summary>
        /// Gemeinsame Funktion für das hinzufügen von Dateien per Drag und Drop oder Browserauswahl
        /// </summary>
        /// <param name="filePaths"></param>
        public void AddFiles(string[] filePaths)
        {
            // Die ListView soll gefüllt werden, auch wenn die Konvertierung nicht stattfinden kann, damit der Benutzer seine Eingabe überprüfen kann
            foreach (var file in filePaths)
            {
                FileNames.Add(Path.GetFileName(file));
                // Statusleiste mit zurücksetzen
                ConvertingFile = "";
                ConvertingProgress = 0;
            }
            foreach (var file in filePaths)
            {
                bool allSameFormat = false;
                allSameFormat = CheckIfSameFormats(filePaths);
                if (!allSameFormat)
                {
                    break;
                }

                // Zielformatwahl erscheinen lassen, soweit konvertierungsgeeignete Dateien ausgewählt
                if (!Formats.Contains(Path.GetExtension(file).ToLower().Trim('.')))
                {
                    MessageBox.Show("Die Konvertierung des ausgewählten Dateiformats wird leider noch nicht unterstützt.", "Konvertierung nicht möglich");
                    ZielformatVisibility = "Hidden";
                    break;
                }
                if (!exceptionWasThrown)
                {
                    // Da nach erster Ausführung auf "Visible"
                    InfoText = "";
                    ZielformatVisibility = "Visible";
                    ButtonVisibility = "Visible";
                    this.filePaths = filePaths;
                }
            }
        }
        /// <summary>
        /// Checks, message and returns if the formats of all the files are the same
        /// </summary>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        private bool CheckIfSameFormats(string[] filePaths)
        {
            foreach (var file in filePaths)
            {
                // TODO Zur Laufzeit werden irgendwo noch Dateien blockiert!
                // TODO Erklärung?
                // Die erste Datei MUSS UMBEDINGT separat in eine Variable abgespeichert werden, damit diese wieder freigegeben wird. Wird im if durch FileNames.First() abgefragt, bleibt die erste datei zur Laufszeit des Programms gesperrt.
                string firstFile = FileNames.First();
                if (!Path.GetExtension(file).ToLower().Trim('.').Equals(Path.GetExtension(firstFile).ToLower().Trim('.'))) // Prüfen, ob alle Dateien das selbe Format besitzen
                {
                    MessageBox.Show("Nicht alle deiner Dateien besitzen das selbe Format.", "Unterschiedliche Formate");
                    ZielformatVisibility = "Hidden";
                    return false;
                }
            }
            return true;
        }

        public bool CanExecuteBrowse(object param)
        {
            return true;
        }
    }
}
