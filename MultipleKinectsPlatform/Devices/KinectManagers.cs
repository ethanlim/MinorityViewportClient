using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Kinect;         //Require the SDK Library
using System.Windows.Media.Imaging;

namespace MultipleKinectsPlatform.Devices
{
    class KinectManagers
    {
        private List<KinectSensor> kinects;

        public KinectManagers(){
            this.kinects = this.GetSensors();
        }

        ~KinectManagers(){

            
        }

        public void Shutdown(){
            for (var kinects = 0; kinects < KinectSensor.KinectSensors.Count; kinects++)
            {
                this.kinects[kinects].Stop();
            }
        }

        public void DepthFromSensor(ushort sensorId, DepthImageFormat format)
        {
            KinectSensor ofInterestSensor = kinects[sensorId];

            ofInterestSensor.Start();

            if(!ofInterestSensor.DepthStream.IsEnabled){

                kinects[sensorId].DepthStream.Enable(format);
            }
        }
        
        /**
         * Private Methods
         */
        private List<KinectSensor> GetSensors()
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

        private BitmapSource Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    short[] pixelData = new short[depthFrame.PixelDataLength];
                    int stride = depthFrame.Width * 2;
                    depthFrame.CopyPixelDataTo(pixelData);

                    BitmapSource img = BitmapImage.Create(depthFrame.Width, depthFrame.Height, 96, 96, System.Windows.Media.PixelFormats.Gray16, null, pixelData, stride);

                    return img;
                }
                else
                {
                    return null;
                }
            }
        }
        
    }
}
