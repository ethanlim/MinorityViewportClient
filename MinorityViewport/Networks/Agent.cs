using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultipleDepthSensorsPlatformClient.MultipleDepthSensorsPlatform.Networks
{
    abstract class Agent
    {
        public abstract bool CheckForInternetConnection();
        public abstract void SendData(string json,DateTime curTime);
        public abstract uint RegisterClientId(string physical_loc, string ip_addr);
        public abstract void DeregisterClient(uint clientId);
        public abstract void RegisterSensorsUniqueId(string sensorList_JSON, uint clientId);
    }
}
