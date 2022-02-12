using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA.ExcelData
{
    public class ReportData
    {
        public static ReportData Report { get; set; }
        public List<string> Emps { get; set; } = new();
        public List<string> Lrps { get; set; } = new();
    }
}
