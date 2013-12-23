using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace MultipleKinectsPlatformClient.MultipleKinectsPlatform.Networks
{
    class SkeletonAgent : Agent
    {
        private Uri endPoint = new Uri("http://localhost:1626");
        
        public override void SendData(string json)
        {
            HttpWebRequest httpForSensorData = null;
            WebResponse responseForSensorData = null;

            bool retry=false;

            do
            {
                try
                {
                    httpForSensorData = (HttpWebRequest)WebRequest.Create(this.endPoint+"/sensors/data");

                    httpForSensorData.Accept = "application/json";
                    httpForSensorData.ContentType = "application/json";
                    httpForSensorData.Method = "POST";
                    httpForSensorData.Headers["SENSOR_JSON"] = json;            //pack json in header

                    responseForSensorData = httpForSensorData.GetResponse();
                }
                catch (WebException webex)
                {
                    retry = true;
                }
            } while (retry);

            if (responseForSensorData != null)
            {
                var stream = responseForSensorData.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
            }
        }

        public override uint RegisterClientId()
        {
            uint givenClientId = 0;

            HttpWebRequest httpToRequestForClientId = null;
            WebResponse responseFromObtainedClientId = null;

            bool retry = false;

            do
            {
                try
                {
                    httpToRequestForClientId = (HttpWebRequest)WebRequest.Create(this.endPoint+"/client/new");

                    httpToRequestForClientId.Accept = "application/json";
                    httpToRequestForClientId.ContentType = "application/json";
                    httpToRequestForClientId.Method = "POST";

                    responseFromObtainedClientId = httpToRequestForClientId.GetResponse();
                }
                catch (WebException webex)
                {
                    retry = true;
                }

            } while (retry);

            return givenClientId;
        }

        public override void DeregisterClient()
        {
            HttpWebRequest httpToDeregistration = null;
            WebResponse responseFromDeregistration = null;

            bool retry = false;

            do
            {
                try
                {
                    httpToDeregistration = (HttpWebRequest)WebRequest.Create(this.endPoint + "/client/deregister");

                    httpToDeregistration.Accept = "application/json";
                    httpToDeregistration.ContentType = "application/json";
                    httpToDeregistration.Method = "POST";

                    responseFromDeregistration = httpToDeregistration.GetResponse();
                }
                catch (WebException webex)
                {
                    retry = true;
                }
            } while (retry);
        }
    }
}
