using ESMA.Chromedriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA.Controllers
{
    public class ProcessController : ChromeController
    {
        public override Task RunAsync(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                int attempts = 0;
                var progressPercentage = 0.0;

                var pairs = new Dictionary<int, string>
                {
                    {15, "Тестирование, проверка работоспособности" },
                };

                for (int i = 0, namesCount = 0; i < IData.Processes.Count; i++, namesCount++)
                {
                    while (true)
                    {
                        try
                        {
                            //Если счетчик имен станет максимальным
                            if (namesCount == IData.Processes[i].P_Names.Count) namesCount = 0;
                            //Запрос на отмену
                            token.ThrowIfCancellationRequested();

                            IData.Window.Dispatcher.Invoke(()
                                        => IData.Window.Info.Text
                                        = $"{IData.Processes[i].IdProcess}\n" +
                                          $"{IData.Processes[i].P_Description}\n" +
                                          $"{IData.Processes[i].P_Names[namesCount].Name}");
                            //Новая сессия
                            //-(
                            webDriver = new ChromeDriver(cds, chromeOptions);
                            webDriver.Navigate().GoToUrl($"http://10.23.218.250:7790/pls/portal30/!ais_sys.dyn_obj_card.load?OBJ_ID=5533&WIN_TYPE=4&DATA_VALUE={IData.Processes[i].IdProcess}");
                            //)-
                            //Поиск фреймов
                            //-(
                            string[] frames = { "[name=\"CARD_FRAME_MARK\"]", "[name=\"CARD_FRAME_DETAIL\"]" };

                            for (int j = 0; j < frames.Length; j++)
                            {
                                if (j == 1) webDriver.SwitchTo().ParentFrame();
                                webDriver.SwitchTo().Frame(webDriver.FindElement(By.CssSelector(frames[j])));
                                webDriver.FindElement(By.Name("p_username")).SendKeys(Login);
                                webDriver.FindElement(By.Name("p_password")).SendKeys(Password);
                                webDriver.FindElement(By.TagName("button")).Click();
                            }

                            webDriver.Navigate().Refresh();
                            webDriver.SwitchTo().Frame(webDriver.FindElement(By.CssSelector("[name=\"CARD_FRAME_MARK\"]")));
                            webDriver.FindElement(By.CssSelector("a[href=\"javascript:redirect('6');\"]")).Click();
                            //)-
                            Thread.Sleep(500);
                            //Вставка имени
                            //-(
                            webDriver.FindElement(By.Name("BT_ADD_RLP")).Click();
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                            webDriver.FindElement(By.XPath("//td//img[@src='/images/list.gif']")).Click();
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                            webDriver.FindElement(By.XPath($"//*[contains(@href, '{IData.Processes[i].P_Names[namesCount].Name.Split(" ")[0]}')]")).Click(); //Вставка имени                                                                                                                                                    
                            //)-                                                                                                                                                       
                            //Календарь
                            ArrayList arrayList = new ArrayList
                            {
                                new List<string>
                                {
                                    "//tr[@id='COLROW3']//img",
                                    "//tr[@id='COLROW4']//img"
                                },
                                new List<int>
                                {
                                    0,
                                    10
                                }
                            };

                            for (int j = 0, w = 1; j < arrayList.Count; j++)
                            {
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[w]);
                                //Даты
                                var dateEdit = arrayList[0] as List<string>;
                                Thread.Sleep(500);
                                webDriver.FindElement(By.XPath(dateEdit[j])).Click();

                                webDriver.SwitchTo().Window(webDriver.WindowHandles[++w]);
                                //Вставка часов
                                new SelectElement(webDriver.FindElement(By.XPath("/html/body/div[1]/table[2]/tbody/tr/td/select[1]")))
                                .SelectByIndex(int.Parse(IData.Processes[i].P_TimeStart.ToString("HH")));

                                //Прибавка минут
                                var addMin = arrayList[1] as List<int>;
                                //Вставка минут
                                new SelectElement(webDriver.FindElement(By.XPath("/html/body/div[1]/table[2]/tbody/tr/td/select[2]"))).SelectByIndex(int.Parse(IData.Processes[i].P_TimeStart.ToString("mm")) + addMin[j]);
                                webDriver.ExecuteJavaScript($"javascript:dateSelected={DateTime.Now.Day};closeCalendar();");

                                webDriver.SwitchTo().Window(webDriver.WindowHandles[--w]);
                            }

                            //Работы
                            webDriver.FindElement(By.XPath("//tr[@id='COLROW8']//img")).Click();

                            for (int j = 0; j < pairs.Count; j++)
                            {
                                if (IData.Processes[i].P_Job == pairs.Values.ToList()[j])
                                {
                                    webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                                    webDriver.ExecuteJavaScript($"javascript:f_sel('{pairs.Keys.ToList()[j]}')");
                                }
                            }

                            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                            webDriver.FindElement(By.Name("INSERT")).Click();
                            webDriver.FindElement(By.Name("BT_CLS")).Click();

                            progressPercentage += 1.0 / IData.Processes.Count * 100.0;
                            progress.Report(progressPercentage);
                            IData.Processes[i].P_Status = "Завершено";
                            webDriver?.Quit();
                            break;
                        }
                        catch (OperationCanceledException)
                        {
                            IData.Processes[i].P_Status = "Сброшено";
                            webDriver?.Quit();
                            throw new OperationCanceledException("Программа сброшена");
                        }
                        catch (WebDriverTimeoutException)
                        {
                            webDriver?.Quit();
                            throw;
                        }
                        catch (Exception)
                        {
                            if (attempts == 2)
                            {
                                IData.Processes[i].P_Status = "Ошибка";
                                webDriver?.Quit();
                                throw;
                            }
                            attempts++;
                            webDriver?.Quit();
                        }
                    }
                }
            });
        }
    }
}
