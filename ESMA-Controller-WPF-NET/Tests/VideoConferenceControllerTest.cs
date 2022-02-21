using ESMA.DataCollections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.Tests
{
    public class VideoConferenceControllerTest
    {
        public ObservableCollection<EmpList> TestList { get; set; }
        public bool RunTest()
        {
           
            void StartBrowser(int index)
            {
                Console.WriteLine("Browser run");
            }
            void ChangeFrame()
            {
                Console.WriteLine("Frame is changed");
            }
            void Work(int index, int innerIndex, List<string> source)
            {
                Console.WriteLine(source[innerIndex]);
            }
            //Внесение персонала по сопровождению.
            void Escort(int index, int innerIndex, List<string> source)
            {
                Console.WriteLine(source[innerIndex]);
            }
            void CloseConference(int index)
            {
                var pairs = new Dictionary<int, string>
                    {
                        {3, "Отменено"},
                        {4, "Проведено"}
                    };

                if (true) //закрытие конфы
                {
                    Console.WriteLine("Conference is closed");
                }
            }

            for (int i = 0; i < TestList.Count; i++)
            {
                while (true)
                {
                    StartBrowser(i);
                    //oper.personal - true
                    if (true)
                    {
                        //создание нового списка имен на основе отмеченных
                        var names = TestList[i];
                        var newNames = new List<string>();

                        for (int l = 0; l < names.Count; l++)
                        {
                            if (names[l].IsChecked)
                            {
                                newNames.Add(names[l].Name);
                            }
                        }

                        ChangeFrame();
                        for (int j = 0; j < newNames.Count; j++)
                        {
                            Work(i, j, newNames);
                        }
                        if (false)
                        {
                            //новый список имен для сопровождения
                            //создание нового списка имен на основе отмеченных
                            var namesEscort = IData.VideoConferences[i].VC_Names_For_Content;
                            var newNamesEscort = new List<string>();

                            for (int l = 0; l < namesEscort.Count; l++)
                            {
                                if (namesEscort[l].IsChecked)
                                {
                                    newNamesEscort.Add(namesEscort[l].Name);
                                }
                            }
                            //close oper.personal
                            //deleted
                            //content
                            for (int k = 0; k < IData.VideoConferences[i].VC_Names_For_Content.Count; k++)
                            {
                                Escort(i, k, newNamesEscort);
                            }
                        }
                    }
                    CloseConference(i);
                    Console.WriteLine("Завершено");
                    break;
                }
            }
            return true;
        }
    }
}
