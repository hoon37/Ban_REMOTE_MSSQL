using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ban_REMOTE_MSSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLog[] remoteEventLogs;
            try
            {
                remoteEventLogs = EventLog.GetEventLogs(Environment.MachineName.Trim());

                Regex rx = new Regex(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");

                foreach (EventLog log in remoteEventLogs)
                {
                    if (log.Log.Trim() == "Security")
                    {
                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (entry.InstanceId == 4625)
                            {
                                MatchCollection matches = rx.Matches(entry.Message);
                                if (matches.Count > 0)
                                    Console.WriteLine("\tMessage: " + matches[0].Value);
                                
                                //Console.WriteLine("\tMessage: " + entry.Message);
                            }
                        }
                    }

                    //if (log.Log.Trim() == "Application")
                    //{
                    //    foreach (EventLogEntry entry in log.Entries)
                    //    {
                    //        if (entry.EventID == 18456)
                    //        {
                    //            Console.WriteLine("\tMessage: " + entry.Message);
                    //        }
                    //    }
                    //}
                }
            }
            catch (InvalidOperationException)
            {

            }
        }
    }
}
