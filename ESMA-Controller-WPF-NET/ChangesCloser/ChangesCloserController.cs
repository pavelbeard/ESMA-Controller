using ESMA.Chromedriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA.ChangesCloser
{
    public class ChangesCloserController : ChromeController
    {
        public override Task RunAsync(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                int attempts = 0;
                var progressPercentage = 0.0;

                double total = IData.ChangesCloserElements.Count;

                for (int i = 0; i < total; i++)
                {
                    try
                    {
                        NewSession(i);
                        ChangeFrame("frame_1", "/html/body/table[4]/tbody/tr/td/form/input[36]");
                        Thread.Sleep(750);
                        //еще раз меняем окно
                        ChangeFrame("frame_1");
                        Thread.Sleep(750);
                        //Открываем выпадающий список и ставим решено
                        new SelectElement(webDriver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/form/table/tbody/tr[5]/td/select"))).SelectByIndex(3);
                        Thread.Sleep(750);
                        //Открываем выпадающий список и ставим еще раз решено
                        new SelectElement(webDriver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/form/table/tbody/tr[7]/td/select"))).SelectByIndex(1);
                        Thread.Sleep(750);
                        //сохраняем
                        webDriver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/form/input[35]")).Click();
                        Thread.Sleep(750);
                        //еще раз меняем окно
                        ChangeFrame("frame_1");
                        Thread.Sleep(750);
                        //Открываем выпадающий список и ставим закрыто
                        new SelectElement(webDriver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/form/table/tbody/tr[5]/td/select"))).SelectByIndex(3);
                        Thread.Sleep(750);
                        //сохранить и выйти
                        webDriver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/form/input[36]")).Click();
                        Thread.Sleep(500);
                        //считаем процент
                        progressPercentage += 1.0 / total * 100.0;
                        progress.Report(progressPercentage);
                        //уведомление об успешном уничтожении
                        IData.ChangesCloserElements[i].CCE_Status = "Завершено";
                        webDriver?.Quit();
                    }
                    catch (Exception)
                    {
                        IData.ChangesCloserElements[i].CCE_Status = "Ошибка";
                        webDriver?.Quit();
                        throw;
                    }
                }
            });
        }

        //Начало новой сессии
        private void NewSession(int i)
        {
            webDriver = new ChromeDriver(cds, chromeOptions);
            webDriver.Navigate().GoToUrl($"http://10.23.218.250:7790/pls/portal30/escort.p_create_denials.p_recovery_sets?v_id_denial={IData.ChangesCloserElements[i].IdCCE}");
            webDriver.FindElement(By.Name("p_username")).SendKeys(Login);
            webDriver.FindElement(By.Name("p_password")).SendKeys(Password);
            webDriver.FindElement(By.TagName("button")).Click();
        }
        //Смена внутреннего окна 
        private void ChangeFrame(string frameName, string button = "")
        {
            webDriver.SwitchTo().Frame(webDriver.FindElement(By.Name(frameName)));
            if (button != "")
                webDriver.FindElement(By.XPath(button)).Click();
        }
    }
}
