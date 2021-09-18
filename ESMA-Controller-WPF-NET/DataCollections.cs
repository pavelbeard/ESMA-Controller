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
            Add("Тобольчик А.А.");
            Add("Пчелкина Ю.М.");
            Add("Жаворонкина Н.В.");
            Add("Носкина Е.А.");
            Add("Жданова Н.В.");
            Add("Васильева И.А.");
            Add("Жукова Ю.М.");
            Add("Кутакова Н.М.");
            Add("Степачева И.Н.");
            Add("Глубокова Е.Н.");
            Add("Бородин П.А.");
            Add("Хромов Д.А.");
            Add("Степанов М.А.");
        }
    }

    public static class ValuesCollection
    {
        public static string Login { get => "login"; }
        public static string Password { get => "pass"; }
        public static string SilentMode { get => "silentMode"; }
        public static string FileConference { get => "fileConference"; }
        public static string FileChanges { get => "fileChanges"; }
        public static string FileProcesses { get => "fileProcesses"; }

        public static List<string> RegistryValues
        {
            get => new List<string>
            {
                Login,
                Password,
                SilentMode,
                FileConference,
                FileChanges,
                FileProcesses
            };
        }
    }
}
