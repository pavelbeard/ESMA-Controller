using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA
{
    public class ConfigurationFileCreator
    {
        //Folders
        //Logs folder
        public string LogsFolder { get; set; }
        //Names folder
        public string NamesFolder { get; set; }
        //Tasks folder
        public string TasksFolder { get; set; }
        //Names files
        public string CurrentNamesFile { get; set; }
        public string ListNamesFile { get; set; }
        //Authorization
        public string Login { get; set; }
        public string Password { get; set; }
        //SilentMode
        public bool SilentMode { get; set; }
    }

    public static class ConfigData
    {
        
        private static readonly string namesListFile = Path.Combine(Environment.CurrentDirectory, "NamesList.txt");
        private static readonly string configurationFilePath = Path.Combine(Environment.CurrentDirectory, "Configuration.json");
        private static readonly string initialDirectory = Path.Combine(Environment.CurrentDirectory, "Tasks");
        private static readonly string empListJSON = Path.Combine(Environment.CurrentDirectory, "EmpList.json");
        private static readonly string namesListFileJSON = Path.Combine(Environment.CurrentDirectory, "NamesList.json");
        private static readonly string boostListFileJSON = Path.Combine(Environment.CurrentDirectory, "BoostList.json");

        public static string BoostListFileJSON
        {
            get => File.Exists(boostListFileJSON) ? boostListFileJSON : "null";
        }
        public static string NamesListFileJSON
        {
            get => File.Exists(namesListFileJSON) ? namesListFileJSON : "null";
        }
        public static string EmpListFileJSON
        {
            get => File.Exists(empListJSON) ? empListJSON: "null";
        }
        public static string ConfigurationFilePath
        {
            get => File.Exists(configurationFilePath) ? configurationFilePath : null;
        }
        public static string NamesListFile
        {
            get => File.Exists(namesListFile) ? namesListFile : "null";
        }

        public static List<string> NamesList
        {
            get
            {
                if (File.Exists(namesListFile))
                {
                    return File.ReadAllLines(namesListFile).ToList();
                }
                else
                {
                    return new List<string> { "null" };
                }
            }
        }

        public static string InitialDirectory 
        { 
            get
            {
                if (ConfigurationFilePath != null)
                {
                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(configurationFilePath));

                    string pathToCheck = t["TasksFolder"];

                    if (Directory.Exists(pathToCheck))
                    {
                        return t["TasksFolder"];
                    }
                    else
                    {
                        return initialDirectory;
                    }
                }
                else
                {
                    return initialDirectory;
                }
            }
        }
        public static string TablesConfigPath { get; set; }
        public static string TaskFilePath { get; set; }
        public static string NativeTaskFolderPath
        {
#if DEBUG
            get => Path.Combine(Environment.CurrentDirectory, "Tasks");
            //get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ESMA-Controller-Documents", "Tasks"); 
#elif RELEASE
            get => Path.Combine(Environment.CurrentDirectory, "Tasks");
#endif
        }
        public static string LogsFolderPath
        {
#if DEBUG
            get => Path.Combine(Environment.CurrentDirectory, "Logs");
            //get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ESMA-Controller-Documents", "Logs"); 
#elif RELEASE
            get => Path.Combine(Environment.CurrentDirectory, "Logs");
#endif
        }
        public static string ReportsFolderPath
        {
            get => Path.Combine(Environment.CurrentDirectory, "Reports");
        }
    }
}
