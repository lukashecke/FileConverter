using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FileConverter.Model
{
    /// <summary>
    /// Konvertiert und speichert die Datei.
    /// </summary>
    static class Converter
    {
        private static string savingPath = $@"C:\Users\{Environment.UserName.ToString().ToLower()}\Desktop";
        /// <summary>
        /// Konvertiert BMP, GIF, GIF, PNG, oder TIFF zu einer PNG Datei.
        /// </summary>
       // TODO using wenn datei zum konvertieren benutzen sonst blockiert!!!!!
        public static void ImageToPng(string filePath)
        {
            /* Microsoft Do
             * Windows Presentation Foundation (WPF) systemeigene Unterstützung für die Komprimierung und die decokomprimierung von Images von 
             * Bitmap (BMP), 
             * Graphics Interchange Format (GIF), 
             * Joint Photographics Experts Group (GIF), 
             * Portable Network Graphics (PNG) und 
             * Tagged Image File Format (TIFF).
             */
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            // Create source.
            BitmapImage bi = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            string fullName = Path.Combine(savingPath, fileName + ".png");
            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bi));
                encoder.Save(fileStream);
            }
        }

        public static void ImageToJPG(string filePath)
        {
            /* Microsoft Do
             * Windows Presentation Foundation (WPF) systemeigene Unterstützung für die Komprimierung und die decokomprimierung von Images von 
             * Bitmap (BMP), 
             * Graphics Interchange Format (GIF), 
             * Joint Photographics Experts Group (GIF), 
             * Portable Network Graphics (PNG) und 
             * Tagged Image File Format (TIFF).
             */
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            // Create source.
            BitmapImage bi = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            string fullName = Path.Combine(savingPath, fileName + ".jpg");
            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bi));
                encoder.Save(fileStream);
            }
        }
    }
}
