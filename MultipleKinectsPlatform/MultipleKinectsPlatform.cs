using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Kinect;                                                 //Require the SDK Library
using System.Windows.Media.Imaging;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Data;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Devices;

namespace MultipleKinectsPlatformClient
{
    class Core
    {
        private ushort clientId=0;
        private KinectManagers kinectMgr;
        private MultipleKinectsPlatform.Networks.NetworkManagers networkMgr;
        private MultipleKinectsPlatform.Networks.Agent comAgent;
        private event EventHandler<DepthReadyArgs> DepthReady;
        private event EventHandler<SkeletonReadyArgs> SkeletonReady;
        private bool sendSkeletonStreamEnabled = false;

        public Core()
        {
            this.kinectMgr = new KinectManagers();
            this.networkMgr = new MultipleKinectsPlatform.Networks.NetworkManagers();
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

            if (this.sendSkeletonStreamEnabled)
            {
                this.comAgent = this.networkMgr.GetAgent(MultipleKinectsPlatform.Networks.NetworkManagers.AgentType.Skeleton);
            }

            this.SkeletonReady = handler;
        }

        public List<KinectSensor> ListOfSensors()
        {
            return this.kinectMgr.GetListOfSensors();
        }

        /**
         * Callbacks
         */

        /**
         *   Depth Data Ready Handler
         */ 
        private void DepthEventHandler(object sender, DepthReadyArgs e)
        {
            this.DepthReady(sender, new DepthReadyArgs{ defaultEventArg = e, depthImage = e.depthImage });
        }

        /**
         *  Skeleton Data Ready Handler
         */ 
        private void SkeletonEventHandler(object sender, SkeletonReadyArgs e)
        {
            this.SkeletonReady(sender, new SkeletonReadyArgs { defaultEventArg = e, allSkeletons = e.allSkeletons,kinectId=e.kinectId});

            Microsoft.Kinect.Skeleton[] obtainedSkeletons = e.allSkeletons;

            List<MultipleKinectsPlatform.Data.Skeleton> convertedSkeleton = MultipleKinectsPlatform.Data.Skeleton.ConvertKinectSkeletons(obtainedSkeletons,this.clientId,e.kinectId);

            if(this.sendSkeletonStreamEnabled)
            {
                string skeletonJSON = MultipleKinectsPlatform.Data.Skeleton.ConvertToJSON(convertedSkeleton);

                if (!skeletonJSON.Equals(""))
                {
                    comAgent.SendData(skeletonJSON);
                }
            }
        }
    }
}
