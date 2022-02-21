using NUnit.Framework;
using System.IO;
using ESMA;
using ESMA.DataCollections;
using Newtonsoft.Json;
using ESMA.Tests;
using System.Collections.ObjectModel;

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
    }
}