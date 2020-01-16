using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Diagnostics;

namespace WhySoSerious
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// Obrazek użyty do zaznaczenia kroków jest darmowy przy wspomnieniu autora (www.flaticon.com/authors/icongeek26)
    /// został zmodyfikowany kolorystycznie by pasował do motywu graficznego gry
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private int tryb = 0; //0 - menu główne, 1 - rysowanie planszy, 2 - gra, 3 - koniec
        int licznik_czasu = 0;
        string ostatnia_komorka = "";
        string sciezka_org = "";
        string sciezka = "";
        int klatka_animacji = 0;
        //string wspolrzedne = "";
        int fails = 0;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            //Licznik czasu
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Tick += timer_animacja;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //-----------------Rozpoczęcie gry
            //Wybór poziomu trudności
            int poziom = (int)slider.Value;

            //Ukrycie elementów interface'u
            tytulGry.Visibility = Visibility.Hidden;
            button1.Visibility = Visibility.Hidden;
            button2.Visibility = Visibility.Hidden;
            label1.Visibility = Visibility.Hidden;
            label2.Visibility = Visibility.Hidden;
            label3.Visibility = Visibility.Hidden;
            slider.Visibility = Visibility.Hidden;

            //I część - generowanie planszy
            //Tworzenie obiektu planszy - klasa Board
            //Konstruktor tworzy obiekt i generuje trasę
            Board plansza = new Board(poziom);

            //Rysowanie planszy
            for (int i = 0; i < poziom; i++)
            {
                for(int j = 0; j < poziom; j++)
                {
                    CreateAButton(i, j);
                }
            }

            //Animacja kolejnych kroków
            sciezka_org = plansza.road;
            sciezka = sciezka_org;
            licznik_czasu = 0;
            timer.Tick -= timer_Tick;
            timer.Start();
            tryb = 1;
        }

        void timer_animacja(object sender, EventArgs e)
        {
            if(tryb == 1)
            {
                if (sciezka.Length > 0)
                {
                    string wspolrzedne = sciezka.Remove(2, sciezka.Length - 2);

                    foreach (var item in gra.Children)
                    {
                        if (item is Button)
                        {
                            var button = (Button)item;
                            string buttonId = button.Name.Remove(0, 1);
                            if (buttonId == wspolrzedne)
                            {
                                button.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("footprints.png", UriKind.Relative)),
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                            }
                        }
                    }
                    //k++;
                    sciezka = sciezka.Remove(0, 2);
                }
                else
                {
                    //Gdy już koniec
                    foreach (var item in gra.Children)
                    {
                        if (item is Button)
                        {
                            var button = (Button)item;
                            if (button.Name.Length == 3)
                            {
                                button.Content = "";
                            } 
                        }
                    }

                    timer.Stop();
                    timer.Tick += timer_Tick;
                    licznik_czasu = 0;
                    MessageBox.Show("Jesteś w gotowości?", "Start!");
                    //II część - ruch gracza

                    timer.Start();
                    zegar.Visibility = Visibility.Visible;
                    //Pętla w której gracz wybiera kolejne kroki
                    //Sprawdzenie, czy ruch jest dozwolony
                    sciezka = sciezka_org;
                    tryb = 2;
                }
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            if(tryb == 2)
            {
                ostatnia_komorka = ((sender as Button).Name.ToString()).Remove(0, 1);

                string wspolrzedne = sciezka.Remove(2, sciezka.Length - 2);
                //MessageBox.Show(wspolrzedne + " Wybrana scieżka: " + ostatnia_komorka + "\nSciezka: " + sciezka);
                if (ostatnia_komorka == wspolrzedne)
                {
                    //Została wybrana prawidłowa komórka
                    foreach (var item in gra.Children)
                    {
                        if (item is Button)
                        {
                            var button = (Button)item;
                            string buttonId = button.Name.Remove(0, 1);
                            if (buttonId == wspolrzedne)
                            {
                                button.Content = new Image
                                {
                                    Source = new BitmapImage(new Uri("footprints.png", UriKind.Relative)),
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                            }

                        }
                    }
                    sciezka = sciezka.Remove(0, 2);
                }
                else
                {
                    fails++;
                    if(fails < 3) MessageBox.Show("Błędna ścieżka!\nMasz jeszcze " + (3-fails) + " próby!", "Uwaga!");
                    if(fails == 3)
                    {
                        MessageBox.Show("Niestety nie udało Ci się tym razem!", "Uwaga!");
                        sciezka = "";
                    }
                }

                if (sciezka == "")
                {
                    //Koniec gry
                    
                    timer.Stop();

                    //Wyświetlenie podsumowania - wynik
                    int wynik;
                    if (fails < 3)
                    {
                        wynik = (int)(Math.Pow(10, ((int)slider.Value - fails - 2)) - licznik_czasu);
                        MessageBox.Show("Udało Ci się ukończyć poziom!\nIlość punktów:\n" + wynik.ToString(), "Gratulacje!");
                        //Zapisanie wyników do pliku
                        DateTime data = DateTime.Now;

                        using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(@"wyniki.txt", true))
                        {
                            file.WriteLine(data.ToString() + "\t" + wynik.ToString() + " pkt");
                        }
                    }
                    
                    //Reset timera
                    licznik_czasu = 0;
                    fails = 0;
                    //Wyświetlenie ekranu tytułowego i ukrycie elementów gry
                    wylaczenie_UI();

                    tryb = 0;
                }
            }
        }

        private void CreateAButton(int y, int x)
        {
            Button btn = new Button();
            btn.Height = 100;
            btn.Width = 100;
            btn.Name = "B" + y.ToString() + x.ToString();
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF931010"));
            btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC9FF39"));
            btn.Click += new RoutedEventHandler(button_Click);
            btn.Tag = 1;

            gra.Children.Add(btn);
            btn.Margin = new Thickness(  x * (btn.Width*2+5),  y * (btn.Height * 2+5), -500, 0);
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
            if(tryb == 0)
            {
                this.Close();
            } else
            {
                //Gdy gra jest włączona to wyjście wychodzi do menu głównego
                wylaczenie_UI();

                tryb = 0;
            }
            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            licznik_czasu++;
            zegar.Content = "Czas:\n" + (licznik_czasu / 60).ToString() + ":" + (licznik_czasu % 60).ToString();
        }

        private void wylaczenie_UI()
        {
            foreach (var item in gra.Children)
            {
                if (item is Button)
                {
                    var button = (Button)item;
                    var buttonId = Convert.ToInt32(button.Tag);
                    if (buttonId == 1)
                    {
                        button.Visibility = Visibility.Hidden;
                    }

                }
            }

            tytulGry.Visibility = Visibility.Visible;
            button1.Visibility = Visibility.Visible;
            button2.Visibility = Visibility.Visible;
            label1.Visibility = Visibility.Visible;
            label2.Visibility = Visibility.Visible;
            label3.Visibility = Visibility.Visible;
            slider.Visibility = Visibility.Visible;
            zegar.Visibility = Visibility.Hidden;

            timer.Stop();
        }
    }
}
