using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ServerUDPv2
{

    class MainClass
    {
        static readonly int port = 8080;

        internal static void Main(string[] args)
        {
            IntOperations operacje = new IntOperations();
            uint data_ = 0;
            operacje.setOperation(ref data_, 62);
            operacje.setAnswer(ref data_, 13);
            operacje.setID(ref data_, 54);
            operacje.printAllFields(ref data_);


            Console.WriteLine("I'm a server!");
            int recv = 0; //otrzymane dane
            byte[] data = new byte[1024];
            byte[] receivedBytes = new byte[4];
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            /*
            if (BitConverter.IsLittleEndian)
                Array.Reverse(receivedBytes);
            */

            socket.Bind(endpoint);// Bindowanie jakiekolwiek przychodzące połaczenie do tego gniazda

            Console.WriteLine("Waiting for a client...");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, port);
            EndPoint tmpRemote = (EndPoint)sender; //program zatrzyma sie i czeka na połączenie, client przechodzi do tej zmiennej

            recv = socket.ReceiveFrom(data, ref tmpRemote);
            Console.WriteLine("Encoding " + Encoding.ASCII.GetString(data, 0, recv)); // konwertowanie bajtów na string, ktory wyswietlimy w konsoli
          

            while (true)
            {
                if (!socket.Connected)
                {
                    Console.WriteLine("Client disconnected");
                    break;
                }

                Console.WriteLine("Success - Client connected");

                data = new byte[1024];
                recv = socket.ReceiveFrom(data, ref tmpRemote);

                if (recv == 0)
                {
                    break;
                }
                 Console.WriteLine("Received int: {0}", recv);
                 Console.WriteLine("BitConverter" + BitConverter.GetBytes(recv));
                 Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            }
        }

    }

}

//byte[] recevedBytes = socket.Receive();
/*
Console.WriteLine("Message received from {0}", tmpRemote.ToString());
Console.WriteLine("BitConverter "+BitConverter.ToInt32(data, 0)); // konwertowanie bajtów na string, ktory wyswietlimy w konsoli
Console.WriteLine("Encoding "+Encoding.ASCII.GetString(data, 0, recv)); // konwertowanie bajtów na string, ktory wyswietlimy w konsoli
Console.WriteLine("Received int: {0}",recv);
string welcome = "Welcome to my server!";
data = Encoding.ASCII.GetBytes(welcome); //kodowanie welcome

if (socket.Connected) //jesli nastąpi połączenie
{
    socket.Send(data);
}
*/
