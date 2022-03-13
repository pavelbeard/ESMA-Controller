using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.DataCollections.CoreDataCollections
{
    public class Changes : INotifyPropertyChanged
    {
        private ObservableCollection<EmpUnit> c_Names;
        private string c_Status;

        public int IdChanges { get; set; }
        public string C_Description { get; set; }
        public DateTime C_TimeStart { get; set; }
        public DateTime C_TimeEnd { get; set; }
        public string C_Job { get; set; }
        public ObservableCollection<EmpUnit> C_Names
        {
            get { return c_Names; }
            set
            {
                c_Names = value;
                OnPropertyChanged("C_Names");
            }
        }
        public string C_Status
        {
            get => c_Status;
            set
            {
                c_Status = value;
                OnPropertyChanged("C_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
