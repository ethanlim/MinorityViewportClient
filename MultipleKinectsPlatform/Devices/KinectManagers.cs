using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Kinect;         //Require the SDK Library
using System.Windows.Media.Imaging;
using System.Windows;

namespace MultipleKinectsPlatformClient.MultipleKinectsPlatform.Devices
{
    public class DepthReadyArgs : EventArgs
    {
        public EventArgs defaultEventArg { get; set; }
        public BitmapSource depthImage { get; set; }
        public String kinectId { get; set; }
    }

    public class SkeletonReadyArgs:EventArgs
    {
        public EventArgs defaultEventArg{get;set;}
        public Skeleton[] allSkeletons { get; set; }
        public String kinectId { get; set; }
    }
    
    class KinectManagers
    {
        private List<KinectSensor> kinects;
        public event EventHandler<DepthReadyArgs> DepthReady;
        public event EventHandler<SkeletonReadyArgs> SkeletonReady;
     
        public KinectManagers(){
            this.kinects = this.InitialiseSensors();
        }

        ~KinectManagers(){
            this.Shutdown();
        }

        public void Shutdown(){

            try
            {
                for (var kinects = 0; kinects < KinectSensor.KinectSensors.Count; kinects++)
                {
                    this.kinects[kinects].Stop();
                }
            }
            catch (Exception ex)
            {
                Console.Write("Error Shutting Down Kinects : " + ex.Message);
            }
        }

        public void DepthFromSensor(ushort sensorId, 
                                    DepthImageFormat format,
                                    EventHandler<DepthReadyArgs> handler)
        {
            KinectSensor ofInterestSensor = kinects[sensorId];

            ofInterestSensor.Start();

            if(!ofInterestSensor.DepthStream.IsEnabled){
                ofInterestSensor.DepthStream.Enable(format);
                ofInterestSensor.DepthFrameReady += this.SensorDepthFrameReady;
                this.DepthReady += handler;
            }
        }

        public void StopStreams(ushort sensorId)
        {
            this.kinects[sensorId].Dispose();
        }

        public void SkeletonFromSensor(ushort sensorId,
                                       TransformSmoothParameters smoothParam,
                                       EventHandler<SkeletonReadyArgs> handler)
        {
            KinectSensor ofInterestSensor = kinects[sensorId];

            if (!ofInterestSensor.SkeletonStream.IsEnabled)
            {
                ofInterestSensor.SkeletonStream.Enable();
                ofInterestSensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                /* Attached the calling object's handler */
                this.SkeletonReady += handler;
            }
        }

        public List<KinectSensor> GetListOfSensors()
        {
            if (this.kinects != null)
            {
                InitialiseSensors();
                return this.kinects;
            }
            else
            {
                return null;
            }
        }

        /**
         * Private Methods
         */
        private List<KinectSensor> InitialiseSensors()
        {
            List<KinectSensor> listOfKinects = new List<KinectSensor>();

            for (var kinects = 0; kinects < KinectSensor.KinectSensors.Count; kinects++)
            {
                listOfKinects.Add(KinectSensor.KinectSensors[kinects]);
            }

            return listOfKinects;
        }

        /**
         *      Callbacks
         */

        private void SensorDepthFrameReady( object sender, 
                                            DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    short[] pixelData = new short[depthFrame.PixelDataLength];
                    int stride = depthFrame.Width * 2;
                    depthFrame.CopyPixelDataTo(pixelData);

                    BitmapSource img = BitmapImage.Create(depthFrame.Width, depthFrame.Height, 96, 96, System.Windows.Media.PixelFormats.Gray16, null, pixelData, stride);

                    KinectSensor sensor = (KinectSensor)sender;

                    this.DepthReady(sender,new DepthReadyArgs {defaultEventArg=e,depthImage=img,kinectId=sensor.UniqueKinectId});
                }
            }
        }

        private void SensorSkeletonFrameReady(object sender, 
                                              SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }
                
                Skeleton[] totalSkeletonsObserved = new Skeleton[skeletonFrame.SkeletonArrayLength];

                skeletonFrame.CopySkeletonDataTo(totalSkeletonsObserved);

                KinectSensor sensor = (KinectSensor)sender;

                this.SkeletonReady(sender, new SkeletonReadyArgs { defaultEventArg = e, allSkeletons = totalSkeletonsObserved,kinectId=sensor.UniqueKinectId});
            }
        }
        
    }
}
