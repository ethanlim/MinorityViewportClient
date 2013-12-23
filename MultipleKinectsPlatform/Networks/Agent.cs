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
        public abstract uint RegisterClientId(string physical_loc, string ip_addr);
        public abstract void DeregisterClient(uint clientId);
    }
}
