using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace FlightGearWebApp.Models
{
    public class NetworkConnection
    {
        int tryPulse = 500;
        public volatile bool stop = true;
        private TcpClient myTcpClient;
        Thread connect;
        public static Mutex mutex = new Mutex();
        public double Lon { get; set; }
        public double Lat { get; set; }
        public double Throttle { get; set; }
        public double Rudder { get; set; }
        public string Ip{ get; set;}
        public int Port { get; set; }
       
     
        public void Connect()
        {
            if (!stop)
            {
                return;
            }
            stop = false;

            this.myTcpClient = new TcpClient();
            this.connect = new Thread(() =>
            {
                while (!myTcpClient.Connected)
                {
                    try
                    {
                        myTcpClient.Connect(Ip, Port);
                        Thread.Sleep(tryPulse);
                    }
                    catch (Exception)
                    {
                    }
                }           
            });
            this.connect.Start();
        }
        public void Disconnect()
        {
            
            if (stop)
            {
                return;
            }         
            connect.Abort();
            this.myTcpClient.Close();
            stop = true;
        }


        public string ParseValue(string toBeParsed)
        {
            string[] result = toBeParsed.Split('=');
            result = result[1].Split('\'');
            result = result[1].Split('\'');

            return result[0];
        }
        public void Write()   //later make it with no args
        {
            string command = "";  
            NetworkStream writeStream = this.myTcpClient.GetStream();  
            //Lon
            command = "get /position/longitude-deg\r\n";
            int byteCount = Encoding.ASCII.GetByteCount(command); 
            byte[] sendData = new byte[byteCount];  
            sendData = Encoding.ASCII.GetBytes(command);   

            writeStream.Write(sendData, 0, sendData.Length); 
            StreamReader STR = new StreamReader(writeStream);
            string lon = ParseValue(STR.ReadLine());
            Lon = double.Parse(lon);

            //Lat
            command = "get /position/latitude-deg\r\n";
            byteCount = Encoding.ASCII.GetByteCount(command); 
            sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);   
            writeStream.Write(sendData, 0, sendData.Length); 
            STR = new StreamReader(writeStream);
            string lat = ParseValue(STR.ReadLine());
            Lat = double.Parse(lat);
            //Throttle
            command = "get /controls/engines/engine/throttle\r\n";
            byteCount = Encoding.ASCII.GetByteCount(command); 
            sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);   
            writeStream.Write(sendData, 0, sendData.Length); 
            STR = new StreamReader(writeStream);
            string throttle = ParseValue(STR.ReadLine());
            Throttle = double.Parse(throttle);
            //Rudder
            command = "get /controls/flight/rudder\r\n";
            byteCount = Encoding.ASCII.GetByteCount(command); 
            sendData = new byte[byteCount];  //create a buffer
            sendData = Encoding.ASCII.GetBytes(command);
            writeStream.Write(sendData, 0, sendData.Length);       
            STR = new StreamReader(writeStream);
            string rudder = ParseValue(STR.ReadLine());
            Rudder = double.Parse(rudder);
        }
    public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("NetworkConnection");
            writer.WriteElementString("Ip", this.Ip);
            writer.WriteElementString("Port", this.Port.ToString());
            writer.WriteElementString("Lon", this.Lon.ToString());
            writer.WriteElementString("Lat", this.Lat.ToString());
            writer.WriteElementString("Throttle", this.Throttle.ToString());
            writer.WriteElementString("Rudder", this.Rudder.ToString());
            writer.WriteEndElement();
        }
    }
}