using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.DataCollections
{
    public class CloseCode : ObservableCollection<string>
    {
        public CloseCode()
        {
            Add("Отменено");
            Add("Проведено");
            Add("--------------------");
        }
    }
}
