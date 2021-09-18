using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace MyLibrary
{
    public class JsonIO : IJsonService
    {
        public string Path { get; set; }

        public void SaveFileAsync<T>(string configFile, Dictionary<string, T> data)
        {
            //using(FileStream fs = )
        }

        public void EditFile<T>(string configFile, Dictionary<string, T> data)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject(File.ReadAllText(configFile));
            for (int i = 0; i < data.Count; i++)
            {
                jsonObject[data.Keys.ToList()[i]] = data.Values.ToList()[i];
            }
            string toWrite = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText(configFile, toWrite);
        }

        public dynamic OpenFileDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Открыть файл",
                RestoreDirectory = true,
                Filter = ".json|*.json"
            };

            if (ofd.ShowDialog() == true)
            {
                return JsonConvert.DeserializeObject(File.ReadAllText(ofd.FileName)); 
            }
            return default;
        }

        public Task EditFileAsync<T>(string configFile, Dictionary<string, T> data)
        {
            return Task.Run(() => EditFile(configFile, data));
        }

        public Task<dynamic> OpenFileDialogAsync()
        {
            return Task.Run(() => OpenFileDialog());
        }

        public Task SaveFileDialogAsync<T>(Dictionary<string, T> data)
        {
            return Task.Run(() =>
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Title = "Сохранить конфигурацию",
                    RestoreDirectory = true,
                    Filter = "Сохранить файл в формате .json|*.json"
                };

                if (sfd.ShowDialog() == true)
                {
                    EditFile(sfd.FileName, data);
                    Path = sfd.FileName;
                }
            });
        }

        public Task<dynamic> GetFieldOnJson(string fileName, string field)
        {
            return Task.Run(() => { return JsonConvert.DeserializeObject(File.ReadAllText(fileName)); });
        }
    }
}
