using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleKinectsPlatformClient.MultipleKinectsPlatform.Networks
{
    abstract class Agent
    {
        public abstract void SendData(string json);
        public abstract uint RegisterClientId();
        public abstract void DeregisterClient();
    }
}
