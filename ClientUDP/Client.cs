using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace ClientUDP
{
    class Client
    {
        static readonly int PORT_NR = 8080;
        static readonly String SERVER_IP = "127.0.0.1";
        //static readonly String SERVER_IP = "192.168.43.152";
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(SERVER_IP), PORT_NR);

        //funkcja do wysylania intow
        void send(uint tmp)
        {
            UdpClient client = new UdpClient(SERVER_IP, PORT_NR); // tworzymy socket
           
            byte[] dataToSend = BitConverter.GetBytes(tmp); //zamieniamy dane, które chcemy wysłać, na tablice bajtow

            if(BitConverter.IsLittleEndian) //zamieniamy liczbe zapisaną w tablicy z danymi z formatu LittleEndian na BigEndian
                Array.Reverse(dataToSend);

            client.Send(dataToSend, dataToSend.Length); //wysyla dane   
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
            operacje.setOperation(ref data, 62);
            operacje.setAnswer(ref data, 13);
            operacje.setID(ref data, 54);
            operacje.printAllFields(ref data);



            Console.WriteLine("\n\nClient running...");

            while (true){
                String tmp = Console.ReadLine();
                Client client = new Client();
                
                if (tmp.Length <= 0)
                    continue;

                uint x = Convert.ToUInt32(tmp); //tutaj jak się poda SŁOWO a nie LICZBĘ to wywala exception!
                client.send(data);
            }
        }
    }
}
