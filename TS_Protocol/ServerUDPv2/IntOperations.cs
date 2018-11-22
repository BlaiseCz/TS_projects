using System;

namespace ClientUDP
{
    /*
     * Pole operacji - 6bit
     * Pole odpowiedzi - 4 bit
     * Pole identyfikatora - 8 bit
     * Reszta - póki co same 0
     */
    public class IntOperations
    {
        uint data;

        public void setOperation(ref uint data, uint operation) //data - zmienna na której chcemy operować, operation - zmienna która chcemy wrzucic do pola Operation
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

        public uint getOperation(ref uint data){
            const uint mask = 0b11111100000000000000000000000000; //moja maska pola operacji
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 26;
        }

        public void setAnswer(ref uint data, uint answer) //data - zmienna na której chcemy operować, Answer - zmienna która chcemy wrzucic do pola answer
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

        public uint getAnswer(ref uint data)
        {
            const uint mask = 0b00000011110000000000000000000000; //moja maska pola answer
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 22;
        }

        public void setID(ref uint data, uint id) //data - zmienna na której chcemy operować, id - zmienna która chcemy wrzucic do pola identyfikator
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

        public uint getID(ref uint data)
        {
            const uint mask = 0b00000000001111111100000000000000; //moja maska pola id
            this.data = data;
            uint tmp = data;
            tmp = tmp & mask;
            return tmp >> 14;
        }

        public string convertToBinary(uint numb){
            String number = numb.ToString();
            int fromBase = 10;
            int toBase = 2;

            String result = Convert.ToString(Convert.ToInt32(number, fromBase), toBase);
            return result;
        }

        public void setAllFields(ref uint data, uint operation, uint answer, uint ID){
            setOperation(ref data, operation);
            setAnswer(ref data, answer);
            setID(ref data, ID);
        }

        public void printAllFields(ref uint data){
            Console.Write("Operation Field: {0}" +
                          "\nAnswer Field: {1}" +
                          "\nID Field: {2}" +
                          "\nBinary Data Number: {3}" +
                          "\nBinary Operation Field: {4}" +
                          "\nBinary Answer Field: {5}" +
                          "\nBinary ID Field: {6}",
                          getOperation(ref data),
                          getAnswer(ref data),
                          getID(ref data),
                          convertToBinary(data),
                          convertToBinary(getOperation(ref data)),
                          convertToBinary(getAnswer(ref data)),
                          convertToBinary(getID(ref data))
                          );
        }

        public IntOperations(){}
    }
}

