using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.ChangesCloser
{
    public class ChangesCloserElement : INotifyPropertyChanged
    {
        private string cce_Status;

        public int IdCCE { get; set; }
        public string CCE_Description { get; set; }
        public string CCE_Status
        {
            get => cce_Status;
            set
            {
                cce_Status = value;
                OnPropertyChanged("CCE_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
