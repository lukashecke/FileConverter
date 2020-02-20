using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileConverter.Model
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        private T current;
        public T Current
        {
            get
            {
                return this.current;
            }
            set
            {
                this.current = value;
            }
        }
    }
}
