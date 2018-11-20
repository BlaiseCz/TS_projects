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
        const uint ID_REQUST = 1; //wysyla jesli klient chce nadania ID TICK
        const uint ID_SENT = 2; //odbiera takie polecenie, jezeli nadano ID TICK
        const uint NUMB_SEND = 3; //wysyla jesli klient wyslal liczbe TICK
        const uint ANSWER_CONFIRM = 4; //odbiera i odpowiada na pytanie czy zgadnieto TICK
        const uint END_CONNCECTION = 5; //jesli klient opuszcza polaczenie wysylana jest taka informacja TICK
        const uint TIME_END = 7; //odbiera jesli w zadanym czasie nie zgadnieto TICKs
        const uint NUMB_REQUEST = 8; //wysylane jesli chce, aby klient wyslal liczbe TICK
        const uint SECND_CLIENT_AWAIT = 9; //wysylane jezeli nie jest polaczony drugi klient TICK
        const uint TIME_REM_HUNDREDS = 12; 
        const uint TIME_REM_TENS = 11;
        const uint TIME_REM_UNITY = 10;

        IntOperations operations = new IntOperations();
        string IP_ADDR_SERVER = "";
        readonly int PORT = 8080;
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0); //endpoint
        private UdpClient client; //deklaracja clienta UDP
        private readonly int BYTES_TO_SEND = 3; //jaka dlugosc pola danych w bajtach chcemy wyslac
        uint ID; //ID klienta
        public List<ReceivedData> receiveds = new List<ReceivedData>();
        char[] TimeLeft = new char[3];

        Client(){
            Console.WriteLine("Podaj Adres IP serwera ");
            IP_ADDR_SERVER = Console.ReadLine();

            Console.WriteLine("Podaj Port ");
            PORT = Convert.ToInt32(Console.ReadLine());

            client = new UdpClient(IP_ADDR_SERVER, PORT); //tworzy klienta
            ID = 0; //narazie na sztywno
            Console.WriteLine(RemoteIpEndPoint.AddressFamily);
        }

        private void TimeLeftManage(int j, uint time){
            if(time < 10){
                TimeLeft[j] = Convert.ToChar(time);
                Console.WriteLine(time);
                Console.WriteLine(Convert.ToChar(time));
            }
            else
            {
                TimeLeft[j] = '-'; 
            }

            Console.WriteLine("Time Left: {2}{1}{0}",
                              TimeLeft[2],
                              TimeLeft[1],
                              TimeLeft[0]);

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
                IPEndPoint RemoteIpEndPoint_ = new IPEndPoint(IPAddress.Any, 0); //endpoint
                byte[] receivedBytes = client.Receive(ref RemoteIpEndPoint_); //bajty, do których zapisujemy to co otrzymalismy
                //operations.printAllFields(ref receivedBytes); //drukuje to co otrzymal

                receiveds.Add(new ReceivedData(receivedBytes, RemoteIpEndPoint_)); //wrzuca otrzymany wynik razem z EndPointerm do Tablicy Oderbanych danych
            }
        }

        //do faktycznej obslugi
        void OperateRequest(ReceivedData data)
        {
            byte[] recvd_data = data.getData();
            IPEndPoint endPoint_ = data.GetEndPoint();

            uint operation = operations.GetOperation(ref recvd_data);
            switch (operation)
            {
                case ID_SENT:
                    Console.WriteLine("ID request sent");
                    ID = operations.GetID(ref recvd_data);
                    Console.WriteLine("My ID: {0}", ID);
                    break;

                case NUMB_REQUEST:
                    Console.WriteLine("Give me a number: ");
                    uint Answer = Convert.ToUInt32(Console.ReadLine());
                    Send(Answer, NUMB_SEND);
                    break;

                case ANSWER_CONFIRM:
                    if(operations.GetAnswer(ref recvd_data) == 1)
                    {
                        Console.WriteLine("Correct number ;)");
                        Send(0, END_CONNCECTION);
                        Console.WriteLine("Closing connection...");
                        this.client.Close();
                    }
                    else{
                        Console.WriteLine("Wrong number...");
                    }
                    break;
                                 
                case TIME_END:
                    Console.WriteLine("Time's up! I'm closing client :(");
                    client.Close();
                    break;

                case SECND_CLIENT_AWAIT:
                    Console.WriteLine("Waiting for another client. Please wait");
                    break;

                case TIME_REM_UNITY:
                    TimeLeftManage(0, operations.GetAnswer(ref recvd_data));
                    break;

                case TIME_REM_TENS:
                    TimeLeftManage(1, operations.GetAnswer(ref recvd_data));
                    break;

                case TIME_REM_HUNDREDS:
                    TimeLeftManage(2, operations.GetAnswer(ref recvd_data));
                    break;
            }
        }

        void ManageRequests()
        {
            //Console.WriteLine("Request Manager Started");
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
                    Console.WriteLine("Server confirmed sth");
                }

                receiveds.RemoveAt(0); // usuwa, bo zostalo obsluzone
            }
        }

        public static void Main(string[] args)
        {
            IntOperations operations = new IntOperations();
            Console.WriteLine("Client is running...");
            Client client = new Client();

            Thread recv_thr = new Thread(client.Receive);
            recv_thr.Start();

            client.Send(0, ID_REQUST); //prosba o uzyskanie ID sesji

            while(true){
                Thread.Sleep(100);
                client.ManageRequests();
            }
        }
    }
}
