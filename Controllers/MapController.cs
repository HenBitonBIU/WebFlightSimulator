using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax_Minimal.Models;
using System.Xml;
using System.Text;
using System.Net;

namespace Ajax_Minimal.Controllers
{
    public class MapController : Controller
    {
        const int MaxTime = 1000000;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult display(string ip, int port, int time = MaxTime)
        {
            IPAddress ipAdd;
            if (IPAddress.TryParse(ip, out ipAdd))
            {
                if (ipAdd.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                    || ipAdd.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    InfoModel.Instance.NetworkConnection.Ip = ip;
                    InfoModel.Instance.NetworkConnection.Port = port;
                    InfoModel.Instance.Time = time;
                    InfoModel.Instance.ConnectNetwork(); // connect to server for reading.

                    Session["time"] = time;
                    Session["isNetworkDisplay"] = "1";
                    return View();
                }
            }
            InfoModel.Instance.FilePath = ip;
            InfoModel.Instance.Time = time;
            Session["time"] = time;
            Session["isNetworkDisplay"] = "0";

            return View();
        }

        public ActionResult save(string ip, int port, int time, int timeout, string filePath)
        {
            InfoModel.Instance.NetworkConnection.Ip = ip;
            InfoModel.Instance.NetworkConnection.Port = port;
            InfoModel.Instance.Time = time;
            InfoModel.Instance.Timeout = timeout;
            InfoModel.Instance.FilePath = AppDomain.CurrentDomain.BaseDirectory + filePath + ".csv";
            InfoModel.Instance.ConnectNetwork(); // connect to server for reading.
            Session["time"] = time;
            Session["timeout"] = timeout;
            return View();
        }

        [HttpPost]
        public string OpenNewFile()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CreateFile(fileName);

            return fileName;
        }

        [HttpPost]
        public string WriteToFile()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.WriteToFile(fileName);

            return fileName;
        }

        [HttpPost]
        public string CloseFile()
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CloseFile(fileName);

            return fileName;
        }

        // These function initializes an XML format of the Network object.
        [HttpPost]
        public string GetNetwork()
        {
            var network = InfoModel.Instance.NetworkConnection;

            network.Write();

            return ToXml(network);
        }
        public void Disconnect()
        {
            var network = InfoModel.Instance.NetworkConnection;
            network.Disconnect();
        }
        private string ToXml(NetworkConnection network)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("NetworkConnections");
            network.ToXml(writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
    }
}