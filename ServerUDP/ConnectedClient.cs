using System;
using System.Net;

namespace Server_Asyn
{
    public class ConnectedClient
    {
        IPEndPoint endPoint;
        uint ID;
        public ConnectedClient(uint ID, IPEndPoint endPoint)
        {
            this.ID = ID;
            this.endPoint = endPoint;
        }

        public IPEndPoint getEndpoint(){
            return endPoint;
        }
        public uint getID(){
            return ID;
        }
    }
}
