using System;

namespace Client_Asyn
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

        public void SetOperation(ref uint data, uint operation) //data - zmienna na której chcemy operować, operation - zmienna która chcemy wrzucic do pola Operation
        {

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

            data = x | y; //operacja OR na liczba z zapisanymi polami wszystkimi innymi niz operacji i tym z operacj
        }

        public uint GetOperation(ref uint data)
        {
            const uint mask = 0b11111100000000000000000000000000; //moja maska pola operacji
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 26;
        }

        public void SetAnswer(ref uint data, uint answer) //data - zmienna na której chcemy operować, Answer - zmienna która chcemy wrzucic do pola answer
        {
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
        }

        public uint GetAnswer(ref uint data)
        {
            const uint mask = 0b00000011110000000000000000000000; //moja maska pola answer
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 22;
        }

        public void SetID(ref uint data, uint id) //data - zmienna na której chcemy operować, id - zmienna która chcemy wrzucic do pola identyfikator
        {
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
        }

        public uint GetID(ref uint data)
        {
            const uint mask = 0b00000000001111111100000000000000; //moja maska pola id
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 14;
        }

        public void SetACK(ref uint data, uint state)
        {
            if (state > 1) //jezeli liczba ID jest za duza - przekracza 8 bitów - nie wykonuje tej funkcji
                return; //wychodzi z funkcji

            this.data = data;
            const uint mask = 0b00000000000000000010000000000000; //moja maska flagi ACK
            const uint negM = 0b11111111111111111101111111111111; //zanegowana maska flagi ACK

            uint y = mask; //inicjalizacja liczby do ktorej bede zapisywal POLE OPERACJI
            y = y >> 13; //przesuniecie, zeby dzialac na liczbach takich jakie chcemy zapiac
            y = state & y; //operacja AND na Y i na podanej liczbie
            y = y << 13; // uzuplenione pole operacji zapisane na pierwszych 6ciu bitach. Reszta bitow pusta

            uint x = data & negM; // liczba z zapisanymi innymi polami niż pole operacji. Pole operacji jest puste

            data = x | y; //operacja OR na liczba z zapisanymi polami wszystkimi innymi niz operacji i tym z operacja
        }

        public uint GetACK(ref uint data)
        {
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

        public void setAllFields(ref uint data, uint operation, uint answer, uint ID)
        {
            SetOperation(ref data, operation);
            SetAnswer(ref data, answer);
            SetID(ref data, ID);
        }

        public void printAllFields(ref uint data)
        {
            Console.Write("Operation Field: {0}" +
                          "\nAnswer Field: {1}" +
                          "\nID Field: {2}" +
                          "\nACK flag: {3}" +
                          "\nBinary Data Number: {4}" +
                          "\nBinary Operation Field: {5}" +
                          "\nBinary Answer Field: {6}" +
                          "\nBinary ID Field: {7}",
                          GetOperation(ref data),
                          GetAnswer(ref data),
                          GetID(ref data),
                          GetACK(ref data),
                          ConvertToBinary(data),
                          ConvertToBinary(GetOperation(ref data)),
                          ConvertToBinary(GetAnswer(ref data)),
                          ConvertToBinary(GetID(ref data))
                          );
        }

        public uint ByteToUint(ref byte[] to_convert)
        {
            //if (BitConverter.IsLittleEndian) //zamieniamy liczbe zapisaną w tablicy z danymi z formatu LittleEndian na BigEndian
            //    Array.Reverse(to_convert);
            return BitConverter.ToUInt32(to_convert, 0); //konwersja z bytep[] na uint
            //return Convert.ToUInt32(to_convert); //konwersja z bytep[] na uint
        }

        public IntOperations() { }
    }
}

