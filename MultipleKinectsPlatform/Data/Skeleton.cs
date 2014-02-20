using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Kinect;
using System.IO;

namespace MultipleKinectsPlatformClient.MultipleKinectsPlatform.Data
{
    [DataContract(Name="Skeleton")]
    public class Skeleton
    {
        [DataMember(Name="joints")]
        public List<Joint> Joints;

        [DataMember]
        public int skeleton_id;

        [DataMember]
        public float pos_x;

        [DataMember]
        public float pos_y;

        [DataMember]
        public float pos_z;

        [DataMember]
        public uint client_id;

        [DataMember]
        public string sensor_id;

        [DataMember]
        public string trackingMode;

        public Skeleton(List<Joint> givenJoints,float i_x,float i_y,float i_z,uint i_clientId,string i_sensorId,int i_skeletonId,string i_trackingMode)
        {
            Joints = givenJoints;
            pos_x = i_x;
            pos_y = i_y;
            pos_z = i_z;
            client_id = i_clientId;
            sensor_id = i_sensorId;
            skeleton_id = i_skeletonId;
            trackingMode = i_trackingMode;
        }

        public static List<Skeleton> ConvertKinectSkeletons(Microsoft.Kinect.Skeleton[] obtainedSkeletons,uint clientId, string kinectId)
        {
            List<Skeleton> convertedSkeletons = new List<Skeleton>();

            foreach (Microsoft.Kinect.Skeleton skeleton in obtainedSkeletons)
            {
                if (skeleton.TrackingState.Equals(SkeletonTrackingState.PositionOnly) || skeleton.TrackingState.Equals(SkeletonTrackingState.Tracked))
                {
                /* Get all joints of the skeleton */

                List<Joint> convertedJoints = new List<Joint>();

                int numberOfJoints = skeleton.Joints.Count;

                for (ushort cur_joint = 0; cur_joint < numberOfJoints; cur_joint++)
                {
                    Joint.JointType type = (Joint.JointType)cur_joint;

                    foreach (Microsoft.Kinect.Joint joint in skeleton.Joints)
                    {
                        if (joint.JointType.ToString() == type.ToString())
                        {
                            SkeletonPoint points = joint.Position;

                            MultipleKinectsPlatform.Data.Joint convertedJoint = new MultipleKinectsPlatform.Data.Joint(points.X, points.Y, points.Z,joint.TrackingState.ToString());

                            convertedJoints.Add(convertedJoint);

                            break;
                        }
                    }
                }

                    /* Get the position of the skeleton */

                    SkeletonPoint skeletonPos = skeleton.Position;

                    MultipleKinectsPlatform.Data.Skeleton convertedSkeleton = new Skeleton(convertedJoints,skeletonPos.X,skeletonPos.Y,skeletonPos.Z,clientId,kinectId,skeleton.TrackingId,skeleton.TrackingState.ToString());

                    convertedSkeletons.Add(convertedSkeleton);
                }
            }

            return convertedSkeletons;
        }

        public static string ConvertToJSON(List<MultipleKinectsPlatform.Data.Skeleton> skeletonsToBeSerialise)
        {
            string json = "";

            if (skeletonsToBeSerialise.Count > 0)
            {
                MemoryStream memStream = new MemoryStream();

                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(List<MultipleKinectsPlatform.Data.Skeleton>));

                jsonSer.WriteObject(memStream, skeletonsToBeSerialise);

                json = System.Text.Encoding.UTF8.GetString(memStream.GetBuffer(), 0, Convert.ToInt32(memStream.Length));
            }

            return json;
        }
    }
}
