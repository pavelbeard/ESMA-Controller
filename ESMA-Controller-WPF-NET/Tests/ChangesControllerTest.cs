using ESMA.DataCollections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.Tests
{
    public class ChangesControllerTest
    {
        public ObservableCollection<EmpListChanges> List { get; set; }
        public bool RunTest()
        {
            var pairs = new Dictionary<int, string>
                {
                    {15,    "Тестирование, проверка работоспособности" },
                    {132,   "Дополнительные работы при проведении совещания/конференции" },
                    {129,   "Контроль проведения совещания/конференции" },
                    {131,   "Организация совещания/конференции" },
                    {116,   "Другие работы" }
                };

            for (int i = 0; i < List.Count; i++)
            {
                while (true)
                {
                    try
                    {
                        //Запрос на приостановку
                        void NewSession()
                        {
                            //Новая сессия
                            Console.WriteLine("New Session");
                        }
                        void ChangeFrame()
                        {
                            //Смена фрейма
                            //--{
                            Console.WriteLine("Change Frame");
                            //}--
                        }

                        NewSession();
                        ChangeFrame();

                        //формирование нового списка имен на основе отмеченных
                        var names = List[i];
                        var newNames = new List<string>();
                        var newTimeStart = new List<DateTime>();
                        var newTimeEnd = new List<DateTime>();

                        for (int j = 0; j < names.Count; j++)
                        {
                            if (names[j].IsChecked)
                            {
                                newNames.Add(names[j].Name);
                                newTimeStart.Add(names[j].TimeStart);
                                newTimeEnd.Add(names[j].TimeEnd);
                            }
                        }

                        //Внесение опер персонала
                        //--{
                        for (int j = 0; j < newNames.Count; j++)
                        {
                            string[] hour = { newTimeStart[j].ToString("HH"), newTimeEnd[j].ToString("HH") };
                            string[] min = { newTimeStart[j].ToString("mm"), newTimeEnd[j].ToString("mm") };

                            //Календарь
                            //--{
                            for (int k = 0; k < 2; k++)
                            {
                                //если календарь - нижний
                                if (k == 1)
                                {
                                    //переход на окно второго календаря
                                    //часы
                                    Console.WriteLine($"Time start: {hour[k]}:{min[k]}");
                                    //минуты
                                }
                                else
                                {
                                    //часы
                                    Console.WriteLine($"Time end: {hour[k]}:{min[k]}");
                                    //минуты
                                }
                            }
                            // }--
                            //Вставка имени
                            //--{
                            Console.WriteLine(newNames[j]);
                            // }--
                            //Вставка работ
                            //--{
                            //for (int k = 0; k < pairs.Count; k++)
                            //{
                            //    if (IData.Changes[i].C_Job == pairs.Values.ToList()[k])
                            //    {
                            //        Console.WriteLine(pairs.Keys.ToList()[k]);
                            //    }
                            //}
                        }
                        //}--
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("operationCanceledException");
                        return false;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Exception");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
