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
        delegate void ConvertionHandler(int counter);
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
        public void Convert(int counter=1)
        {
            /* Microsoft Dokumentation
             * Windows Presentation Foundation (WPF) systemeigene Unterstützung für die Komprimierung und die decokomprimierung von Images von 
             * Bitmap (BMP), 
             * Graphics Interchange Format (GIF), 
             * Joint Photographics Experts Group (JIF), 
             * Portable Network Graphics (PNG) und 
             * Tagged Image File Format (TIFF).
             */

            convertionHandler(counter);
        }
        private void ConvertToJpg(int counter=1)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName;
            if (counter == 0)
            {
                fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            }
            else
            {
                fullName = Path.Combine(savingPath, fileName + "(" + counter + ")." + format.ToLower());

            }
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            try
            {
                using (var fileStream = new FileStream(fullName, FileMode.Create))
                {
                    jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                    jpegBitmapEncoder.Save(fileStream);

                }
            }
            catch (IOException ex) // Wird geworfen, wenn Datei aus anderem Format, jedoch mit selbem Dateinamen gleichzeitig von einem anderen Thread konvertiert bzw. erzeugt wird
            {
                ConvertToTiff(counter++);
            }
        }
        private void ConvertToPng(int counter=1)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName;
            if (counter == 0)
            {
                fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            }
            else
            {
                fullName = Path.Combine(savingPath, fileName + "(" + counter + ")." + format.ToLower());

            }
            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            try
            {
                using (var fileStream = new FileStream(fullName, FileMode.Create))
                {
                    pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                    pngBitmapEncoder.Save(fileStream);

                }
            }
            catch (IOException ex) // Wird geworfen, wenn Datei aus anderem Format, jedoch mit selbem Dateinamen gleichzeitig von einem anderen Thread konvertiert bzw. erzeugt wird
            {
                ConvertToTiff(counter++);
            }
        }
        private void ConvertToBmp(int counter=1)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName;
            if (counter == 0)
            {
                fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            }
            else
            {
                fullName = Path.Combine(savingPath, fileName + "(" + counter + ")." + format.ToLower());

            }
            BmpBitmapEncoder bmpBitmapEncoder = new BmpBitmapEncoder();
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            try
            {
                using (var fileStream = new FileStream(fullName, FileMode.Create))
                {
                    bmpBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                    bmpBitmapEncoder.Save(fileStream);

                }
            }
            catch (IOException ex) // Wird geworfen, wenn Datei aus anderem Format, jedoch mit selbem Dateinamen gleichzeitig von einem anderen Thread konvertiert bzw. erzeugt wird
            {
                ConvertToTiff(counter++);
            }
        }
        private void ConvertToGif(int counter=1)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName;
            if (counter == 0)
            {
                fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            }
            else
            {
                fullName = Path.Combine(savingPath, fileName + "(" + counter + ")." + format.ToLower());

            }
            GifBitmapEncoder gifBitmapEncoder = new GifBitmapEncoder();
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            try
            {
                using (var fileStream = new FileStream(fullName, FileMode.Create))
                {
                    gifBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                    gifBitmapEncoder.Save(fileStream);

                }
            }
            catch (IOException ex) // Wird geworfen, wenn Datei aus anderem Format, jedoch mit selbem Dateinamen gleichzeitig von einem anderen Thread konvertiert bzw. erzeugt wird
            {
                ConvertToTiff(counter++);
            }
        }
        private void ConvertToTiff(int counter=1)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fullName;
            if (counter == 1)
            {
                fullName = Path.Combine(savingPath, fileName + "." + format.ToLower());
            }
            else
            {
                fullName = Path.Combine(savingPath, fileName + "(" + counter + ")." + format.ToLower());

            }
            TiffBitmapEncoder tiffBitmapEncoder = new TiffBitmapEncoder();
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bi.EndInit();
            try
            {
                using (var fileStream = new FileStream(fullName, FileMode.Create))
                {
                    tiffBitmapEncoder.Frames.Add(BitmapFrame.Create(bi));
                    tiffBitmapEncoder.Save(fileStream);
                    
                }
            }
            catch (IOException ex) // Wird geworfen, wenn Datei aus anderem Format, jedoch mit selbem Dateinamen gleichzeitig von einem anderen Thread konvertiert bzw. erzeugt wird
            {
                ConvertToTiff(++counter);
            }
        }
    }
}
