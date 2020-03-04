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
        private static string savingPath = $@"C:\Users\{Environment.UserName.ToString().ToLower()}\Desktop\File Converter";
        private List<string> filePaths;
        private int amountConvertedFiles;
        private int amountOfFiles;
        private List<BackgroundWorker> backgroundWorkers = new List<BackgroundWorker>();
        public ICommand BrowseCommand
        {
            get; set;
        }
        public ICommand ConvertCommand
        {
            get; set;
        }
        public ICommand CancelCommand
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
        private string infoText = "Bitte wähle deine Dateien aus.";
        public string InfoText
        {
            get
            {
                if (this.infoText == null)
                {
                    this.infoText = "Bitte wähle deine Dateien aus.";
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
                this.comboBoxSelectedIndex = value;
                this.OnPropertyChanged("ComboBoxSelectedIndex");
            }
        }
        private ObservableCollection<string> fileNames = new ObservableCollection<string>() { "Hier reinziehen möglich." };
        public ObservableCollection<string> FileNames
        {
            get
            {
                if (this.fileNames == null)
                {
                    this.fileNames = new ObservableCollection<string>() { "Hier reinziehen möglich." };
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
            this.BrowseCommand = new RelayCommand(ExecuteBrowseCommand, CanExecuteBrowse);
            this.ConvertCommand = new RelayCommand(ExecuteConvertCommand, CanExecuteConvert);
            this.CancelCommand = new RelayCommand(ExecuteCancelCommand, CanExecuteCancel);
            formats.Add("png");
            formats.Add("jpg");
            formats.Add("bmp");
            formats.Add("gif");
            formats.Add("tiff");
        }
        private void ExecuteCancelCommand(object obj)
        {
            // TODO funktioniert noch nicht so ganz, oder doch? => Multithreading...
            foreach (var worker in backgroundWorkers)
            {
                worker.CancelAsync();
            }
            InfoText = "Die Konvertierung wurde abgebrochen";
            CancelVisibility = "Hidden";
            ComboBoxSelectedIndex = -1; // Um die Auswahl in der Kombobox für/ vor die nächste Auführung zu leeren
        }

        private bool CanExecuteCancel(object arg)
        {
            return true;
        }

        private void ExecuteConvertCommand(object obj)
        {
            backgroundWorkers.Clear(); // Nach einem Abbruch bleiben diese sonst auf altem Wert, was in der Schleife zu fehlern führen kann
            CreateSavingDirectory();
            amountConvertedFiles = 0;
            amountOfFiles = filePaths.Count;
            ButtonVisibility = "Hidden";
            ZielformatVisibility = "Hidden";
            InfoText = "Konvertierung läuft...";
            CancelVisibility = "Visible";


            string[] filePathsArray = filePaths.ToArray();

            // Früher einen BackgroundWorker gestartet, der DoWorkSerial ausführt
            // Muss passieren sonst bleiben die Backgroundworker null
            for (int j = 0; j < amountOfFiles; j++)
            {
                backgroundWorkers.Add(new BackgroundWorker());// BackgroundWorker quasi grafische Threads // liste aus backgroundworkern für jede konvertierung 1
            }

            int i = 0;
            foreach (var worker in backgroundWorkers) // Nicht parallel starten, weil jeder ein Element erhalten muss und eine parallele Iteration eins auslassen könnte
            {
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += worker_DoWorkParallel;
                worker.RunWorkerAsync(filePathsArray[i]); // löst do_work Event aus
                i++;
            }
        }
        private void worker_DoWorkParallel(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker; // Get the BackgroundWorker that raised this event.
            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
            string filePath = e.Argument as string; // TODO was genau ist diese as? Cast von LINQ?
            ConvertFileParallel(filePath);
            // worker.ProgressChanged += worker_ProgressChanged; mach ich noch in der Convert Methode
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            this.filePaths.Remove(filePath);
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            backgroundWorkers.Remove(worker); // Array und removen
            worker.DoWork -= worker_DoWorkParallel;
            worker.RunWorkerCompleted -= worker_RunWorkerCompleted;

            if (backgroundWorkers.Count() == 0) // Wait for all backgroundworker, then...
            {
                CancelVisibility = "Hidden";
                InfoText = "Konvertierung abgeschlossen!";
                ComboBoxSelectedIndex = -1; // Um die Auswahl in der Kombobox für/ vor die nächste Auführung zu leeren
            }
        }
        private void ConvertFileParallel(string file)
        {
            ConvertingFile = file;
            Converter converter = new Converter(file, Formats.Current, savingPath);
            converter.Convert(); // TODO Ändern!
            amountConvertedFiles++;
            //  ConvertingProgress muss Zahl zwischen 0 und 100 zurückgeben
            ConvertingProgress = (int)((Convert.ToDouble(amountConvertedFiles) / amountOfFiles) * 100);
        }

        private void CreateSavingDirectory()
        {
            Directory.CreateDirectory(savingPath);
        }

        private bool CanExecuteConvert(object arg)
        {
            if (Formats.Current != null)
            {
                return true;
            }
            return false;
        }

        public void ExecuteBrowseCommand(object param)
        {
            //Task t = new Task(() =>
            //{
            //    InfoText = "Dateien werden geladen...";
            //});
            //t.Start();
            //Task.WaitAll(t);
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
                // Wenn Dateien ausgewählt wurden soll der Defaultwert gelöscht werden
                FileNames.Clear();
                // TODO Text wird noch nicht aktualisiert
                InfoText = "Dateien werden geladen...";
                AddFiles(gettedFilePaths);
            }
            //// TODO "Dateien werden geladen..."
            //BackgroundWorker worker = new BackgroundWorker();
            //worker.DoWork += worker_executeBrowse;
            //worker.RunWorkerAsync();
            //InfoText = "Dateien werden geladen...";

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
                // Da nach erster Ausführung auf "Visible"
                InfoText = "";
                ZielformatVisibility = "Visible";
                ButtonVisibility = "Visible";
                this.filePaths = filePaths.ToList<string>();
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
