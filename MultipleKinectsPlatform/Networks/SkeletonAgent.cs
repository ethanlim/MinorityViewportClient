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
            var http = (HttpWebRequest)WebRequest.Create(this.endPoint);

            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            http.Headers["JSON"] = json;            //pack json in header

            WebResponse response = null;
            bool tryAgain=false;

            do
            {
                try
                {
                    response = http.GetResponse();

                    if (http.HaveResponse)
                        tryAgain = false;
                }
                catch (WebException webEx)
                {
                    tryAgain = true;
                }
            } while (tryAgain);

            if (response != null)
            {
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
            }
        }
    }
}
