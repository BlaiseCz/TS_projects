using System;

namespace Server_Asyn
{
    /*
     * Pole operacji - 6bit
     * Pole odpowiedzi - 4 bit
     * Pole identyfikatora - 8 bit
     * Flaga ACK - 1 bit
     * Reszta - póki co same 0
     */
    public class IntOperations
    {
        uint data;

        uint Convertbyte3tobyte4(ref byte[] data_byte)
        {
            ///////////////////////////////////////// zamiana byte[] na uint
            uint data = 0;
            byte[] to_uint = new byte[4];

            to_uint[0] = 0;
            to_uint[1] = data_byte[0];
            to_uint[2] = data_byte[1];
            to_uint[3] = data_byte[2];

            data = BitConverter.ToUInt32(to_uint, 0);
            /////////////////////////////////////////
            return data;
        }

        void Convertbyte4tobyte3(ref byte[] data_byte, uint data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            data_byte[2] = tmp[3];
            data_byte[1] = tmp[2];
            data_byte[0] = tmp[1];
        }

        public void SetOperation(ref byte[] data_byte, uint operation) //data - zmienna na której chcemy operować, operation - zmienna która chcemy wrzucic do pola Operation
        {
            uint data = Convertbyte3tobyte4(ref data_byte);
            //Console.Write("\nSet operation data: " + data);

            if (operation > 63) //jezeli liczba jest za duza - przekracza 6 bitów - nie wykonuje tej funkcji
                return; //wychodzi z funkcji

            this.data = data;
            const uint mask = 0b11111100000000000000000000000000; //moja maska pola operacji
            const uint negM = 0b00000011111111111111111111111111; //zanegowana maska pola operacji

            uint y = mask; //inicjalizacja liczby do ktorej bede zapisywal POLE OPERACJI
            y = y >> 26; //przesuniecie, zeby dzialac na liczbach takich jakie chcemy zapiac
            y = operation & y; //operacja AND na Y i na podanej liczbie
            y = y << 26; // uzuplenione pole operacji zapisane na pierwszych 6ciu bitach. Reszta bitow pusta

            uint x = data & negM; // liczba z zapisanymi innymi polami niż pole operacji. Pole operacji jest puste

            data = x | y; //operacja OR na liczba z zapisanymi polami wszystkimi innymi niz operacji i tym z operacji

            Convertbyte4tobyte3(ref data_byte, data);
        }

        public uint GetOperation(ref byte[] data_byte)
        {
            uint data = Convertbyte3tobyte4(ref data_byte);

            const uint mask = 0b11111100000000000000000000000000; //moja maska pola operacji
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 26;
        }

        public void SetAnswer(ref byte[] data_byte, uint answer) //data - zmienna na której chcemy operować, Answer - zmienna która chcemy wrzucic do pola answer
        {
            uint data = Convertbyte3tobyte4(ref data_byte);

            if (answer > 15) //jezeli liczba jest za duza - przekracza 4 bity - nie wykonuje tej funkcji
                return; //wychodzi z funkcji

            this.data = data;
            const uint mask = 0b00000011110000000000000000000000; //moja maska pola answer
            const uint negM = 0b11111100001111111111111111111111; //zanegowana maska pola answer

            uint y = mask; //inicjalizacja liczby do ktorej bede zapisywal POLE OPERACJI
            y = y >> 22; //przesuniecie, zeby dzialac na liczbach takich jakie chcemy zapiac
            y = answer & y; //operacja AND na Y i na podanej liczbie
            y = y << 22; // uzuplenione pole operacji zapisane na pierwszych 6ciu bitach. Reszta bitow pusta

            uint x = data & negM; // liczba z zapisanymi innymi polami niż pole operacji. Pole operacji jest puste

            data = x | y; //operacja OR na liczba z zapisanymi polami wszystkimi innymi niz operacji i tym z operacja


            //////////////////////////////////zamiana byte[4] na byte[3]
            Convertbyte4tobyte3(ref data_byte, data);
        }

        public uint GetAnswer(ref byte[] data_byte)
        {
            uint data = Convertbyte3tobyte4(ref data_byte);

            const uint mask = 0b00000011110000000000000000000000; //moja maska pola answer
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 22;
        }

