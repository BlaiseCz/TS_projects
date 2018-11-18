using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections;

namespace Server_Asyn
{
    class Server
    {

        const uint K_UZYSK_ID = 1;
        const uint S_UZYSK_ID = 2;

        IntOperations operations = new IntOperations();
        private readonly int PORT = 8080; //ustawiamy port na jakim włączamy serwer
        private UdpClient client; //deklaracja clienta UDP
        private readonly int BYTES_TO_SEND = 4; //jaka dlugosc pola danych w bajtach chcemy wyslac
        IDictionary map = new Dictionary<uint, IPEndPoint>(); //to bedzie do przechowywania informacji o podlaczonych klientach
        public List<ReceivedData> receiveds = new List<ReceivedData>();

        //konstuktor
        Server(){
            this.client = new UdpClient(PORT); //ustawiamy port dla clienta

        }

        public void Send(uint answer, uint operation, uint ID, IPEndPoint endPoint)
        {
            byte[] bytes_to_send = new byte[3]; //to wysylam
            operations.setAllFields(ref bytes_to_send, operation, answer, ID, 0); //ustawiam pola do wyslania
            client.Send(bytes_to_send, 3, endPoint); //wysyla byte[3]
        }

        private uint GenerateID(){
            Random rand = new Random();
            return (uint)rand.Next(0, 255);
        }

        //do faktycznej obslugi
        private void OperateRequest(ReceivedData data){
            byte[] recvd_data = data.getData();
            IPEndPoint endPoint = data.GetEndPoint();

            uint operation = operations.GetOperation(ref recvd_data);
            switch(operation){
                case 1:
                    uint generatedID = GenerateID();
                    Send(0, 2, generatedID, endPoint);
                    break;
            }
        }


        //do przesiania requestow
        private void ManageRequests(){
            Console.WriteLine("Request Manager Started");
            IntOperations operations = new IntOperations();
            byte[] recvd_data = new byte[3];
            //jezeli dane maja ACK = 1 nic z nimi nie rob
            //jezeli inaczej - rob to co masz zrobic i odeslij to samo z ACK

            if (receiveds.Count > 0) //jezeli sa dane w kontenerze zapisanych odebranych danych
            { 
                recvd_data = receiveds[0].getData(); //wyłuskanie otrzymanych danych

                if(operations.GetACK(ref recvd_data) == 0) //jezeli ACK = 0
                {
                    operations.SetACK(ref recvd_data, 1); //odsylam potwierdzenie odbioru
                    client.Send(recvd_data, 3, receiveds[0].GetEndPoint()); // -//-
                    OperateRequest(receiveds[0]); //przechodze do wlasciwej funkcji obslugi requestow - sciagam ostatni request
                }

                else //jezeli ACK = 1, no to nie robi nic
                {
                    Console.WriteLine("Klient cos potwierdzil");
                }

                receiveds.RemoveAt(0); // usuwa, bo zostalo obsluzone
            }
        }


        public void Receive()
        {
            while(true){
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0); //endpoint
                byte[] receivedBytes = client.Receive(ref RemoteIpEndPoint); //bajty, do których zapisujemy to co otrzymalismy
                operations.printAllFields(ref receivedBytes); //drukuje to co otrzymal
                receiveds.Add(new ReceivedData(receivedBytes, RemoteIpEndPoint)); //wrzuca otrzymany wynik razem z EndPointerm do Tablicy Oderbanych danych
            }
        }
         

        public static void Main(string[] args)
        {
            Console.WriteLine("Server running!");

            Server server = new Server();

            Thread recv_thr = new Thread(server.Receive);
            recv_thr.Start();


            while (true)
            {
                
                //Console.Write(".");
                Thread.Sleep(1000);
                Console.WriteLine("Dlugosc tablicy z odebranymi danymi: " + server.receiveds.Count);
                server.ManageRequests();

                //server.ManageRequests();
            }
        }
    }
}
