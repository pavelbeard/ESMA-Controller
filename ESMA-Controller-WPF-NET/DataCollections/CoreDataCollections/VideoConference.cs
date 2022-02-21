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
    public class VideoConference : INotifyPropertyChanged
    {
        private ObservableCollection<EmpUnit> vc_Names;
        private ObservableCollection<EmpUnit> vc_Names_for_content;
        private string vc_Status;

        public int IdConference { get; set; }
        public string VC_Theme { get; set; }
        public DateTime VC_Date { get; set; }
        public DateTime VC_TimeStart { get; set; }
        public DateTime VC_TimeEnd { get; set; }
        public string VC_Job { get; set; }
        public bool OperPersonal { get; set; }
        public ObservableCollection<EmpUnit> VC_Names
        {
            get => vc_Names;
            set
            {
                vc_Names = value;
                OnPropertyChanged("VC_Names");
            }
        }
        public ObservableCollection<EmpUnit> VC_Names_For_Content
        {
            get => vc_Names_for_content;
            set
            {
                vc_Names_for_content = value;
                OnPropertyChanged("VC_Names_For_Content");
            }
        }
        public bool Escort { get; set; }
        public bool CloseConference { get; set; }
        public string CloseCode { get; set; }
        public string VC_Status
        {
            get => vc_Status;
            set
            {
                vc_Status = value;
                OnPropertyChanged("VC_Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
