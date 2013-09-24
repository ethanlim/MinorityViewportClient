using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Kinect;
using System.Timers;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Data;
using MultipleKinectsPlatformClient.MultipleKinectsPlatform.Devices;

namespace MultipleKinectsPlatformClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core platform;

        public class SensorData
        {
            public string sensorId { get; set; }
            public string sensorStatus { get; set; }
            public string imageStream { get; set; }
            public string depthStream { get; set; }
            public string skeletonStream { get; set; }

            public SensorData()
            {
                imageStream = "Not Enabled";
                depthStream = "Not Enabled";
                skeletonStream = "Not Enabled";
            }
        }

        public MainWindow()
        {  
            /* Initialise the Main Window */
            InitializeComponent();
       
            /* Attach Callbacks to the Main Window once Loaded */
            this.Loaded += MainWindow_Loaded;

            /* Create an MultiKinectPlatform object */
            this.platform = new Core();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbStatus.Text = Properties.Resources.KinectInitialising;
            
            List<KinectSensor> activeSensorList = this.platform.ListOfSensors();

            if (activeSensorList.Count != 0)
            {
                this.tbStatus.Visibility = System.Windows.Visibility.Hidden;
                this.PopulateSensorList(activeSensorList);
            }

            platform.Begin();

            platform.GetDepthStream(0,this.DepthImageReady);

            platform.GetSkeletonStream(0, this.SkeletonReady,true,"localhost");
        }

        private void PopulateSensorList(List<KinectSensor> displaySensors)
        {
           this.sensorsList.Items.Clear();

           foreach(KinectSensor sensor in displaySensors){

                SensorData newSensorOnDisplay = new SensorData();

                if (sensor !=null && sensor.ColorStream!=null && sensor.ColorStream.IsEnabled)
                { 
                    newSensorOnDisplay.imageStream = "Enabled";
                }

                if (sensor != null && sensor.DepthStream!=null && sensor.DepthStream.IsEnabled)
                {
                    newSensorOnDisplay.depthStream = "Enabled";
                }

                if (sensor != null && sensor.SkeletonStream!=null && sensor.SkeletonStream.IsEnabled)
                {
                    newSensorOnDisplay.skeletonStream = "Enabled";
                }

                newSensorOnDisplay.sensorId = sensor.UniqueKinectId;
                newSensorOnDisplay.sensorStatus = sensor.Status.ToString();

                sensorsList.Items.Add(newSensorOnDisplay);
            }
        }

        private void DepthImageReady(object sender, DepthReadyArgs e)
        {
            this.imgMain.Source = e.depthImage;

            this.PopulateSensorList(this.platform.ListOfSensors());
        }

        private void SkeletonReady(object sender, SkeletonReadyArgs e)
        {
            this.PopulateSensorList(this.platform.ListOfSensors());
        }

    }
}
