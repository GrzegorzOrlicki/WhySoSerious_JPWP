using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WhySoSerious
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //-----------------Rozpoczęcie gry
            //Wybór poziomu trudności
            string poziom = "latwy";

            //Ukrycie elementów interface'u
            tytulGry.Visibility = Visibility.Hidden;
            button1.Visibility = Visibility.Hidden;
            button2.Visibility = Visibility.Hidden;
            button3.Visibility = Visibility.Hidden;
            //Pokazanie menu gry z prawej strony

            //Licznik czasu

            //Licznik pomyłek
            int fails = 0;
            //Przycisk zakończenia rozgrywki

            //I część - generowanie planszy
            //Tworzenie obiektu planszy - klasa Board
            //Konstruktor tworzy obiekt i generuje trasę
            int max_tab = 4;
            Board plansza = new Board(poziom, max_tab);

            //Wygenerowanie ścieżki
            
            Random random = new Random();
            //String pokazujący zawartość tablicy planszy
            MessageBox.Show(plansza.wypisz_tablice());

            int[] index = new int[2] { max_tab - 1, random.Next(max_tab) }; //tablica przechowująca bieżące współrzędne tablicy
            MessageBox.Show(index[0].ToString() + " " + index[1].ToString());
            plansza.tab[index[0], index[1]] += 1;
            plansza.road = index[0].ToString() + index[1].ToString();

            
            MessageBox.Show(plansza.wypisz_tablice());

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
                    if (plansza.tab[index[0], index[1]] == 0b11110)
                    {
                        plansza.road = plansza.road.Remove(plansza.road.Length - 2, 2);
                        index[0] = plansza.road[plansza.road.Length - 2];
                        index[1] = plansza.road[plansza.road.Length - 1];
                        //Wskazanie, że nie jest już na pewno drogą
                        plansza.tab[index[0], index[1]] -= 1;
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

                    if (((plansza.tab[index[0], index[1]] & (1 << (kierunek + 1))) >> (kierunek + 1)) == 1)
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
                    if (plansza.tab[index_next[0], index_next[1]] % 2 == 1)
                    {
                        continue;
                    }
                    //MessageBox.Show("Następna komórka nie jest ścieżką");

                    //Dodanie współrzędnych nowej komórki do stringa określającego ścieżkę
                    plansza.road += index_next[0].ToString() + index_next[1].ToString();

                    //Gdy już jest znaleziona pasująca komórka
                    plansza.tab[index[0], index[1]] |= 1;
                    plansza.tab[index[0], index[1]] |= (1 << (kierunek + 1));

                    //Przyspieszenie sprawdzania - zaznaczenie połączenia z poprzednią komórką
                    switch (kierunek)
                    {
                        case 0:
                            plansza.tab[index_next[0], index_next[1]] |= (1 << (2 + 1));
                            break;
                        case 1:
                            plansza.tab[index_next[0], index_next[1]] |= (1 << (3 + 1));
                            break;
                        case 2:
                            plansza.tab[index_next[0], index_next[1]] |= (1 << (0 + 1));
                            break;
                        case 3:
                            plansza.tab[index_next[0], index_next[1]] |= (1 << (1 + 1));
                            break;
                    }

                    index[0] = index_next[0];
                    index[1] = index_next[1];
                    czy_mozliwy_ruch = true;
                }
                MessageBox.Show(plansza.wypisz_tablice() + "\nBieżacy xy: " + index[0].ToString() + " " + index[1].ToString() + "\nNastepny xy: " + index_next[0].ToString() + " " + index_next[1].ToString() + "\nSciezka" + plansza.road);
                //Warunek zakończenia algorytmu
                if (index[0] == 0) czy_koniec_generowania = true;
            }
            MessageBox.Show(plansza.wypisz_tablice() + "\nBieżacy xy: " + index[0].ToString() + " " + index[1].ToString() + "\nNastepny xy: " + index_next[0].ToString() + " " + index_next[1].ToString() + "\nSciezka" + plansza.road);

            //Rysowanie planszy

            //Animacja kolejnych kroków

            //II część - ruch gracza
            //Pętla w której gracz wybiera kolejne kroki
            //Sprawdzenie, czy ruch jest dozwolony

            //Jeśli nie, to wybierz jeszcze raz

            //Jeśli tak, to sprawdz, czy zgadza się ruch z wygenerowanym

            //Jeśli tak, to:
            //Animacja dobrego ruchu

            //Sprawdź, czy koniec

            //Koniec gry
            //Wyświetlenie podsumowania - wynik

            //Zapisanie wyników do pliku

            //Zapisanie wyników do pliku najlepszych wyników

            //Ukrycie elementów gry

            //Wyświetlenie ekranu tytułowego
        }
        
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //-----------------Wyświetlenie wyników
            //Wyskakujący komunikat z wynikami (z pliku)
            //String przechowujący sformatowany tekst z pliku
            string tekst = System.IO.File.ReadAllText(@"wyniki.txt");
            MessageBoxResult result = MessageBox.Show(tekst + "\n-----------------------------------------------\nCzy chcesz usunąć tablicę wyników?", "Wyniki", MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    //Kod kasujący zawartość tabeli najlepszych wyników
                    System.IO.File.WriteAllText(@"wyniki.txt", "");
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //-----------------Zamknięcie gry
            this.Close();
        }
    }
}
