using NUnit.Framework;
using ESMA;
using ESMA.DataCollections;
using ESMA.Tests;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Linq;

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

        //��������� ���������� �����������
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
        public void TestDictionary()
        {
            var emps = new List<string>
            {
                "���������� �.�.",
                "�������� �.�.",
                "����������� �.�.",
                "������� �.�.",
                "������� �.�.",
                "��������� �.�.",
                "������ �.�.",
                "�������� �.�.",
                "��������� �.�.",
                "��������� �.�.",
                "������� �.�.",
                "������ �.�.",
                "�������� �.�."
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
                "���������� �.�.", 
                "��������� �.�.", 
                "���������� �.�.", 
                "������� �.�.", 
                "��������� �.�.", 
                "������ �.�.", 
                "������ �.�.",
                "������ �.�.",
                "������ �.�."
            };
            
            //�������� ���������� ������ � ��������
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

            //�� ����� �������� ����� ��� �������
            //var tuple = (Rows: new List<int>(), Emps: new List<string>());

            //for (int empsCounter = 0; empsCounter < names.Count; empsCounter++)
            //{
            //    for (int row = 3; row <= 14; row++)
            //    {
            //        if (emps[row - 3] == "����������� �.�.")
            //        {
            //            System.Console.WriteLine("� ����");
            //        }
            //        if (emps[row - 3] == "��������� �.�.")
            //        {
            //            System.Console.WriteLine("� ����");
            //            System.Console.WriteLine("���������");
            //            System.Console.WriteLine("�.� 4.5, 8.4, 11, 6.9, 13, 4, 20,\n24, 3, 2, 8, 5.9, 5, 5.13, �14.1 ,14.4");
            //        }
            //        //���, ������� ������� ����� � ������������ � �������
            //        if (emps[row - 3] == names[empsCounter])
            //        {
            //            System.Console.WriteLine();
            //            System.Console.WriteLine($"E{row}:E{row} ���������"); ;
            //            System.Console.WriteLine($"D{row}:D{row} �.� 4.5, 8.4, 11, 6.9, 13, 4, 20,\n24, 3, 2, 8, 5.9, 5, 5.13, �14.1 ,14.4"); ;
            //            tuple.Rows.Add(row);
            //            tuple.Emps.Add(names[empsCounter]);
            //            System.Console.WriteLine();
            //        }
            //    }
            //}

            ////����������� ������ ��� �� �������
            //for (int i = 0; i < tuple.Rows.Count; i++)
            //{

            //}
        }
    }
}