using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Diagnostics;

namespace ServerUDPv2
{
    class Server
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        UdpClient udpClient = new UdpClient(8080);
        byte[] receivedBytes = null;

        void receive()
        {
            IntOperations operations = new IntOperations();
            receivedBytes = udpClient.Receive(ref RemoteIpEndPoint);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(receivedBytes);

            uint tmp = BitConverter.ToUInt32(receivedBytes, 0);

            Console.Write("\nReceived int: {0}, " +
                          "\nRemoteIPEndPoint {1}" +
                          "\nAnswer Field: {2}",
                          BitConverter.ToUInt32(receivedBytes, 0),
                          RemoteIpEndPoint.Address,
                          operations.getAnswer(ref tmp)
                         );
        }

        public static void Main(string[] args)
        {

            Console.WriteLine("Server running...");
            Server server = new Server();
            while (true)
            {
                server.receive();
            }
        }
    }
}

/*

class Server
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        UdpClient udpClient = new UdpClient(8080);
        byte[] receivedBytes = null;

        void receive()
        {
            receivedBytes = udpClient.Receive(ref RemoteIpEndPoint);
            
            if (BitConverter.IsLittleEndian)
                Array.Reverse(receivedBytes);

            Console.Write("\nReceived int: {0}, " +
                          "\nRemoteIPEndPoint {1}",
                          BitConverter.ToInt32(receivedBytes, 0),
                          RemoteIpEndPoint.Address
                         );
        }

        public static void Main(string[] args)
        {

            Console.WriteLine("Server running...");
            Server server = new Server();
            while (true)
            {
                server.receive();
            }
        }
    }

*/


