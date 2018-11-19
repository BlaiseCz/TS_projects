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
        const uint NUMB_RCV = 3; //odbiera jesli klient wyslal liczbe
        const uint ANSWER_CONFIRM = 4; //odpowiada na pytanie czy zgadnieto
        const uint END_CONNCECTION = 5; //jesli klient opuszcza polaczenie wysylana jest taka informacja
        const uint TEN_SEC_REMIND = 6; //wywołanie przypomnienia
        const uint TIME_END = 7; //wysyla jesli w zadanym czasie nie zgadnieto
        const uint NUMB_REQUEST = 8; //wysylane jesli chce, aby klient wyslal liczbe
        const uint SECND_CLIENT_AWAIT = 9; //wysylane jezeli nie jest polaczony drugi klient


        IntOperations operations = new IntOperations();
        private readonly int PORT = 8080; //ustawiamy port na jakim włączamy serwer
        private UdpClient client; //deklaracja clienta UDP
        private readonly int BYTES_TO_SEND = 3; //jaka dlugosc pola danych w bajtach chcemy wyslac


        List<ConnectedClient> clients = new List<ConnectedClient>(); //lista przechowujaca polaczonych klientow - ID i endpoint
        private List<ReceivedData> receiveds = new List<ReceivedData>(); //list przechowujaca dane odebrane od klientow
        private uint secretNumber = 11; //liczba tajna - dorobic metode generujaca ja z przedzialu 0 - 15

        //konstuktor
        Server()
        {
            this.client = new UdpClient(PORT); //ustawiamy port dla clienta
        }

        public void Send(uint answer, uint operation, uint ID, IPEndPoint endPoint)
        {
            byte[] bytes_to_send = new byte[3]; //to wysylam
            operations.setAllFields(ref bytes_to_send, operation, answer, ID, 0); //ustawiam pola do wyslania
            client.Send(bytes_to_send, BYTES_TO_SEND, endPoint); //wysyla byte[3]
            Console.WriteLine("Send!");
        }

        private uint GenerateID()
        {
            Random rand = new Random();
            return (uint)rand.Next(0, 255);
        }

        //do faktycznej obslugi requestow
        private void OperateRequest(ReceivedData data)
        {
            byte[] recvd_data = data.getData(); //wyluskuje odebrane bajty 
            IPEndPoint endPoint = data.GetEndPoint(); //wyluskuje endpoint z ktorego zostala informacja odebrana
            uint ID;
            uint operation = operations.GetOperation(ref recvd_data);
            switch (operation)
            {
                //Obsluga requestu dot. nadania ID
                case ID_REQUST:
                    Console.WriteLine("Request for ID..");
                    uint generatedID = GenerateID(); //generuje ID
                    Send(0, ID_SENT, generatedID, endPoint); //wysyla z poleceniem ID_SENT
                    Send(0, NUMB_REQUEST, generatedID, endPoint);
                    clients.Add(new ConnectedClient(generatedID, endPoint)); //tutaj dodaje do mapy endpoint i ID sesji
                    Console.WriteLine("Done..");
                    //drukuje dodanych do struktury klientow:
                    Console.WriteLine("Clients: ");
                    foreach (ConnectedClient cl in clients)
                    {
                        Console.WriteLine("ID: {0}, IP: {1}", cl.getID(), cl.getEndpoint().Address);
                    }
                    break;

                case NUMB_RCV:
                    //odsyla info z operacja ANSWER_CONFIRM i uzupelnionym polem operacji odpowiednio
                    uint received_num = operations.GetAnswer(ref recvd_data);
                    ID = operations.GetID(ref recvd_data);
                    if (received_num == secretNumber)
                    {
                        Console.WriteLine("Correct number! :)");
                        Send(1, ANSWER_CONFIRM, ID, endPoint);
                        Send(0, NUMB_REQUEST, ID, endPoint);
                    }
                    else
                    {
                        Console.WriteLine("Wrong number :( ");
                        Send(0, ANSWER_CONFIRM, ID, endPoint);
                    }
                    break;

                case END_CONNCECTION: //jesli klient opuszcza polaczenie wysylana jest taka informacja
                        Console.WriteLine("Client leaves..");
                        ID = operations.GetID(ref recvd_data);
                        Send(1, END_CONNCECTION, ID, endPoint);
                        Console.WriteLine("Client left..");
                    break;

                case TEN_SEC_REMIND:
                        Console.WriteLine("10s reminder..");
                        ID = operations.GetID(ref recvd_data);
                        Send(0, TEN_SEC_REMIND, ID, endPoint);
                    break;

                case TIME_END:
                        Console.WriteLine("Time's up!");
                        ID = operations.GetID(ref recvd_data);
                        Send(0, TIME_END, ID, endPoint);
                    break;
         
                case SECND_CLIENT_AWAIT:
                        Console.WriteLine("Waiting for second client...");
                        ID = operations.GetID(ref recvd_data);
                        Send(0, SECND_CLIENT_AWAIT, ID, endPoint);
                    break;
            }
        }

        //do przesiania requestow
        private void ManageRequests()
        {
            //Console.WriteLine("Request Manager Started");
            byte[] recvd_data = new byte[3];
            //jezeli dane maja ACK = 1 nic z nimi nie rob
            //jezeli inaczej - rob to co masz zrobic i odeslij to samo z ACK

            if (receiveds.Count > 0) //jezeli sa dane w kontenerze zapisanych odebranych danych
            {
                recvd_data = receiveds[0].getData(); //wyłuskanie otrzymanych danych

                if (operations.GetACK(ref recvd_data) == 0) //jezeli ACK = 0
                {
                    operations.SetACK(ref recvd_data, 1); //odsylam potwierdzenie odbioru
                    client.Send(recvd_data, 3, receiveds[0].GetEndPoint()); // -//-
                    OperateRequest(receiveds[0]); //przechodze do wlasciwej funkcji obslugi requestow a potem sciagam ostatni request
                }

                else //jezeli ACK = 1, no to nie robi nic
                    Console.WriteLine("Acknowledge..."); //w sumie to niewazne co potwierdza

                receiveds.RemoveAt(0); // usuwa ostatni request, bo zostalo obsluzone
            }
        }

        public void Receive()
        {
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0); //endpoint
                byte[] receivedBytes = client.Receive(ref RemoteIpEndPoint); //bajty, do których zapisujemy to co otrzymalismy
                operations.printImpFields(ref receivedBytes); //drukuje to co otrzymal
                receiveds.Add(new ReceivedData(receivedBytes, RemoteIpEndPoint)); //wrzuca otrzymany wynik razem z EndPointerm do Tablicy Oderbanych danych
                Console.WriteLine("Odebrano");
            }
        }

        //jezeli dwoch klientow bedzie polaczonych, to trzeba wystartować watek z ta metoda
        public void Inform()
        {
            uint TimeLeft = 0; //tutaj trzeba zachować czas do końca sesji
            Thread.Sleep(10000); //usypia watek na 10sek.
            foreach (ConnectedClient cl in clients) //do kazdego połączonego klienta wysyla info, że zostalo TimeLeft czasu do konca rozgrywki
            {
                Send(TimeLeft, TEN_SEC_REMIND, cl.getID(), cl.getEndpoint());
            }
        }


        public static void Main(string[] args)
        {
            Console.WriteLine("Server is running!");

            Server server = new Server();

            Thread recv_thr = new Thread(server.Receive); //tworzy watek nasłuchujacy
            recv_thr.Start(); //startuje nasluchujacy watek


            while (true)
            {
                //Console.Write(".");
                Thread.Sleep(1000); //a to po to, żeby się to wszystko tak szybko w konsoli nie działo - można zmniejszyc docelowo

                if (server.clients.Count > 1) //jezeli podlaczonych jest wiecej niz jeden klient mozna startowac z obsluga
                    server.ManageRequests();

                else //jezeli nie podlaczonych jest dwoch klientow, trzeba przeslac do ewentualnych klientow, że czekamy
                {
                    Thread.Sleep(1000); //zeby nie zasypywac klienta spamem o tym, że czekamy na drugiego klienta
                    //wyslij informacje o tym, ze czekamy na drugiego klienta - kod operacji SECND_CLIENT_AWAIT
                }

            }
        }
    }
}
