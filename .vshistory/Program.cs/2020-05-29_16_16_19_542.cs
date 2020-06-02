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
            }

            //로그가져오기
            EventLog[] remoteEventLogs;

            try
            {
                remoteEventLogs = EventLog.GetEventLogs(Environment.MachineName.Trim());

                Regex rx = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

                string IPs = string.Empty;

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
                                    IPs += matches[0].Value + ", ";
                                    //FirewallAPI.AddInboudRuleIPBlock("REMOTE_BAN-" + matches[0].Value, FirewallAPI.Protocol.Any, matches[0].Value);
                                    FirewallAPI.AddInboudRuleIPBlock("REMOTE_BAN-" + matches[0].Value, FirewallAPI.Protocol.Any, "100.100.100.100,111.111.111.111");
                                }
                                    //Console.WriteLine("\tSecurity: " + matches[0].Value);
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
                                    Console.WriteLine("\tApplication: " + matches[0].Value);
                            }
                        }
                    }
                }

                //if (!IPs.Trim().Equals(string.Empty))
                //{
                //    IPs = IPs.Substring(0, IPs.Length - 2);
                //    FirewallAPI.AddInboudRuleIPBlock("REMOTE_BAN", FirewallAPI.Protocol.Any, IPs);
                //}
            }
            catch (Exception ex)
            {
                ErrorLog.WriteError(ex.Message);
            }
        }
    }
}
