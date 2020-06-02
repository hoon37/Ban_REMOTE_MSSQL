using nowLibrary;
using System;
using System.Collections;
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
            Dictionary<string, int> ipTable = new Dictionary<string, int>();
            int cnt = 0;

            try
            {
                remoteEventLogs = EventLog.GetEventLogs(Environment.MachineName.Trim());

                Regex rx = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

                foreach (EventLog log in remoteEventLogs)
                {
                    if (log.Log.Trim() == "Security" && r)
                    {
                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (entry.InstanceId == 4625)
                            {
                                MatchCollection matches = rx.Matches(entry.Message);
                                if (matches.Count > 0)
                                {
                                    if (ipTable.ContainsKey(matches[0].Value))
                                        ipTable[matches[0].Value] += 1;
                                    else
                                        ipTable.Add(matches[0].Value, 1);

                                    //FirewallAPI.AddInboudRuleIPBlock("REMOTE_BAN-" + matches[0].Value, FirewallAPI.Protocol.Any, matches[0].Value);
                                }
                            }
                        }
                    }

                    if (log.Log.Trim() == "Application" && m)
                    {
                        foreach (EventLogEntry entry in log.Entries)
                        {
                            if (entry.InstanceId == 3221243928)
                            {
                                MatchCollection matches = rx.Matches(entry.Message);
                                if (matches.Count > 0)
                                {
                                    if (ipTable.ContainsKey(matches[0].Value))
                                        ipTable[matches[0].Value] += 1;
                                    else
                                        ipTable.Add(matches[0].Value, 1);
                                }
                            }
                        }
                    }
                }

                StringBuilder IPs = new StringBuilder(string.Empty);
                foreach(KeyValuePair<string, int> item in ipTable) {
                    if (item.Value >= fc)
                        IPs.Append(item.Key).Append(',');
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
