using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.ExcelData
{
    public class EmployesList
    {
        public int Count { get; set; }
        public List<string> FullNameList { get; set; } = new();
        public List<string> ShortNameList { get; set; } = new();

        public void LengthCheck()
        {
            if (FullNameList.Count > ShortNameList.Count)
            {
                for (int i = 0; i < FullNameList.Count - ShortNameList.Count; i++)
                {
                    ShortNameList.Add("Иванов И.И.");
                }
            }
            else
            {
                for (int i = 0; i < ShortNameList.Count - FullNameList.Count; i++)
                {
                    FullNameList.Add("Иванов Иван Иванович");
                }
            }

            Count = ShortNameList.Count;
        }
    }
}
