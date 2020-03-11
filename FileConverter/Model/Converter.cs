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
    public class Converter
    { 
        delegate void ConvertionHandler();
        private ConvertionHandler convertionHandler;
        private string filePath;
        private string format;
        private string savingPath;

        public Converter(string filePath, string format, string savingPath)
        {
            this.filePath = filePath;
            this.format = format;
            this.savingPath = savingPath;
            switch (format.ToLower())
            {
                case "jpg":
                    convertionHandler = ConvertToJpg;
                    break;
                case "png":
                    convertionHandler = ConvertToPng;
                    break;
                case "bmp":
                    convertionHandler = ConvertToBmp;
                    break;
                case "gif":
                    convertionHandler = ConvertToGif;
                    break;
                case "tiff":
                    convertionHandler = ConvertToTiff;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="format"></param>
        public void Convert()
        {
            /* Microsoft Dokumentation
             * Windows Presentation Foundation (WPF) systemeigene Unterstützung für die Komprimierung und die decokomprimierung von Images von 
             * Bitmap (BMP), 
             * Graphics Interchange Format (GIF), 
             * Joint Photographics Experts Group (JIF), 
             * Portable Network Graphics (PNG) und 
             * Tagged Image File Format (TIFF).
             */

            convertionHandler();
        }
        private void ConvertToJpg()
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            BitmapImage bi = new BitmapImage();
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                jpegBitmapEncoder.Save(fileStream);
            }
        }
        private void ConvertToPng()
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            BitmapImage bi = new BitmapImage();
            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                pngBitmapEncoder.Save(fileStream);
            }
        }
        private void ConvertToBmp()
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            BitmapImage bi = new BitmapImage();
            BmpBitmapEncoder bmpBitmapEncoder = new BmpBitmapEncoder();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                bmpBitmapEncoder.Save(fileStream);
            }
        }
        private void ConvertToGif()
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            BitmapImage bi = new BitmapImage();
            GifBitmapEncoder gifBitmapEncoder = new GifBitmapEncoder();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                gifBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                gifBitmapEncoder.Save(fileStream);
            }
        }
        private void ConvertToTiff()
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            TiffBitmapEncoder tiffBitmapEncoder = new TiffBitmapEncoder();
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            using (var fileStream = new FileStream(fullName, FileMode.Create))
            {
                tiffBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                tiffBitmapEncoder.Save(fileStream);
            }
        }
    }
}
