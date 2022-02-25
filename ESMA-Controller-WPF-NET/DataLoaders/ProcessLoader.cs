using ESMA.Chromedriver;
using ESMA.DataCollections;
using ESMA.DataCollections.CoreDataCollections;
using ESMA.ViewModel;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ESMA.DataLoaders
{
    public class ProcessLoader : ChromeController
    {
        public override BindingList<T> LoadData<T>(IProgress<int> progress)
        {
            throw new NotImplementedException();
        }
        public override Task<BindingList<T>> LoadDataAsync<T>(IProgress<int> progress)
        {
            return Task.Run(() =>
            {
                try
                {
                    LoginWindow();
                    progress.Report(33);

                    //changeframe
                    webDriver.SwitchTo().Frame("frame2");

                    Thread.Sleep(750);

                    webDriver.FindElement(By.XPath("//img[@title='Центральная страница']")).Click();
                    webDriver.FindElement(By.XPath("//a[@onclick=\"openMenu('mod_4',4);return(false);\"]")).Click();
                    webDriver.FindElement(By.XPath("//a[@onclick=\"locFunc('!ais_sys.dyn_header.show',254); return false;\"]")).Click();
                    webDriver.SwitchTo().Frame("main_frame");

                    var table = LoadLrpTable("ЛР ГТП");
                    Thread.Sleep(500);
                    progress.Report(67);

                    var toLoad = new List<Process>();

                    //независимые списки имен
                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
                    string file = t["EmpListFile"];

                    var list = new EmpList(file);

                    var newList = new ObservableCollection<EmpUnit>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].IsChecked)
                        {
                            newList.Add(new EmpUnit { Name = list[i].Name, IsChecked = list[i].IsChecked });
                        }
                    }

                    for (int i = 0; i < table[0].Count; i++)
                    {
                        toLoad.Add(new Process
                        {
                            IdProcess = int.Parse(table[0][i]),
                            P_Description = table[1][i],
                            P_TimeStart = DateTime.Parse("00:00"),
                            P_Event = table[2][i],
                            P_Names = newList
                        });
                    }

                    Thread.Sleep(500);
                    progress.Report(100);
                    webDriver?.Quit();
                    progress.Report(0);
                    return new BindingList<T>((IList<T>)toLoad.OrderBy(x => x.IdProcess).ToList());
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e}", e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    webDriver?.Quit();
                    return null;
                }
            });
        }
    }
}
