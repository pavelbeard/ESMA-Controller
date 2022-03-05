﻿using ESMA.Chromedriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ESMA.ViewModel;
using System.Threading.Tasks;
using System.Windows;
using ESMA.DataCollections.CoreDataCollections;
using ESMA.DataCollections;
using Newtonsoft.Json;
using System.IO;

namespace ESMA.DataLoaders
{
    public class ChangesLoader : ChromeController
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
                    progress.Report(10);

                    //changeframe
                    webDriver.SwitchTo().Frame("frame2");

                    Thread.Sleep(750);

                    webDriver.FindElement(By.XPath("//*[@id=\"img_homes\"]")).Click();
                    //webDriver.FindElement(By.XPath("//img[@title='Центральная страница']")).Click();
                    webDriver.FindElement(By.XPath("//a[@onclick=\"openMenu('mod_4',4);return(false);\"]")).Click();
                    webDriver.FindElement(By.XPath("//a[@onclick=\"locFunc('!ais_sys.dyn_header.show',254); return false;\"]")).Click();
                    webDriver.SwitchTo().Frame("main_frame");

                    Thread.Sleep(500);
                    progress.Report(25);

                    var table = LoadLrpTable("ЛР ОР") ?? throw new Exception("Ошибка, таблица не заполнена");

                    var toLoad = new List<Changes>();

                    //-//
                    dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
                    string file = t["EmpListFile"];

                    var list = new EmpList(file);

                    ObservableCollection<EmpUnitChanges> NewList(DateTime ts, DateTime te)
                    {
                        var newList = new ObservableCollection<EmpUnitChanges>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].IsChecked)
                            {
                                newList.Add(new EmpUnitChanges 
                                { 
                                    Name = list[i].Name, 
                                    IsChecked = list[i].IsChecked,
                                    TimeStart = ts,
                                    TimeEnd = te
                                });
                            }
                        }

                        return newList;
                    }

                    for (int i = 0; i < table[0].Count; i++)
                    {
                        toLoad.Add(new Changes
                        {
                            IdChanges = int.Parse(table[0][i]),
                            C_Description = $"{table[2][i]}:{table[1][i]}",
                            C_Names = NewList(DateTime.Parse("00:00"), DateTime.Parse("00:00"))
                        });
                    }
                    progress.Report(75);

                    progress.Report(100);
                    webDriver?.Quit();
                    progress.Report(0);
                    return new BindingList<T>((IList<T>)toLoad.OrderBy(x => x.IdChanges).ToList());
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
