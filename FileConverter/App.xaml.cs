using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileConverter
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Einstiegspunkt des Programms
                Window mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // Unbehandelte Exceptions abfangen
                MessageBox.Show("Programm konnte aufgrund einer unbehandelten Exception nicht gestartet werden!"+Environment.NewLine+ex.Message, "Absturz");
            }
        }
    }
}
