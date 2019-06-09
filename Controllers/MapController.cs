using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlightGearWebApp.Models;
using System.Xml;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace FlightGearWebApp.Controllers
{
    public class MapController : Controller
    {
        // GET: Map display
        public ActionResult Display(string ip, int port, int time = 0)
        {
            Debug.WriteLine("Hello from MapController with network display unknown!");
            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                    || ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
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
      
            InfoModel.Instance.FilePath = AppDomain.CurrentDomain.BaseDirectory + ip + ".csv";
            InfoModel.Instance.Time = port;

            Session["time"] = port;
            Session["isNetworkDisplay"] = "0";
            InfoModel.Instance.OpenFileRead(InfoModel.Instance.FilePath);

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
            string file = InfoModel.Instance.FilePath;
            InfoModel.Instance.OpenFileWrite(file);
            return file;
        }

        [HttpPost]
        public string CloseFileRead()   //NEW
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CloseFileRead(fileName);
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
        public string CloseFileWrite()   
        {
            string fileName = InfoModel.Instance.FilePath;
            InfoModel.Instance.CloseFileWrite(fileName);
            return fileName;
        }
        [HttpPost]
        public string GetInfoModelXML() 
        {
            var Info = InfoModel.Instance;
            if (Info.isMoreFileLines)
            {
                Info.ReadFileValues();
            }
            return this.InfoModelToXML(Info);
        }
        [HttpPost]
        public string Disconnect()
        {
            var connection = InfoModel.Instance.NetworkConnection;
            connection.Disconnect();
            return "";
        }
        // These function initializes an XML format of the Network object.
        [HttpPost]
        public string GetNetworkXML()   //NEW
        {
            var network = InfoModel.Instance.NetworkConnection;

            network.Write(); // read lat and lon from server to network object.

            return this.NetworkToXML(network);
        }
        private string NetworkToXML(NetworkConnection network)  
        {
            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(builder, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("NetworkConnections");     
            network.ToXml(writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return builder.ToString();
        }

        public string InfoModelToXML(InfoModel Info)
        {
            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(builder, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("InfoModels");
            Info.ToXml(writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return builder.ToString();
        }

    }

}