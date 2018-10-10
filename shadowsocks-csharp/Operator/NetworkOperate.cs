using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Shadowsocks
{
    class NetworkOperate
    {
        public long PingNode(string adress)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "ping test data";
            byte[] buf = Encoding.ASCII.GetBytes(data);
            PingReply reply = pingSender.Send(adress, 1999, buf, options);
            int time = 0;
            while (time < 3)
            {
                if (reply.Status == IPStatus.Success)
                {
                    return reply.RoundtripTime;
                }
                time++;
            }
            return 9999;
        }
    }
}
