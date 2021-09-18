using ESMA.Chromedriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA
{
    public class ChangesCreatorController : ChromeController
    {
        //Создатель ЗИ
        public Task<bool> CreateAsync(IProgress<double> progress)
        {
            return Task.Run<bool>(() => 
            {
                int attempts = 0;
                var progressPercentage = 0.0;

                double total = IData.CTCs.Count;

                //Цикл перебора старых ЗИ
                for (int i = 0; i < total; i++)
                {
                    try
                    {
                        NewSession(i);
                        ChangeFrame("frame_1", "/html/body/table[1]/tbody/tr/td[2]/div/table/tbody/tr/td[2]/img[2]");
                        Thread.Sleep(500);
                        //Вызываем метод открытия календаря два раза
                        ChangeDate("//*[@id=\"th_258\"]/td/img[2]", "//*[@id=\"th_258\"]/td/img[1]", IData.CTCs[i].CTC_DateStart, IData.CTCs[i].CTC_TimeStart);
                        Thread.Sleep(500);
                        ChangeDate("//*[@id=\"th_259\"]/td/img[2]", "//*[@id=\"th_259\"]/td/img[1]", IData.CTCs[i].CTC_DateEnd, IData.CTCs[i].CTC_TimeEnd);
                        Thread.Sleep(500);
                        //Сохраняем работу
                        webDriver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/form/input[35]")).Click();
                        Thread.Sleep(1000);
                        //Согласовываем
                        //Меняем внутреннее окно на родительское
                        webDriver.SwitchTo().ParentFrame();
                        ChangeFrame("frame_2");
                        Thread.Sleep(1000);
                        //Вызываем метод согласования
                        JobCoordination("/html/body/form[1]/table[3]/tbody/tr/td/table/tbody/tr[2]/td[3]/img");
                        //Меняем внутреннее окно на родительское
                        webDriver.SwitchTo().ParentFrame();
                        ChangeFrame("frame_2");
                        JobCoordination("/html/body/form[1]/table[3]/tbody/tr/td/table/tbody/tr[3]/td[3]/img", "по телефону");
                        //Меняем внутреннее окно и сохраняем новый ЗИ
                        webDriver.SwitchTo().ParentFrame();
                        ChangeFrame("frame_1", "/html/body/table[4]/tbody/tr/td/form/input[36]");
                        //считаем процент
                        progressPercentage += 1.0 / total * 100.0;
                        progress.Report(progressPercentage);
                        Thread.Sleep(200);
                        //уведомление об успешном создании
                        IData.CTCs[i].CTC_Status = "Завершено";
                        //закрываем драйвер
                        webDriver?.Quit();
                    }
                    catch (Exception)
                    {
                        IData.CTCs[i].CTC_Status = "Ошибка";
                        webDriver?.Quit();
                        throw;
                    }
                }

                return true;
            });
        }

        //Начало новой сессии
        private void NewSession(int i)
        {
            webDriver = new ChromeDriver(cds, chromeOptions);
            webDriver.Navigate().GoToUrl($"http://10.23.218.250:7790/pls/portal30/escort.p_create_denials.p_recovery_sets?v_id_denial={IData.CTCs[i].IdCTC}");
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
        //Смена времени начала/конца работ
        private void ChangeDate(string clearCalendar, string openCalendar, DateTime date, DateTime time)
        {
            //Открывается новое окно - смена текущего окна
            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
            //Очищаем старое время
            webDriver.FindElement(By.XPath(clearCalendar)).Click();
            //Открываем календарь
            webDriver.FindElement(By.XPath(openCalendar)).Click();
            //После открытия календаря - меняем окно
            Thread.Sleep(500);
            webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
            //Календарь открыт
            //-//Меняем месяц
            webDriver.FindElement(By.XPath("//*[@id=\"spanMonth\"]")).Click();
            Thread.Sleep(500);
            webDriver.FindElement(By.XPath($"//*[@id=\"m{date.Month - 1}\"]")).Click();
            Thread.Sleep(500);
            //-//Вставка часов
            webDriver.FindElement(By.XPath($@"//select[@id='time_houre']//option[@value='{int.Parse(time.ToString("HH"))}']")).Click();
            Thread.Sleep(500);
            //-//Вставка минут
            webDriver.FindElement(By.XPath($@"//select[@id='time_min']//option[@value='{int.Parse(time.ToString("mm"))}']")).Click();
            Thread.Sleep(500);
            //Закрываем календарь
            webDriver.ExecuteJavaScript($"javascript:dateSelected={date:dd};closeCalendar();");
            Thread.Sleep(500);
            //После открытием календаря - меняем окно
            Thread.Sleep(500);
            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
        }
        //Согласование работ
        private void JobCoordination(string button, string note = "")
        {
            
            //согласовываем
            webDriver.FindElement(By.XPath(button)).Click();
            //меняем окно
            webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
            //нажимаем кнопку
            webDriver.FindElement(By.XPath("/html/body/table[4]/tbody/tr/td/input[2]")).Click();
            Thread.Sleep(1500);
            //пишем заметку
            webDriver.SwitchTo().Alert().SendKeys(note);
            Thread.Sleep(1000);
            //принимаем
            webDriver.SwitchTo().Alert().Accept();
            Thread.Sleep(500);
            //меняем окно
            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
        }
    }
}
