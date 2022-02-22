using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.DataCollections.CoreDataCollections
{
    public class ChangesCreate : INotifyPropertyChanged
    {
        private string ctc_Status;
        public int IdCTC { get; set; }
        public string CTC_Description { get; set; }
        public DateTime CTC_DateStart { get; set; }
        public DateTime CTC_DateEnd { get; set; }
        public DateTime CTC_TimeStart { get; set; }
        public DateTime CTC_TimeEnd { get; set; }
        public string CTC_Status
        {
            get => ctc_Status;
            set
            {
                ctc_Status = value;
                OnPropertyChanged("CTC_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
