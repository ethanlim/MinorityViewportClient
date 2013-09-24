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
        [DataMember(Name="Joints")]
        public List<Joint> Joints;

        [DataMember]
        public float pos_x;

        [DataMember]
        public float pos_y;

        [DataMember]
        public float pos_z;

        public Skeleton(List<Joint> givenJoints,float i_x,float i_y,float i_z)
        {
            Joints = givenJoints;
            pos_x = i_x;
            pos_y = i_y;
            pos_z = i_z;
        }

        public static List<Skeleton> ConvertKinectSkeletons(Microsoft.Kinect.Skeleton[] obtainedSkeletons)
        {
            List<Skeleton> convertedSkeletons = new List<Skeleton>();

            foreach (Microsoft.Kinect.Skeleton skeleton in obtainedSkeletons)
            {
                /* Get all joints of the skeleton */

                List<Joint> convertedJoints = new List<Joint>();

                foreach (Microsoft.Kinect.Joint joint in skeleton.Joints)
                {
                    SkeletonPoint points  = joint.Position;

                    MultipleKinectsPlatform.Data.Joint convertedJoint = new MultipleKinectsPlatform.Data.Joint(points.X,points.Y,points.Z);

                    convertedJoints.Add(convertedJoint);
                }

                /* Get the position of the skeleton */

                SkeletonPoint skeletonPos = skeleton.Position;

                MultipleKinectsPlatform.Data.Skeleton convertedSkeleton = new Skeleton(convertedJoints,skeletonPos.X,skeletonPos.Y,skeletonPos.Z);

                convertedSkeletons.Add(convertedSkeleton);
            }

            return convertedSkeletons;
        }

        public static string ConvertToJSON(List<MultipleKinectsPlatform.Data.Skeleton> skeletonsToBeSerialise)
        {
            MemoryStream memStream = new MemoryStream();

            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(List<MultipleKinectsPlatform.Data.Skeleton>));

            jsonSer.WriteObject(memStream, skeletonsToBeSerialise);
    
            string json = System.Text.Encoding.UTF8.GetString(memStream.GetBuffer(), 0, Convert.ToInt32(memStream.Length));

            return json;
        }
    }
}
