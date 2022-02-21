using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.DataCollections.CoreDataCollections
{
    public class MWEmpList : ObservableCollection<EmpUnit>
    {
        public MWEmpList(List<EmpUnit> source)
        {
            foreach (var unit in source)
            {
                Add(new EmpUnit { Name = unit.Name, IsChecked = unit.IsChecked });
            }
        }
    }
}