        public void SetID(ref byte[] data_byte, uint id) //data - zmienna na której chcemy operować, id - zmienna która chcemy wrzucic do pola identyfikator
        {
            uint data = Convertbyte3tobyte4(ref data_byte);


            if (id > 255) //jezeli liczba ID jest za duza - przekracza 8 bitów - nie wykonuje tej funkcji
                return; //wychodzi z funkcji

            this.data = data;
            const uint mask = 0b00000000001111111100000000000000; //moja maska pola id
            const uint negM = 0b11111111110000000011111111111111; //zanegowana maska pola id

            uint y = mask; //inicjalizacja liczby do ktorej bede zapisywal POLE OPERACJI
            y = y >> 14; //przesuniecie, zeby dzialac na liczbach takich jakie chcemy zapiac
            y = id & y; //operacja AND na Y i na podanej liczbie
            y = y << 14; // uzuplenione pole operacji zapisane na pierwszych 6ciu bitach. Reszta bitow pusta

            uint x = data & negM; // liczba z zapisanymi innymi polami niż pole operacji. Pole operacji jest puste

            data = x | y; //operacja OR na liczba z zapisanymi polami wszystkimi innymi niz operacji i tym z operacja

            //////////////////////////////////zamiana byte[4] na byte[3]
            Convertbyte4tobyte3(ref data_byte, data);
        }

        public uint GetID(ref byte[] data_byte)
        {
            uint data = Convertbyte3tobyte4(ref data_byte);

            const uint mask = 0b00000000001111111100000000000000; //moja maska pola id
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 14;
        }

        public void SetACK(ref byte[] data_byte, uint state)
        {

            uint data = Convertbyte3tobyte4(ref data_byte);


            this.data = data;
            const uint mask = 0b00000000000000000010000000000000; //moja maska flagi ACK
            const uint negM = 0b11111111111111111101111111111111; //zanegowana maska flagi ACK

            uint y = mask; //inicjalizacja liczby do ktorej bede zapisywal POLE OPERACJI
            y = y >> 13; //przesuniecie, zeby dzialac na liczbach takich jakie chcemy zapiac
            y = state & y; //operacja AND na Y i na podanej liczbie
            y = y << 13; // uzuplenione pole operacji zapisane na pierwszych 6ciu bitach. Reszta bitow pusta

            uint x = data & negM; // liczba z zapisanymi innymi polami niż pole operacji. Pole operacji jest puste

            data = x | y; //operacja OR na liczba z zapisanymi polami wszystkimi innymi niz operacji i tym z operacja

            //////////////////////////////////zamiana byte[4] na byte[3]
            Convertbyte4tobyte3(ref data_byte, data);

        }

        public uint GetACK(ref byte[] data_byte)
        {
            uint data = Convertbyte3tobyte4(ref data_byte);

            const uint mask = 0b00000000000000000010000000000000; //moja maska flagi ACK
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 13;
        }

        public string ConvertToBinary(uint numb)
        {
            String number = numb.ToString();
            int fromBase = 10;
            int toBase = 2;

            String result = Convert.ToString(Convert.ToInt32(number, fromBase), toBase);
            return result;
        }

        public void setAllFields(ref byte[] data_byte, uint operation, uint answer, uint ID, uint ACK)
        {
            SetOperation(ref data_byte, operation);
            SetAnswer(ref data_byte, answer);
            SetID(ref data_byte, ID);
            SetACK(ref data_byte, ACK);
        }

        public void setAllFields(ref byte[] data_byte, uint operation, uint answer, uint ID)
        {
            SetOperation(ref data_byte, operation);
            SetAnswer(ref data_byte, answer);
            SetID(ref data_byte, ID);
        }

        public void printAllFields(ref byte[] data_byte)
        {
            Console.Write("Operation Field: {0}" +
                          "\nAnswer Field: {1}" +
                          "\nID Field: {2}" +
                          "\nACK flag: {3}" +
                          "\nBinary Data Number: {4}" +
                          "\nBinary Operation Field: {5}" +
                          "\nBinary Answer Field: {6}" +
                          "\nBinary ID Field: {7}",
                          GetOperation(ref data_byte),
                          GetAnswer(ref data_byte),
                          GetID(ref data_byte),
                          GetACK(ref data_byte),
                          ConvertToBinary(data),
                          ConvertToBinary(GetOperation(ref data_byte)),
                          ConvertToBinary(GetAnswer(ref data_byte)),
                          ConvertToBinary(GetID(ref data_byte))
                          );
        }

        public void printImpFields(ref byte[] data_byte)
        {
            Console.WriteLine("ID: {0}," +
                              "\nAnswer: {1}" +
                              "\nOperation: {2}",
                              GetID(ref data_byte),
                              GetAnswer(ref data_byte),
                              GetOperation(ref data_byte)
                             );
        }

        public uint ByteToUint(ref byte[] to_convert)
        {
            //if (BitConverter.IsLittleEndian) //zamieniamy liczbe zapisaną w tablicy z danymi z formatu LittleEndian na BigEndian
            //    Array.Reverse(to_convert);
            return BitConverter.ToUInt32(to_convert, 0); //konwersja z byte[] na uint
            //return Convert.ToUInt32(to_convert); //konwersja z bytep[] na uint
        }

        public IntOperations() { }
    }
}

