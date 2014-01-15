using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;    
using System.Windows.Media.Imaging;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Data;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Devices;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Networks;
using Microsoft.Kinect;                                        //Require the SDK Library

namespace MultipleKinectsPlatformClient
{
    class Core
    {
        public DateTime curTime;
        private System.Windows.Threading.DispatcherTimer mainTimer;

        private uint clientId=0;
        private KinectManagers kinectMgr;
        private MultipleKinectsPlatform.Networks.NetworkManagers networkMgr;
        
        private event EventHandler<DepthReadyArgs> DepthReady;
        private event EventHandler<SkeletonReadyArgs> SkeletonReady;
        private bool sendSkeletonStreamEnabled = false;

        private MultipleKinectsPlatform.Networks.Agent comAgent;

        public Core()
        {
            this.curTime = this.GetTimeFromServer();
            mainTimer = new System.Windows.Threading.DispatcherTimer();
            mainTimer.Tick += new EventHandler(MainTimerTick);
            mainTimer.Interval = new TimeSpan(0, 0, 1);
            mainTimer.Start();

            this.kinectMgr = new KinectManagers();
            this.kinectMgr.Shutdown();
            this.networkMgr = new MultipleKinectsPlatform.Networks.NetworkManagers();

            this.comAgent = this.networkMgr.GetAgent(MultipleKinectsPlatform.Networks.NetworkManagers.AgentType.Skeleton);
        }

        ~Core()
        {
            if (this.sendSkeletonStreamEnabled)
            {
                this.comAgent.DeregisterClient(this.clientId);
            }
        }
        
        public void ShutDown()
        {
            this.kinectMgr.Shutdown();
        }

        public void GetDepthStream(ushort sensorId, EventHandler<DepthReadyArgs> handler)
        {
            this.kinectMgr.DepthFromSensor(sensorId, DepthImageFormat.Resolution640x480Fps30, this.DepthEventHandler);

            this.DepthReady += handler;
        }

        public void GetSkeletonStream(ushort sensorId, EventHandler<SkeletonReadyArgs> handler,bool sendStream,string URL)
        {
            TransformSmoothParameters param = new TransformSmoothParameters();

            this.kinectMgr.SkeletonFromSensor(sensorId, param,this.SkeletonEventHandler);

            this.sendSkeletonStreamEnabled = sendStream;

            this.SkeletonReady = handler;
        }

        public uint GetClientId()
        {
            this.clientId = this.comAgent.RegisterClientId("Home", this.GetLocalIP());

            return this.clientId;
        }

        public void StopStreams(ushort sensorId)
        {
            this.kinectMgr.StopStreams(sensorId);
        }

        public List<KinectSensor> ListOfSensors()
        {
            return this.kinectMgr.GetListOfSensors();
        }

        public uint GetNumOfAvaliableSensors()
        {
            return (uint)this.kinectMgr.GetListOfSensors().Count;
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }

            return localIP;
        }

        private DateTime GetTimeFromServer()
        {
            NTPClient timeClient = new NTPClient("0.nettime.pool.ntp.org");

            return timeClient.GetTime();
        }
                
        /**
         *   Depth Data Ready Handler
         */ 
        private void DepthEventHandler(object sender, DepthReadyArgs e)
        {
            this.DepthReady(sender, new DepthReadyArgs{ defaultEventArg = e, depthImage = e.depthImage,kinectId = e.kinectId });
        }

        /**
         *  Skeleton Data Ready Handler
         */ 
        private void SkeletonEventHandler(object sender, SkeletonReadyArgs e)
        {
            this.SkeletonReady(sender, new SkeletonReadyArgs { defaultEventArg = e, allSkeletons = e.allSkeletons,kinectId=e.kinectId});

            Microsoft.Kinect.Skeleton[] obtainedSkeletons = e.allSkeletons;

            List<MultipleKinectsPlatform.Data.Skeleton> convertedSkeletons = MultipleKinectsPlatform.Data.Skeleton.ConvertKinectSkeletons(obtainedSkeletons,this.clientId,e.kinectId);

            if(this.sendSkeletonStreamEnabled)
            {
                string skeletonJSON = MultipleKinectsPlatform.Data.Skeleton.ConvertToJSON(convertedSkeletons);

                if (!skeletonJSON.Equals(""))
                {
                    comAgent.SendData(skeletonJSON);
                }
            }
        }

        private void MainTimerTick(object sender, EventArgs args)
        {
           this.curTime = this.curTime.AddSeconds(1);
        }
    }
}
