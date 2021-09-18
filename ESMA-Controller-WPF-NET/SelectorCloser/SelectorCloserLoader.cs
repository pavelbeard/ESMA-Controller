using ESMA.Chromedriver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.SelectorCloser
{
    public class SelectorCloserLoader : ChromeController
    {
        public override Task<BindingList<T>> LoadDataAsync<T>(IProgress<int> progress)
        {
            try
            {
                //дальше пойдет код
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
