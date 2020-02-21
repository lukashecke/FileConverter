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
        private static BitmapEncoder encoder;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="format"></param>
        public static void Convert(string[] filePaths, string format)
        {
            /* Microsoft Dokumentation
             * Windows Presentation Foundation (WPF) systemeigene Unterstützung für die Komprimierung und die decokomprimierung von Images von 
             * Bitmap (BMP), 
             * Graphics Interchange Format (GIF), 
             * Joint Photographics Experts Group (JIF), 
             * Portable Network Graphics (PNG) und 
             * Tagged Image File Format (TIFF).
             */
            foreach (var file in filePaths)
            {
                // TODO using wenn datei zum konvertieren benutzen sonst blockiert!!!!!
                string fileName = Path.GetFileNameWithoutExtension(file);
                string fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());

                BitmapImage bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(file, UriKind.RelativeOrAbsolute);
                bi.EndInit();




                switch (format.ToLower())
                {
                    case "jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case "png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case "bmp":
                        encoder = new BmpBitmapEncoder();
                        break;
                    case "gif":
                        encoder = new GifBitmapEncoder();
                        break;
                    case "tiff":
                        encoder = new TiffBitmapEncoder();
                        break;
                    default:
                        break;
                }
                using (var fileStream = new FileStream(fullName, FileMode.Create))
                {
                    encoder.Frames.Add(BitmapFrame.Create(bi));
                    encoder.Save(fileStream);
                }
            }
        }
    }
}
