using ESMA.Chromedriver;
using ESMA.Controllers;
using ESMA.ExcelData;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA_Controller_Tester
{
    [TestFixture]
    public class Tests
    {
        CancellationTokenSource cts = new();

        [Test]
        public void TestRunAsyncExceptionLoop()
        {
            //Assert.CatchAsync<WebDriverTimeoutException>(async () => await VideoConferenceController.RunAsync(cts.Token, 3, 4, 1));
        }
        [Test]
        public void TestRunAsync()
        {
            //VideoConferenceController.RunAsync(cts.Token, 3, 4, 0).IsCompletedSuccessfully)
            var t = Task.Run(() => { bool a = true; return a; });

            //var t1 = VideoConferenceController.RunAsync(cts.Token, 3, 4, 0).IsCompleted;

            //Assert.AreEqual(true, VideoConferenceController.RunAsync(cts.Token, 3, 4, 0).Result);
        }

        [Test]
        public void TestLengthCheck()
        {
            EmployesList el = new();

            Assert.AreEqual(13, el.Count);
        }
    }
}