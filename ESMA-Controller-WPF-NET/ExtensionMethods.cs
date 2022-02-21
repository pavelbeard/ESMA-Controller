using ESMA.ChangesCloser;
using ESMA.DataCollections.CoreDataCollections;
using ESMA.ViewModel;
using Microsoft.Win32;
using MyLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESMA
{
    public static class ExtensionMethods
    {
        public static Task OpenConfig(this IJsonService app, string file)
        {
            return Task.Run(() =>
            {
                var t = JsonConvert.DeserializeAnonymousType(File.ReadAllText(file), new
                {
                    Conferences = new BindingList<VideoConference>(),
                    Changes = new BindingList<Changes>(),
                    Processes = new BindingList<Process>(),
                    CTCs = new BindingList<ChangesCreate>(),
                    ChangesCloserElements = new BindingList<ChangesCloserElement>()
                });

                IData.Window.Dispatcher.Invoke(() =>
                {
                    IData.Window.videoList?.Clear();
                    IData.Window.changesList?.Clear();
                    IData.Window.processList?.Clear();
                    IData.Window.chCreateList?.Clear();
                    IData.Window.cceList?.Clear();

                    IData.Window.videoList = t.Conferences;
                    IData.Window.Conference.ItemsSource = IData.Window.videoList;
                    IData.Window.VC.Header = $"Конференции\n{Path.GetFileName(file)}";

                    IData.Window.changesList = t.Changes;
                    IData.Window.Changes.ItemsSource = IData.Window.changesList;
                    IData.Window.C.Header = $"ЗИ\n{Path.GetFileName(file)}";

                    IData.Window.processList = t.Processes;
                    IData.Window.Process.ItemsSource = IData.Window.processList;
                    IData.Window.P.Header = $"ГТП\n{Path.GetFileName(file)}";

                    IData.Window.chCreateList = t.CTCs;
                    IData.Window.ChangesCreate.ItemsSource = IData.Window.chCreateList;
                    IData.Window.CC.Header = $"Создание ЗИ\n{Path.GetFileName(file)}";

                    IData.Window.cceList = t.ChangesCloserElements;
                    IData.Window.ChangesClose.ItemsSource = IData.Window.cceList;
                    IData.Window.CTCl.Header = $"Уничтожение ЗИ\n{Path.GetFileName(file)}";
                });
            });
        }
    }
}
