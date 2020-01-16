using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Klasa przeznaczona do generowania planszy
//Opis wartości w tablicy:
//0 - całkowicie pusta
//1 - wygenerowana ścieżka
//2 -

//Plansza ma współrzędne 0,0 w lewym, górnym rogu
//Gra rozpoczyna się w losowej komórce maksymalnie dolnego wiersza

//Kierunki:
//0 - góra
//1 - prawo
//2 - dół
//3 - lewo

//Liczba w tablicy
//0bit - brak ścieżki/ścieżka
//1bit - czy była sprawdzana góra?
//2bit - czy była sprawdzana prawo?
//3bit - czy była sprawdzana dół?
//4bit - czy była sprawdzana lewo?

namespace WhySoSerious
{
    class Board
    {
        //tablica przechowująca stan planszy
        public int[,] tab = new int[10,10];
        
        private int max_tab;
        public string road ="";

        public Board(int poziom)
        {
            //max_tab = wielkosc;
            for(int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    tab[y, x] = 0;
                }
            }
            //Określenie wielkości tablicy dla poziomu trudności
            max_tab = poziom;

            Random random = new Random();
            //String pokazujący zawartość tablicy planszy

            int[] index = new int[2] { max_tab - 1, random.Next(max_tab) }; //tablica przechowująca bieżące współrzędne tablicy
           
            tab[index[0], index[1]] += 1;
            road = index[0].ToString() + index[1].ToString();

            int[] index_next = new int[2]; //tablica przechowująca wylosowywane następne współrzędne
            //int kierunek = 0; // zmienna przechowująca wylosowany kierunek

            //Pętla algorytmu generowania ścieżki
            bool czy_koniec_generowania = false;
            while (!czy_koniec_generowania)
            {
                //Pętla wyboru następnego kierunku
                bool czy_mozliwy_ruch = false;
                while (!czy_mozliwy_ruch)
                {

                    //Czy zostały wybrane wszystkie kierunki dla tego pola?
                    if (tab[index[0], index[1]] == 0b11110)
                    {
                        road = road.Remove(road.Length - 2, 2);
                        index[0] = road[road.Length - 2];
                        index[1] = road[road.Length - 1];
                        //Wskazanie, że nie jest już na pewno drogą
                        tab[index[0], index[1]] -= 1;
                        continue;
                    }

                    int kierunek = random.Next(4);

                    //Obliczenie następnego pola na bazie kierunku
                    switch (kierunek)
                    {
                        case 0:
                            index_next[0] = index[0] - 1;
                            index_next[1] = index[1];
                            break;
                        case 1:
                            index_next[0] = index[0];
                            index_next[1] = index[1] + 1;
                            break;
                        case 2:
                            index_next[0] = index[0] + 1;
                            index_next[1] = index[1];
                            break;
                        case 3:
                            index_next[0] = index[0];
                            index_next[1] = index[1] - 1;
                            break;
                    }

                    //Czy dany kierunek już był wylosowany?

                    if (((tab[index[0], index[1]] & (1 << (kierunek + 1))) >> (kierunek + 1)) == 1)
                    {
                        continue;
                    }
                    //MessageBox.Show("Dany kierunek nie był wylosowany");

                    //Czy następna komórka znajduje się w polu gry?
                    //Jeśli nie to zaznaczenie, że już był sprawdzany dany kierunek i wylosowanie innego
                    if ((index_next[0] < 0) || (index_next[0] >= max_tab) || (index_next[1] < 0) || (index_next[1] >= max_tab))
                    {
                        continue;
                    }
                    // MessageBox.Show("Następna komórka znajuje się w polu gry");

                    //Czy następna komórka jest już ścieżką?
                    if (tab[index_next[0], index_next[1]] % 2 == 1)
                    {
                        continue;
                    }
                    //MessageBox.Show("Następna komórka nie jest ścieżką");

                    //Dodanie współrzędnych nowej komórki do stringa określającego ścieżkę
                    road += index_next[0].ToString() + index_next[1].ToString();

                    //Gdy już jest znaleziona pasująca komórka
                    tab[index[0], index[1]] |= 1;
                    tab[index[0], index[1]] |= (1 << (kierunek + 1));

                    //Przyspieszenie sprawdzania - zaznaczenie połączenia z poprzednią komórką
                    switch (kierunek)
                    {
                        case 0:
                            tab[index_next[0], index_next[1]] |= (1 << (2 + 1));
                            break;
                        case 1:
                            tab[index_next[0], index_next[1]] |= (1 << (3 + 1));
                            break;
                        case 2:
                            tab[index_next[0], index_next[1]] |= (1 << (0 + 1));
                            break;
                        case 3:
                            tab[index_next[0], index_next[1]] |= (1 << (1 + 1));
                            break;
                    }

                    index[0] = index_next[0];
                    index[1] = index_next[1];
                    czy_mozliwy_ruch = true;
                }
               
                if (index[0] == 0) czy_koniec_generowania = true;
            }
        }

        public string wypisz_tablice()
        {
            string tablica = "";

            for (int y = 0; y < max_tab; y++)
            {
                for (int x = 0; x < max_tab; x++)
                {
                    tablica += tab[y, x].ToString();
                    tablica += '\t'.ToString();
                }
                tablica += '\n'.ToString();
            }

            return tablica;
        }
    }
}
