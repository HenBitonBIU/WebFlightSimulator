
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlightGearWebApp.Models;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace FlightGearWebApp.Models
{
    public class InfoModel
    {
        private static InfoModel s_instace = null;

        public static InfoModel Instance
        {
            get
            {
                if (s_instace == null)
                {
                    s_instace = new InfoModel();
                }
                return s_instace;
            }
        }

        public int Time { get; set; }
        public int Timeout { get; set; }
        public string FilePath { get; set; }
        public float Lat { get; private set; }
        public float Lon { get; private set; }
        public bool isMoreFileLines { get; private set; }
        public string EOFBOOL { get; set; }
        private StreamWriter streamWriter;
        private StreamReader streamReader;

        public NetworkConnection NetworkConnection { get; private set; }
        public static Mutex WriteStreaMutex = new Mutex();
        public static Mutex WriteFileMutex = new Mutex();

        private InfoModel()
        {
            NetworkConnection = new NetworkConnection();
        }

        public void ConnectNetwork()
        {
            NetworkConnection.Connect();
        }


        public void OpenFileWrite(string filePath)
        {
            this.streamWriter = new StreamWriter(filePath);
        }
        public void CreateFile(string filePath)
        {
            Debug.WriteLine("creates a new string writer");
            this.streamWriter = new StreamWriter(filePath);
        }
        public void OpenFileRead(string filePath)
        {
            this.isMoreFileLines = true;
            EOFBOOL = "0";
            this.streamReader = new StreamReader(filePath);
        }
    public void WriteToFile(string filePath)
        {
            string toWrite = this.NetworkConnection.Lon.ToString() + "," + this.NetworkConnection.Lat.ToString() + "," + 
                this.NetworkConnection.Throttle.ToString() + "," + this.NetworkConnection.Rudder.ToString();
            this.streamWriter.WriteLine(toWrite); // the writing needs to be done in another func.
        }
        public void ReadFileValues()
        {
            string line = streamReader.ReadLine();
            if (line == null)
            {
                this.EOFBOOL = "1";
                this.isMoreFileLines = false;
                this.CloseFileRead(this.FilePath);
            }
            else
            {
                string[] values = line.Split(',');
                this.Lon = float.Parse(values[0]);
                this.Lat = float.Parse(values[1]);       
            }
        }

        public void CloseFileRead(string filePath)  
        {
            this.streamReader.Close();
        }

        public void CloseFileWrite(string filePath)
        {
            this.streamWriter.Close();
        }
        public void ToXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("InfoModel");
            xmlWriter.WriteElementString("Lat", this.Lat.ToString());
            xmlWriter.WriteElementString("Lon", this.Lon.ToString());
            xmlWriter.WriteElementString("isEOF",this.EOFBOOL);
            xmlWriter.WriteEndElement();
        }
    }
}