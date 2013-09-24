using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleKinectsPlatform.MultipleKinectsPlatform.Networks
{
    class NetworkManagers
    {
        public enum AgentType {Skeleton,Depth,Color}

        public NetworkManagers()
        {
            
        }

        ~NetworkManagers()
        {
        }

        public Agent GetAgent(AgentType type){

            Agent newAgent = null;

            switch (type)
            {
                case AgentType.Skeleton:

                    newAgent = new SkeletonAgent();

                    break;
                case AgentType.Depth:
                    break;
                case AgentType.Color:
                    break;
            }

            return newAgent;
        }
    }
}
