using CefSharp;
using CefSharp.WinForms;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Shadowsocks.Operator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shadowsocks.CefUI
{
    public partial class MainCef : Form
    {
        public MainCef()
        {
            InitializeComponent();
        }
        public static ChromiumWebBrowser CWebBrowser;
        private void MainCef_Load(object sender, EventArgs e)
        {
            InitCWebBroswer();
            if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "cli-config.json"))
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "cli-config.json", "{}");
           }
        }
        private void InitCWebBroswer()
        {
            //"file:///" + AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + "/ui/main/index.html"
            CWebBrowser = new ChromiumWebBrowser("https://proxy.g41.moe/ui/main/index.html");
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CWebBrowser.BrowserSettings.FileAccessFromFileUrls = CefState.Enabled;
            CWebBrowser.BrowserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            CWebBrowser.RegisterJsObject("loginObject", new LoginObject());
            CWebBrowser.RegisterJsObject("mainObject", new MainObject());
            CWebBrowser.RegisterJsObject("nodeObject", new NodeObject());
            CWebBrowser.RegisterJsObject("homeObject", new HomeObject());
            CWebBrowser.MenuHandler = new MenuHandler();
            this.Controls.Add(CWebBrowser);
        }

        private void MainCef_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GlobalVaribles.GlobalVaribles.isConencted)
            {
                Shadowsocks.ShadowsocksVaribles._controller.Stop();
            }
            Application.Exit();
        }
    }

    public class LoginObject
    {
        public void Login(string username, string password, bool remacc, bool rempass)
        {
            try
            {
                MySqlConnection msqlConnection = new MySqlConnection("Host = " + GlobalVaribles.GlobalVaribles.DB_Adress + "; Database = " + GlobalVaribles.GlobalVaribles.DB_DBName + "; Username = " + GlobalVaribles.GlobalVaribles.DB_Username + "; Password = " + GlobalVaribles.GlobalVaribles.DB_Password + ";SslMode = none;");
                msqlConnection.Open();
                string sql = "SELECT pass, user_name, email, port, passwd, method, protocol, obfs FROM user WHERE email = \"" + username + "\"";
                MySqlDataAdapter result = new MySqlDataAdapter(sql, msqlConnection);
                DataSet dataset = new DataSet();
                result.Fill(dataset, "tb");
                msqlConnection.Close();
                if (dataset.Tables[0].Rows.Count < 1)
                {
                    MainCef.CWebBrowser.ExecuteScriptAsync("alertError('用户名错误，请检查您的用户名是否正确！')");
                }
                else if (dataset.Tables[0].Rows[0][0].ToString() == GetSHA256Hash.GetSHA256HashFromString(password))
                {
                    string clijson = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "cli-config.json");
                    JObject cliobj = JObject.Parse(clijson);
                    if(remacc)
                    if (cliobj.Property("Account") != null)
                    {
                        cliobj["Account"].Replace(username);
                    }
                    else
                    {
                        cliobj.Add(new JProperty("Account", username));
                    }
                    else
                    if (cliobj.Property("Account") != null)
                    {
                        cliobj["Account"].Replace("");
                    }
                    else
                    {
                        cliobj.Add(new JProperty("Account", ""));
                    }
                    if (rempass)
                    if (cliobj.Property("Password") != null)
                    {
                        cliobj["Password"].Replace(password);
                    }
                    else
                    {
                        cliobj.Add(new JProperty("Password", password));
                    }
                    else
                    if (cliobj.Property("Password") != null)
                    {
                        cliobj["Password"].Replace("");
                    }
                    else
                    {
                        cliobj.Add(new JProperty("Password", ""));
                    }
                    File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "cli-config.json", cliobj.ToString());
                    GlobalVaribles.GlobalVaribles.LOGIN_username = dataset.Tables[0].Rows[0][1].ToString();
                    GlobalVaribles.GlobalVaribles.LOGIN_port = int.Parse(dataset.Tables[0].Rows[0][3].ToString());
                    GlobalVaribles.GlobalVaribles.LOGIN_password = dataset.Tables[0].Rows[0][4].ToString();
                    GlobalVaribles.GlobalVaribles.LOGIN_method = dataset.Tables[0].Rows[0][5].ToString();
                    GlobalVaribles.GlobalVaribles.LOGIN_protocol = dataset.Tables[0].Rows[0][6].ToString();
                    GlobalVaribles.GlobalVaribles.LOGIN_obfs = dataset.Tables[0].Rows[0][7].ToString();
                    MainCef.CWebBrowser.ExecuteScriptAsync("setGetTransfer(true);");
                    MainCef.CWebBrowser.ExecuteScriptAsync("loginSuccess('登陆成功！','" + GlobalVaribles.GlobalVaribles.LOGIN_username + "')");
                }
                else
                {
                    MainCef.CWebBrowser.ExecuteScriptAsync("alertError('密码错误，请检查您的密码是否正确！')");
                }
            }
            catch (Exception exception)
            {
                MainCef.CWebBrowser.ExecuteScriptAsync("alertError('发生内部错误，请联系管理员！')");
            }
        }
        public string GetRemAcc()
        {
            string clijson = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "cli-config.json");
            JObject cliobj = JObject.Parse(clijson);
            if (cliobj.Property("Account") != null)
            {
                return cliobj["Account"].ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetRemPass()
        {
            string clijson = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "cli-config.json");
            JObject cliobj = JObject.Parse(clijson);
            if (cliobj.Property("Password") != null)
            {
                return cliobj["Password"].ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetTransfer()
        {
            try
            {
                MySqlConnection msqlConnection;
                MySqlDataAdapter result;
                string sql;
                DataSet dataset;
                NetworkOperate no = new NetworkOperate();
                msqlConnection = new MySqlConnection("Host = " + GlobalVaribles.GlobalVaribles.DB_Adress + "; Database = " + GlobalVaribles.GlobalVaribles.DB_DBName + "; Username = " + GlobalVaribles.GlobalVaribles.DB_Username + "; Password = " + GlobalVaribles.GlobalVaribles.DB_Password + ";SslMode = none;");
                msqlConnection.Open();
                sql = "SELECT u, d, transfer_enable, class_expire FROM user WHERE port = " + GlobalVaribles.GlobalVaribles.LOGIN_port;
                result = new MySqlDataAdapter(sql, msqlConnection);
                dataset = new DataSet();
                result.Fill(dataset, "tb");
                msqlConnection.Close();
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    double all = double.Parse(dataset.Tables[0].Rows[0][2].ToString());
                    double used = double.Parse(dataset.Tables[0].Rows[0][0].ToString()) + double.Parse(dataset.Tables[0].Rows[0][1].ToString());
                    return ((all - used)/1024/1024/1024).ToString("0.0");
                }
                else
                {
                    return "0";
                }
            }
            catch
            {
                return "0";
            }
        }
        public void Logout()
        {
            GlobalVaribles.GlobalVaribles.LOGIN_username = null;
        }
    }
    public class NodeObject
    {
        public void GetNodeList()
        {
            try
            {
                MainCef.CWebBrowser.ExecuteScriptAsync("clearNodeList()");
                MySqlConnection msqlConnection = new MySqlConnection("Host = " + GlobalVaribles.GlobalVaribles.DB_Adress + "; Database = " + GlobalVaribles.GlobalVaribles.DB_DBName + "; Username = " + GlobalVaribles.GlobalVaribles.DB_Username + "; Password = " + GlobalVaribles.GlobalVaribles.DB_Password + ";SslMode = none;");
                msqlConnection.Open();
                string sql = "SELECT id, name, type, node_ip, node_connector FROM ss_node";
                MySqlDataAdapter result = new MySqlDataAdapter(sql, msqlConnection);
                DataSet dataset = new DataSet();
                result.Fill(dataset, "tb");
                msqlConnection.Close();
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                    {
                        if (int.Parse(dataset.Tables[0].Rows[i][0].ToString()) == 1 || int.Parse(dataset.Tables[0].Rows[i][0].ToString()) == 2) continue; //保留节点
                        if (dataset.Tables[0].Rows[i][2].ToString() == "0") continue; //隐藏
                        string location = IPAPI.GetIpLocation(dataset.Tables[0].Rows[i][3].ToString());
                        string people = dataset.Tables[0].Rows[i][4].ToString();
                        MainCef.CWebBrowser.ExecuteScriptAsync("addNode('" + dataset.Tables[0].Rows[i][0].ToString() + "','" + dataset.Tables[0].Rows[i][1].ToString() + "','" + dataset.Tables[0].Rows[i][3].ToString() + "','" + location + "','" + people + "')");
                    }
                }
                else
                {
                    MainCef.CWebBrowser.ExecuteScriptAsync("alertError('当前没有可用节点！')");
                }
            }
            catch
            {
                MainCef.CWebBrowser.ExecuteScriptAsync("alertError('发生内部错误，请联系管理员！')");
            }
        }
        public bool IsConnected()
        {
            return GlobalVaribles.GlobalVaribles.isConencted;
        }
        public bool Connect(string id)
        {
            try
            {
                MySqlConnection msqlConnection;
                MySqlDataAdapter result;
                string sql;
                DataSet dataset;
                NetworkOperate no = new NetworkOperate();
                msqlConnection = new MySqlConnection("Host = " + GlobalVaribles.GlobalVaribles.DB_Adress + "; Database = " + GlobalVaribles.GlobalVaribles.DB_DBName + "; Username = " + GlobalVaribles.GlobalVaribles.DB_Username + "; Password = " + GlobalVaribles.GlobalVaribles.DB_Password + ";SslMode = none;");
                msqlConnection.Open();
                sql = "SELECT id, name, server, type FROM ss_node WHERE id='" + id + "'";
                result = new MySqlDataAdapter(sql, msqlConnection);
                dataset = new DataSet();
                result.Fill(dataset, "tb");
                msqlConnection.Close();
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    SSRConfigOperate ssrco = new SSRConfigOperate();
                    string ipaddress = ssrco.JsonConfigGenerate(dataset.Tables[0].Rows[0][2].ToString());
                    File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "gui-config.json", ipaddress, Encoding.UTF8);
                    Shadowsocks.ShadowsocksVaribles.InitController();
                    Shadowsocks.ShadowsocksVaribles._controller.Start();
                    GlobalVaribles.GlobalVaribles.isConencted = true;
                    GlobalVaribles.GlobalVaribles.ServerID = int.Parse(id);
                    GlobalVaribles.GlobalVaribles.ServerName = dataset.Tables[0].Rows[0][1].ToString();
                    MainCef.CWebBrowser.ExecuteScriptAsync("alertError('链接成功，您可畅游网络世界！')");
                    return true;
                }
                else
                {
                    MainCef.CWebBrowser.ExecuteScriptAsync("alertError('您可能没有选择节点！')");
                    return false;
                }
            }
            catch (Exception e)
            {
                MainCef.CWebBrowser.ExecuteScriptAsync("alertError('发生内部错误，请联系管理员！')");
                return false;
            }
        }
        public void Disconnect()
        {
            Shadowsocks.ShadowsocksVaribles._controller.Stop();
            GlobalVaribles.GlobalVaribles.isConencted = false;
            MainCef.CWebBrowser.ExecuteScriptAsync("alertError('您已成功断开链接！')");
        }
        public string getConnectedServerName()
        {
            return GlobalVaribles.GlobalVaribles.ServerName;
        }
    }
    public class MainObject
    {
        public bool IfLogin()
        {
            if (GlobalVaribles.GlobalVaribles.LOGIN_username != null)
                return true;
            else
                return false;
        }
        public void CheckVerson(string verson)
        {
            if(verson != "1.0.0")
            {
                MainCef.CWebBrowser.ExecuteScriptAsync("alertFatal('您的客户端版本已过期！')");
            }
        }
        public void Exit()
        {
            Application.Exit();
        }
    }
    public class HomeObject
    {
        public bool GetAnnouncement()
        {
            try
            {
                MySqlConnection msqlConnection;
                MySqlDataAdapter result;
                string sql;
                DataSet dataset;
                NetworkOperate no = new NetworkOperate();
                msqlConnection = new MySqlConnection("Host = " + GlobalVaribles.GlobalVaribles.DB_Adress + "; Database = " + GlobalVaribles.GlobalVaribles.DB_DBName + "; Username = " + GlobalVaribles.GlobalVaribles.DB_Username + "; Password = " + GlobalVaribles.GlobalVaribles.DB_Password + ";SslMode = none;");
                msqlConnection.Open();
                sql = "SELECT id, date, content FROM announcement";
                result = new MySqlDataAdapter(sql, msqlConnection);
                dataset = new DataSet();
                result.Fill(dataset, "tb");
                msqlConnection.Close();
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dataset.Tables[0].Rows.Count; i++)
                    {
                        MainCef.CWebBrowser.ExecuteScriptAsync("addAnnouncement('" + dataset.Tables[0].Rows[i][2].ToString().Replace("\n", "") + "', '" + dataset.Tables[0].Rows[i][1].ToString() + "')");
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                MainCef.CWebBrowser.ExecuteScriptAsync("alertError('发生内部错误，请联系管理员！')");
                return false;
            }
        }
        public bool GetUserinfo()
        {
            try
            {
                MySqlConnection msqlConnection;
                MySqlDataAdapter result;
                string sql;
                DataSet dataset;
                NetworkOperate no = new NetworkOperate();
                msqlConnection = new MySqlConnection("Host = " + GlobalVaribles.GlobalVaribles.DB_Adress + "; Database = " + GlobalVaribles.GlobalVaribles.DB_DBName + "; Username = " + GlobalVaribles.GlobalVaribles.DB_Username + "; Password = " + GlobalVaribles.GlobalVaribles.DB_Password + ";SslMode = none;");
                msqlConnection.Open();
                sql = "SELECT id, class, class_expire, node_speedlimit, node_connector, money FROM user WHERE port = " + GlobalVaribles.GlobalVaribles.LOGIN_port;
                result = new MySqlDataAdapter(sql, msqlConnection);
                dataset = new DataSet();
                result.Fill(dataset, "tb");
                msqlConnection.Close();
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    string classs = dataset.Tables[0].Rows[0][1].ToString();
                    string node_speedlimit = dataset.Tables[0].Rows[0][3].ToString();
                    if (double.Parse(node_speedlimit) <= 0) node_speedlimit = "不限速";
                    else node_speedlimit += "M";
                    string node_connector = dataset.Tables[0].Rows[0][4].ToString();
                    if (int.Parse(node_connector) == 0) node_connector = "无限制";
                    else node_connector += "台设备";
                    string money = dataset.Tables[0].Rows[0][5].ToString();
                    MainCef.CWebBrowser.ExecuteScriptAsync("updateUserinfo('" + classs + "', '" + dataset.Tables[0].Rows[0][2].ToString() + "', '" + node_speedlimit + "', '" + node_connector + "', '" + money + "')");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                MainCef.CWebBrowser.ExecuteScriptAsync("alertError('发生内部错误，请联系管理员！')");
                return false;
            }
        }
    }

    internal class MenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //清除上下文菜单
            model.Clear();
            //throw new NotImplementedException();
        }
        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            // throw new NotImplementedException();
            return false;
        }
        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            //    throw new NotImplementedException();
        }
        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
            //  throw new NotImplementedException();
        }
    }
}
