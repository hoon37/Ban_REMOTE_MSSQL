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
                            if (entry.InstanceId == 4625)
                            {
                                Console.WriteLine("\tMessage: " + entry.Message);
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
