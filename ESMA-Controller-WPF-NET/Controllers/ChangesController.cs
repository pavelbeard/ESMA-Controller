using ESMA.Chromedriver;
using ESMA.ExcelData;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ESMA.Controllers
{
    public class ChangesController : ChromeController
    {
        public Task TestReport(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                int attempts = 0;
                var progressPercentage = 0.0;

                IEnumerable<int> PercentageMax()
                {
                    foreach (var c in IData.Changes)
                    {
                        yield return c.C_Names.Count;
                    }
                }

                double total = PercentageMax().Sum();

                for (int i = 0; i < IData.Changes.Count; i++)
                {
                label1:
                    try
                    {
                        //Внесение опер персонала
                        //--{
                        for (int j = 0; j < IData.Changes[i].C_Names.Count; j++)
                        {
                            IData.Window.Dispatcher.Invoke(()
                                    => IData.Window.Info.Text
                                    = $"{IData.Changes[i].IdChanges}\n" +
                                      $"{IData.Changes[i].C_Description}\n" +
                                      $"{IData.Changes[i].C_Names[j].Name}");

                            //}--
                            progressPercentage += 1.0 / total * 100.0;
                            progress.Report(progressPercentage);
                            Thread.Sleep(700);
                        }
                        //}--
                        Thread.Sleep(500);
                        IData.Changes[i].C_Status = "Завершено";

                        webDriver?.Quit();
                    }
                    catch (OperationCanceledException)
                    {
                        IData.Changes[i].C_Status = "Сброшено";
                        MessageBox.Show("Программа сброшена", "Стоп", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        webDriver?.Quit();
                        break;
                    }
                    catch (Exception e)
                    {
                        if (attempts == 2)
                        {
                            IData.Changes[i].C_Status = "Ошибка";
                            MessageBox.Show(e.Message, "Исключение", MessageBoxButton.OK, MessageBoxImage.Error);
                            webDriver?.Quit();
                            break;
                        }
                        attempts++;
                        webDriver?.Quit();
                        goto label1;
                    }
                }
            });
        }

        public override Task RunAsync(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                int attempts = 0;
                var progressPercentage = 0.0;
                var pairs = new Dictionary<int, string>
                {
                    {15,    "Тестирование, проверка работоспособности" },
                    {132,   "Дополнительные работы при проведении совещания/конференции" },
                    {129,   "Контроль проведения совещания/конференции" },
                    {131,   "Организация совещания/конференции" },
                    {116,   "Другие работы" }
                };

                IEnumerable<int> PercentageMax()
                {
                    foreach (var c in IData.Changes)
                    {
                        yield return c.C_Names.Count;
                    }
                }

                double total = PercentageMax().Sum();

                for (int i = 0; i < IData.Changes.Count; i++)
                {
                    while (true)
                    {
                        try
                        {
                            //Запрос на приостановку
                            token.ThrowIfCancellationRequested();
                            void NewSession()
                            {
                                //Новая сессия
                                webDriver = new ChromeDriver(cds, chromeOptions);
                                webDriver.Navigate().GoToUrl($"http://10.23.218.250:7790/pls/portal30/escort.p_create_denials.p_recovery_sets?v_id_denial={IData.Changes[i].IdChanges}");
                                webDriver.FindElement(By.Name("p_username")).SendKeys(Login);
                                webDriver.FindElement(By.Name("p_password")).SendKeys(Password);
                                webDriver.FindElement(By.TagName("button")).Click();
                            }
                            void ChangeFrame()
                            {
                                //Смена фрейма
                                //--{
                                webDriver.SwitchTo().Frame(webDriver.FindElement(By.Name("frame_2")));
                                webDriver.FindElement(By.XPath("//td//a[@id='5']")).Click();
                                //}--
                            }

                            NewSession();
                            ChangeFrame();

                            //формирование нового списка имен на основе отмеченных
                            var names = IData.Changes[i].C_Names;
                            var newNames = new List<string>();

                            for(int j = 0; j < names.Count; j++)
                            {
                                if(names[j].IsChecked)
                                {
                                    newNames.Add(names[j].Name);
                                }
                            }

                            //Внесение опер персонала
                            //--{
                            for (int j = 0; j < newNames.Count; j++)
                            {
                                IData.Window.Dispatcher.Invoke(()
                                        => IData.Window.Info.Text
                                        = $"{IData.Changes[i].IdChanges}\n" +
                                          $"{IData.Changes[i].C_Description}\n" +
                                          $"{IData.Changes[i].C_Names[j]}");

                                webDriver.FindElement(By.Name("BT_ADD_RLP")).Click();

                                string[] frames = { "//*[@id=\"COLROW3\"]/td/img[1]", "//*[@id=\"COLROW4\"]/td/img[1]" };
                                string[] windows = { $"javascript:objdCalendar.dateSelected={DateTime.Now.Day};objdCalendar.selDate();", $"javascript:dateSelected={DateTime.Now.Day};closeCalendar();" };

                                string[] h = { IData.Changes[i].C_TimeStart.ToString("HH"), IData.Changes[i].C_TimeEnd.ToString("HH") };
                                string[] m = { IData.Changes[i].C_TimeStart.ToString("mm"), IData.Changes[i].C_TimeEnd.ToString("mm") };

                                webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                                //Календарь
                                //--{
                                for (int k = 0; k < frames.Length; k++)
                                {
                                    webDriver.FindElement(By.XPath(frames[k])).Click();
                                    //если календарь - нижний
                                    if (k == 1)
                                    {
                                        //переход на окно второго календаря
                                        webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                                        //часы
                                        new SelectElement(webDriver.FindElement(By.XPath("/html/body/div[1]/table[2]/tbody/tr/td/select[1]")))
                                        .SelectByIndex(int.Parse(h[k]));
                                        //минуты
                                        new SelectElement(webDriver.FindElement(By.XPath("/html/body/div[1]/table[2]/tbody/tr/td/select[2]")))
                                        .SelectByIndex(int.Parse(m[k]));
                                        //Дни
                                        webDriver.ExecuteJavaScript(windows[k]);
                                        //возврат к исходному окну
                                        webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                                    }
                                    else
                                    {
                                        //часы
                                        new SelectElement(webDriver.FindElement(By.XPath("/html/body/div[2]/table[2]/tbody/tr/td[2]/div/table[2]/tbody/tr/td/select[1]")))
                                        .SelectByIndex(int.Parse(h[k]));
                                        //минуты
                                        new SelectElement(webDriver.FindElement(By.XPath("/html/body/div[2]/table[2]/tbody/tr/td[2]/div/table[2]/tbody/tr/td/select[2]")))
                                        .SelectByIndex(int.Parse(m[k]));
                                        //Дни
                                        webDriver.ExecuteJavaScript(windows[k]);
                                    }
                                }
                                // }--
                                //Вставка имени
                                //--{
                                webDriver.FindElement(By.XPath("//tr[@id='COLROW2']//td//img")).Click();
                                webDriver.SwitchTo().Alert().Accept();

                                Thread.Sleep(500);

                                webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                                webDriver.FindElement(By.XPath("//table[@id='W_TABLE']//td//img[@src='/images/ais_sys/backg.gif']")).Click();

                                if (webDriver.FindElement(By.XPath("//select//option[@value='EMPL_LASTNAME']")).Selected)
                                {
                                    webDriver.FindElement(By.XPath("//select//option[@value='EMPL_NAME']")).Click();
                                    webDriver.FindElement(By.XPath("//select//option[@value='like']")).Click();
                                }

                                webDriver.FindElement(By.XPath("//input[@name='p_value1']")).Clear();
                                webDriver.FindElement(By.XPath("//input[@name='p_value1']")).SendKeys(newNames[j]);
                                webDriver.FindElement(By.XPath("//input[@name='bt_where']")).Click();
                                webDriver.FindElement(By.XPath("//td//a")).Click();
                                // }--
                                //Вставка работ
                                //--{
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                                webDriver.FindElement(By.XPath("//tr[@id='COLROW5']//td//img")).Click();

                                for (int k = 0; k < pairs.Count; k++)
                                {
                                    if (IData.Changes[i].C_Job == pairs.Values.ToList()[k])
                                    {
                                        webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                                        webDriver.ExecuteJavaScript($"javascript:f_sel('{pairs.Keys.ToList()[k]}')");
                                    }
                                }

                                webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                                webDriver.FindElement(By.Name("INSERT")).Click();
                                webDriver.FindElement(By.Name("BT_CLS")).Click();
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[0]);
                                webDriver.SwitchTo().ParentFrame();
                                webDriver.SwitchTo().Frame(webDriver.FindElement(By.Name("frame_2")));
                                //}--
                                progressPercentage += 1.0 / total * 100.0;
                                progress.Report(progressPercentage);
                                Thread.Sleep(200);
                            }
                            //}--
                            Thread.Sleep(500);
                            IData.Changes[i].C_Status = "Завершено";
                            webDriver?.Quit();
                            break;
                        }
                        catch (OperationCanceledException)
                        {
                            IData.Changes[i].C_Status = "Сброшено";
                            webDriver?.Quit();
                            throw new OperationCanceledException("Программа сброшена");
                        }
                        catch (Exception)
                        {
                            if (attempts == 2)
                            {
                                IData.Changes[i].C_Status = "Ошибка";
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
