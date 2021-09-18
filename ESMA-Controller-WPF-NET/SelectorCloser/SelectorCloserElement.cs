using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.SelectorCloser
{
    public class SelectorCloserElement : INotifyPropertyChanged
    {
        private string selector_Status;

        public int IdSelector { get; set; }
        public string Selector_Description { get; set; }
        public DateTime Selector_DateStart { get; set; }
        public DateTime Selector_DateEnd { get; set; }
        public DateTime Selector_TimeStart { get; set; }
        public DateTime Selector_TimeEnd { get; set; }
        public string Selector_Status
        {
            get => selector_Status;
            set
            {
                selector_Status = value;
                OnPropertyChanged("Selctor_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
