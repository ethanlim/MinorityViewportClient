using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Kinect;         //Require the SDK Library
using System.Windows.Media.Imaging;

namespace MultipleKinectsPlatform
{
    class Core
    {
        private Devices.KinectManagers kinectMgr;

        public Core()
        {
            this.kinectMgr = new Devices.KinectManagers();
        }

        ~Core()
        {

        }

        public void Begin()
        {
            this.GetDepthStream(0);
        }

        public void End()
        {
            this.kinectMgr.Shutdown();
        }

        public BitmapSource GetDepthStream(){

            BitmapSource depthImg = null;

            return depthImg;
        }

        public BitmapSource GetDepthStream(ushort sensorId){

            BitmapSource depthImg = null;

            this.kinectMgr.DepthFromSensor(sensorId, DepthImageFormat.Resolution80x60Fps30);

            return depthImg;
        }

        public List<KinectSensor> ListOfSensors()
        {
            return this.kinectMgr.GetListOfSensors();
        }
    }
}
