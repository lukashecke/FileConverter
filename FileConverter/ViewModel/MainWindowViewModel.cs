using FileConverter.Commands;
using FileConverter.Model;
using Microsoft.Win32;
using System;
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
        private string[] filePaths;
        private int amountConvertedFiles;
        private int amountOfFiles;
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
            this.ConvertCommand = new RelayCommand(ExecuteConvertCommandAsync, CanExecuteConvert);
            formats.Add("png");
            formats.Add("jpg");
            formats.Add("bmp");
            formats.Add("gif");
            formats.Add("tiff");
        }

        // TODO Fragen: Soll das Command überhaupt async enden und was funktioniert hier genau wie
        private void ExecuteConvertCommandAsync(object obj)
        {
            CreateSavingDirectory();
            amountConvertedFiles = 0;
            amountOfFiles = filePaths.Length;
            ButtonVisibility = "Hidden";
            ZielformatVisibility = "Hidden";
            InfoText = "Konvertierung läuft...";

            BackgroundWorker worker = new BackgroundWorker();
            // TODO Wo melde ich dieses Ereignis manuell ab? Garbage Collection macht das automatisch?
            worker.DoWork += worker_DoWorkParallel;
            //worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void worker_DoWorkParallel(object sender, DoWorkEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            string[] filePathsArray = filePaths.ToArray();
            Task[] tasks = new Task[amountOfFiles];
            for (int i = 0; i < amountOfFiles; i++)
            {
                // Die Dauer der Konvertierung hat hier kaum Einfluss auf die Laufzeit
                tasks[i] = ConvertFile(filePathsArray[i]);
                //tasks[i].Wait(50);
            }
            Task.WaitAll(tasks);
            //sw.Stop();
            InfoText = "Konvertierung abgeschlossen!";
            // Um die Auswahl in der Kombobox für/ vor die nächste Auführung zu leeren
            ComboBoxSelectedIndex = -1;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            foreach (var file in filePaths)
            {
                ConvertingFile = file;
                Converter.ConvertAsync(file, Formats.Current, savingPath);
                // Die Dauer der Konvertierung hat hier extremen Einfluss auf die Laufzeit
                // Thread.Sleep(50);
                amountConvertedFiles++;
                //  ConvertingProgress muss Zahl zwischen 0 und 100 zurückgeben
                ConvertingProgress = (int)((Convert.ToDouble(amountConvertedFiles) / amountOfFiles) * 100);
            }
            //sw.Stop();
            InfoText = "Konvertierung abgeschlossen!";
        }
        private async Task ConvertFile(string file)
        {
            ConvertingFile = file;
            await Converter.ConvertAsync(file, Formats.Current, savingPath);
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
                AddFiles(gettedFilePaths);
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
                // Da nach erster Ausführung auf "Visible"
                InfoText = "";
                ZielformatVisibility = "Visible";
                ButtonVisibility = "Visible";
                this.filePaths = filePaths;
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
