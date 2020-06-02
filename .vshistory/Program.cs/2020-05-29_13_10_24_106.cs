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

                Regex rx = new Regex(@"(((\d{1,2})|(1\d{2}})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2}})|(1\d{2})|(2[0-4]\d)|(25[0-5]))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                foreach (EventLog log in remoteEventLogs)
                {
                    if (log.Log.Trim() == "Security")
                    {
                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (entry.InstanceId == 4625)
                            {
                                MatchCollection matches = rx.Matches(entry.Message);
                                Console.WriteLine("\tMessage: " + matches[0].Value);
                                
                                //Console.WriteLine("\tMessage: " + entry.Message);
                            }
                        }
                    }

                    if (log.Log.Trim() == "Application")
                    {
                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (entry.InstanceId == 3221243928)
                            {
                                Console.WriteLine("\tMessage: " + entry.Message);
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {

            }
        }
    }
}
