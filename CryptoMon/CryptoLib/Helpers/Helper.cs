using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib.Helpers
{
    public static class Helper
    {
        public static void WriteToEventLog(Exception ex)
        {
            WriteToEventLog($"{ex.Message} \r\n {ex.InnerException} \r\n {ex.StackTrace} ");
        }
        public static void WriteToEventLog(string message)
        {
            string cs = "CryptoMon Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }
    }
}
