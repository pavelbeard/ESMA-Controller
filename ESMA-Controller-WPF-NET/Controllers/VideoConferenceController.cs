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
using System.Windows;

namespace ESMA.Controllers
{
    public partial class VideoConferenceController : ChromeController
    {
        private readonly DateTime _startDate = IData.StartDateValue;
        private readonly DateTime _endDate = IData.EndDateValue;

        public override Task RunAsync(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                WebDriverWait wait;
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
                int k = 0;
                //Если запрос на приостановление есть
                if (StopTask.PauseRequest)
                {
                    i = StopTask.CurrentI;
                    j = StopTask.CurrentJ;
                    k = StopTask.CurrentK;
                    time = StopTask.CurrentTime;
                    progressPercentage = (double)StopTask.CurrentData;
                    progress.Report((double)StopTask.CurrentData);
                    StopTask.PauseRequest = token.IsCancellationRequested;
                }

                void StartBrowser(int index)
                {
                    webDriver = new ChromeDriver(cds, chromeOptions);
                    webDriver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 10);
                    wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
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
                //Внесение персонала по сопровождению.
                void Escort(int index, int innerIndex)
                {
                    webDriver.SwitchTo().Window(webDriver.WindowHandles[0]);
                    webDriver.SwitchTo().Frame(webDriver.FindElement(By.CssSelector("[name='CARD_FRAME_MARK']")));
                    //wait for element
                    var element = wait.Until(d => d.FindElement(By.XPath("/html/body/div/table[1]/tbody/tr/td/table/tbody/tr/td[6]/a")));
                    //click
                    webDriver.FindElement(By.XPath("/html/body/div/table[1]/tbody/tr/td/table/tbody/tr/td[6]/a")).Click();
                    webDriver.FindElement(By.CssSelector("input[value='Добавить']")).Click();
                    //change window
                    webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                    //wait for element
                    element = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"W_TABLE\"]/tbody/tr[1]/td/img[2]")));
                    //click on img
                    webDriver.FindElement(By.XPath("//*[@id=\"W_TABLE\"]/tbody/tr[1]/td/img[2]")).Click();
                    //click on input element
                    webDriver.FindElement(By.CssSelector("input[type='text']")).Click();
                    //send keys
                    webDriver.FindElement(By.CssSelector("input[type='text']"))
                        .SendKeys(IData.VideoConferences[index].VC_Names_For_Content[innerIndex]);
                    //request
                    webDriver.FindElement(By.CssSelector("input[value='Запрос']")).Click();
                    //wait for element
                    element = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"1\"]/td[2]/input")));
                    //click checkbox
                    webDriver.FindElement(By.XPath("//*[@id=\"1\"]/td[2]/input")).Click();
                    //save
                    webDriver.FindElement(By.CssSelector("input[value='Сохранить']")).Click();
                    //change on 0 frame
                    webDriver.SwitchTo().Window(webDriver.WindowHandles[0]);
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
                                k = StopTask.CurrentK;
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
                                if (IData.VideoConferences[i].Escort)
                                {
                                    //close oper.personal
                                    webDriver.FindElement(By.CssSelector("input[value='Закрыть']")).Click();
                                    //content
                                    for (; k < IData.VideoConferences[i].VC_Names_For_Content.Count; k++)
                                    {
                                        token.ThrowIfCancellationRequested();

                                        Escort(i, k);

                                    }
                                }
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
                                StopTask.CurrentK = k;
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
                            StopTask.CurrentK = k;
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
}
