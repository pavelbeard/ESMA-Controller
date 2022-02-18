using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESMA
{
    public static class StopTask
    {
        public static bool IsScan { get; set; }
        public static bool PauseRequest { get; set; }
        public static bool StopRequest { get; set; }
        public static bool Exception { get; set; }
        public static bool NoConnectionException { get; set; }
        public static int CurrentI { get; set; }
        public static int CurrentJ { get; set; }
        public static int CurrentTime { get; set; }
        public static object CurrentData { get; set; }
        public static int CurrentK { get; internal set; }
    }
}
