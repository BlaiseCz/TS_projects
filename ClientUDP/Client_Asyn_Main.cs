using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client_Asyn
{
    class Client
    {
        IntOperations operations = new IntOperations();
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0); //endpoint
        readonly string IP_ADDR_SERVER = "127.0.0.1";
        readonly int PORT = 8080;
        private UdpClient client; //deklaracja clienta UDP
        //private readonly int BYTES_TO_SEND = 4; //jaka dlugosc pola danych w bajtach chcemy wyslac
        uint ID; //ID klienta
        //public List<ReceivedData> receiveds = new List<ReceivedData>();


        Client(){
            client = new UdpClient(IP_ADDR_SERVER, PORT);
            ID = 0; //narazie na sztywno
            Console.WriteLine(RemoteIpEndPoint.AddressFamily);
        }

        public void Send(uint answer, uint operation){
            byte[] bytes_to_send = new byte[3]; //to wysylam
            operations.setAllFields(ref bytes_to_send, operation, answer, ID, 0); //ustawiam pola do wyslania
            client.Send(bytes_to_send, 3); //wysyla byte[3]
        }

        public void Receive()
        {
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0); //endpoint
                byte[] receivedBytes = client.Receive(ref RemoteIpEndPoint); //bajty, do których zapisujemy to co otrzymalismy
                operations.printAllFields(ref receivedBytes); //drukuje to co otrzymal


                //receiveds.Add(new ReceivedData(receivedBytes, RemoteIpEndPoint)); //wrzuca otrzymany wynik razem z EndPointerm do Tablicy Oderbanych danych
            }
        }

        private void ManageRequests()
        {
            Console.WriteLine("Request Manager Started");
            IntOperations operations = new IntOperations();
            byte[] recvd_data = new byte[3];
            //jezeli dane maja ACK = 1 nic z nimi nie rob
            //jezeli inaczej - rob to co masz zrobic i odeslij to samo z ACK
        }

        public static void Main(string[] args)
        {
            IntOperations operations = new IntOperations();
            Console.WriteLine("Client running...");
            Client client = new Client();

            Thread recv_thr = new Thread(client.Receive);
            recv_thr.Start();

            client.Send(0, 1); //prosba o uzyskanie ID sesji


            while (true)
            {

                byte[] tmp = new byte[3];

            }
        }
    }
}
