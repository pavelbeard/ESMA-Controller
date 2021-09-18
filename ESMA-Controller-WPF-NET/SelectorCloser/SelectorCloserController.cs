using ESMA.Chromedriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESMA.SelectorCloser
{
    public class SelectorCloserController : ChromeController
    {
        public override Task RunAsync(CancellationToken token, IProgress<double> progress)
        {
            return Task.Run(() => 
            {
                int attempts = 0;
                var progressPercentage = 0.0;

                double total = 0;

                //дальше пойдет код
            });
        }
    }
}
