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
    class ServerUDPv201
    {
        static readonly int port = 8080; //port na którym obsługuje serwer

        public static List<ClientInfo> clientList = new List<ClientInfo>(); //zebrane dane o klientach połaczonych z serwerem
        public static int[] uniqueID = new int[8]; // lista unikalnych ID
        public static UdpClient socket = null;  //gniazdo serwera UDP
        public static IPEndPoint clientEndpoint = null; //adres ip i port clienta
        public static byte[] receivedData = null;
        //uint data = 0;

        public static void Main(string[] args)
        {
            IntOperations operacje = new IntOperations();
            Console.WriteLine(" Hello World!\n I'm a new UDP Server!\n Come in :-)\n\n");


            while(true)
            {
                Console.WriteLine("Server is running...");
                socket = new UdpClient(port);
                clientEndpoint = new IPEndPoint(IPAddress.Any, port);
                receivedData = socket.Receive(ref clientEndpoint);
                socket.Connect(clientEndpoint);
               
                ReceivedMsg(receivedData);

                // showClients();
                socket.Close();
            }
            Console.WriteLine("Server is not running...\nSee you later!");
        }

        private static void ReceivedMsg(byte[] data)
        {
          //  Console.WriteLine("int " + (int)data[0] + " int " + (int)data[1] + " gowna " + data[2] + data[3]);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            string receivedMessage = Encoding.ASCII.GetString(data);
           
            Console.WriteLine("length: " + receivedMessage.Length);
            Console.WriteLine("= 21: " + receivedMessage.Equals("21"));
            Console.WriteLine("to string: " + receivedMessage.ToString());
            Console.WriteLine("Hash code: " + receivedMessage.GetHashCode());
            Console.WriteLine("<From client:> " + receivedMessage.ToString() + "\r\n");
          //  Console.WriteLine("int " + (int)data[0] + " int " + (int)data[1] + " gowna " + data[2] + data[3]);

         }

        private static void showClients() //wyswietla listę z klientami, ktorzy się połączyli
        {
            if (clientList.Count > 0)
            {
                foreach (ClientInfo tmp in clientList)
                {
                    Console.WriteLine("Client ip:{0} id: {1}", tmp.ip, tmp.SessionID);
                }
                Console.WriteLine("Amount of clients: " + clientList.Count);
            }
            else
            {
                Console.WriteLine("List is empty :-(");
            }
        }
    }
    class ClientInfo
    {
        public IPAddress ip;
        public byte SessionID { set; get; }
        bool invited { set; get; }

        public ClientInfo(IPAddress ip = null)
        {
            this.ip = ip;
            
        }
    }
}
