using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ESMA.DataCollections.CoreDataCollections
{
    public class Process : INotifyPropertyChanged
    {
        private ObservableCollection<EmpUnit> p_Names;
        private string p_Status;

        public int IdProcess { get; set; }
        public string P_Description { get; set; }
        public DateTime P_TimeStart { get; set; }
        public string P_Job { get; set; }
        public string P_Event { get; set; }
        public ObservableCollection<EmpUnit> P_Names
        {
            get => p_Names;
            set
            {
                p_Names = value;
                OnPropertyChanged("P_Names");
            }
        }
        public string P_Status
        {
            get => p_Status;
            set
            {
                p_Status = value;
                OnPropertyChanged("P_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
