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

namespace MultipleKinectsPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core platform;
        private Timer sensorStatusTimer;

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

        private void InitSensorStatusTimer()
        {
            sensorStatusTimer = new Timer();
            sensorStatusTimer.Elapsed += new ElapsedEventHandler(SensorStatusTimer_Elapsed);
            sensorStatusTimer.Interval = 500; 
            sensorStatusTimer.Start();
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
            
            InitSensorStatusTimer();

            platform.Begin();

            platform.GetDepthStream(0,this.DepthImageReady);
        }

        private void PopulateSensorList(List<KinectSensor> displaySensors)
        {
            foreach(KinectSensor sensor in displaySensors){

                SensorData newSensorOnDisplay = new SensorData();

                if (sensor.ColorStream.IsEnabled)
                {
                    newSensorOnDisplay.imageStream = "Enabled";
                }

                if (sensor.DepthStream.IsEnabled)
                {
                    newSensorOnDisplay.depthStream = "Enabled";
                }

                if (sensor.SkeletonStream.IsEnabled)
                {
                    newSensorOnDisplay.skeletonStream = "Enabled";
                }

                newSensorOnDisplay.sensorId = sensor.UniqueKinectId;
                newSensorOnDisplay.sensorStatus = sensor.Status.ToString();

                sensorsList.Items.Add(newSensorOnDisplay);
            }
        }

        private void SensorStatusTimer_Elapsed(object sender, EventArgs e)
        {
            List<KinectSensor> sensorList = this.platform.ListOfSensors();

            this.PopulateSensorList(sensorList);
        }

        private void DepthImageReady(object sender, Devices.DepthReadyArgs e)
        {
            this.imgMain.Source = e.depthImage;
        }
    }
}
