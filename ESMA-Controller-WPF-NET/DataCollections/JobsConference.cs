using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.DataCollections
{
    public class JobsConference : ObservableCollection<string>
    {
        public JobsConference()
        {
            Add("Контроль проведения совещания/конференции");
            Add("Совещания");
            Add("--------------------");
        }
    }
}
