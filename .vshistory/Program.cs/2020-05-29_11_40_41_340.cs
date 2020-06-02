using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

                foreach (EventLog log in remoteEventLogs)
                {
                    if (log.Log.Trim() == "Security")
                    {
                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (entry.EventID == 4625)
                            {
                                Console.WriteLine("\tMessage: " + entry.InstanceId.ToString());
                            }
                        }
                    }

                    if (log.Log.Trim() == "Application")
                    {
                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (entry.EventID == 18456)
                            {
                                Console.WriteLine("\tMessage: " + entry.InstanceId.ToString());
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
