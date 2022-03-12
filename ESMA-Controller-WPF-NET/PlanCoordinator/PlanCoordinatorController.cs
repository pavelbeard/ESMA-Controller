using ESMA.Chromedriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA
{
    public class PlanCoordinatorController : ChromeController
    {
        public Task<bool> Coordinate(IProgress<double> progress)
        {
            return Task.Run(() => 
            {
                //Логинимся
                LoginWindow("http://10.23.218.250:7790/pls/portal30/!ais_sys.dyn_home_page.show");
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(5));
                //раскрываем окно полностью
                webDriver.Manage().Window.Maximize();
                //Организация и планирование
                var element = wait.Until(el => el.FindElement(By.XPath("//*[@id=\"menu_body\"]/div[1]/ul/li[4]/table/tbody/tr/td[2]/a")));
                element.Click();
                //Суточное планирование работ
                element = wait.Until(el => el.FindElement(By.XPath("//*[@id=\"mod_4\"]/li[2]/table/tbody/tr/td[2]/a")));
                element.Click();
                //Смена фрейма
                webDriver.SwitchTo().Frame("main_frame");
                //Нажимаем на выпадающий список
                Thread.Sleep(1000);
                element = wait.Until(el => el.FindElement(By.XPath("//*[@id=\"select2-currentTeamName-container\"]")));
                element.Click();
                //ищем рвб-11
                element = wait.Until(el => el.FindElement(By.XPath("/html/body/span/span/span[1]/input")));
                element.SendKeys("рвб-11");
                //Выбираем связь совещаний - РВБ-11
                webDriver.FindElement(By.XPath("/html/body/span/span/span[2]/ul/li")).Click();
                Thread.Sleep(750);

                bool CheckNoData()
                {
                    try
                    {
                        webDriver.FindElement(By.XPath("//*[@id=\"noData\"]"));
                        webDriver?.Quit();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                int once = 0;

                while (true)
                {
                    //проверяем конец плана
                    if (CheckNoData()) break;
                    //Если в статусе плана на день написано "Согласован" - жмем дальше
                    if (webDriver.FindElement(By.XPath("/html/body/form/div[1]/div/div[2]/div[3]/div[2]")).Text == "Согласован")
                    {
                        webDriver.FindElement(By.XPath("/html/body/form/div[1]/div/div[2]/div[1]/img[2]")).Click();
                        Thread.Sleep(500);
                    }
                    else
                    {
                        //список развертывания
                        var expands = webDriver.FindElements(By.XPath("//*[@title='Развернуть']"));
                        //add counts to list
                        var rowsCnt = new List<int>();

                        for (int i = 0; i < expands.Count; i++)
                        {
                            rowsCnt
                                .Add(int
                                .Parse(webDriver
                                .FindElement(By.XPath($"//*[@id=\"DATA_TABLE\"]/tbody/tr[{i + 1}]/td/div[3]/span/b"))
                                .Text));
                        }
                        //развертываем те списки, где больше нуля
                        for (int i = 0; i < expands.Count; i++)
                        {
                            if (rowsCnt[i] > 0)
                            {
                                expands[i].Click();
                            }
                        }
                        //меняем время
                        //список начальных значений времени
                        var times = webDriver.FindElements(By.XPath("//*[@name='time_begin']"));
                        //заполняем эти поля
                        var hour = new StringBuilder("08");
                        for (int i = 0, j = 10; i < times.Count; i++, j++)
                        {
                            //если минуты будут равны 59 - "обнуляем"
                            if (j == 59)
                            {
                                j = 10;
                                hour = new StringBuilder("09");
                            }

                            times[i].SendKeys($"");
                        }
                        //меняем тип транспорта
                        var typeTrans = webDriver.FindElements(By.XPath("//*[@name='type_trans']"));
                        //change
                        foreach (var item in typeTrans)
                        {
                            var select = new SelectElement(item);
                            select.SelectByIndex(0);
                        }

                        //список работников для сверки
                        var N = webDriver.FindElements(By.XPath("//*[contains(@name, 'emplName')]"));
                        //переменная числа, обозначающая референта
                        int refr = 0;

                        for (int i = 1; i <= N.Count; i++)
                        {
                            //в дальнейшем здесь будет вываливаться список референтов
                            if (N[i - 1].Text == "Степанов Михаил Александрович")
                            {
                                refr = i;
                            }
                        }
                        //Выбираем ст/элмех. ОДИН РАЗ
                        Thread.Sleep(1000);
                        if (once == 0)
                        {
                            webDriver.FindElement(By.XPath($"//*[@id=\"DATA_TABLE\"]/tbody/tr[{refr}]/td/div[1]/div/img")).Click();
                            once++;
                        }
                        else
                        {
                            var el = webDriver.FindElement(By.XPath("//*[text()='Степанов Михаил Александрович']"));
                            new Actions(webDriver).MoveToElement(el).Perform();
                        }
                        Thread.Sleep(1000);
                        //считаем количество строк в таблице ст/элмех.
                        var t = webDriver.FindElement(By.XPath($"//*[@id=\"DATA_TABLE\"]/tbody/tr[{refr}]/td/div[3]/span")).Text;
                        //общее кол-во строк
                        int rows = 0;

                        var matches = Regex.Matches(t, @"\d+").ToList();

                        if (matches.Count == 1)
                        {
                            rows = int.Parse(matches[0].Value);
                        }
                        else
                        {
                            rows = int.Parse(matches[0].Value) + int.Parse(matches[1].Value);
                        }
                        //если строк нет, то составляем план
                        if (rows != 0)
                        {
                            //выбираем и вставляем следующее время
                            for (int i = 0; i < rows; i++)
                            {
                                webDriver.FindElement(By.XPath($"//*[@id=\"DATA_TABLE\"]/tbody/tr[{refr + 1 + i}]/td[10]")).Click(); 
                                webDriver.FindElement(By.XPath($"//*[@id=\"DATA_TABLE\"]/tbody/tr[{refr + 1 + i}]/td[10]/input[@name=\"time_begin\"]")).SendKeys($"08{10 + i}");
                            }
                            //проверяем поле на наличие жд-транспорта и другой фигни
                            for (int i = 0; i < rows + 1; i++)
                            {
                                var field7 = webDriver.FindElement(By.XPath($"//*[@id=\"DATA_TABLE\"]/tbody/tr[{refr + 1 +  i}]/td[7]/select"));
                                var select = new SelectElement(field7);
                                select.SelectByIndex(0);
                                Thread.Sleep(200);
                            } 
                        }
                        //нажимаем сохранить
                        webDriver.FindElement(By.XPath("/html/body/form/div[1]/div/div[3]/div[1]/button[1]")).Click();
                        Thread.Sleep(1000);
                        //На согласование
                        webDriver.FindElement(By.XPath("/html/body/form/div[1]/div/div[3]/div[1]/button[4]")).Click();
                        Thread.Sleep(500);
                        //Соглашаемся
                        webDriver.SwitchTo().Alert().Accept();
                        Thread.Sleep(800);
                        //Согласовать
                        webDriver.FindElement(By.XPath("/html/body/form/div[1]/div/div[3]/div[1]/button[5]")).Click();
                        Thread.Sleep(1000);
                        //Диалоговое окно
                        List<IWebElement> frames = new(webDriver.FindElements(By.TagName("iframe")));
                        webDriver.SwitchTo().Frame(frames[0].GetAttribute("id").ToString());
                        Thread.Sleep(500);
                        //OK
                        webDriver.FindElement(By.XPath("/html/body/div[3]/div/a[1]")).Click();
                        Thread.Sleep(500);
                        //возврат на mainframe
                        webDriver.SwitchTo().DefaultContent();
                        webDriver.SwitchTo().Frame("main_frame");
                    }
                }

                return true;
            });
        }
    }
}
