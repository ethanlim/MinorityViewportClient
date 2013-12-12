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
        private System.Windows.Threading.DispatcherTimer frameRateTimer;
        private System.Windows.Threading.DispatcherTimer sensorsListRefreshTimer;
        private double skeletonFramesRecv;
        private double depthFramesRecv;

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

            depthFramesRecv = 0;
            skeletonFramesRecv = 0;
            frameRateTimer = new System.Windows.Threading.DispatcherTimer();
            frameRateTimer.Tick += new EventHandler(FrameRateEvent);
            frameRateTimer.Interval = new TimeSpan(0,0,1);
            frameRateTimer.Start();

            sensorsListRefreshTimer = new System.Windows.Threading.DispatcherTimer();
            sensorsListRefreshTimer.Tick += new EventHandler(RefreshSensorList);
            sensorsListRefreshTimer.Interval = new TimeSpan(0, 0, 5);
            sensorsListRefreshTimer.Start();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SensorsInitialisation();

            this.PopulateSensorSelection(true);
        }

        private void SensorsInitialisation()
        {
            List<KinectSensor> activeSensorList = this.platform.ListOfSensors();
 
            for (ushort sensorId = 0; sensorId < activeSensorList.Count; sensorId += 1)
            {
                platform.GetDepthStream(sensorId, this.DepthImageReady);

                platform.GetSkeletonStream(sensorId, this.SkeletonReady, true, "localhost");
            }
        }

        private void PopulateSensorSelection(bool firstTime)
        {
            List<KinectSensor> activeSensorList = this.platform.ListOfSensors();
            int selectedItemIdx = 0;

            if (!firstTime)
            {
                selectedItemIdx = displaySensorMenu.SelectedIndex;
            }

            displaySensorMenu.Items.Clear();

            for (ushort sensorId = 0; sensorId < activeSensorList.Count; sensorId += 1)
            {
                displaySensorMenu.Items.Add(activeSensorList[sensorId].UniqueKinectId);
            }

            displaySensorMenu.SelectedIndex = selectedItemIdx;
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
            this.depthFramesRecv += 1;

            if ((string)displaySensorMenu.SelectedValue == e.kinectId)
            {
                this.imgMain.Source = e.depthImage;
            }
           
        }

        private void SkeletonReady(object sender, SkeletonReadyArgs e)
        {
            skeletonFramesRecv += 1;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            frameRateTimer.Stop();

            platform.ShutDown();

            Application.Current.Shutdown();
        }

        private void FrameRateEvent(object sender, EventArgs args)
        {
            this.combinedDepthFrameRate.Content = depthFramesRecv + " fps";
            this.combinedSkeletonFrameRate.Content = skeletonFramesRecv + " fps";

            this.depthFramesRecv = 0;
            this.skeletonFramesRecv = 0;
        }

        private void RefreshSensorList(object sender, EventArgs args)
        {
            this.PopulateSensorList(this.platform.ListOfSensors());

            this.PopulateSensorSelection(false);
        }

    }
}
