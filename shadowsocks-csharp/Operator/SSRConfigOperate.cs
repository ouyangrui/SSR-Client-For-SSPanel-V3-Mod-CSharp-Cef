using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks
{
    class SSRConfigOperate
    {
        public class Server
        {
            public string remarks { get; set; }

            public string id { get; set; }

            public string server { get; set; }

            public int server_port { get; set; }

            public int server_udp_port { get; set; }

            public string password { get; set; }

            public string method { get; set; }

            public string protocol { get; set; }

            public string protocolparam { get; set; }

            public string obfs { get; set; }

            public string obfsparam { get; set; }

            public string remarks_base64 { get; set; }

            public string group { get; set; }

            public bool enable { get; set; }

            public bool udp_over_tcp { get; set; }
        }

        public class Config
        {
            List<Server> Server = new List<Server>();
            public List<Server> configs
            {
                get { return Server; }
                set { Server = value; }
            }

            public int index { get; set; }

            public bool random { get; set; }

            public int sysProxyMode { get; set; }

            public bool shareOverLan { get; set; }

            public int localPort { get; set; }

            public string localAuthPassword { get; set; }

            public string dnsServer { get; set; }

            public int reconnectTimes { get; set; }

            public int randomAlgorithm { get; set; }

            public bool randomInGroup { get; set; }

            public int TTL { get; set; }

            public int connectTimeout { get; set; }

            public int proxyRuleMode { get; set; }

            public bool proxyEnable { get; set; }

            public bool pacDirectGoProxy { get; set; }

            public int proxyType { get; set; }

            public int proxyHost { get; set; }

            public int proxyPort { get; set; }

            public int proxyAuthUser { get; set; }

            public int proxyAuthPass { get; set; }

            public int proxyUserAgent { get; set; }

            public int authUser { get; set; }

            public int authPass { get; set; }

            public bool autoBan { get; set; }

            public bool sameHostForSameTarget { get; set; }

            public int keepVisitTime { get; set; }

            public bool isHideTips { get; set; }

            public bool nodeFeedAutoUpdate { get; set; }

            List<ServerSubscribes> ServerSubscribes = new List<ServerSubscribes>();
            public List<ServerSubscribes> serverSubscribes
            {
                get { return ServerSubscribes; }
                set { ServerSubscribes = value; }
            }

            List<Token> Token = new List<Token>();
            public List<Token> token
            {
                get { return Token; }
                set { Token = value; }
            }

            List<PortMap> PortMap = new List<PortMap>();
            public List<PortMap> portMap
            {
                get { return PortMap; }
                set { PortMap = value; }
            }
        }

        public class ServerSubscribes
        {

        }

        public class Token
        {

        }

        public class PortMap
        {

        }

        public string JsonConfigGenerate(string ip, int mode = 1)
        {
            try
            {

                List<Config> listconfig = new List<Config>();
                Config config = new Config();
                List<Server> server = new List<Server>();
                Server servernode1 = new Server();
                servernode1.remarks = "";
                servernode1.id = "";
                servernode1.server = ip;
                servernode1.server_port = GlobalVaribles.GlobalVaribles.LOGIN_port;
                servernode1.server_udp_port = 0;
                servernode1.password = GlobalVaribles.GlobalVaribles.LOGIN_password;
                servernode1.method = GlobalVaribles.GlobalVaribles.LOGIN_method;
                servernode1.protocol = GlobalVaribles.GlobalVaribles.LOGIN_protocol;
                servernode1.protocolparam = "";
                servernode1.obfs = GlobalVaribles.GlobalVaribles.LOGIN_obfs;
                servernode1.obfsparam = "";
                servernode1.remarks_base64 = "";
                servernode1.group = "G41ProxyClient";
                servernode1.enable = true;
                servernode1.udp_over_tcp = false;
                server.Add(servernode1);
                config.configs = server;
                config.index = 0;
                config.random = true;
                config.sysProxyMode = 3;
                config.shareOverLan = false;
                config.localPort = 1088;
                config.localAuthPassword = "";
                config.dnsServer = "";
                config.reconnectTimes = 2;
                config.randomAlgorithm = 3;
                config.randomInGroup = false;
                config.TTL = 0;
                config.connectTimeout = 5;
                config.proxyRuleMode = mode;
                config.proxyEnable = false;
                config.pacDirectGoProxy = false;
                config.proxyType = 0;
                config.proxyPort = 0;
                config.autoBan = false;
                config.sameHostForSameTarget = false;
                config.keepVisitTime = 180;
                config.isHideTips = false;
                config.nodeFeedAutoUpdate = true;
                List<ServerSubscribes> listserverSubscribes = new List<ServerSubscribes>();
                ServerSubscribes serverSubscribes = new ServerSubscribes();
                listserverSubscribes.Add(serverSubscribes);
                config.serverSubscribes = listserverSubscribes;
                List<Token> listtoken = new List<Token>();
                Token token = new Token();
                listtoken.Add(token);
                config.token = listtoken;
                List<PortMap> listportMap = new List<PortMap>();
                PortMap portMap = new PortMap();
                listportMap.Add(portMap);
                config.portMap = listportMap;
                return Newtonsoft.Json.JsonConvert.SerializeObject(config);
            }
            catch (Exception exception)
            {
                // Dialog dlg = new Dialog("提示", exception.ToString());
                // dlg.ShowDialog();
                return "{}";
            }
        }
    }
}
