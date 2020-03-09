using FileConverter.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace FileConverter
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.DataContext = new MainWindowViewModel();
            InitializeComponent();
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Dateienliste aus vorherigem Durchlauf löschen
                ((MainWindowViewModel)this.DataContext).Files.FilePaths.Clear();

                string[] droppedFilePaths =
                e.Data.GetData(DataFormats.FileDrop, true) as string[];
               ((MainWindowViewModel)this.DataContext).AddFiles(droppedFilePaths);
            }
        }
    }
}

