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
    public class EmpList : ObservableCollection<EmpCollection>
    {
        public EmpList(string fileName)
        {
            var file = File.ReadAllText(fileName);

            var listsDeserialized = JsonConvert.DeserializeObject<JSONEmpCollection>(file);

            for (int i = 0; i < listsDeserialized.NamesList.Count; i++)
                Add(new EmpCollection
                {
                    Name = listsDeserialized.NamesList[i],
                    IsChecked = listsDeserialized.CheckList[i]
                });
        }
    }
    //вспомогательный класс для формирования списков
    public class EmpCollection
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; } 
    }
    //класс для сериализации/десериализации
    public class JSONEmpCollection
    {
        public List<string> NamesList { get; set; } = new();
        public List<bool> CheckList { get; set; } = new();
    }
}
