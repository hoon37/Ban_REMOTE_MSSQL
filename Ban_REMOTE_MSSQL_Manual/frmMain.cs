using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ban_REMOTE_MSSQL_Manual
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private static INetFwPolicy2 _firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

        public enum Protocol
        {
            Tcp = 6, Udp = 0x11, Any = 0x100
        }

        private void btnSetup_Click(object sender, EventArgs e)
        {
            string FirewallName = "***REMOTE_BAN***";
            string IPs = txtIPs.Text.Trim().Replace(" ", string.Empty);

            IPs += "," + GetBlockIP(FirewallName).Trim();

            RemoveInboundRule(FirewallName);
            AddInboudRuleIPBlock(FirewallName, Protocol.Any, IPs.ToString());
        }

        private static bool IsInboundRuleExist(string name)
        {
            return _firewallPolicy.Rules.Cast<INetFwRule>().Any(r => r.Name == name);
        }

        public static string GetBlockIP(string name)
        {
            if (IsInboundRuleExist(name))
                return (from r in _firewallPolicy.Rules.Cast<INetFwRule>() where r.Name == name select r.RemoteAddresses).First();
            else
                return string.Empty;
        }

        public static bool RemoveInboundRule(string name)
        {
            if (IsInboundRuleExist(name) == false)
                return true;

            INetFwRule rule = _firewallPolicy.Rules.Cast<INetFwRule>().FirstOrDefault(r => r.Name == name);

            if (rule != null)
                _firewallPolicy.Rules.Remove(name);

            return IsInboundRuleExist(name) == false;
        }

        public static bool AddInboudRuleIPBlock(string name, Protocol protocol, string ip)
        {
            if (IsInboundRuleExist(name) == true)
                return true;

            INetFwRule newRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            newRule.Name = name;
            newRule.Protocol = (int)protocol;
            newRule.InterfaceTypes = "All";
            newRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            newRule.Enabled = true;
            newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            newRule.RemoteAddresses = ip;
            _firewallPolicy.Rules.Add(newRule);

            return IsInboundRuleExist(name);
        }
    }
}
