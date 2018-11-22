using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace ServerUDP
{
    class MainClass
    {
        public const byte HEADER_AUTH_FAILED = 0x00;
        public const byte HEADER_AUTH_SUCCEEDED = 0x01;
        public const byte HEADER_AUTH_TOKEN = 0x02;
        public const byte HEADER_SERVER_RESPONSE = 0xFF;

        public const int port = 600;

        public const string password = "pass";


        public static List<ClientInfo> clientList = new List<ClientInfo>();
        public static UdpClient serverSocket = null;
        public static IPEndPoint clientEndpoint = null;
        public static byte[] receivedData = null;

        static void Main(string[] args)
        {

            /* Server return headers (first byte):
             * 0x00 = (Authentication failed). Client's ip-address will be removed from clientlist.
             * 0x01 = (Authentication succeeded). ClientInfo.authenticated will be set true for corresponding ip-address.
             * 0x02 = (Authentication required). Authentication token will be created and sent to the client with this header. Client's ip-address and token will be added to clientlist aswell.
             */


            while (true)
            {
                serverSocket = new UdpClient(port);
                clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                receivedData = serverSocket.Receive(ref clientEndpoint);
                serverSocket.Connect(clientEndpoint);

                ClientStatus cs = CheckAuthenticationStatus(clientEndpoint.Address);
                Console.WriteLine(cs + "\r\n");

                switch (cs)
                {
                    case ClientStatus.CLIENT_AUTH_NEW:
                        GenerateSendToken();
                        break;

                    case ClientStatus.CLIENT_AUTH_RECEIVED:
                        CheckAuthResponse(receivedData);
                        break;

                    case ClientStatus.CLIENT_AUTH_OK:
                        HandleData(receivedData);
                        break;
                }

                serverSocket.Close();
            }
        }







        public static void HandleData(byte[] data)
        {
            string receivedMessage = Encoding.ASCII.GetString(data);
            Console.WriteLine("<From client:> " + receivedMessage + "\r\n");
            var response = Encoding.ASCII.GetBytes("<SERVER RESPONSE:> " + receivedMessage);


            //Header 0xFF is used to tell client that it is still on list of authenticated
            //clients and can send packets normally.
            serverSocket.Send((new byte[] { HEADER_SERVER_RESPONSE }).Concat(response).ToArray(), response.Length + 1);
        }







        public static void CheckAuthResponse(byte[] data)
        {
            string response = Encoding.ASCII.GetString(data);
            int clientId = GetClientIdByIp(clientEndpoint.Address);

            if (response.Equals(ComputeHash(clientList[clientId].token + password)))
            {
                clientList[clientId].authenticated = true;
                serverSocket.Send(new byte[] { HEADER_AUTH_SUCCEEDED }, 1);
                return;
            }

            clientList.RemoveAt(clientId);
            serverSocket.Send(new byte[] { HEADER_AUTH_FAILED }, 1);
        }





        public static void GenerateSendToken()
        {
            string guidString = Guid.NewGuid().ToString();
            byte[] guid = Encoding.ASCII.GetBytes(guidString);
            byte[] finalPacket = (new byte[] { HEADER_AUTH_TOKEN }).Concat(guid).ToArray();

            serverSocket.Send(finalPacket, finalPacket.Length);

            clientList.Add(new ClientInfo(clientEndpoint.Address));
            clientList[GetClientIdByIp(clientEndpoint.Address)].token = guidString;
        }







        public static ClientStatus CheckAuthenticationStatus(IPAddress ip)
        {

            ClientInfo clientdata = GetClientInfoByIp(ip);

            if (clientdata != null)
            {
                if (clientdata.authenticated)
                    return ClientStatus.CLIENT_AUTH_OK;

                else
                    return ClientStatus.CLIENT_AUTH_RECEIVED;
            }

            else
                return ClientStatus.CLIENT_AUTH_NEW;

        }





        public static ClientInfo GetClientInfoByIp(IPAddress ip)
        {
            return clientList.Find(i => (i.ip.Equals(ip)));
        }


        public static int GetClientIdByIp(IPAddress ip)
        {
            return clientList.FindIndex(i => (i.ip.Equals(ip)));
        }




        public static String ComputeHash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Join("", hash.ComputeHash(Encoding.UTF8.GetBytes(value)).Select(item => item.ToString("x2")));
            }
        }

    }



    class ClientInfo
    {
        public IPAddress ip;
        public bool authenticated = false;
        public string token;

        public ClientInfo(IPAddress ip = null)
        {
            this.ip = ip;
        }
    }

    enum ClientStatus
    {
        CLIENT_AUTH_NEW = 0,
        CLIENT_AUTH_RECEIVED = 2,
        CLIENT_AUTH_OK = 1
    }
}


/*
 *         public static void Main(string[] args)
        {





            Console.WriteLine("I'm a server!");
            int recv; //otrzymane dane
            byte[] data = new byte[1024];
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 904);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Bind(endpoint);// Bindowanie jakiekolwiek przychodzące połaczenie do tego gniazda

            Console.WriteLine("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 904);
            EndPoint tmpRemote = (EndPoint)sender; //program zatrzyma sie i czeka na połączenie, client przechodzi do tej zmiennej

            recv = socket.ReceiveFrom(data, ref tmpRemote);

            Console.Write("Message received from {0}", tmpRemote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv)); // konwertowanie bajtów na string, ktory wyswietlimy w konsoli

            string welcome = "Welcome to my server!";
            data = Encoding.ASCII.GetBytes(welcome);

            if(socket.Connected) //jesli nastąpi połączenie
            {
                socket.Send(data);
            }


            while (true)
            {
                if (!socket.Connected)
                {
                    Console.WriteLine("Client disconnected");
                    break;
                }

                data = new byte[1024];
                recv = socket.ReceiveFrom(data, ref tmpRemote);

                if (recv == 0) {
                    break;
                }

                Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            }
        }
 * 
 */
