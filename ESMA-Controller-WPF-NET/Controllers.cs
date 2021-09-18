using ESMA.Chromedriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ESMA.Controllers
{
    public partial class VideoConferenceController : ChromeController
    {
        private readonly DateTime _startDate = IData.StartDateValue;
        private readonly DateTime _endDate = IData.EndDateValue;

        public Task TestProgressBar(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                int attempts = 0;
                int time = 0;
                bool allOperPersonalNeeded = IData.VideoConferences.All(x => x.OperPersonal == false);
                var progressPercentage = 0.0;

                IEnumerable<int> PercentageMax()
                {
                    if (!allOperPersonalNeeded)
                    {
                        foreach (var c in IData.VideoConferences.Where(x => x.OperPersonal == true))
                        {
                            yield return c.VC_Names.Count;
                        }
                    }
                    else
                    {
                        yield return IData.VideoConferences.Count;
                    }
                }

                double total = PercentageMax().Sum();

                int i = 0;
                int j = 0;

                if (StopTask.PauseRequest)
                {
                    i = StopTask.CurrentI;
                    j = StopTask.CurrentJ;
                    time = StopTask.CurrentTime;
                    progressPercentage = (double)StopTask.CurrentData;
                    progress.Report((double)StopTask.CurrentData);
                    StopTask.PauseRequest = token.IsCancellationRequested;
                }

                void StartBrowser(int index)
                {
                    IData.Window.Dispatcher.Invoke(() => IData.Window.StatusBar.Content = $"index = {index}");
                    Thread.Sleep(1000);
                }
                void ChangeFrame()
                {
                    Thread.Sleep(1000);
                }
                void Work(int index, int innerIndex)
                {
                    IData.Window.Dispatcher.Invoke(() => IData.Window.StatusBar.Content = $"index = {index}, innerIndex = {j}");
                    Thread.Sleep(1000);
                }
                void CloseConference(int index)
                {
                    IData.Window.Dispatcher.Invoke(() => IData.Window.StatusBar.Content = $"index = {index}");
                    Thread.Sleep(1000);
                }

                for (; i < IData.VideoConferences.Count; i++)
                {
                    Label1:
                    try
                    {
                        if (StopTask.Exception)
                        {
                            j = StopTask.CurrentJ;
                            time = StopTask.CurrentTime;
                            progressPercentage = (double)StopTask.CurrentData;
                            progress.Report((double)StopTask.CurrentData);
                            StopTask.Exception = false;
                        }

                        token.ThrowIfCancellationRequested();

                        StartBrowser(i);
                        if (IData.VideoConferences[i].OperPersonal)
                        {
                            ChangeFrame();
                            for (; j < IData.VideoConferences[i].VC_Names.Count; j++)
                            {
                                token.ThrowIfCancellationRequested();

                                time += IData.VideoConferences[i].VC_TimeEnd.Subtract(IData.VideoConferences[i].VC_TimeStart).Hours;
                                IData.Window.Dispatcher.Invoke(()
                                    => IData.Window.Info.Text
                                    = $"{IData.VideoConferences[i].IdConference}\n" +
                                      $"{IData.VideoConferences[i].VC_Theme}\n" +
                                      $"{IData.VideoConferences[i].VC_Names[j]}\n" +
                                      $"Всего времени: {TimeSpan.FromHours(time).Days}д:" +
                                      $"{TimeSpan.FromHours(time).Hours}ч:" +
                                      $"{TimeSpan.FromHours(time).Minutes}м:" +
                                      $"{TimeSpan.FromHours(time).Seconds}с");

                                Work(i, j);
                                progressPercentage += 1.0 / total * 100.0;
                                progress.Report(progressPercentage);
                            }
                            j = 0;
                        }
                        else if (allOperPersonalNeeded)
                        {
                            progressPercentage += 1.0 / total * 100.0;
                            progress.Report(progressPercentage);
                        }
                        CloseConference(i);
                        IData.VideoConferences[i].VC_Status = "Завершено";
                        IData.Window.Dispatcher.Invoke(() => IData.Window.Info.Text = default);
                    }
                    catch (OperationCanceledException)
                    {
                        if (StopTask.PauseRequest)
                        {
                            StopTask.PauseRequest = token.IsCancellationRequested;
                            StopTask.CurrentI = i;
                            StopTask.CurrentJ = j;
                            StopTask.CurrentTime = time;
                            StopTask.CurrentData = progressPercentage;
                            IData.VideoConferences[i].VC_Status = "Приостановлено";
                            MessageBox.Show("Программа остановлена", "Пауза", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        if (StopTask.StopRequest)
                        {
                            IData.VideoConferences[i].VC_Status = "Сброшено";
                            MessageBox.Show("Программа сброшена", "Стоп", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        break;
                    }
                    catch (Exception e)
                    {
                        StopTask.Exception = true;
                        StopTask.CurrentJ = j;
                        StopTask.CurrentTime = time;
                        StopTask.CurrentData = progressPercentage;
                        if (attempts == 2)
                        {
                            attempts = 0;
                            MessageBox.Show($"{e}");
                            IData.VideoConferences[i].VC_Status = "Ошибка";
                            break;
                        }
                        attempts++;
                        webDriver?.Quit();
                        goto Label1;
                    }
                }
            });
        }

        public override Task RunAsync(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                int attempts = 0;
                int time = 0;
                bool allOperPersonalNeeded = IData.VideoConferences.All(x => x.OperPersonal == false);
                var progressPercentage = 0.0;

                IEnumerable<int> PercentageMax()
                {
                    if (!allOperPersonalNeeded)
                    {
                        foreach (var c in IData.VideoConferences.Where(x => x.OperPersonal == true))
                        {
                            yield return c.VC_Names.Count;
                        }
                    }
                    else
                    {
                        yield return IData.VideoConferences.Count;
                    }
                }
                
                double total = PercentageMax().Sum();
                //Новые значения счетчиков конференций и имен
                int i = 0;
                int j = 0;
                //Если запрос на приостановление есть
                if (StopTask.PauseRequest)
                {
                    i = StopTask.CurrentI;
                    j = StopTask.CurrentJ;
                    time = StopTask.CurrentTime;
                    progressPercentage = (double)StopTask.CurrentData;
                    progress.Report((double)StopTask.CurrentData);
                    StopTask.PauseRequest = token.IsCancellationRequested;
                }

                void StartBrowser(int index)
                {
                    webDriver = new ChromeDriver(cds, chromeOptions);
                    webDriver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 10);
                    webDriver.Navigate().GoToUrl($"http://10.23.218.250:7780/pls/portal30/!ais_sys.dyn_obj_card.load?OBJ_ID=6536&WIN_TYPE=4&DATA_VALUE={IData.VideoConferences[index].IdConference}");    //<-Необходимо добавить цикл и переменную номера совещаний в строку
                    webDriver.FindElement(By.Name("p_username")).SendKeys(Login);
                    webDriver.FindElement(By.Name("p_password")).SendKeys(Password);
                    webDriver.FindElement(By.TagName("button")).Click();
                    webDriver.Navigate().Refresh();
                }
                void ChangeFrame()
                {
                    //Переключение на фрейм
                    webDriver.SwitchTo().ParentFrame();
                    webDriver.SwitchTo().Frame(webDriver.FindElement(By.CssSelector("[name='CARD_FRAME_MARK']")));
                    webDriver.FindElement(By.CssSelector("a[href=\"javascript:redirect('4');\"]")).Click();
                    Thread.Sleep(500);
                    webDriver.FindElement(By.XPath(".//*[text()='Оперативный персонал']/.")).Click();
                    webDriver.FindElement(By.XPath("//input[@name='BT_GEN']")).Click();
                    Thread.Sleep(500);
                }
                void Work(int index, int innerIndex)
                {
                    ArrayList arrayList = new()
                    {
                        new List<string>()
                        {
                            "//tr[@id='COLROW_CONF_RESP_TIMEB']//td//img[@src='/images/list.gif']",
                            "//tr[@id='COLROW_CONF_RESP_TIMEE']//td//img[@src='/images/list.gif']"
                        },
                        new List<int>()
                        {
                            int.Parse(IData.VideoConferences[index].VC_TimeStart.ToString("HH")),
                            int.Parse(IData.VideoConferences[index].VC_TimeEnd.ToString("HH"))

                        },
                        new List<int>()
                        {
                            int.Parse(IData.VideoConferences[index].VC_TimeStart.ToString("mm")),
                            int.Parse(IData.VideoConferences[index].VC_TimeEnd.ToString("mm"))
                        }
                    };

                    for (int cr = 0; cr < 2; cr++)
                    {
                        webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                        //Редактор дат
                        var dateEdit = arrayList[0] as List<string>;
                        Thread.Sleep(500);
                        webDriver.FindElement(By.XPath(dateEdit[cr])).Click();
                        //Окно календаря
                        webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                        //Часы
                        var hours = arrayList[1] as List<int>;
                        var selectHours = new SelectElement(webDriver.FindElement(By.XPath("/html/body/div[1]/table[2]/tbody/tr/td/select[1]")));
                        selectHours.SelectByIndex(hours[cr]);
                        //-//webDriver.FindElement(By.XPath($"//*[contains(@value, '{hours[cr]}')]")).Click();
                        //Минуты
                        var minutes = arrayList[2] as List<int>;
                        selectHours = new SelectElement(webDriver.FindElement(By.XPath("/html/body/div[1]/table[2]/tbody/tr/td/select[2]")));
                        selectHours.SelectByIndex(minutes[cr]);
                        //Месяцы
                        webDriver.FindElement(By.XPath("//*[@id=\"spanMonth\"]")).Click();
                        webDriver.FindElement(By.XPath($"//*[@id=\"m{int.Parse(IData.VideoConferences[index].VC_Date.ToString("MM")) - 1}\"]")).Click();
                        Thread.Sleep(300);
                        //Дни
                        webDriver.ExecuteJavaScript($"dateSelected={IData.VideoConferences[index].VC_Date:dd};closeCalendar();");
                    }

                    webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                    webDriver.FindElement(By.XPath("//tr[@id='COLROW_EMPL_NAME']//img[@src='/images/list.gif']")).Click();
                    webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                    webDriver.FindElement(By.XPath("//table[@id='W_TABLE']//td//img[@src='/images/ais_sys/backg.gif']")).Click();

                    if (webDriver.FindElement(By.XPath("//select//option[@value='EMPL_LASTNAME']")).Selected)
                    {
                        webDriver.FindElement(By.XPath("//select//option[@value='EMPL_NAME']")).Click();
                        webDriver.FindElement(By.XPath("//select//option[@value='like']")).Click();
                    }

                    webDriver.FindElement(By.XPath("//input[@name='p_value1']")).Clear();
                    webDriver.FindElement(By.XPath("//input[@name='p_value1']")).SendKeys(IData.VideoConferences[index].VC_Names[innerIndex]);
                    webDriver.FindElement(By.XPath("//input[@name='bt_where']")).Click();
                    webDriver.FindElement(By.XPath("//td//a")).Click();
                    webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                    webDriver.FindElement(By.XPath("//tr[@id='COLROW_W_TEXT']//img")).Click();
                    webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);

                    Thread.Sleep(300);

                    webDriver.FindElement(By.XPath("//td//input[@type='text']")).SendKeys(IData.VideoConferences[index].VC_Job);
                    webDriver.FindElement(By.XPath("//td//input[@value='Найти']")).Click();

                    var pairs = new Dictionary<int, string>
                    {
                        {94,    "Совещания" },
                        {129,   "Контроль проведения совещания/конференции" },
                        {131,   "Организация совещания/конференции" }
                    };

                    for (int i = 0; i < pairs.Count; i++)
                    {
                        if (pairs.Values.ToList()[i] == IData.VideoConferences[index].VC_Job)
                        {
                            webDriver.ExecuteJavaScript($"javascript:f_sel('{pairs.Keys.ToList()[i]}')");
                        }
                    }

                    Thread.Sleep(1000);
                    webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                    webDriver.FindElement(By.CssSelector("input[value='Добавить']")).Click();
                    Thread.Sleep(500);
                }
                void CloseConference(int index)
                {
                    var pairs = new Dictionary<int, string>
                    {
                        {3, "Отменено"},
                        {4, "Проведено"}
                    };

                    if (IData.VideoConferences[index].CloseConference) //закрытие конфы
                    {
                        void Choose()
                        {
                            //webDriver.FindElement(By.XPath("//select[@id='CONF_STATUS_ID']")).Click();
                            for (int i = 0; i < pairs.Count; i++)
                            {
                                if (IData.VideoConferences[index].CloseCode == pairs.Values.ToList()[i])
                                {
                                    webDriver.FindElement(By.XPath($"//tr[@id='COLROW_CONF_STATUS_ID']//td//select//option[@value='{pairs.Keys.ToList()[i]}']")).Click();
                                }
                            }
                            webDriver.FindElement(By.XPath("//tr[@id='COLROW_USER_NAME_OPERATOR']//td//img")).Click();
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                            webDriver.FindElement(By.XPath("//table[@id='W_TABLE']//td//img[@src='/images/ais_sys/backg.gif']")).Click();

                            if (webDriver.FindElement(By.XPath("//select//option[@value='EMPL_LASTNAME']")).Selected)
                            {
                                webDriver.FindElement(By.XPath("//select//option[@value='EMPL_NAME']")).Click();
                                webDriver.FindElement(By.XPath("//select//option[@value='like']")).Click();
                            }

                            webDriver.FindElement(By.XPath("//input[@name='p_value1']")).Clear();
                            webDriver.FindElement(By.XPath("//input[@name='p_value1']")).SendKeys(IData.VideoConferences[index].VC_Names[^1]);
                            webDriver.FindElement(By.XPath("//input[@name='bt_where']")).Click();
                            webDriver.FindElement(By.XPath("//td//a")).Click();
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                            webDriver.FindElement(By.CssSelector("[name='UPDATE_TOP']")).Click();
                        }
                        void Conducted()
                        {
                            if (IData.VideoConferences[index].OperPersonal)
                            {
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[1]).Close();
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[0]);
                            }

                            webDriver.SwitchTo().ParentFrame();
                            webDriver.SwitchTo().Frame(webDriver.FindElement(By.CssSelector("[name='CARD_FRAME_DETAIL']")));
                            webDriver.FindElement(By.Name("BT_MODIFY")).Click();

                            ArrayList arrayList = new ArrayList
                            {
                                new List<string>
                                {
                                    "//tr[@id='COLROW_CONF_TIMEB_F']//td//img",
                                    "//tr[@id='COLROW_CONF_TIMEE_F']//td//img"
                                },
                                new List<string>
                                {
                                    IData.VideoConferences[index].VC_TimeStart.ToString("HH"),
                                    IData.VideoConferences[index].VC_TimeEnd.ToString("HH")
                                },
                                new List<string>
                                {
                                    IData.VideoConferences[index].VC_TimeStart.ToString("mm"),
                                    IData.VideoConferences[index].VC_TimeEnd.ToString("mm")
                                },
                            };

                            for (int n = 0, w = 1; n < 2; n++)
                            {
                                var colRows = arrayList[0] as List<string>;
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[w]);
                                webDriver.FindElement(By.XPath(colRows[n])).Click();
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[++w]);

                                var h = arrayList[1] as List<string>;
                                webDriver.FindElement(By.XPath($"//select[@name='HH']//option[text()='{h[n]}']")).Click();
                                var m = arrayList[2] as List<string>;
                                webDriver.FindElement(By.XPath($"//select[@name='MI']//option[text()='{m[n]}']")).Click();
                                #region FOR_FIRE_EVENT
                                //if (int.Parse(h[n]) <= 9)
                                //    webDriver.FindElement(By.XPath($"//select[@name='HH']//option[text()='0{int.Parse(h[n])}']")).Click();
                                //else
                                //    webDriver.FindElement(By.XPath($"//select[@name='HH']//option[text()='{int.Parse(h[n])}']")).Click();

                                //if (int.Parse(m[n]) <= 9)
                                //    webDriver.FindElement(By.XPath($"//select[@name='MI']//option[text()='0{int.Parse(m[n])}']")).Click();
                                //else
                                //    webDriver.FindElement(By.XPath($"//select[@name='MI']//option[text()='{int.Parse(m[n])}']")).Click();
                                #endregion

                                webDriver.FindElement(By.CssSelector("[name='BT_SEL']")).Click();
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[--w]);
                            }
                            Choose();
                        }
                        void Canceled()
                        {
                            if (IData.VideoConferences[index].OperPersonal)
                            {
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[1]).Close();
                                webDriver.SwitchTo().Window(webDriver.WindowHandles[0]);
                            }

                            webDriver.SwitchTo().ParentFrame();
                            webDriver.SwitchTo().Frame(webDriver.FindElement(By.CssSelector("[name='CARD_FRAME_DETAIL']")));

                            webDriver.FindElement(By.Name("BT_MODIFY")).Click();

                            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                            Choose();
                        }

                        switch (IData.VideoConferences[index].CloseCode)
                        {
                            case "Проведено": Conducted(); break;
                            case "Отменено": Canceled(); break;
                            default:
                                break;
                        }
                    }
                }

                for (; i < IData.VideoConferences.Count; i++)
                {

                    while (true)
                    {
                        try
                        {
                            //Если произошло исключение по вине ЕСМА
                            if (StopTask.Exception)
                            {
                                j = StopTask.CurrentJ;
                                time = StopTask.CurrentTime;
                                progressPercentage = (double)StopTask.CurrentData;
                                progress.Report((double)StopTask.CurrentData);
                                StopTask.Exception = false;
                            }

                            token.ThrowIfCancellationRequested();
                            StartBrowser(i);
                            if (IData.VideoConferences[i].OperPersonal)
                            {
                                ChangeFrame();
                                for (; j < IData.VideoConferences[i].VC_Names.Count; j++)
                                {
                                    token.ThrowIfCancellationRequested();

                                    time += IData.VideoConferences[i].VC_TimeEnd.Subtract(IData.VideoConferences[i].VC_TimeStart).Hours;
                                    IData.Window.Dispatcher.Invoke(()
                                        => IData.Window.Info.Text
                                        = $"{IData.VideoConferences[i].IdConference}\n" +
                                          $"{IData.VideoConferences[i].VC_Theme}\n" +
                                          $"{IData.VideoConferences[i].VC_Names[j]}\n" +
                                          $"Всего времени: {TimeSpan.FromHours(time).Days}д:" +
                                          $"{TimeSpan.FromHours(time).Hours}ч:" +
                                          $"{TimeSpan.FromHours(time).Minutes}м:" +
                                          $"{TimeSpan.FromHours(time).Seconds}с");

                                    Work(i, j);
                                    progressPercentage += 1.0 / total * 100.0;
                                    progress.Report(progressPercentage);
                                }
                                j = 0;
                            }
                            else if (allOperPersonalNeeded)
                            {
                                IData.Window.Dispatcher.Invoke(()
                                        => IData.Window.Info.Text
                                        = $"*Без внесения опер. персонала" +
                                          $"{IData.VideoConferences[i].IdConference}\n" +
                                          $"{IData.VideoConferences[i].VC_Theme}\n");

                                progressPercentage += 1.0 / total * 100.0;
                                progress.Report(progressPercentage);
                            }
                            CloseConference(i);
                            IData.VideoConferences[i].VC_Status = "Завершено";
                            webDriver?.Quit();
                            break;

                        }
                        catch (OperationCanceledException)
                        {
                            if (StopTask.PauseRequest)
                            {
                                StopTask.PauseRequest = token.IsCancellationRequested;
                                StopTask.CurrentI = i;
                                StopTask.CurrentJ = j;
                                StopTask.CurrentTime = time;
                                StopTask.CurrentData = progressPercentage;
                                IData.VideoConferences[i].VC_Status = "Приостановлено";
                                webDriver?.Quit();
                                throw;
                            }
                            else if (StopTask.StopRequest)
                            {
                                IData.VideoConferences[i].VC_Status = "Сброшено";
                                webDriver?.Quit();
                                throw;
                            }
                            else
                            {
                                webDriver?.Quit();
                                throw;
                            }
                        }
                        catch (WebDriverTimeoutException)
                        {
                            IData.VideoConferences[i].VC_Status = "Ошибка";
                            webDriver?.Quit();
                            StopTask.NoConnectionException = true;
                            throw;
                        }
                        catch (Exception)
                        {
                            StopTask.Exception = true;
                            StopTask.CurrentJ = j;
                            StopTask.CurrentTime = time;
                            StopTask.CurrentData = progressPercentage;
                            if (attempts == 2)
                            {
                                IData.VideoConferences[i].VC_Status = "Ошибка";
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
    public class ChangesController : ChromeController
    {
        public Task TestReport(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                ReportData reportData = new();
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
                                      $"{IData.Changes[i].C_Names[j]}");

                            //}--
                            //Вставка имени в отчет
                            reportData.Emps.Add(IData.Changes[i].C_Names[j]);

                            progressPercentage += 1.0 / total * 100.0;
                            progress.Report(progressPercentage);
                            Thread.Sleep(700);
                        }
                        //Вставка работы в отчет
                        reportData.Lrps.Add(IData.Changes[i].IdChanges.ToString());
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
                ReportData.Report = reportData;
            });
        }

        public override Task RunAsync(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                ReportData reportData = new();
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

                            //Внесение опер персонала
                            //--{
                            for (int j = 0; j < IData.Changes[i].C_Names.Count; j++)
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
                                webDriver.FindElement(By.XPath("//input[@name='p_value1']")).SendKeys(IData.Changes[i].C_Names[j]);
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
                                //Вставка имени в отчет
                                reportData.Emps.Add(IData.Changes[i].C_Names[j]);

                                progressPercentage += 1.0 / total * 100.0;
                                progress.Report(progressPercentage);
                                Thread.Sleep(200);
                            }
                            //Вставка работы в отчет
                            reportData.Lrps.Add(IData.Changes[i].IdChanges.ToString());
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
                ReportData.Report = reportData;
            });
        }
    }
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
                                          $"{IData.Processes[i].P_Names[namesCount]}");
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
                            webDriver.FindElement(By.XPath($"//*[contains(@href, '{IData.Processes[i].P_Names[namesCount].Split(" ")[0]}')]")).Click(); //Вставка имени
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

    public partial class VideoConferenceController : ChromeController
    {
        #region DEBUG
#if RELEASE
        public Task Debug(string fd)
        {
            return Task.Run(() => 
            {
                int attempts = 0;

                void StartBrowser(int index)
                {
                    webDriver = new ChromeDriver(cds, chromeOptions);
                    webDriver.Navigate().GoToUrl($"http://10.23.218.250:7780/pls/portal30/!ais_sys.dyn_obj_card.load?OBJ_ID=6536&WIN_TYPE=4&DATA_VALUE={IData.VideoConferences[index].IdConference}");    //<-Необходимо добавить цикл и переменную номера совещаний в строку
                    webDriver.FindElement(By.Name("p_username")).SendKeys(Login);
                    webDriver.FindElement(By.Name("p_password")).SendKeys(Password);
                    webDriver.FindElement(By.TagName("button")).Click();
                    webDriver.Navigate().Refresh();
                }
                void Edit(int index)
                {
                    int countOfFields = int.Parse(webDriver.FindElement(By.XPath("/html/body/table/tbody/tr/td/table/tbody/tr/td/div/nobr/span")).Text.Select(x => x).Where(s => char.IsDigit(s)).ToArray());
                    for (int i = 1; i <= countOfFields; i++)
                    {
                        webDriver.FindElement(By.XPath($"//*[@id={i}]/td[3]/a")).Click();

                        Thread.Sleep(1000);
                        ArrayList arrayList = new ArrayList
                            {
                                new List<string>()
                                {
                                    "//tr[@id='COLROW_CONF_RESP_TIMEB']//td//img[@src='/images/list.gif']",
                                    "//tr[@id='COLROW_CONF_RESP_TIMEE']//td//img[@src='/images/list.gif']"
                                },
                                new List<int>()
                                {
                                    int.Parse(IData.VideoConferences[index].VC_TimeStart.ToString("HH")),
                                    int.Parse(IData.VideoConferences[index].VC_TimeEnd.ToString("HH"))

                                },
                                new List<int>()
                                {
                                    int.Parse(IData.VideoConferences[index].VC_TimeStart.ToString("mm")),
                                    int.Parse(IData.VideoConferences[index].VC_TimeEnd.ToString("mm"))
                                }
                            };


                        webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                        //Редактор дат
                        var dateEdit = arrayList[0] as List<string>;

                        DateTime a1 = DateTime.Parse(webDriver.FindElement(By.XPath("//*[@id=\"CONF_RESP_TIMEB\"]")).GetAttribute("value"));
                        DateTime a2 = DateTime.Parse(webDriver.FindElement(By.XPath("//*[@id=\"CONF_RESP_TIMEE\"]")).GetAttribute("value"));

                        //Дни
                        if (a1.Day != a2.Day)
                        {
                            webDriver.FindElement(By.XPath(dateEdit[0])).Click();
                            //Окно календаря
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                            //Месяцы
                            webDriver.FindElement(By.XPath("//*[@id=\"spanMonth\"]")).Click();
                            webDriver.FindElement(By.XPath($"//*[@id=\"m{int.Parse(IData.VideoConferences[index].VC_Date.ToString("MM")) - 1}\"]")).Click();
                            //Дата для переделки: например с 1 числа на 8 
                            webDriver.FindElement(By.XPath($"{fd}")).Click();
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                        }
        #region old
                        //if (IData.VideoConferences[index].VC_Date.ToString("dd") == $"{fd}")
                        //{
                        //    //Дата для переделки: например с 1 числа на 8 
                        //    webDriver.FindElement(By.XPath($"{sd}")).Click();
                        //}
                        //else
                        //{
                        //    webDriver.FindElement(By.XPath($"{sd}")).Click();
                        //}
                        //if (webDriver.FindElement(By.XPath($"//*[@id=\"content\"]/table/tbody/tr[4]/td[7]/a")).Text.Trim() !=
                        //    IData.VideoConferences[index].VC_Date.ToString("dd"))
                        //{
                        //    webDriver.FindElement(By.XPath($"//*[@href='javascript:dateSelected={IData.VideoConferences[index].VC_Date.ToString("dd")};closeCalendar();']")).Click();
                        //}
                        //else
                        //{
                        //    webDriver.SwitchTo().Window(webDriver.WindowHandles[2]).Close();
                        //} 
        #endregion

        #region old
                        //for (int cr = 0; cr < 2; cr++)
                        //{
                        //    webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                        //    //Редактор дат
                        //    var dateEdit = arrayList[0] as List<string>;

                        //    DateTime a1 = DateTime.Parse(webDriver.FindElement(By.XPath("//*[@id=\"CONF_RESP_TIMEB\"]")).GetAttribute("value"));
                        //    DateTime a2 = DateTime.Parse(webDriver.FindElement(By.XPath("//*[@id=\"CONF_RESP_TIMEE\"]")).GetAttribute("value"));

                        //    webDriver.FindElement(By.XPath(dateEdit[cr])).Click();
                        //    //Окно календаря
                        //    webDriver.SwitchTo().Window(webDriver.WindowHandles[2]);
                        //    //Месяцы
                        //    webDriver.FindElement(By.XPath("//*[@id=\"spanMonth\"]")).Click();
                        //    webDriver.FindElement(By.XPath($"//*[@id=\"m{int.Parse(IData.VideoConferences[index].VC_Date.ToString("MM")) - 1}\"]")).Click();
                        //    //Дни
                        //    if (a1.Day != a2.Day)
                        //    {
                        //        //Дата для переделки: например с 1 числа на 8 
                        //        webDriver.FindElement(By.XPath($"{sd}")).Click();
                        //    }
                        //    //if (IData.VideoConferences[index].VC_Date.ToString("dd") == $"{fd}")
                        //    //{
                        //    //    //Дата для переделки: например с 1 числа на 8 
                        //    //    webDriver.FindElement(By.XPath($"{sd}")).Click();
                        //    //}
                        //    //else
                        //    //{
                        //    //    webDriver.FindElement(By.XPath($"{sd}")).Click();
                        //    //}
                        //    //if (webDriver.FindElement(By.XPath($"//*[@id=\"content\"]/table/tbody/tr[4]/td[7]/a")).Text.Trim() !=
                        //    //    IData.VideoConferences[index].VC_Date.ToString("dd"))
                        //    //{
                        //    //    webDriver.FindElement(By.XPath($"//*[@href='javascript:dateSelected={IData.VideoConferences[index].VC_Date.ToString("dd")};closeCalendar();']")).Click();
                        //    //}
                        //    //else
                        //    //{
                        //    //    webDriver.SwitchTo().Window(webDriver.WindowHandles[2]).Close();
                        //    //}
                        //} 
        #endregion


                        if (a1.Day != a2.Day)
                        {
                            webDriver.FindElement(By.XPath("/html/body/table/tbody/tr/td/table/tbody/tr/td/div/form/input[12]")).Click();
                            webDriver.FindElement(By.XPath("/html/body/table/tbody/tr/td/table/tbody/tr/td/div/form/input[14]")).Click();
                        }
                        else
                        {
                            webDriver.FindElement(By.XPath("/html/body/table/tbody/tr/td/table/tbody/tr/td/div/form/input[14]")).Click();
                        }
                        webDriver.SwitchTo().Window(webDriver.WindowHandles[0]);

                        webDriver.SwitchTo().ParentFrame();
                        webDriver.SwitchTo().Frame(webDriver.FindElement(By.CssSelector("[name='CARD_FRAME_MARK']")));
                        Thread.Sleep(500);
                    }

                    webDriver?.Quit();
                }
                void ChangeFrameDebug()
                {
                    //Переключение на фрейм
                    webDriver.SwitchTo().ParentFrame();
                    webDriver.SwitchTo().Frame(webDriver.FindElement(By.CssSelector("[name='CARD_FRAME_MARK']")));

                    webDriver.FindElement(By.CssSelector("a[href=\"javascript:redirect('4');\"]")).Click();
                    Thread.Sleep(500);
                    webDriver.FindElement(By.XPath(".//*[text()='Оперативный персонал']/.")).Click();
                    Thread.Sleep(500);
                }

                for (int i = 0; i < IData.VideoConferences.Count; i++)
                {
                label1:
                    try
                    {
                        StartBrowser(i);
                        ChangeFrameDebug();
                        Edit(i);
                        webDriver?.Quit();
                        IData.VideoConferences[i].VC_Status = "Завершено";
                    }
                    catch (Exception exception)
                    {
                        Action temp = () =>
                        {
                            webDriver?.Quit();
                            MessageBox.Show($"{exception}");
                        };

                        if (attempts == 2)
                        {
                            attempts = 0;
                            temp();
                            IData.VideoConferences[i].VC_Status = "Ошибка";
                            webDriver?.Quit();
                            return;
                        }
                        attempts++;
                        webDriver?.Quit();
                        goto label1;
                    }
                }
            });
        }
#endif
        #endregion
        #if DEBUG
        public static Task<bool> RunAsync(CancellationToken token, int videoConferences, int videoConferencesNames, int testException)
        {
            return Task.Run(() =>
            {
                int attempts = 0;
                int time = 0;
               
                //Новые значения счетчиков конференций и имен
                int i = 0;
                int j = 0;
                //Если запрос на приостановление есть
                if (StopTask.PauseRequest)
                {
                    i = StopTask.CurrentI;
                    j = StopTask.CurrentJ;
                    time = StopTask.CurrentTime;

                    StopTask.PauseRequest = token.IsCancellationRequested;
                }

                void StartBrowser(int index)
                {
                    Trace.WriteLine("BrowserStarted");
                    Thread.Sleep(1000);
                    if (testException == 1)
                    {
                        throw new WebDriverTimeoutException("test1");
                    }
                }
                void ChangeFrame()
                {
                    //Переключение на фрейм
                    Trace.Write("FrameChanged");
                    Thread.Sleep(500);
                }
                void Work(int index, int innerIndex)
                {
                    Trace.WriteLine("Working...");
                    Thread.Sleep(500);
                    if (testException == 2)
                    {
                        throw new Exception("esmaException");
                    }
                }
                void CloseConference(int index)
                {
                    Trace.WriteLine("Conference closed");
                    Thread.Sleep(1000);
                }


                for (; i < videoConferences; i++)
                {
                    while (true)
                    {
                        try
                        {
                            //Если произошло исключение по вине ЕСМА
                            if (StopTask.Exception)
                            {
                                j = StopTask.CurrentJ;
                                time = StopTask.CurrentTime;
                                StopTask.Exception = false;
                            }

                            token.ThrowIfCancellationRequested();
                            StartBrowser(i);
                            if (true)
                            {
                                ChangeFrame();
                                for (; j < videoConferencesNames; j++)
                                {
                                    token.ThrowIfCancellationRequested();
                                    Work(i, j);
                                }
                                j = 0;
                            }
                            CloseConference(i);
                            break;
                        }
                        catch (OperationCanceledException)
                        {
                            if (StopTask.PauseRequest)
                            {
                                StopTask.PauseRequest = token.IsCancellationRequested;
                                StopTask.CurrentI = i;
                                StopTask.CurrentJ = j;
                                StopTask.CurrentTime = time;
                                throw;
                            }
                            if (StopTask.StopRequest)
                            {
                                throw;
                            }
                        }
                        catch (WebDriverTimeoutException)
                        {
                            StopTask.NoConnectionException = true;
                            throw;
                        }
                        catch (Exception)
                        {
                            StopTask.Exception = true;
                            StopTask.CurrentJ = j;
                            StopTask.CurrentTime = time;
                            if (attempts == 2)
                            {
                                Trace.WriteLine("????");
                                throw;
                            }
                            attempts++;
                        } 
                    }
                }
                return true;
            });
        }
        #endif
    }

}
