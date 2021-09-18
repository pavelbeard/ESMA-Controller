using ESMA.Chromedriver;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA.ChangesCloser
{
    public class ChangesCloserLoader : ChromeController
    {
        public override Task<BindingList<T>> LoadDataAsync<T>(IProgress<int> progress)
        {
            return Task.Run(() =>
            {
                try
                {
                    LoginWindow("http://10.23.218.250:7790/", By.XPath("//a[@href='/pls/portal30/escort.p_operative.p_main']"));

                    progress.Report(10);

                    webDriver.FindElement(By.XPath("//img[@title='Центральная страница']")).Click();
                    webDriver.FindElement(By.XPath("//a[@onclick=\"openMenu('mod_4',4);return(false);\"]")).Click();
                    webDriver.FindElement(By.XPath("//a[@onclick=\"locFunc('!ais_sys.dyn_header.show',254); return false;\"]")).Click();
                    webDriver.SwitchTo().Frame("main_frame");

                    Thread.Sleep(500);
                    progress.Report(25);

                    var table = LoadLrpTable("ЛР ОР") ?? throw new Exception("Ошибка, таблица не заполнена");

                    var toLoad = new List<ChangesCloserElement>();
                    for (int i = 0; i < table[0].Count; i++)
                    {
                        toLoad.Add(new ChangesCloserElement
                        {
                            IdCCE = int.Parse(table[0][i]),
                            CCE_Description = $"{table[2][i]}:{table[1][i]}",
                        });
                    }
                    progress.Report(75);

                    progress.Report(100);
                    webDriver?.Quit();
                    progress.Report(0);
                    return new BindingList<T>((IList<T>)toLoad.OrderBy(x => x.IdCCE).ToList());
                }
                catch (Exception)
                {

                    throw;
                }
            });
        }
    }
}
