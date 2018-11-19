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
        //kody pol operacji
        const uint ID_REQUST = 1; //wysyla jesli klient chce nadania ID
        const uint ID_SENT = 2; //odbiera takie polecenie, jezeli nadano ID
        const uint NUMB_SEND = 3; //wysyla jesli klient wyslal liczbe
        const uint ANSWER_CONFIRM = 4; //odbiera i odpowiada na pytanie czy zgadnieto
        const uint END_CONNCECTION = 5; //jesli klient opuszcza polaczenie wysylana jest taka informacja
        const uint TEN_SEC_REMIND = 6; //wywołanie przypomnienia
        const uint TIME_END = 7; //odbiera jesli w zadanym czasie nie zgadnieto
        const uint NUMB_REQUEST = 8; //wysylane jesli chce, aby klient wyslal liczbe
        const uint SECND_CLIENT_AWAIT = 9; //wysylane jezeli nie jest polaczony drugi klient

        IntOperations operations = new IntOperations();
        readonly string IP_ADDR_SERVER = "127.0.0.1";
        readonly int PORT = 8080;
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0); //endpoint
        private UdpClient client; //deklaracja clienta UDP
        private readonly int BYTES_TO_SEND = 3; //jaka dlugosc pola danych w bajtach chcemy wyslac
        uint ID; //ID klienta
        public List<ReceivedData> receiveds = new List<ReceivedData>();


        Client(){
            Console.WriteLine("Podaj Adres IP serwera ");
            IP_ADDR_SERVER = Console.ReadLine();

            Console.WriteLine("Podaj Port ");
            PORT = Convert.ToInt32(Console.ReadLine());

            client = new UdpClient(IP_ADDR_SERVER, PORT); //tworzy klienta
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
                //operations.printAllFields(ref receivedBytes); //drukuje to co otrzymal

                receiveds.Add(new ReceivedData(receivedBytes, RemoteIpEndPoint)); //wrzuca otrzymany wynik razem z EndPointerm do Tablicy Oderbanych danych
            }
        }

        //do faktycznej obslugi
        private void OperateRequest(ReceivedData data)
        {
            byte[] recvd_data = data.getData();
            IPEndPoint endPoint = data.GetEndPoint();

            uint operation = operations.GetOperation(ref recvd_data);
            switch (operation)
            {
                case ID_SENT:
                    Console.WriteLine("Wyslano ID");
                    ID = operations.GetID(ref recvd_data);
                    break;

                case NUMB_REQUEST:
                    Console.WriteLine("Podaj liczbe: ");
                    uint Answer = Convert.ToUInt32(Console.ReadLine());
                    Send(Answer, NUMB_SEND);
                    break;
            }
        }

        private void ManageRequests()
        {
            //Console.WriteLine("Request Manager Started");
            IntOperations operations = new IntOperations();
            byte[] recvd_data = new byte[3];
            //jezeli dane maja ACK = 1 nic z nimi nie rob
            //jezeli inaczej - rob to co masz zrobic i odeslij to samo z ACK

            if (receiveds.Count > 0) //jezeli sa dane w kontenerze zapisanych odebranych danych
            {
                recvd_data = receiveds[0].getData(); //wyłuskanie otrzymanych danych

                if (operations.GetACK(ref recvd_data) == 0) //jezeli ACK = 0 i ID 
                {
                    operations.SetACK(ref recvd_data, 1); //ustawia flage ACK na 1 i odsyla do serwera
                    client.Send(recvd_data, BYTES_TO_SEND); // -//-
                    OperateRequest(receiveds[0]); //przechodze do wlasciwej funkcji obslugi requestow - sciagam ostatni request
                }

                else //jezeli ACK = 1, no to nie robi nic
                {
                    Console.WriteLine("Serwer cos potwierdzil");
                }

                receiveds.RemoveAt(0); // usuwa, bo zostalo obsluzone
            }
        }

        public static void Main(string[] args)
        {
            IntOperations operations = new IntOperations();
            Console.WriteLine("Client running...");
            Client client = new Client();

            Thread recv_thr = new Thread(client.Receive);
            recv_thr.Start();

            client.Send(0, 1); //prosba o uzyskanie ID sesji

            while(true){
                Thread.Sleep(1000);
                client.ManageRequests();
            }
        }
    }
}
