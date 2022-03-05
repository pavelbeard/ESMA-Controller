using NUnit.Framework;
using ESMA;
using ESMA.DataCollections;
using ESMA.Tests;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Linq;
using System;

namespace ESMA_Controller_Tester
{
    [TestFixture]
    public class Tests
    {
        #region old
        //CancellationTokenSource cts = new();

        //[Test]
        //public void TestRunAsyncExceptionLoop()
        //{
        //    //Assert.CatchAsync<WebDriverTimeoutException>(async () => await VideoConferenceController.RunAsync(cts.Token, 3, 4, 1));
        //}
        //[Test]
        //public void TestRunAsync()
        //{
        //    //VideoConferenceController.RunAsync(cts.Token, 3, 4, 0).IsCompletedSuccessfully)
        //    var t = Task.Run(() => { bool a = true; return a; });

        //    //var t1 = VideoConferenceController.RunAsync(cts.Token, 3, 4, 0).IsCompleted;

        //    //Assert.AreEqual(true, VideoConferenceController.RunAsync(cts.Token, 3, 4, 0).Result);
        //} 
        #endregion

        //Тестируем контроллер конференций
        [Test]
        public void TestVCController()
        {
            var list1 = new EmpList(ConfigData.NamesListFileJSON);
            list1[1].IsChecked = true;
            list1[2].IsChecked = true;
            list1[3].IsChecked = true;

            var list2 = new EmpList(ConfigData.NamesListFileJSON);
            list2[4].IsChecked = true;
            list2[5].IsChecked = true;
            list2[6].IsChecked = true;

            var list3 = new EmpList(ConfigData.NamesListFileJSON);
            list3[7].IsChecked = true;
            list3[8].IsChecked = true;
            list3[9].IsChecked = true;

            VideoConferenceControllerTest vcct = new();
            vcct.TestList = new ObservableCollection<EmpList> { list1, list2, list3};


            Assert.AreEqual(true, vcct.RunTest());
        }
        [Test]
        public void TestChangesController()
        {
            var listDt = new List<DateTime>();

            for (int i = 0; i < 13; i++)
            {
                listDt.Add(DateTime.Parse("00:00"));
            }

            var list1 = new EmpListChanges(ConfigData.NamesListFileJSON, listDt, listDt);
            list1[1].IsChecked = true; list1[1].TimeStart = DateTime.Parse("14:55"); list1[1].TimeEnd = DateTime.Parse("15:30");
            list1[2].IsChecked = true; list1[2].TimeStart = DateTime.Parse("14:25"); list1[2].TimeEnd = DateTime.Parse("15:00");
            list1[3].IsChecked = true; list1[3].TimeStart = DateTime.Parse("14:15"); list1[3].TimeEnd = DateTime.Parse("15:12");

            var list2 = new EmpListChanges(ConfigData.NamesListFileJSON, listDt, listDt);
            list2[4].IsChecked = true; list2[4].TimeStart = DateTime.Parse("13:55"); list2[4].TimeEnd = DateTime.Parse("14:30");
            list2[5].IsChecked = true; list2[5].TimeStart = DateTime.Parse("13:25"); list2[5].TimeEnd = DateTime.Parse("14:00");
            list2[6].IsChecked = true; list2[6].TimeStart = DateTime.Parse("13:15"); list2[6].TimeEnd = DateTime.Parse("14:12");

            var list3 = new EmpListChanges(ConfigData.NamesListFileJSON, listDt, listDt);
            list3[7].IsChecked = true; list3[7].TimeStart = DateTime.Parse("12:55"); list3[7].TimeEnd = DateTime.Parse("13:30");
            list3[8].IsChecked = true; list3[8].TimeStart = DateTime.Parse("12:25"); list3[8].TimeEnd = DateTime.Parse("13:00");
            list3[9].IsChecked = true; list3[9].TimeStart = DateTime.Parse("12:15"); list3[9].TimeEnd = DateTime.Parse("13:12");

            ChangesControllerTest cct = new();
            cct.List = new ObservableCollection<EmpListChanges> { list1, list2, list3 };

            Assert.AreEqual(true, cct.RunTest());
        }
        [Test]
        public void TestDictionary()
        {
            var emps = new List<string>
            {
                "Достойнова О.Г.",
                "Пчелкина Ю.М.",
                "Жаворонкина Н.В.",
                "Носкина Е.А.",
                "Жданова Н.В.",
                "Васильева И.А.",
                "Жукова Ю.М.",
                "Кутакова Н.М.",
                "Степачева И.Н.",
                "Глубокова Е.Н.",
                "Бородин П.А.",
                "Хромов Д.А.",
                "Степанов М.А."
            };

            var lrps = new List<string>() 
            { 
                "1121", 
                "1121", 
                "1122", 
                "1122", 
                "1122", 
                "1121", 
                "1122",
                "1123",
                "1121"
            };
            var names = new List<string>() 
            { 
                "Достойнова О.Г.", 
                "Васильева И.А.", 
                "Достойнова О.Г.", 
                "Бородин П.А.", 
                "Васильева И.А.", 
                "Хромов Д.А.", 
                "Жукова Ю.М.",
                "Хромов Д.А.",
                "Жукова Ю.М."
            };
            
            //алгоритм связывания ключей и значений
            var dictionary = new Dictionary<string, List<string>>();
            for (int i = 0; i < names.Count; i++)
            {
                if (dictionary.ContainsKey(names[i]))
                {
                    dictionary[names[i]].Add(lrps[i]);
                }
                else
                {
                    dictionary[names[i]] = new List<string>
                    {
                        lrps[i]
                    };
                }
            }

            foreach (var d in dictionary)
            {
                System.Console.WriteLine($"Key: {d.Key}");
                for (int i = 0; i < d.Value.Count; i++)
                {
                    System.Console.WriteLine($"Value: {d.Value[i]}");
                }
            }
        }
    }
}