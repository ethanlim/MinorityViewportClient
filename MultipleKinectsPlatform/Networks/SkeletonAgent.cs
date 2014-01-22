using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MultipleKinectsPlatformClient.MultipleKinectsPlatform.Networks
{
    class SkeletonAgent : Agent
    {
        private Uri endPoint = new Uri("http://localhost:1626");
        
        public override void SendData(string sensorData_JSON, DateTime curTime)
        {
            try
            {
                HttpWebRequest httpForSensorData = (HttpWebRequest)WebRequest.Create(this.endPoint + "api/sensors/data.json");

                httpForSensorData.Accept = "application/json";
                httpForSensorData.ContentType = "application/json";
                httpForSensorData.Method = "POST";
                httpForSensorData.Headers["SENSOR_JSON"] = sensorData_JSON;            //pack json in header
                httpForSensorData.Headers["TIME_STAMP"] = ((Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();

                httpForSensorData.GetResponse();
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
