using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public class JobsProcess : ObservableCollection<string>
    {
        public JobsProcess()
        {
            Add("Тестирование, проверка работоспособности");
            Add("--------------------");
        }
    }

    public class CloseCode : ObservableCollection<string>
    {
        public CloseCode()
        {
            Add("Отменено");
            Add("Проведено");
            Add("--------------------");
        }
    }

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

    //public static class ValuesCollection
    //{
    //    public static string Login { get => "login"; }
    //    public static string Password { get => "pass"; }
    //    public static string SilentMode { get => "silentMode"; }
    //    public static string FileConference { get => "fileConference"; }
    //    public static string FileChanges { get => "fileChanges"; }
    //    public static string FileProcesses { get => "fileProcesses"; }

    //    public static List<string> RegistryValues
    //    {
    //        get => new List<string>
    //        {
    //            Login,
    //            Password,
    //            SilentMode,
    //            FileConference,
    //            FileChanges,
    //            FileProcesses
    //        };
    //    }
    //}
}
