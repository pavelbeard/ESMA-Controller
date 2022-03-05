using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.DataCollections
{
    public class EmpListChanges : ObservableCollection<EmpUnitChanges>
    {
        public EmpListChanges(string fileName, List<DateTime> timeStart, List<DateTime> timeEnd)
        {
            var file = File.ReadAllText(fileName);

            var listsDeserialized = JsonConvert.DeserializeObject<JSONEmpCollection>(file);

            for (int i = 0; i < listsDeserialized.NamesList.Count; i++)
                Add(new EmpUnitChanges
                {
                    Name = listsDeserialized.NamesList[i],
                    IsChecked = listsDeserialized.CheckList[i],
                    TimeStart = timeStart[i],
                    TimeEnd = timeEnd[i]
                });
        }
    }
    //вспомогательный класс для формирования списков
    public class EmpUnitChanges
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
    }
    //класс для сериализации/десериализации
}
