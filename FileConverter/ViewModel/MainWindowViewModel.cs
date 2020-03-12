using FileConverter.Commands;
using FileConverter.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FileConverter.ViewModel
{    
    public class MainWindowViewModel : ViewModelBase
    {
        #region fields
        private static readonly string savingPath = $@"C:\Users\{Environment.UserName.ToString().ToLower()}\Desktop\File Converter";
        private List<BackgroundWorker> backgroundWorkers = new List<BackgroundWorker>();
        #endregion

        #region properties
        public ICommand BrowseCommand { get; set; }
        public ICommand ConvertCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        #region entities
        private ObservableCollection<string> oFiles;
        public ObservableCollection<string> OFiles
        {
            get
            {
                if (this.oFiles == null)
                {
                    this.oFiles = new ObservableCollection<string>();
                }
                return this.oFiles;
            }
            set
            {
                this.oFiles = value;
                this.OnPropertyChanged("OFiles");
            }
        }
        private Files files;
        public Files Files
        {
            get
            {
                if (this.files == null)
                {
                    this.files = new Files();
                }
                return this.files;
            }
            set
            {
                this.files = value;
                this.OnPropertyChanged("Files");
            }
        }
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
        private string buttonVisibility = "Hidden";
        public string ButtonVisibility
        {
            get
            {
                if (this.buttonVisibility == null)
                {
                    this.buttonVisibility = "Hidden";
                }
                return this.buttonVisibility;
            }
            set
            {
                this.buttonVisibility = value;
                this.OnPropertyChanged("ButtonVisibility");
            }
        }
        private string newVisibility = "Hidden";
        public string NewVisibility
        {
            get
            {
                if (this.newVisibility == null)
                {
                    this.newVisibility = "Hidden";
                }
                return this.newVisibility;
            }
            set
            {
                this.newVisibility = value;
                this.OnPropertyChanged("NewVisibility");
            }
        }
        private string cancelVisibility = "Hidden";
        public string CancelVisibility
        {
            get
            {
                if (this.cancelVisibility == null)
                {
                    this.cancelVisibility = "Hidden";
                }
                return this.cancelVisibility;
            }
            set
            {
                this.cancelVisibility = value;
                this.OnPropertyChanged("CancelVisibility");
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

        private string infoText = $"Bitte wähle deine Dateien aus,\roder ziehe sie links rein.";
        public string InfoText
        {
            get
            {
                if (this.infoText == null)
                {
                    this.infoText = $"Bitte wähle deine Dateien aus,\roder ziehe sie links rein.";
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
                return this.comboBoxSelectedIndex;
            }
            set
            {
                if (value!=-1)
                {
                    ButtonVisibility = "Visible";
                    this.OnPropertyChanged(" ButtonVisibility");
                }
                this.comboBoxSelectedIndex = value;
                this.OnPropertyChanged("ComboBoxSelectedIndex");
            }
        }
        #endregion

        #region constructors
        public MainWindowViewModel()
        {
            Files = new Files();

            this.BrowseCommand = new RelayCommand(ExecuteBrowseCommand, CanExecuteBrowse);
            this.ConvertCommand = new RelayCommand(ExecuteConvertCommand, CanExecuteConvert);
            this.CancelCommand = new RelayCommand(ExecuteCancelCommand, CanExecuteCancel);
            this.NewCommand = new RelayCommand(ExecutNewCommand, CanExecuteNew);
            this.CloseCommand = new RelayCommand(ExecuteCloseCommand, CanExecuteClose);
            // Hier werden bislang die Möglichen Konvertierungsformate festgehalten
            formats.Add("png");
            formats.Add("jpg");
            formats.Add("bmp");
            formats.Add("gif");
            formats.Add("tiff");
        }
        #endregion

        #region commands
        private void ExecuteCloseCommand(object obj)
        {
            Application.Current.MainWindow.Close();
        }
        private void ExecutNewCommand(object obj)
        {
            ButtonVisibility = "Hidden";
            ZielformatVisibility = "Hidden";
            ConvertingProgress = 0;
            ConvertingFile = "";
            Files.FilePaths.Clear();
            OFiles = new ObservableCollection<string>(GetNames(Files.FilePaths));
            NewVisibility = "Hidden";
            InfoText = null; // auf Default zurücksetzen
        }
        public void ExecuteBrowseCommand(object param)
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
            if (gettedFilePaths.Count() > 0)
            {
                // Dateien vom vorherigen Durchlauf entfernen
                Files.FilePaths.Clear();
                AddFiles(gettedFilePaths);
            }
        }
        private void ExecuteConvertCommand(object obj)
        {
            backgroundWorkers.Clear(); // Nach einem Abbruch bleiben diese sonst auf altem Wert, was in der Schleife zu fehlern führen kann
            CreateSavingDirectory();
            Files.amountConvertedFiles = 0;
            Files.amountOfFiles = Files.FilePaths.Count;
            ButtonVisibility = "Hidden";
            ZielformatVisibility = "Hidden";
            InfoText = "Konvertierung läuft...";
            CancelVisibility = "Visible";

            string[] filePathsArray = Files.FilePaths.ToArray();
            for (int j = 0; j < Files.amountOfFiles; j++)
            {
                backgroundWorkers.Add(new BackgroundWorker()); // Muss passieren sonst bleiben die Backgroundworker null
            }

            int i = 0;
            foreach (var worker in backgroundWorkers) // Nicht parallel starten, weil jeder ein Element erhalten muss und eine parallele Iteration eins auslassen könnte
            {
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += worker_ConvertFile;
                worker.RunWorkerAsync(filePathsArray[i]);
                i++;
            }
        }
        private void ExecuteCancelCommand(object obj)
        {
            foreach (var worker in backgroundWorkers)
            {
                worker.CancelAsync();
            }
            InfoText = "Die Konvertierung wurde abgebrochen.";
            CancelVisibility = "Hidden";
            NewVisibility = "Visible";
            ComboBoxSelectedIndex = -1; // Um die Auswahl in der Kombobox für/ vor die nächste Auführung zu leeren
        }
        #endregion

        #region events
        private void worker_ConvertFile(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker; // Get the BackgroundWorker that raised this event.
            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
            string filePath = e.Argument as string;
            ConvertFileParallel(filePath);
            worker.RunWorkerCompleted += worker_ConvertingCompleted;
            Files.FilePaths.Remove(filePath);
        }
        private void worker_ConvertingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            backgroundWorkers.Remove(worker); // Array und removen
            worker.DoWork -= worker_ConvertFile;
            worker.RunWorkerCompleted -= worker_ConvertingCompleted;

            Files.amountConvertedFiles++;
            ConvertingProgress = (int)((Convert.ToDouble(Files.amountConvertedFiles) / Files.amountOfFiles) * 100); // ConvertingProgress muss Zahl zwischen 0 und 100 zurückgeben

            if (ConvertingProgress==100)
            {
                CancelVisibility = "Hidden";
                InfoText = "Konvertierung abgeschlossen.";
                NewVisibility = "Visible";
                ComboBoxSelectedIndex = -1; // Um die Auswahl in der Kombobox für/ vor die nächste Auführung zu leeren
            }
        }
        #endregion

        #region functions
        private void ConvertFileParallel(string file)
        {
            ConvertingFile = file;
            Converter converter = new Converter(file, Formats.Current, savingPath);
            converter.Convert();

            // TODO Frage: Wieso führt die Berechnung des ConvertProgress an dieser Stelle zu fehlern -> 99% bei vielen Dateien
            // ConvertingProgress = (int)((Convert.ToDouble(Files.amountConvertedFiles) / Files.amountOfFiles) * 100); // ConvertingProgress muss Zahl zwischen 0 und 100 zurückgeben
        }
        /// <summary>
        /// Gemeinsame Funktion für das hinzufügen von Dateien per Drag und Drop oder Browserauswahl
        /// </summary>
        /// <param name="filePaths"></param>
        public void AddFiles(string[] filePaths)
        {
            foreach (var file in filePaths)
            {
                if (Directory.Exists(file))
                {
                    SearchFiles(file);
                }
                else
                {
                    Files.FilePaths.Add(file);
                }
            }
            OFiles = new ObservableCollection<string>(GetNames(Files.FilePaths));
            if (CheckIfSameFormats(Files.FilePaths))
            {
                ZielformatVisibility = "Visible";
                InfoText = "";
                NewVisibility = "Hidden";
                ConvertingProgress = 0;
                ConvertingFile = "";
                if (!CheckIfSupportedFormats(Files.FilePaths))
                {
                    MessageBox.Show("Das gewählte Dateiformat wird leider noch nicht unterstützt.", "Nicht unterstützte Formate");
                    ZielformatVisibility = "Hidden";
                    InfoText = "Beachte bitte die Dokumentation.";
                    ButtonVisibility = "Hidden";
                    NewVisibility = "Visible";
                    Files.FilePaths.Clear();
                    ConvertingProgress = 0;
                    ConvertingFile = "";
                }
            }
            else
            {
                MessageBox.Show("Nicht alle deiner Dateien besitzen das selbe Format.", "Unterschiedliche Formate");
                ZielformatVisibility = "Hidden";
                InfoText = "Bitte überprüfe deine Dateien.";
                ButtonVisibility = "Hidden";
                NewVisibility = "Visible";
                Files.FilePaths.Clear();
                ConvertingProgress = 0;
                ConvertingFile = "";
            }
        }

        public void SearchFiles(string path)
        {
            DirectoryInfo ParentDirectory = new DirectoryInfo(path);
            // Searching Files in current Directory
            foreach (FileInfo fileInfo in ParentDirectory.GetFiles())
            {
                Files.FilePaths.Add(fileInfo.FullName);
            }
            // Searching Files in included Directories
            foreach (DirectoryInfo f in ParentDirectory.GetDirectories())
            {
                SearchFiles(f.FullName);
            }
        }
        #endregion

        #region auxiliary functions
        /// <summary>
        /// Gibt die Liste mit nur den Namen der Dateien zurück.
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        private List<string> GetNames(List<string> fileNames)
        {
            List<string> temp = new List<string>();
            foreach (var file in fileNames)
            {
                temp.Add(Path.GetFileName(file));
            }
            return temp;
        }
        private void CreateSavingDirectory()
        {
            Directory.CreateDirectory(savingPath);
        }
        /// <summary>
        /// Checks, message and returns if the formats of all the files are the same
        /// </summary>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        private bool CheckIfSameFormats(List<string> filePaths)
        {
            foreach (var file in filePaths)
            {
                // TODO Erklärung: Zur Laufzeit werden irgendwo noch Dateien blockiert! Auch bei der zweiten Überpfrüfungsmethode? Halbes Workaround: Die erste Datei MUSS UMBEDINGT separat in eine Variable abgespeichert werden, damit diese wieder freigegeben wird. Wird im if durch FileNames.First() abgefragt, bleibt die erste datei zur Laufszeit des Programms gesperrt.
                string firstFile = Files.FilePaths.First();
                if (!Path.GetExtension(file).ToLower().Trim('.').Equals(Path.GetExtension(firstFile).ToLower().Trim('.'))) // Prüfen, ob alle Dateien das selbe Format besitzen
                {
                    return false;
                }
            }
            return true;
        }
        private bool CheckIfSupportedFormats(List<string> filePaths)
        {string firstFile = Files.FilePaths.First();
            if (Formats.Contains(Path.GetExtension(firstFile).ToLower().Trim('.')))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region canExecutes
        private bool CanExecuteCancel(object arg)
        {
            return true;
        }
        private bool CanExecuteConvert(object arg)
        {
            return true;
        }
        public bool CanExecuteBrowse(object param)
        {
            return true;
        }
        private bool CanExecuteNew(object arg)
        {
            return true;
        }
        private bool CanExecuteClose(object arg)
        {
            return true;
        }
        #endregion
    }
}
