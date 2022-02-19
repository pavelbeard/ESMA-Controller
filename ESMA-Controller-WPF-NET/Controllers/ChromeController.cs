using ESMA.DataCollections;
using ESMA.DataLoaders;
using ESMA.ViewModel;
using MyLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ESMA.Chromedriver
{
    public abstract class ChromeController
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int CurrentTableIndex { get; set; }
        protected ObservableCollection<string> NamesArray { get; set; }

        protected WebDriverWait webDriverWait;
        protected IWebDriver webDriver;
        protected ChromeDriverService cds;
        protected ChromeOptions chromeOptions;

        public ChromeController()
        {
            dynamic t = JsonConvert.DeserializeObject(File.ReadAllText(ConfigData.ConfigurationFilePath));
            string file = t["EmpListFile"];

            var list = new EmpList(file);

            var newList = new ObservableCollection<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsChecked)
                {
                    newList.Add(list[i].Name);
                }
            }

            NamesArray = newList;

            cds = ChromeDriverService.CreateDefaultService();
            chromeOptions = new ChromeOptions();

            if (Convert.ToBoolean(t["SilentMode"]))
            {
                cds.HideCommandPromptWindow = true;
                chromeOptions.AddArgument("headless");
            }
            else
            {
                cds.HideCommandPromptWindow = false;
                chromeOptions = new ChromeOptions();
            }
        }
        protected void LoginWindow() 
        {
            var data = new List<string>
            {
                "http://10.23.218.250:7790/",
                "http://10.23.218.250:7780/",
                "//a[@href='/pls/portal30/escort.p_operative.p_main']",
                "//a[@href='/pls/portal30/ais_sys.dyn_index.show']"
            };

            webDriver = new ChromeDriver(cds, chromeOptions);
            webDriver.Manage().Timeouts().PageLoad = new TimeSpan(0, 2, 0);
            switch (CurrentTableIndex)
            {
                case 0:
                    webDriver.Navigate().GoToUrl(data[1]);
                    webDriver.FindElement(By.XPath(data[3])).Click();
                    break;
                case 1:
                case 2:
                    webDriver.Navigate().GoToUrl(data[0]);
                    webDriver.FindElement(By.XPath(data[2])).Click();
                    break;
                default:
                    break;
            }
            webDriver.FindElement(By.Name("p_username")).SendKeys(Login);
            webDriver.FindElement(By.Name("p_password")).SendKeys(Password);
            webDriver.FindElement(By.TagName("button")).Click();
        }
        protected void LoginWindow(string url)
        {
            //применяем опции
            webDriver = new ChromeDriver(cds, chromeOptions);
            webDriver.Manage().Timeouts().PageLoad = new TimeSpan(0, 2, 0);
            webDriver.Navigate().GoToUrl(url);
            //логинимся
            webDriver.FindElement(By.Name("p_username")).SendKeys(Login);
            webDriver.FindElement(By.Name("p_password")).SendKeys(Password);
            webDriver.FindElement(By.TagName("button")).Click();
        }
        protected void LoginWindow(string url, By element)
        {
            //применяем опции
            webDriver = new ChromeDriver(cds, chromeOptions);
            webDriver.Manage().Timeouts().PageLoad = new TimeSpan(0, 2, 0);
            webDriver.Navigate().GoToUrl(url);
            //заходим на сайт
            webDriver.FindElement(element).Click();
            //логинимся
            webDriver.FindElement(By.Name("p_username")).SendKeys(Login);
            webDriver.FindElement(By.Name("p_password")).SendKeys(Password);
            webDriver.FindElement(By.TagName("button")).Click();
        }
        protected List<List<string>> LoadLrpTable(string lrType)
        {
            try
            {
                Thread.Sleep(4000);

                int countTemplate = webDriver.FindElements(By.XPath("//*[@class='expandRow']")).Count;

                var lr = new List<string>();
                for (int i = 1; i <= countTemplate; i++)
                    lr.Add(webDriver.FindElement(By.XPath($"//*[@id=\"DATA_TABLE\"]/tbody/tr[{i}]/td/div[3]/span")).Text);

                var table = new List<List<string>>
                {
                    new List<string>(),
                    new List<string>(),
                    new List<string>(),
                };

                for (int i = 0; i < lr.Count; i++)
                {    
                    if (Regex.Match(lr[i], @$"{lrType}:\s+(\d+)").Success)
                    {
                        int tr1 = i + 1;
                        webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{tr1}]/td/div[1]/div/img")).Click();
                        var lrps = webDriver.FindElements(By.XPath("//*[@id=\"DATA_TABLE\"]/tbody/tr/td[@class='LRP_NUM']")).Count;
                        for (int j = 1; j <= lrps - 1; j++)
                        {
                            int tr2 = i + j + 1;
                            string input = webDriver.FindElement(By.XPath($"//*[@id=\"DATA_TABLE\"]/tbody/tr[{tr2}]/td[2]")).Text;
                            if (input.Contains("ЗИ") && lrType == "ЛР ОР")
                            {
                                table[0].Add(webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{tr2}]/td[2]/a")).Text);
                                table[1].Add(webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{tr2}]/td[3]")).Text);
                                table[2].Add(Regex.Replace(input, @"\d+\s", ""));
                            }
                            if (input.Contains("ГТП") && lrType == "ЛР ГТП")
                            {
                                table[0].Add(webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{tr2}]/td[2]/a")).Text);
                                table[1].Add(webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{tr2}]/td[3]")).Text);
                                table[2].Add(webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{tr2}]/td[12]")).Text);
                            }

                        }
                        webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{tr1}]/td/div[1]/div/img")).Click();
                    }
                }
                return table;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                webDriver?.Quit();
                return null;
            }
        }
        public virtual Task RunAsync(CancellationToken token, IProgress<double> progress)
        {
            return null;
        }
        public virtual BindingList<T> LoadData<T>(IProgress<int> progress)
        {
            return null;
        }
        public virtual Task<BindingList<T>> LoadDataAsync<T>(IProgress<int> progress)
        {
            return null;
        }
        protected bool CheckElement(By element)
        {
            try
            {
                webDriver.FindElement(element);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
