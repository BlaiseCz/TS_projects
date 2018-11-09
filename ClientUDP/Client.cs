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


        void send(int tmp)
        {
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

        public static void Main(string[] args)
        {
            IntOperations operacje = new IntOperations();
            uint data = 0;
            operacje.setOperation(ref data, 32);
            operacje.setAnswer(ref data, 13);
            operacje.setID(ref data, 54);
            Console.Write("Operation Field: {0}" +
                          "\nAnswer Field: {1}" +
                          "\nID Field: {2}" +
                          "\nBinary Data Number: {3}" +
                          "\nBinary Operation Field: {4}" +
                          "\nBinary Answer Field: {5}" +
                          "\nBinary ID Field: {6}",
                          operacje.getOperation(ref data),
                          operacje.getAnswer(ref data),
                          operacje.getID(ref data),
                          operacje.convertToBinary(data),
                          operacje.convertToBinary(operacje.getOperation(ref data)),
                          operacje.convertToBinary(operacje.getAnswer(ref data)),
                          operacje.convertToBinary(operacje.getID(ref data))
                          );


            /*
            Console.WriteLine("Client running...");
            while (true){
                String tmp = Console.ReadLine();
                Client client = new Client();
                int x = Convert.ToInt32(tmp);
                client.send(x);

            }
            */
        }
    }
}
