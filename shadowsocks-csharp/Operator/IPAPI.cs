using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shadowsocks.Operator
{
    class IPAPI
    {
        public static string GetIpLocation(string ip)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://ip-api.com/json/" + ip);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            string json = (string)responseString;
            JObject jsonobj = JObject.Parse(json);
            if (jsonobj.Property("countryCode") != null)
            {
                return jsonobj["countryCode"].ToString();
            }
            else
            {
                return "UNKNOWN";
            }
            
        }
    }
}
