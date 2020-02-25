using FileConverter.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
                ((MainWindowViewModel)this.DataContext).FileNames.Clear();

                string[] droppedFilePaths =
                e.Data.GetData(DataFormats.FileDrop, true) as string[];

                List<string> temp = new List<string>();
                foreach (var path in droppedFilePaths)
                {
                    if (Directory.Exists(path))
                    {
                        // Übergebene Ordner werden ausgelesen
                        temp.AddRange(Directory.GetFiles(path));
                    }
                    else
                    {
                        temp.Add(path);
                    }
                }
                ((MainWindowViewModel)this.DataContext).AddFiles(temp.ToArray());
            }
        }
    }
}

