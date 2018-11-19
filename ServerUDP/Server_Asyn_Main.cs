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

        const uint ID_REQUST = 1; //odbiera jesli klient chce nadania ID
        const uint ID_SENT = 2; //wysyla takie polecenie, jezeli nadano ID
        const uint NUMB_SEND = 3; //odbiera jesli klient wyslal liczbe
        const uint ANSWER_CONFIRM = 4; //odpowiada na pytanie czy zgadnieto
        const uint END_CONNCECTION = 5; //jesli klient opuszcza polaczenie wysylana jest taka informacja
        const uint TEN_SEC_REMIND = 6; //wywołanie przypomnienia
        const uint TIME_END = 7; //wysla jesli w zadanym czasie nie zgadnieto
        const uint NUMB_REQUEST = 8; //wysylane jesli chce, aby klient wyslal liczbe


        IntOperations operations = new IntOperations();
        private readonly int PORT = 8080; //ustawiamy port na jakim włączamy serwer
        private UdpClient client; //deklaracja clienta UDP
        private readonly int BYTES_TO_SEND = 3; //jaka dlugosc pola danych w bajtach chcemy wyslac

        //to trzeba zmienic bo nie współpracuje z foreach'em na vektor
        List<ConnectedClient> clients = new List<ConnectedClient>();
        public List<ReceivedData> receiveds = new List<ReceivedData>();
        uint secretNumber = 11;

        //konstuktor
        Server(){
            this.client = new UdpClient(PORT); //ustawiamy port dla clienta
        }

        public void Send(uint answer, uint operation, uint ID, IPEndPoint endPoint)
        {
            byte[] bytes_to_send = new byte[3]; //to wysylam
            operations.setAllFields(ref bytes_to_send, operation, answer, ID, 0); //ustawiam pola do wyslania
            client.Send(bytes_to_send, BYTES_TO_SEND, endPoint); //wysyla byte[3]
            Console.WriteLine("Wyslano");
        }

        private uint GenerateID(){
            Random rand = new Random();
            return (uint)rand.Next(0, 255);
        }

        //do faktycznej obslugi requestow
        private void OperateRequest(ReceivedData data)
        {
            byte[] recvd_data = data.getData(); //wyluskuje odebrane bajty 
            IPEndPoint endPoint = data.GetEndPoint(); //wyluskuje endpoint z ktorego zostala informacja odebrana

            uint operation = operations.GetOperation(ref recvd_data);
            switch(operation)
            {
                //Obsluga requestu dot. nadania ID
                case ID_REQUST:
                    uint generatedID = GenerateID(); //generuje ID
                    Send(0, ID_SENT, generatedID, endPoint); //wysyla z poleceniem ID_SENT
                    Send(0, NUMB_REQUEST, generatedID, endPoint);
                    clients.Add(new ConnectedClient(generatedID, endPoint)); //tutaj dodaje do mapy endpoint i ID sesji

                    foreach(ConnectedClient cl in clients){
                        Console.WriteLine(cl.getID() + "  " + cl.getEndpoint().Address);
                    }
                    break;

                    //obebrano liczbe
                case NUMB_SEND:
                    break;
            }
        }

        //do przesiania requestow
        private void ManageRequests(){
            //Console.WriteLine("Request Manager Started");
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
                operations.printImpFields(ref receivedBytes); //drukuje to co otrzymal
                receiveds.Add(new ReceivedData(receivedBytes, RemoteIpEndPoint)); //wrzuca otrzymany wynik razem z EndPointerm do Tablicy Oderbanych danych
                Console.WriteLine("Odebrano");
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
                //Console.WriteLine("Dlugosc tablicy z odebranymi danymi: " + server.receiveds.Count);
                server.ManageRequests();
            }
        }
    }
}
