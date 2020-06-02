using nowLibrary;
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
            bool r = false;
            bool m = false;

            int fc = 3;

            //초기화
            try {
                ///https://github.com/Enichan/Ini
                var ini = new IniFile();
                ini.Load("ban_settng.ini");

                r = ini["Application"]["REMOTE"].ToBool();
                m = ini["Application"]["MSSQL"].ToBool();

                fc = ini["Rule"]["FailedCount"].ToInt();
            }
            catch (Exception ex) {
                ErrorLog.WriteError(ex.Message);
                return;
            }

            //로그가져오기
            EventLog[] remoteEventLogs;

            try
            {
                remoteEventLogs = EventLog.GetEventLogs(Environment.MachineName.Trim());

                Regex rx = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

                StringBuilder IPs = new StringBuilder(string.Empty);

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
                                {
                                    IPs.Append(matches[0].Value + ",");
                                    //FirewallAPI.AddInboudRuleIPBlock("REMOTE_BAN-" + matches[0].Value, FirewallAPI.Protocol.Any, matches[0].Value);
                                }
                            }
                        }
                    }

                    if (log.Log.Trim() == "Application")
                    {
                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (entry.InstanceId == 3221243928)
                            {
                                MatchCollection matches = rx.Matches(entry.Message);
                                if (matches.Count > 0)
                                    IPs.Append(matches[0].Value + ","); //Console.WriteLine("\tApplication: " + matches[0].Value);
                            }
                        }
                    }
                }

                if (!IPs.ToString().Trim().Equals(string.Empty)) {
                    FirewallAPI.RemoveInboundRule("***REMOTE_BAN***");
                    IPs.Remove(IPs.Length - 1, 1);
                    FirewallAPI.AddInboudRuleIPBlock("***REMOTE_BAN***", FirewallAPI.Protocol.Any, IPs.ToString());
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteError(ex.Message);
            }
        }
    }
}
