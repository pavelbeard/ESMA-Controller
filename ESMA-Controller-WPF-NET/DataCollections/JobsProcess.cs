using System.Collections.ObjectModel;

namespace ESMA.DataCollections
{
    public class JobsProcess : ObservableCollection<string>
    {
        public JobsProcess()
        {
            Add("Тестирование, проверка работоспособности");
            Add("--------------------");
        }
    }
}
