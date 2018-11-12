using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ServerUDPv2
{
    /*
     * u mnie musi być wątek z odliczaniem tych 10 sekund 
     * powiadomienie o zakończeniu połaczania
     * wysylam dwa komunikaty
     * 1. 10 sekund minelo 
     * 2. ze niepoprawna liczba (!reciveddata==secretInt) send bledny int else to poprawna odpowiedz
     */
    class Server
    {

        UdpClient server = new UdpClient(8080);
        public static IPEndPoint IpEndPoint = null;
        byte[] receivedBytes = null;
        public static List<ClientInfo> clientList = new List<ClientInfo>(); //zebrane dane o klientach połaczonych z serwerem
        public static List<uint> uniqueID = new List<uint>(); // lista unikalnych ID

        public static void Main(string[] args)
        {

            Console.WriteLine(" Hello World!\n I'm a new UDP Server!\n Come in :-)\n\n");

            Console.WriteLine("Server is running...");
            Server server = new Server();


            while (true)
            {

                IpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                IntOperations operations = new IntOperations();

                ClientInfo client = new ClientInfo(IpEndPoint.Address);
                if(clientList.Find())
                {
                    Console.WriteLine("You're old guest!");
                }
                else
                {
                    Console.WriteLine("New Guest \nWelcome!");
                    clientList.Add(client);
                }

                uint value;
                value = server.Receive();
                if (operations.getAnswer(ref value) == (uint)15)
                {
                    Console.WriteLine("Correct answer!");
                }
                else
                {
                    Console.WriteLine("Try again...");
                }
                ShowClients();
            }
        }

       

        int RemainigTime(int id1, int id2)   // [(id.sesji 1 + id.sesji 2) * 99] % 100 + 30
        {
            return ((id1 + id2) * 99) % 100 + 30;
        }

        int SecretNum() //tajna liczba do odgadnięcia przez połączonych klientów
        {
            Random rand = new Random();
            return rand.Next(0, 127);
        }
        //cykliczne przesyłanie komunikatów z informacją, ile czasu pozostało do zakończenia (co 10 sekund)

        uint Receive()
        {
            IntOperations operations = new IntOperations();
            receivedBytes = server.Receive(ref IpEndPoint);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(receivedBytes);

            uint tmp = BitConverter.ToUInt32(receivedBytes, 0);
            /*
            Console.Write("\nReceived int: {0}, " +
                          "\nRemoteIPEndPoint {1}" +
                          "\nAnswer Field: {2}\n",
                          BitConverter.ToUInt32(receivedBytes, 0),
                          IpEndPoint.Address,
                          operations.getAnswer(ref tmp)
                         );

            */
            Console.WriteLine("\nReceived int: {0}", BitConverter.ToUInt32(receivedBytes, 0));
            return tmp;
        }

        public static ClientInfo GetClientInfoByIp(IPAddress ip)
        {
            return clientList.Find(i => (i.ip.Equals(ip)));
        }


        public static int GetClientIdByIp(IPAddress ip)
        {
            return clientList.FindIndex(i => (i.ip.Equals(ip)));
        }

        public static void ShowClients() //wyswietla listę z klientami, ktorzy się połączyli
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

        private byte _SessionID;
        public IPAddress ip;
        public byte SessionID
        {
            get
            {
                return _SessionID;
            }
            set
            {
                _SessionID = value;
            }
        }

        public ClientInfo(IPAddress ip = null)
        {
            this.ip = ip;
            SessionID = GenerateID();
        }

        bool IsNew(List<uint> uni)
        {
            if(uni.Contains(this.SessionID))
                return false;
            else
                return true;
        }

        private static byte GenerateID() //generowanie id sesji
        {
            byte ID;
            Random rand = new Random();
            ID = (byte)rand.Next(0, 255);
            return ID;
        }
    }
}

