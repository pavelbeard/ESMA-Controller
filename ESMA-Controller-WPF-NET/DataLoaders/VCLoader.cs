using ESMA.Chromedriver;
using ESMA.ViewModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA.DataLoaders
{
    public partial class VCLoader : ChromeController
    {
        private DateTime _startDate;
        private DateTime _endDate;

        public override BindingList<T> LoadData<T>(IProgress<int> progress)
        {
            throw new NotImplementedException();
        }
        public override Task<BindingList<T>> LoadDataAsync<T>(IProgress<int> progress)
        {
            return Task.Run(() =>
            {
                _startDate = IData.StartDateValue;
                _endDate = IData.EndDateValue;

                try
                {
                    LoginWindow();
                    progress.Report(10);

                    webDriver.SwitchTo().Frame("top_frame");

                    var topFrame = new string[]
                    {
                        "/html/body/table/tbody/tr[1]/td[3]/table/tbody/tr[1]/td/div/a[6]",
                        "//*[@id=\"msviGlobalToolbar\"]/table/tbody/tr[1]/td[3]/a"
                    };

                    Thread.Sleep(1000);

                    for (int i = 0; i < topFrame.Length; i++)
                        webDriver.FindElement(By.XPath(topFrame[i])).Click();

                    webDriver.SwitchTo().ParentFrame();
                    webDriver.SwitchTo().Frame("main_frame");
                    webDriver.SwitchTo().Frame("top_resources_frame");

                    var selectMSK = new string[]
                    {
                        "//select[@name='SSTR_ID']",
                        "//option[@value='22']"
                    };

                    Thread.Sleep(1000);

                    foreach (var item in selectMSK)
                        webDriver.FindElement(By.XPath(item)).Click();

                    Calendar();
                    progress.Report(30);

                    webDriver.SwitchTo().ParentFrame();
                    webDriver.SwitchTo().Frame("main_resources_frame");

                    if (CheckElement(By.XPath("//*[@id='DATA_TABLE']/tbody/tr[1]/td/a[1]")))
                    {
                        webDriver.ExecuteJavaScript("javascript:click(2,0,51506);");

                        var table = new List<List<string>>();

                        string Hours(int index)
                        {
                            webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{index}]/td[2]/a")).Click();
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]);
                            webDriver.SwitchTo().Frame("CARD_FRAME_DETAIL");
                            string hours = webDriver.FindElement(By.XPath("//*[@id=\"COLROW21\"]/td")).Text != ":"
                                ?
                                webDriver.FindElement(By.XPath("//*[@id=\"COLROW21\"]/td")).Text
                                :
                                "0";
                            //allHours += hours.Length == 4 ? int.Parse(hours.Substring(0, 2)) : int.Parse(hours.Substring(0, 1));
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[1]).Close();
                            webDriver.SwitchTo().Window(webDriver.WindowHandles[0]);

                            webDriver.SwitchTo().ParentFrame();
                            webDriver.SwitchTo().Frame("main_frame");
                            webDriver.SwitchTo().Frame("main_resources_frame");
                            return hours;
                        }

                        if (_startDate == _endDate)
                        {
                            for (int i = 2; i <= int.Parse(webDriver.FindElement(By.XPath("//*[@id='DATA_TABLE']/tbody/tr[1]/td/a[2]")).Text) + 1; i++)
                            {
                                table.Add(new List<string>
                            {
                                webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[2]/a")).Text,
                                webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[3]")).Text,
                                webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[5]")).Text,
                                webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[7]")).Text,
                                $"Служба: {webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[14]")).Text}\n" +
                                $"Тема: {webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[15]")).Text}\n" +
                                $"Статус: {webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[16]")).Text}\n" +
                                $"Трудозатраты опер.персонала: {Hours(i)}"
                            });
                            }
                        }
                        else
                        {
                            webDriver.FindElement(By.XPath("//*[@id='W_ELM1']/td[2]/select/option[15]")).Click();
                            webDriver.FindElement(By.XPath("//*[@id='W_ELM1']/td[3]/select/option[13]")).Click();
                            webDriver.FindElement(By.XPath("//*[@id='W_ELM1']/td[4]/input[2]")).SendKeys("НС:РЦС");
                            webDriver.FindElement(By.XPath("/html/body/table/tbody/tr/td/table/tbody/tr/td/div/form/input[2]")).Click();
                            Thread.Sleep(500);

                            //int allHours = 0;

                            for (int i = 2; i <= int.Parse(webDriver.FindElement(By.XPath("//*[@id='DATA_TABLE']/tbody/tr[1]/td/a[2]")).Text) + 1; i++)
                            {
                                string ch = webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[14]")).Text;
                                if (ch == "НС" || ch == "РЦС")
                                {
                                    table.Add(new List<string>
                                {
                                    webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[2]/a")).Text,
                                    webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[3]")).Text,
                                    webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[5]")).Text,
                                    webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[7]")).Text,
                                    $"Служба: {webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[14]")).Text}\n" +
                                    $"Тема: {webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[15]")).Text}\n" +
                                    $"Статус: {webDriver.FindElement(By.XPath($"//*[@id='DATA_TABLE']/tbody/tr[{i}]/td[16]")).Text}\n" +
                                    $"Трудозатраты опер.персонала: {Hours(i)}"
                                });
                                }
                            }
                        }

                        progress.Report(60);
                        Thread.Sleep(1000);

                        DateTime CmpDayHour(string hour, string day)
                        {
                            if (DateTime.Parse(hour).ToString("hh") == DateTime.Parse(day).ToString("dd"))
                            {
                                return DateTime.Parse(hour).AddHours(1);
                            }
                            else
                            {
                                return DateTime.Parse(hour);
                            }
                        }

                        var toLoad = new List<VideoConference>();
                        for (int i = 0; i < table.Count; i++)
                        {
                            toLoad.Add(new VideoConference
                            {
                                IdConference = int.Parse(table[i][0]),
                                VC_Date = DateTime.Parse(table[i][1]),
                                VC_TimeStart = DateTime.Parse(table[i][2]),
                                VC_TimeEnd = CmpDayHour(table[i][3], table[i][1]),
                                VC_Theme = table[i][4],
                                VC_Names = new ObservableCollection<string>(NamesArray),
                                OperPersonal = true,
                                CloseConference = true
                            });
                        }

                        progress.Report(100);
                        Thread.Sleep(1000);
                        webDriver?.Quit();
                        progress.Report(0);

                        return _startDate == _endDate ?
                            new BindingList<T>((IList<T>)toLoad.OrderBy(x => x.IdConference).ToList()) :
                            new BindingList<T>((IList<T>)toLoad.OrderBy(x => x.VC_Date).ThenBy(x => x.IdConference).ToList());
                    }
                    else
                    {
                        webDriver?.Quit();
                        progress.Report(0);
                        throw new Exception("Конференции не созданы");
                    }
                }
                catch (Exception)
                {
                    webDriver?.Quit();
                    throw;
                }
            });
        }
        private void Calendar()
        {
            var eraseCalendar = new string[]
            {
                "//img[@onclick=\"clear_value_n('DATE_BEGIN');;\"]",
                "//img[@onclick=\"clear_value_n('DATE_END');;\"]"
            };

            Thread.Sleep(1000);

            foreach (var item in eraseCalendar)
                webDriver.FindElement(By.XPath(item)).Click();

            var fillCalendar = new string[]
            {
                "//img[@onclick=\"show_date(document.forms[0].DATE_BEGIN,0,0,'','','');;\"]",
                "//img[@onclick=\"show_date(document.forms[0].DATE_END,0,0,'','','');;\"]"
            };

            Thread.Sleep(1000);

            if ((_startDate != DateTime.MinValue && _endDate != DateTime.MinValue) || !(_startDate == _endDate))
            {
                string[] days = { _startDate.ToString("dd"), _endDate.ToString("dd") };
                string[] months = { _startDate.ToString("MM"), _endDate.ToString("MM") };

                for (int i = 0; i < fillCalendar.Length; i++)
                {
                    webDriver.FindElement(By.XPath(fillCalendar[i])).Click();
                    webDriver.FindElement(By.XPath("//span[@id='spanMonth']")).Click();
                    webDriver.FindElement(By.XPath($"//td[@id='m{int.Parse(months[i]) - 1}']")).Click();
                    webDriver.FindElement(By.XPath($"//a[@href='javascript:objdCalendar.dateSelected={int.Parse(days[i])};objdCalendar.selDate();']")).Click();
                }
            }
            else
            {
                foreach (var item in fillCalendar)
                {
                    webDriver.FindElement(By.XPath(item)).Click();
                    webDriver.FindElement(By.XPath($"//a[@href='javascript:objdCalendar.dateSelected={DateTime.Now.Day};objdCalendar.selDate();']")).Click();
                }
            }

            webDriver.FindElement(By.XPath("//a[@href=\"javascript:var p_id='5498';ch_font(p_id);document.WHERE_PLAN.OBJL_ID.value=p_id;show_plans(6536,2);;\"]")).Click();
        }
    }
}
