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
using Microsoft.Kinect;

namespace MultipleKinectsPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core platform;
        public class SensorData
        {
            public ushort sensorId { get; set; }
            public string sensorStatus { get; set; }
            public string sensorStream { get; set; }
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

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbStatus.Text = Properties.Resources.KinectInitialising;

            List<KinectSensor> sensorList = this.platform.ListOfSensors();



            platform.Begin();
        }

    }
}
