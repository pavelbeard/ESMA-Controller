using ESMA.Chromedriver;
using ESMA.ViewModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA
{
    public class CTCLoader : ChromeController
    {
        public override Task<BindingList<T>> LoadDataAsync<T>(IProgress<int> progress)
        {
            return Task.Run(() => 
            {
                try
                {
                    LoginWindow("http://10.23.218.250:7790/", By.XPath("//a[@href='/pls/portal30/escort.p_operative.p_main']"));

                    progress.Report(10);

                    //changeframe
                    webDriver.SwitchTo().Frame("frame2");

                    var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
                    Thread.Sleep(750);

                    //ТЕСТОВАЯ СТРОКА. ДОЛЖНА ОПРОБОВАТЬСЯ В БЛИЖАЙШЕМ ПОДКЛЮЧЕНИИ К ЕСМЕ!
                    //wait.Until(d => d.FindElement(By.XPath("//img[@title='Центральная страница']")));

                    webDriver.FindElement(By.XPath("//img[@title='Центральная страница']")).Click();
                    webDriver.FindElement(By.XPath("//a[@onclick=\"openMenu('mod_4',4);return(false);\"]")).Click();
                    webDriver.FindElement(By.XPath("//a[@onclick=\"locFunc('!ais_sys.dyn_header.show',254); return false;\"]")).Click();
                    webDriver.SwitchTo().Frame("main_frame");

                    Thread.Sleep(500);
                    progress.Report(25);

                    var table = LoadLrpTable("ЛР ОР") ?? throw new Exception("Ошибка, таблица не заполнена");

                    var toLoad = new List<ChangesCreate>();
                    for (int i = 0; i < table[0].Count; i++)
                    {
                        toLoad.Add(new ChangesCreate
                        {
                            IdCTC = int.Parse(table[0][i]),
                            CTC_Description = $"{table[2][i]}:{table[1][i]}",
                            CTC_DateStart = DateTime.Parse(DateTime.Now.ToString("dd/MM/yy")),
                            CTC_DateEnd = DateTime.Parse(DateTime.Now.ToString("dd/MM/yy")),
                            CTC_TimeStart = DateTime.Parse(DateTime.Now.ToString("HH:mm")),
                            CTC_TimeEnd = DateTime.Parse(DateTime.Now.ToString("HH:mm"))
                        });
                    }
                    progress.Report(75);

                    progress.Report(100);
                    webDriver?.Quit();
                    progress.Report(0);
                    return new BindingList<T>((IList<T>)toLoad.OrderBy(x => x.IdCTC).ToList());
                }
                catch (Exception)
                {

                    throw;
                }
            });
        }
    }
}
