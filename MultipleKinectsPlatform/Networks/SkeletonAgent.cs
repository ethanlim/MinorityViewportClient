using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace MultipleKinectsPlatformClient.MultipleKinectsPlatform.Networks
{
    class SkeletonAgent : Agent
    {
        private Uri endPoint = new Uri("http://localhost:1626");
        
        public override void SendData(string json)
        {
            HttpWebRequest httpForSensorData = null;
            WebResponse responseForSensorData = null;

            uint failedAttempts = 0;
            bool retry;

            do
            {
                try
                {
                    retry = false;

                    httpForSensorData = (HttpWebRequest)WebRequest.Create(this.endPoint+"/web/api/sensors/data.json");

                    httpForSensorData.Accept = "application/json";
                    httpForSensorData.ContentType = "application/json";
                    httpForSensorData.Method = "POST";
                    httpForSensorData.Headers["SENSOR_JSON"] = json;            //pack json in header

                    responseForSensorData = httpForSensorData.GetResponse();
                }
                catch (WebException webex)
                {
                    retry = true;
                    failedAttempts += 1;
                }
                
            } while (retry);

       
        }

        public override uint RegisterClientId(string physical_loc,string ip_addr)
        {
            uint givenClientId = 0;
            WebResponse responseFromObtainedClientId = null;
            HttpWebRequest httpToRequestForClientId = (HttpWebRequest)WebRequest.Create(this.endPoint + "/web/api/clients/register.json");

            httpToRequestForClientId.Accept = "application/json";
            httpToRequestForClientId.ContentType = "application/json";
            httpToRequestForClientId.Method = "POST";

            httpToRequestForClientId.Headers["PHYSICAL_LOC"] = physical_loc;
            httpToRequestForClientId.Headers["IP_ADDR"] = ip_addr;

            try{
               
                responseFromObtainedClientId = httpToRequestForClientId.GetResponse();

            }
            catch (WebException webex)
            {
                Console.Write(webex.Message);
            }

            if (responseFromObtainedClientId != null)
            {
                givenClientId = Convert.ToUInt32(responseFromObtainedClientId.Headers["ASSIGNED_CLIENT_ID"]);
            }
            else
            {
                givenClientId = 0;
            }

            return givenClientId;
        }

        public override void DeregisterClient(uint clientId)
        {
            HttpWebRequest httpToDeregistration = (HttpWebRequest)WebRequest.Create(this.endPoint + "/web/api/clients/deregister.json");

            httpToDeregistration.Accept = "application/json";
            httpToDeregistration.ContentType = "application/json";
            httpToDeregistration.Method = "POST";
            httpToDeregistration.Headers["CLIENT_ID"] = clientId.ToString();

            try
            {
                WebResponse responseFromDeregistration = httpToDeregistration.GetResponse();
            }
            catch (WebException webex)
            {
                Console.Write(webex.Message);
            }
        }
    }
}
