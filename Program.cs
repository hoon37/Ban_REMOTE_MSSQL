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

            const string FirewallName = "***REMOTE_BAN***";
            const string RemoteCategory = "Security";
            const string MSSQLCategory = "Application";
            string[] whitelist;

            //초기화
            try {
                ///https://github.com/Enichan/Ini
                var ini = new IniFile();
                ini.Load("ban_setting.ini");

                r = ini["Application"]["REMOTE"].ToBool();
                m = ini["Application"]["MSSQL"].ToBool();

                fc = ini["Rule"]["FailedCount"].ToInt();

                whitelist = ini["whitelist"]["ip"].ToString().Split(',');
            }
            catch (Exception ex) {
                ErrorLog.WriteError(ex.Message);
                return;
            }

            //로그가져오기
            //EventLog[] remoteEventLogs;
            Dictionary<string, int> ipTable = new Dictionary<string, int>();

            try
            {
                EventLog[] ele = Array.FindAll(EventLog.GetEventLogs(Environment.MachineName.Trim()), x => ((x.Log.Trim() == RemoteCategory && r) || (x.Log.Trim() == MSSQLCategory && m)));

                Regex rx = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

                foreach (EventLog log in ele)
                {
                    IEnumerable<EventLogEntry> Iele = log.Entries.Cast<EventLogEntry>().Where(x => x.InstanceId == 4625 || x.InstanceId == 3221243928);

                    foreach (EventLogEntry entry in Iele)
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

                    //foreach (EventLogEntry entry in log.Entries)
                    //{
                    //    if (entry.InstanceId == 4625 || entry.InstanceId == 3221243928)
                    //    {
                    //        MatchCollection matches = rx.Matches(entry.Message);

                    //        if (matches.Count > 0)
                    //        {
                    //            if (ipTable.ContainsKey(matches[0].Value))
                    //                ipTable[matches[0].Value] += 1;
                    //            else
                    //                ipTable.Add(matches[0].Value, 1);

                    //            //FirewallAPI.AddInboudRuleIPBlock("REMOTE_BAN-" + matches[0].Value, FirewallAPI.Protocol.Any, matches[0].Value);
                    //        }
                    //    }
                    //}
                }

                //foreach (EventLog log in remoteEventLogs) {
                //    if ((log.Log.Trim() == "Security" && r) || (log.Log.Trim() == "Application" && m)) {
                //        foreach (EventLogEntry entry in log.Entries) {
                //            if (entry.InstanceId == 4625 || entry.InstanceId == 3221243928) {
                //                MatchCollection matches = rx.Matches(entry.Message);

                //                if (matches.Count > 0) {
                //                    if (ipTable.ContainsKey(matches[0].Value))
                //                        ipTable[matches[0].Value] += 1;
                //                    else
                //                        ipTable.Add(matches[0].Value, 1);

                //                    //FirewallAPI.AddInboudRuleIPBlock("REMOTE_BAN-" + matches[0].Value, FirewallAPI.Protocol.Any, matches[0].Value);
                //                }
                //            }
                //        }
                //    }
                //}

                StringBuilder IPs = new StringBuilder(string.Empty);
                System.Collections.Generic.IEnumerable<KeyValuePair<string, int>> items = ipTable.Where(x => x.Value >= fc);
                foreach (KeyValuePair<string, int> item in items)
                    IPs.Append(item.Key).Append(',');

                //foreach (KeyValuePair<string, int> item in ipTable) {
                //    if (item.Value >= fc)
                //        IPs.Append(item.Key).Append(',');
                //}

                if (!IPs.ToString().Trim().Equals(string.Empty))
                {
                    foreach (string ip in FirewallAPI.GetBlockIP(FirewallName).Split(','))
                        IPs.Append(ip.Trim()).Append(',');

                    FirewallAPI.RemoveInboundRule(FirewallName);
                    IPs.Remove(IPs.Length - 1, 1);

                    if (whitelist != null) {
                        foreach (string whiteip in whitelist)
                            IPs.Replace(whiteip, string.Empty);
                        IPs.Replace(",,", ",");
                    }

                    FirewallAPI.AddInboudRuleIPBlock(FirewallName, FirewallAPI.Protocol.Any, IPs.ToString());
                }
            }
            catch (Exception ex)
            {
                ErrorLog.WriteError(ex.Message);
            }
        }
    }
}
