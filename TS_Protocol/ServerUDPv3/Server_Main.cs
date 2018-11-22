using System;

namespace ServerUDPv3
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Server Running...");
            Server server = new Server();
            server.Receive();

            while(true){}

        }
    }
}
