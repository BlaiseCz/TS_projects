using System;
using System.Net;

namespace Server_Asyn
{
    public class ReceivedData
    {
        byte[] receivedData;
        IPEndPoint endPoint;

        public ReceivedData(byte[] data, IPEndPoint endPoint)
        {
            this.receivedData = data;
            this.endPoint = endPoint;
        }

        public byte[] getData(){
            return this.receivedData;
        }

        public IPEndPoint GetEndPoint(){
            return this.endPoint;
        }
    }
}
