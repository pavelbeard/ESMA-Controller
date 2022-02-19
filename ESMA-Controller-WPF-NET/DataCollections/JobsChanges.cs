using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.DataCollections
{
    public class JobsChanges : ObservableCollection<string>
    {
        public JobsChanges()
        {
            Add("Организация совещания/конференции");
            Add("Контроль проведения совещания/конференции");
            Add("Дополнительные работы при проведении совещания/конференции");
            Add("--------------------");
        }
    }
}
