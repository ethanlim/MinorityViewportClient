using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace MultipleKinectsPlatformClient.MultipleKinectsPlatform.Data
{
    [DataContract(Name="Joint")]
    public class Joint
    {
        [DataMember]
        public float X;

        [DataMember]
        public float Y;

        [DataMember]
        public float Z;

        [DataMember]
        public string trackedMode;

        public enum JointType{
            AnkleLeft,
            AnkleRight,
            ElbowLeft,
            ElbowRight,
            FootLeft,
            FootRight,
            HandLeft,
            HandRight,
            Head,
            HipCenter,
            HipLeft,
            HipRight,
            KneeLeft,
            KneeRight,
            ShoulderCenter,
            ShoulderLeft,
            ShoulderRight,
            Spine,
            WristLeft,
            WristRight
        }

        public Joint(float i_x, float i_y, float i_z,string i_trackingMode)
        {
            X = i_x;
            Y = i_y;
            Z = i_z;
            trackedMode = i_trackingMode;
        }

    }
}
