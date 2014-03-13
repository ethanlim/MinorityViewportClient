using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;    
using System.Windows.Media.Imaging;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Data;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Devices;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Networks;
using Microsoft.Kinect;
using System.Threading;
using System.ComponentModel;                                        //Require the SDK Library

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

        private MultipleKinectsPlatform.Networks.Agent comAgent;

        public Core()
        {
            try
            {
                this.curTime = this.GetTimeFromServer();
            }
            catch (Exception ServerTimeException)
            {
                 /* TODO : Implement fail safe if connecting to time server fails */
            }

            mainTimer = new System.Windows.Threading.DispatcherTimer();
            mainTimer.Tick += new EventHandler(MainTimerTick);
            mainTimer.Interval = new TimeSpan(0, 0, 1);
            mainTimer.Start();

            this.kinectMgr = new KinectManagers();
            this.kinectMgr.Shutdown();
            this.networkMgr = new MultipleKinectsPlatform.Networks.NetworkManagers();
            
            this.comAgent = this.networkMgr.GetAgent(MultipleKinectsPlatform.Networks.NetworkManagers.AgentType.Skeleton);

            this.clientId = this.comAgent.RegisterClientId("Home", this.GetLocalIP());
            this.comAgent.RegisterSensorsUniqueId(
                                            this.ExtractSensorId_JSON(
                                                 this.kinectMgr.GetListOfSensors()
                                            ),
                                            this.clientId
            );
        }

        ~Core()
        {
            this.comAgent.DeregisterClient(this.clientId);
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

        public void GetSkeletonStream(ushort sensorId, EventHandler<SkeletonReadyArgs> handler,string URL)
        {
            TransformSmoothParameters param = new TransformSmoothParameters();

            this.kinectMgr.SkeletonFromSensor(sensorId, param,this.SkeletonEventHandler);
            
            this.SkeletonReady = handler;
        }

        public uint GetClientId()
        {
            return this.clientId;
        }

        private string ExtractSensorId_JSON(List<KinectSensor> kinects)
        {
            string KinectIds = "";

            KinectIds += "{";
            KinectIds += "\"Sensors\":";
            KinectIds += "[";

            for(int kinect=0;kinect<kinects.Count;kinect+=1)
            {
                KinectIds += "{";
                KinectIds += "\"id\":";

                string escapedKinectId = kinects[kinect].UniqueKinectId.Replace("\\", "\\\\");

                KinectIds += "\"" + escapedKinectId + "\"";

                KinectIds += "}";

                if (kinect != kinects.Count-1)
                {
                    KinectIds += ",";
                }
            }

            KinectIds += "]";
            KinectIds += "}";

            return KinectIds;
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
            NTPClient timeClient = new NTPClient("sg.pool.ntp.org");

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
            this.SkeletonReady(sender, new SkeletonReadyArgs {defaultEventArg = e,allSkeletons = e.allSkeletons,kinectId=e.kinectId});

            List<MultipleKinectsPlatform.Data.Skeleton> convertedSkeletons = MultipleKinectsPlatform.Data.Skeleton.ConvertKinectSkeletons(e.allSkeletons, this.clientId, e.kinectId);


            /****** Trade off if put this into background worker => Too Slow frame rate  *******/
            string skeletonJSON = MultipleKinectsPlatform.Data.Skeleton.ConvertToJSON(convertedSkeletons);

            comAgent.SendData(skeletonJSON, curTime);

            /*********** Background worker ********************************/
            /*
            if(convertedSkeletons.Count>0){

                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += delegate(object s, DoWorkEventArgs args)
                {
                     Put the above two lines here if not to affect the UI 
                };

                worker.RunWorkerAsync();
            }
            */
        }

        private void MainTimerTick(object sender, EventArgs args)
        {
           this.curTime = this.curTime.AddSeconds(1);
        }
    }
}
