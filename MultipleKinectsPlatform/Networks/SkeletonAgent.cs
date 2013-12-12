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
            HttpWebRequest http = null;
            WebResponse response = null;

            bool retry=false;

            do
            {
                try
                {
                    http = (HttpWebRequest)WebRequest.Create(this.endPoint);

                    http.Accept = "application/json";
                    http.ContentType = "application/json";
                    http.Method = "POST";
                    http.Headers["JSON"] = json;            //pack json in header

                    response = http.GetResponse();
                }
                catch (WebException webex)
                {
                    retry = true;
                }
            } while (retry);

            if (response != null)
            {
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
            }
        }
    }
}
