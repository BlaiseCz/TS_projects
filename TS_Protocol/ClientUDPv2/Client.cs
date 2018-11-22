using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace ClientUDPv2
{
    class Client
    {
        static readonly int PORT_NR = 8080;
        static readonly String SERVER_IP = "127.0.0.1";
        //static readonly String SERVER_IP = "192.168.43.152";
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(SERVER_IP), PORT_NR);

        void send(int tmp){
            UdpClient client = new UdpClient(SERVER_IP, PORT_NR);
            byte[] dataToSend = BitConverter.GetBytes(tmp);

            if(BitConverter.IsLittleEndian)
                Array.Reverse(dataToSend);
            

            foreach (byte x in dataToSend)
                Console.Write(x + " ");
            Console.WriteLine();

            client.Send(dataToSend, dataToSend.Length);
            
        }

        void send(String tmp)
        {
            UdpClient client = new UdpClient(SERVER_IP, PORT_NR);
            byte[] dataToSend = Encoding.ASCII.GetBytes(tmp);
            client.Send(dataToSend, dataToSend.Length);
        }

        void sendStatInt(){
            UdpClient client = new UdpClient(SERVER_IP, PORT_NR);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Client running...");
            while (true){
                String tmp = Console.ReadLine();
                Client client = new Client();
                int x = Convert.ToInt32(tmp);
                client.send(x);

            }
        }
    }
}
