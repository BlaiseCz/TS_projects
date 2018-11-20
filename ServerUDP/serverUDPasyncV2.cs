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

        private const uint ID_REQUST = 1; //odbiera jesli klient chce nadania ID
        private const uint ID_SENT = 2; //wysyla takie polecenie, jezeli nadano ID
        private const uint NUMB_RCV = 3; //odbiera jesli klient wyslal liczbe
        private const uint ANSWER_CONFIRM = 4; //odpowiada na pytanie czy zgadnieto
        private const uint END_CONNCECTION = 5; //jesli klient opuszcza polaczenie wysylana jest taka informacja
        private const uint TIME_END = 7; //wysyla jesli w zadanym czasie nie zgadnieto
        private const uint NUMB_REQUEST = 8; //wysylane jesli chce, aby klient wyslal liczbe
        private const uint SECND_CLIENT_AWAIT = 9; //wysylane jezeli nie jest polaczony drugi klient
        private const uint SEND_UNITY = 10; //wyslanie pozostałego czasu rozgrywki
        private const uint SEND_TENS = 11;
        private const uint SEND_HUNDREDS = 12;

        IntOperations operations = new IntOperations();
        private readonly int PORT = 8080; //ustawiamy port na jakim włączamy serwer
        private UdpClient server; //deklaracja clienta UDP
        private readonly int BYTES_TO_SEND = 3; //jaka dlugosc pola danych w bajtach chcemy wyslac

        const uint HOW_MANY_CLIENTS_TO_START_THIS_GAME = 2; // :)
        List<ConnectedClient> Clients = new List<ConnectedClient>(); //lista przechowujaca polaczonych klientow - ID i endpoint
        private List<ReceivedData> receiveds = new List<ReceivedData>(); //list przechowujaca dane odebrane od klientow
        private uint SecretNumber; //liczba tajna - dorobic metode generujaca ja z przedzialu 0 - 15
        private bool IsGameStarted = false;
        private uint GameTime = 10;

        //konstuktor
        Server()
        {
            this.server = new UdpClient(PORT); //ustawiamy port dla clienta
        }

        private void SetSecretNumber()
        {
            Random rand = new Random();
            SecretNumber = (uint)rand.Next(0, 16);
            Console.WriteLine("Our secret number is: {0}", SecretNumber);
        }

        private void SetGameTime() // [(id. sesji 1 + id. sesji 2) * 99] % 100 + 30
        {
            foreach(ConnectedClient c in Clients)
            {
                GameTime += c.getID();
            }
            GameTime = (GameTime * 99) % 100 + 30;
        }
        public void Send(uint answer, uint operation, uint ID, IPEndPoint endPoint)
        {
            byte[] bytes_to_send = new byte[3]; //to wysylam
            operations.setAllFields(ref bytes_to_send, operation, answer, ID, 0); //ustawiam pola do wyslania
            server.Send(bytes_to_send, BYTES_TO_SEND, endPoint); //wysyla byte[3]
            Console.WriteLine("Send! Operation number: {0}", operation);
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
            uint ID = operations.GetID(ref recvd_data); //WYSTARCZY RAZ JAK UZYSKASZ ID, NIE MUSISZ W KAZDEJ OPERACJI, TA METODA SIE WYWOLUJE ZA KAZDYM REQUESTEM OD NOWA
            uint operation = operations.GetOperation(ref recvd_data);
            Console.WriteLine("Our secret number is: {0}", SecretNumber);


            switch (operation)
            {
                //Obsluga requestu dot. nadania ID
                case ID_REQUST:
                        Console.WriteLine("Request for ID..");
                        uint generatedID = GenerateID(); //generuje ID
                        Send(0, ID_SENT, generatedID, endPoint); //wysyla z poleceniem ID_SENT
                        //Send(0, NUMB_REQUEST, generatedID, endPoint); - ZĄDANIE O LICZBĘ NIE TUTAJ, BO MOŻE BYĆ <2 KLIENTOW
                        Clients.Add(new ConnectedClient(generatedID, endPoint)); //tutaj dodaje do mapy endpoint i ID sesji
                        Console.WriteLine("Done..");
                        //drukuje dodanych do struktury klientow:
                        Console.WriteLine("Clients: ");
                        foreach (ConnectedClient cl in Clients)
                        {
                            Console.WriteLine("ID: {0}, IP: {1}", cl.getID(), cl.getEndpoint().Address);
                        }
                    break;

                case NUMB_RCV:
                    //odsyla info z operacja ANSWER_CONFIRM i uzupelnionym polem operacji odpowiednio
                        uint received_num = operations.GetAnswer(ref recvd_data);
                    if (received_num == SecretNumber)
                    {
                        Console.WriteLine("Correct number! :) {0}", received_num);
                        Send(1, ANSWER_CONFIRM, ID, endPoint);
                        //Send(0, NUMB_REQUEST, ID, endPoint); //PO PORAWNIE PODANEJ LICZBIE NIE CHCE ODEBRAC INFORMACJI O TYM, ŻE MAM KOLEJNA LICZBE WYSLAC
                    }
                    else
                    {
                        Console.WriteLine("Wrong number :( {0}", received_num);
                        Send(0, ANSWER_CONFIRM, ID, endPoint);
                        Send(0, NUMB_REQUEST, ID, endPoint);
                    }
                    break;

                case END_CONNCECTION: //jesli klient opuszcza polaczenie wysylana jest taka informacja
                        Console.WriteLine("Client leaves..");
                        Send(1, END_CONNCECTION, ID, endPoint);
                        Console.WriteLine("Client left..");
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
                    server.Send(recvd_data, 3, receiveds[0].GetEndPoint()); // -//-
                    OperateRequest(receiveds[0]); //przechodze do wlasciwej funkcji obslugi requestow a potem sciagam ostatni request
                }

                else //jezeli ACK = 1, no to nie robi nic
                    Console.WriteLine("Acknowledge..."); //w sumie to niewazne co potwierdza

                receiveds.RemoveAt(0); // usuwa ostatni request, bo zostalo obsluzone
            }

            //SPRAWDZA CZY MOZNA WYSTARTOWAC GRE
            if (IsGameStarted == false)
            {
                if (Clients.Count >= HOW_MANY_CLIENTS_TO_START_THIS_GAME)
                {
                    IsGameStarted = true;
                    foreach (ConnectedClient cl in Clients)
                    {
                        Send(0, NUMB_REQUEST, cl.getID(), cl.getEndpoint()); //w tym miejscu wysyla request, zeby klienci wyslali liczbe
                    }
                    SetGameTime();
                    IsGameStarted = true;
                }
                else
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Waiting for second client...");
                    foreach (ConnectedClient cl in Clients)
                    {
                        Send(0, SECND_CLIENT_AWAIT, cl.getID(), cl.getEndpoint()); //w tym miejscu wysyla request, zeby klienci wyslali liczbe
                    }
                }
            }
        }

        public void Receive()
        {
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0); //endpoint
                byte[] receivedBytes = server.Receive(ref RemoteIpEndPoint); //bajty, do których zapisujemy to co otrzymalismy
                operations.printImpFields(ref receivedBytes); //drukuje to co otrzymal
                receiveds.Add(new ReceivedData(receivedBytes, RemoteIpEndPoint)); //wrzuca otrzymany wynik razem z EndPointerm do Tablicy Oderbanych danych
                Console.WriteLine("Received...");
            }
        }
        private uint unit()
        {
            return GameTime % 10;
        }

        private uint tens()
        {
            if (GameTime > 10)
                return ((GameTime - unit()))/10 % 10;
            else return 11;
        }

        private uint hundreds()
        {
            if (GameTime > 100)
                return ((GameTime - unit() - tens()*10) / 100) % 10;
            else
                return 11;
        }
        //jezeli dwoch klientow bedzie polaczonych, to trzeba wystartować watek z ta metoda
        public void Inform()
        {
            while (true)
            {
                if (!IsGameStarted)
                    continue;
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(1000); //usypia watek na 10sek

                        if (GameTime > 0)
                            GameTime--;
                        else
                        {
                            Console.WriteLine("Time's up!");
                            foreach (ConnectedClient c in Clients)
                            {
                                Send(0, TIME_END, c.getID(), c.getEndpoint());
                            }

                            try
                            {
                                server.Close();
                                Console.WriteLine("\nGame finished\nServer off!");
                            }
                            catch (SocketException e)
                            {
                                //Console.WriteLine(e);
                            }
                        }
                    }
                    foreach (ConnectedClient cl in Clients) //do kazdego połączonego klienta wysyla info, że zostalo TimeLeft czasu do konca rozgrywki
                    {
                        Console.WriteLine("Time left : {0}", GameTime);
                        Send(unit(), SEND_UNITY, cl.getID(), cl.getEndpoint());
                        Send(tens(), SEND_TENS, cl.getID(), cl.getEndpoint());
                        Send(hundreds(), SEND_HUNDREDS, cl.getID(), cl.getEndpoint());
                    }
                }
            }
        }


        public static void Main(string[] args)
        {
            Console.WriteLine("Server is running!");

            Server server = new Server();
            server.SetSecretNumber();
           // server.SetGameTime();
            Thread recv_thr = new Thread(server.Receive); //tworzy watek nasłuchujacy
            recv_thr.Start(); //startuje nasluchujacy watek

            Thread TenSecRemind = new Thread(server.Inform); 
            TenSecRemind.Start();

            while (true)
            {
                //Console.Write(".");
                Thread.Sleep(100); //a to po to, żeby się to wszystko tak szybko w konsoli nie działo - można zmniejszyc docelowo

                server.ManageRequests(); //jezeli podlaczonych jest wiecej niz jeden klient mozna startowac z obsluga

            }
        }
    }
}
