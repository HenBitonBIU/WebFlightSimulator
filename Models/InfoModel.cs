using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ajax_Minimal.Models;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Ajax_Minimal.Models
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
                  //  Debug.WriteLine("network in info was null");
                    s_instace = new InfoModel();
                }
               // Debug.WriteLine("Returning network - from the GetNetwrok retquest from js");
                return s_instace;
            }
        }

        public int Timeout { get; set; }
        public int Time { get; set; }
        public string FilePath { get; set; }
        private StreamWriter streamWriter;

        public NetworkConnection NetworkConnection { get; private set; }

        private InfoModel()
        {
            NetworkConnection = new NetworkConnection();
        }

        public void ConnectNetwork()
        {
            NetworkConnection.Connect();
        }

        public void CreateFile(string filePath)
        {
            this.streamWriter = new StreamWriter(filePath);
        }

        public void WriteToFile(string filePath)
        {
            string toWrite = this.NetworkConnection.Lon.ToString() + "," + this.NetworkConnection.Lat.ToString();
            this.streamWriter.WriteLineAsync(toWrite); // the writing needs to be done in another func.
        }

        public void CloseFile(string filePath)
        {
            this.streamWriter.Close();
        }
    }
}