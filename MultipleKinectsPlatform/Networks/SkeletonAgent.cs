using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace MultipleKinectsPlatformClient.MultipleKinectsPlatform.Networks
{
    class SkeletonAgent : Agent
    {
        private ushort httpPort;
        private String host;

        private ushort udpPort;

        private Uri endPoint; 

        public SkeletonAgent()
        {
            this.httpPort = 1626;
            this.host = "d6xhjv1s.d1.comp.nus.edu.sg";

            this.udpPort = 1625;

            this.endPoint = new Uri("http://"+this.host+":"+httpPort);
        }
        
        public override void SendData(string sensorData_JSON, DateTime curTime)
        {
            try
            {
                UdpClient client = new UdpClient("127.0.0.1",this.udpPort);

                String package = "{" + "\"TIME_STAMP\"" + ":" + ((Int32)(curTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString() + "," +
                                       "\"SENSOR_JSON\"" + ":" + sensorData_JSON +
                                 "}";

                byte[] sdata = Encoding.ASCII.GetBytes(package);

                client.Send(sdata,sdata.Length);


                /************ Working Code for HTTP Request => Too Slow ************************************/
                /*
                HttpWebRequest httpForSensorData = (HttpWebRequest)WebRequest.Create(this.endPoint + "api/sensors/data.json");

                ServicePointManager.DefaultConnectionLimit = 20;
                ServicePointManager.UseNagleAlgorithm = false;
                httpForSensorData.ServicePoint.Expect100Continue = false;
                httpForSensorData.Proxy = null;
                httpForSensorData.KeepAlive = true;
                httpForSensorData.PreAuthenticate = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = false;

                httpForSensorData.Accept = "application/json";
                httpForSensorData.ContentType = "application/json";
                httpForSensorData.Method = "POST";

                httpForSensorData.Headers["SENSOR_JSON"] = sensorData_JSON;            //pack json in header
                httpForSensorData.Headers["TIME_STAMP"] = ((Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
             
                using (WebResponse response = (HttpWebResponse) httpForSensorData.GetResponse())
                {
                }
                */
            }
            catch (WebException webex)
            {
                Console.Write(webex.Message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public override uint RegisterClientId(string physical_loc,string ip_addr)
        {
            uint givenClientId = 0;

            try
            {
                HttpWebRequest request = WebRequest.Create(this.endPoint + "api/clients/register.json") as HttpWebRequest;

                request.Method = "POST";
                request.Proxy = null;
                request.Headers["PHYSICAL_LOC"] = physical_loc;
                request.Headers["IP_ADDR"] = ip_addr;

                WebResponse response = request.GetResponse();

                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                String responseData = streamReader.ReadToEnd();
                String rawClientId = responseData.Substring(responseData.LastIndexOf(':') + 1);
                rawClientId = rawClientId.Remove(rawClientId.Length - 1);

                givenClientId = Convert.ToUInt32(rawClientId);
            }
            catch (WebException webex)
            {
                Console.Write(webex.Message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return givenClientId;
        }

        public override void DeregisterClient(uint clientId)
        {
            HttpWebRequest httpToDeregistration = (HttpWebRequest)WebRequest.Create(this.endPoint + "api/clients/deregister.json");

            httpToDeregistration.Proxy = null;
            httpToDeregistration.Accept = "application/json";
            httpToDeregistration.ContentType = "application/json";
            httpToDeregistration.Method = "POST";
            httpToDeregistration.Headers["CLIENT_ID"] = clientId.ToString();

            try
            {
               httpToDeregistration.GetResponse();
            }
            catch (WebException webex)
            {
                Console.Write(webex.Message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public override void RegisterSensorsUniqueId(string sensorList_JSON, uint clientId)
        {
            HttpWebRequest httpToRequestToSendSensorList = (HttpWebRequest)WebRequest.Create(this.endPoint + "api/sensors/register.json");

            httpToRequestToSendSensorList.Accept = "application/json";
            httpToRequestToSendSensorList.ContentType = "application/json";
            httpToRequestToSendSensorList.Method = "POST";
            httpToRequestToSendSensorList.Proxy = null;
            httpToRequestToSendSensorList.Headers["SENSOR_LIST"] = sensorList_JSON;
            httpToRequestToSendSensorList.Headers["CLIENT_ID"] = clientId.ToString();

            try
            {
                httpToRequestToSendSensorList.GetResponse();
            }
            catch (WebException webex)
            {
                Console.Write(webex.Message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
