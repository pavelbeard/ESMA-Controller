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
    public class ReportNames : ObservableCollection<string>
    {
        public ReportNames()
        {
            var file = File.ReadAllText(ConfigData.EmpListFileJSON);
            var lists = new
            {
                FullNameList = new List<string>(),
                ShortNameList = new List<string>()
            };
            var listsDeserialized = JsonConvert.DeserializeAnonymousType(file, lists);

            foreach (string name in listsDeserialized.ShortNameList) Add(name);
        }
    }
}
