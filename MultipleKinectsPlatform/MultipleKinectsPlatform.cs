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
        private event EventHandler<Devices.DepthReadyArgs> DepthReady;

        public Core()
        {
            this.kinectMgr = new Devices.KinectManagers();
        }

        ~Core()
        {

        }

        public void Begin()
        {

        }

        public void End()
        {
            this.kinectMgr.Shutdown();
        }

        public BitmapSource GetDepthStream(){

            BitmapSource depthImg = null;

            return depthImg;
        }

        public void GetDepthStream(ushort sensorId, EventHandler<Devices.DepthReadyArgs> handler)
        {

            this.kinectMgr.DepthFromSensor(sensorId, DepthImageFormat.Resolution640x480Fps30, this.DepthEventHandler);

            this.DepthReady += handler;
        }

        public List<KinectSensor> ListOfSensors()
        {
            return this.kinectMgr.GetListOfSensors();
        }

        /**
         * Callbacks
         */
        private void DepthEventHandler(object sender, Devices.DepthReadyArgs e)
        {
            this.DepthReady(sender, new Devices.DepthReadyArgs{ defaultEventArg = e, depthImage = e.depthImage });
        }
    }
}
