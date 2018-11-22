using System;

namespace ClientUDPv3
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Client running...!");
            Client client = new Client();
            client.send("Hello");
        }
    }
}
