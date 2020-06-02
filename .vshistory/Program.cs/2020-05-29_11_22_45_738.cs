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

            remoteEventLogs = EventLog.GetEventLogs(Environment.MachineName.Trim());

            Console.WriteLine("Number of logs on computer: " + remoteEventLogs.Length);

            foreach (EventLog log in remoteEventLogs)
            {
                Console.WriteLine("Log: " + log.Log);
            }
        }
    }
}
