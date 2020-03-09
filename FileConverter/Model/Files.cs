using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileConverter.Model
{
    public class Files : ModelBase
    {
        public int amountConvertedFiles;
        public int amountAddedFiles;
        public int amountOfFiles;

        private List<string> filePaths = new List<string>();
        /// <summary>
        /// Nur die Namen der zu konvertierenden Dateien. Wird zur Anzeige benutzt und nach dem einlesen in eine ObservableCollection gecastet.
        /// </summary>
        public List<string> FilePaths
        {
            get
            {
                if (this.filePaths == null) // FallbackValue wird dadurch ausgeschlossen im View
                {
                    this.filePaths = new List<string>();
                }
                return this.filePaths;
            }
            set
            {
                this.filePaths = value;
                this.OnPropertyChanged("FilePaths");
            }
        }

    }
}
