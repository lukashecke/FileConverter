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
        public List<string> filePaths;
        public int amountConvertedFiles;
        public int amountAddedFiles;
        public int amountOfFiles;



        private List<string> fileNames = new List<string>() /*{ "Hier reinziehen möglich." }*/;
        public List<string> FileNames
        {
            get
            {
                if (this.fileNames == null) // FallbackValue wird dadurch ausgeschlossen im View
                {
                    this.fileNames = new List<string>() /*{ "Hier reinziehen möglich." }*/;
                }
                return this.fileNames;
            }
            set
            {
                this.fileNames = value;
                this.OnPropertyChanged("FileNames");
            }
        }

    }
}
