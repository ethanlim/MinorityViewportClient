using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Kinect;         //Require the SDK Library
using System.Windows.Media.Imaging;

namespace MultipleKinectsPlatform.Devices
{
    public class DepthReadyArgs : EventArgs
    {
        public EventArgs defaultEventArg { get; set; }
        public BitmapSource depthImage { get; set; }
    }

    class KinectManagers
    {
        private List<KinectSensor> kinects;
        public event EventHandler<DepthReadyArgs> DepthReady;
     
        public KinectManagers(){
            this.kinects = this.InitialiseSensors();
        }

        ~KinectManagers(){}

        public void Shutdown(){
            for (var kinects = 0; kinects < KinectSensor.KinectSensors.Count; kinects++)
            {
                this.kinects[kinects].Stop();
            }
        }

        public void DepthFromSensor(ushort sensorId, 
                                    DepthImageFormat format,
                                    EventHandler<DepthReadyArgs> handler)
        {
            KinectSensor ofInterestSensor = kinects[sensorId];

            ofInterestSensor.Start();

            if(!ofInterestSensor.DepthStream.IsEnabled){
                kinects[sensorId].DepthStream.Enable(format);
                kinects[sensorId].DepthFrameReady += this.SensorDepthFrameReady;
                this.DepthReady += handler;
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

        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    short[] pixelData = new short[depthFrame.PixelDataLength];
                    int stride = depthFrame.Width * 2;
                    depthFrame.CopyPixelDataTo(pixelData);

                    BitmapSource img = BitmapImage.Create(depthFrame.Width, depthFrame.Height, 96, 96, System.Windows.Media.PixelFormats.Gray16, null, pixelData, stride);

                    this.DepthReady(sender,new DepthReadyArgs {defaultEventArg=e,depthImage=img});
                }
            }
        }
        
    }
}
