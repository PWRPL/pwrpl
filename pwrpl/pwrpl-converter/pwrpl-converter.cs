/*
UWAGA: NIE ZMIENIAĆ NUMERÓW OPERACJI, ZE WZGLĘDU NA:
-ZAIMPLEMENTOWANE MAKRA OD WERSJI v.1.7
-ZAIMPLEMENTOWANĄ OBSŁUGĘ ARGUMENTÓW ORAZ API OD WERSJI V.3.01
*/

using Console = pwrpl.KonsolaGUI.Console;

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using System.Xml.Serialization;

using System.Text.RegularExpressions;

using System.Diagnostics;
using System.ComponentModel;

using Goblinfactory.ProgressBar.moddedbyRevok;
using System.ComponentModel.Design.Serialization;
using Newtonsoft.Json.Linq;
// ReSharper disable All

namespace pwrpl_converter
{
    public class Rekord : IEquatable<Rekord>, IComparable<Rekord>
    {
        public int ID { get; set; }

        public string Plik { get; set; }
        public string Klucz { get; set; }
        public string String { get; set; }

        public override string ToString()
        {
            return "ID: " + ID + "\n" + "Plik: " + Plik + "\n" + "Klucz: " + Klucz + "\n" + "String: " + String;
        }
        public override bool Equals(object obiekt)
        {
            if (obiekt == null) return false;
            Rekord obiektrekordu = obiekt as Rekord;
            if (obiektrekordu == null) return false;
            else return Equals(obiektrekordu);
        }
        
        /*
        public int SortujRosnacoWedlugNazwy(string nazwa1, string nazwa2)
        {

            return nazwa1.CompareTo(nazwa2);
        }
        */

        // Domyślny komparator dla typu Rekord.
        public int CompareTo(Rekord porownaniezRekordem)
        {
            // Wartość null oznacza, że ten obiekt jest większy.
            if (porownaniezRekordem == null)
                return 1;

            else
                return this.ID.CompareTo(porownaniezRekordem.ID);
        }
        
        public override int GetHashCode()
        {
            return ID;
        }
        public bool Equals(Rekord other)
        {
            if (other == null) return false;
            return (this.ID.Equals(other.ID));
        }
        // Powinien również nadpisać operatory == i !=.
    }

    public class Linia
    {
        public StreamWriter OtwartyStrumienZapisuDoPliku { get; set; }
        public string Plik { get; set; }
        public string Klucz { get; set; }
        public string String { get; set; }

    }
    

    class pwrpl_converter
    {
        public const string _PWR_nazwaprogramu = "pwrpl-converter";
        public const string _PWR_wersjaprogramu = "v.3.02";
        public const string _PWR_rokwydaniawersji = "2024";
        
        public static char sc = System.IO.Path.DirectorySeparatorChar;
        private static string?[]? g_args;
        private static bool argumenty_czyuwzgledniac;
        readonly static string _PWR_naglowek = $"{_PWR_nazwaprogramu} {_PWR_wersjaprogramu} by Revok ({_PWR_rokwydaniawersji})";

        private readonly static bool wylacz_calkowitepokazywaniepostepow = true; //ustawienie na true przyspiesza operacje w KonsoliGUI (zaimplementowane w operacjach nr. 1, 2, 101)
        readonly static bool wl_pasekpostepu = false; //!!!wlaczenie tej opcji znacznie wydłuża wykonywanie operacji!!!
        private const string skrypt = "pwrpl-converter.cs";
        static public string folderglownyprogramu = Directory.GetCurrentDirectory();
        const string nazwafolderutmp = "tmp";

        public static JObject? cfg;

        static DateTime aktualny_czas = DateTime.Now;

        static List<string> makro_operacje_lista = new List<string>();
        static int makro_aktualny_indeks_listy;
        static bool makro_aktywowane;
        static bool makro_pomyslnezakonczenieoperacjinr2 = false;
        static bool makro_pomyslnezakonczenieoperacjinr100 = false;
        static bool makro_pomyslnezakonczenieoperacjinr101 = false;
        static bool makro_pomyslnezakonczenieoperacjinr200 = false;
        static List<string> makro_bledy_lista = new List<string>(); //element listy: "makro_numeroperacjiwkolejnosci;komunikat_obledzie"
        static List<string> makro_sukcesy_lista = new List<string>(); //element listy: "makro_numeroperacjiwkolejnosci;komunikat_osukcesie"

        static int tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumeryporzadkowe_wybor;
        static string tmpdlawatkow_2xtransifexCOMtxttoJSON_numerporzadkowy; //numer porządkowy bazy lub aktualizacji, np.: #1, #2 itd.
        static int tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor;
        static uint tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP;
        static uint tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii = 1;
        static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP;
        static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP;
        static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP;

        static uint tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii = 1;
        static string tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji;
        static dynamic[] tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_tablicalistdanych;
        static dynamic[] tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_tablicalistdanych;
        static dynamic[] tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_tablicalistdanych;
        static dynamic[] tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych;
        static List<dynamic> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_listakluczy;
        static List<dynamic> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_listakluczy;
        static List<dynamic> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listakluczy;
        static List<dynamic> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy;
        static List<List<dynamic>> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji;
        static List<List<dynamic>> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listastringow;
        static List<List<dynamic>> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow;
        static int tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateSchemaJSONTMP;
        static List<string> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP;
        static List<int> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD;
        static List<int> tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO;

        static List<List<Rekord>> tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__listawatkow_lista_rekordowplikudocelowego;
        static string tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_numerwersjidocelowej;
        static List<List<Rekord>> tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_listaplikowUPDATE_listarekordow;
        static List<dynamic> tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__SchematLokalizacjiWersjiDocelowej_listakluczy;
        static List<int> tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD;
        static List<int> tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO;
        static double tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_postep_aktualnalinia = 1;

        static uint tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_numeraktualnejlinii = 1;
        static string tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_oznaczeniewersji;


        static ProgressBar pasek_postepu;

        private static void InicjalizacjaPaskaPostepu(int ilosc_wszystkich_operacji)
        {
            pasek_postepu = new ProgressBar(PbStyle.DoubleLine, ilosc_wszystkich_operacji, 100, '■');
        }

        internal static void Blad(string tresc)
        {
            //Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(tresc);
            Console.ResetColor();
            Console.Write("\n");

        }
        internal static void Sukces(string tresc)
        {
            //Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(tresc);
            Console.ResetColor();
            Console.Write("\n");

        }
        internal static void Sukces2(string tresc)
        {
            //Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write(tresc);
            Console.ResetColor();
            Console.Write("\n");
        }
        internal static void Informacja(string tresc)
        {
            //Console.BackgroundColor = ConsoleColor.Magenta;
            Console.Write(tresc);
            Console.ResetColor();
            Console.Write("\n");
        }

        private static void Koniec(string token_zdarzenia = "", string api_zapisywanatresczdarzenia = "")
        {
            if (argumenty_czyuwzgledniac == false)
            {
                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();
            }
            else if (argumenty_czyuwzgledniac == true)
            {
                if (token_zdarzenia == "") { token_zdarzenia = API.WygenerujTokenZdarzenia(10); }
                
                API.UtworzZdarzenie(skrypt, token_zdarzenia, api_zapisywanatresczdarzenia);
            }
        }
        
        private static string PoliczPostepWProcentach(double aktualna_linia, double wszystkich_linii)
        {
            double rezultat = (aktualna_linia / wszystkich_linii) * 100;

            return Math.Round(rezultat, 0).ToString();
        }

        private static string PobierzTimestamp(DateTime wartosc)
        {
            return wartosc.ToString("yyyyMMddHHmmss");
        }

        private static bool CzyParsowanieINTUdane(string wartosc)
        {
            bool rezultat_bool = false;
            int rezultat_int = -1;

            if (int.TryParse(wartosc, out rezultat_int))
            {
                rezultat_bool = true;
            }

            return rezultat_bool;
        }

        public static uint PoliczLiczbeLinii(string nazwa_pliku)
        {
            uint liczbalinii = 0;

            if (File.Exists(nazwa_pliku))
            {
                FileStream plik_fs = new FileStream(nazwa_pliku, FileMode.Open, FileAccess.Read);

                try
                {
                    StreamReader plik_sr = new StreamReader(plik_fs);

                    while (plik_sr.Peek() != -1)
                    {
                        plik_sr.ReadLine();
                        liczbalinii++;
                    }

                    plik_sr.Close();

                }
                catch
                {
                    //Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("BLAD: Wystapil nieoczekiwany blad w dostepie do pliku (metoda: PoliczLiczbeLinii).");
                    Console.ResetColor();
                }

                plik_fs.Close();

            }
            else
            {
                //Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BLAD: Nie istnieje wskazany plik (metoda: PoliczLiczbeLinii).");
                Console.ResetColor();
            }

            return liczbalinii;
        }

        public static bool CzyIstniejeDanyKluczWLiscieKluczy(List<dynamic> lista_kluczy, string szukany_dany_klucz)
        {
            bool rezultat = false;

            rezultat = lista_kluczy.Exists(x => x == szukany_dany_klucz);

            return rezultat;
        }

        public static int PobierzNumerIndeksuZListyKluczyIStringow(dynamic[] tablica_list_kluczy_i_stringow, string szukany_dany_klucz)
        {
            int znaleziony_index = -1;

            if (tablica_list_kluczy_i_stringow.Length == 2)
            {
                List<dynamic> lista_kluczy = tablica_list_kluczy_i_stringow[0];
                List<List<dynamic>> lista_stringow = tablica_list_kluczy_i_stringow[1];

                znaleziony_index = lista_kluczy.IndexOf(szukany_dany_klucz);

            }

            return znaleziony_index;
        }

        public static List<string> PobierzNazwyPlikowJSONzFolderu(string nazwa_folderu, bool sortowanie = true)
        {
            List<string> nazwy_plikow_JSON = new List<string>();

            string folderglowny = Directory.GetCurrentDirectory();

            if (Directory.Exists(folderglowny + sc + nazwa_folderu) == true)
            {
                string[] plikiJSONwfolderze_nazwy = Directory.GetFiles(folderglowny + sc + nazwa_folderu, "*.json");
                
                if (sortowanie == true) { Array.Sort(plikiJSONwfolderze_nazwy); }
                
                foreach (string s in plikiJSONwfolderze_nazwy)
                {
                    FileInfo plik_fileinfo = null;

                    try
                    {
                        plik_fileinfo = new FileInfo(s);

                        nazwy_plikow_JSON.Add(plik_fileinfo.Name);
                    }
                    catch (FileNotFoundException e)
                    {
                        Blad("Blad: " + e.Message);
                        continue;
                    }

                }

            }
            else
            {
                Blad("Blad: Nie istnieje folder o nazwie: " + nazwa_folderu);
            }

            return nazwy_plikow_JSON;


        }

        public static string FiltrujString(string tresc_stringa)
        //stringi wczytane poprzez moduł JSON muszą zostać przefiltrowane przez zapisaniem w zmiennych/listach danych itd.
        {
             return tresc_stringa.Replace("\n", "\\n")
                    .Replace("\"", "\\\"")
                    .Replace("\t", "\\t")
                    .Replace("\\\\", "\\\\\\");

        }

        public static void Makro_UruchomienieKolejnejOperacji()
        {
            if (makro_bledy_lista.Count == 0)
            {
                //reset zmiennych po poprzednich operacjach
                makro_pomyslnezakonczenieoperacjinr2 = false;
                makro_pomyslnezakonczenieoperacjinr100 = false;
                makro_pomyslnezakonczenieoperacjinr101 = false;
                makro_pomyslnezakonczenieoperacjinr200 = false;

                int makro_sprawdzeniekolejnejoperacji = makro_aktualny_indeks_listy + 1;

                if (makro_sprawdzeniekolejnejoperacji < makro_operacje_lista.Count)
                {
                    makro_aktualny_indeks_listy++;

                    int makro_operacjadowykonania = int.Parse(makro_operacje_lista[makro_aktualny_indeks_listy]);

                    if (makro_operacjadowykonania == 1)
                    {
                        WeryfikacjaPlikowMetadanych("StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach");
                    }
                    else if (makro_operacjadowykonania == 2)
                    {
                        WeryfikacjaPlikowMetadanych("TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON");
                    }
                    else if (makro_operacjadowykonania == 100)
                    {
                        WdrazanieAktualizacji_WeryfikacjaPlikowMetadanych();
                    }
                    else if (makro_operacjadowykonania == 101)
                    {
                        WdrazanieAktualizacji_WeryfikacjaPlikowMetadanych(true);
                    }
                    else if (makro_operacjadowykonania == 200)
                    {
                        TworzenieStrukturyPlikowLokalizacji_WeryfikacjaPlikowUpdateLocStruct();
                    }
                    else
                    {
                        int makro_numerporzadadkowyoperacji = makro_sprawdzeniekolejnejoperacji + 1;
                        makro_bledy_lista.Add(makro_numerporzadadkowyoperacji + ";Zdefiniowano nieprawidłowy numer operacji w makrze (" + makro_operacjadowykonania + ").");
                        Makro_BladStopujacy();
                    }


                }
                else
                {
                    Sukces2("Wszystkie operacje ze zdefiniowanego makra zostały wykonane.");

                    Koniec();

                }

            }
            else
            {
                Makro_BladStopujacy();
            }


        }

        public static void Makro_BladStopujacy()
        {
            Blad("Wykryto błędy, które uniemożliwiły ukończenie wszystkich zaplanowanych operacji ze zdefiniowanego makra:");

            for (int wib = 0; wib < makro_bledy_lista.Count; wib++)
            {
                string[] dany_komunikat = makro_bledy_lista[wib].Split(';');
                if (dany_komunikat.Length == 2)
                {
                    Blad("[Błąd podczas wykonywania operacji makra: " + dany_komunikat[0] + "/" + makro_operacje_lista.Count + "] " + dany_komunikat[1]);
                }
            }

            Koniec();
        }

        public static void MenuMakr()
        {
            string numer_operacji2_string;
            Console.WriteLine("1. Uruchom zdefiniowane wcześniej makro: \"" + cfg["zdefiniowaneMakro"] + "\"");
            Console.WriteLine("2. Instrukcja zdefiniowania makra.");
            Console.Write("Wpisz numer operacji, którą chcesz wykonać: ");

            numer_operacji2_string = Console.ReadLine();

            if (CzyParsowanieINTUdane(numer_operacji2_string))
            {
                int numer_operacji2_int = int.Parse(numer_operacji2_string);

                if (numer_operacji2_int == 1)
                {
                    string[] makro_operacje_string = cfg["zdefiniowaneMakro"].ToString().Replace(" ", "").Split(';');

                    for (int mis = 0; mis < makro_operacje_string.Length; mis++)
                    {
                        makro_operacje_lista.Add(makro_operacje_string[mis]);

                        //Console.WriteLine(makro_operacje_string[mis]);

                    }

                    /*
                    //DEBUGtest - START
                    Console.WriteLine("makro_operacje_lista.Count: " + makro_operacje_lista.Count);
                    for (int t1 = 0; t1 < makro_operacje_lista.Count; t1++)
                    {
                        Console.WriteLine("makro_operacje_lista[t1]:" + makro_operacje_lista[t1]);
                    }
                    //DEBUGtest - STOP
                    */

                    if (int.Parse(makro_operacje_lista[0]) == 1)
                    {
                        makro_aktywowane = true;
                        makro_aktualny_indeks_listy = 0;

                        WeryfikacjaPlikowMetadanych("StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach");

                    }
                    else
                    {
                        Blad("BŁĄD MAKRA: Nieprawidłowa kolejność wykonywania operacji! Pierwsza operacja musi być zdefiniowana w makrze jako operacja nr.: 1 (weryfikacja).");

                        Koniec();

                    }





                }
                else if (numer_operacji2_int == 2)
                {
                    Console.WriteLine("Prawidłowo zdefiniowane makro umożliwia wykonanie wszystkich wymaganych operacji w zautomatyzowany sposób." +
                                      "Aby zdefiniować makro lub zmodyfikować już istniejące, należy:\n" +
                                      "1. Edytować plik \"cfg.json\" w edytorze tekstowym.\n" +
                                      "2. Odnaleźć linię zawierającą wpis: '\"autoWprowadzanieNazwPlikowWejsciowych\": \"1\",' a jeśli wartość jest ustawiona na 0 to zmienić ją na 1.\n" +
                                      "3. Odnaleźć linię zawierającą wpis: '\"zdefiniowaneMakro\": \"<TUTAJ_ZDEFINIUJ_MAKRO>\",';\n" +
                                      "4. Zamiast tekstu <TUTAJ_ZDEFINIUJ_MAKRO> wpisać makro, które chcemy zdefiniować (przykład poniżej).\n" +
                                      "5. Zapisać plik i uruchomić ponownie pwrpl-converter.\n" +
                                      "Zamiast <TUTAJ_ZDEFINIUJ_MAKRO> wpisz numery operacji, które mają zostać automatycznie wybrane, oddzielając je średnikami ','.\n" +
                                      "Na przykład zdefiniowanie makra: \"1;1; 1;2; 2;1;0;0; 2;2;1;0; 100;1\" spowoduje po kolei wykonywanie przez narzędzie operacji:\n" +
                                      "-Weryfikacja identyfikatorów numerów linii w pliku lokalizacyjnym dla wersji gry " + cfg["wersjaGryPierwszegoProjektuZTransifexCOM"] + "\n" +
                                      "-Weryfikacja identyfikatorów numerów linii w pliku lokalizacyjnym aktualizacji dla wersji gry x.x.x\n" +
                                      "-Konwersja pliku lokalizacji TXT->JSON dla wersji gry: " + cfg["wersjaGryPierwszegoProjektuZTransifexCOM"] + " (bez dołączenia numerów porządkowych i bez dołączenia numerów/id linii)\n" +
                                      "-Konwersja pliku aktualizacji lokalizacji TXT->JSON dla wersji gry: x.x.x (z dołączeniem numerów porządkowych, ale bez dołączenia numerów/id linii)\n" +
                                      "-Wdrażanie aktualizacji do pliku lokalizacji " + cfg["wersjaGryPierwszegoProjektuZTransifexCOM"] + "->x.x.x\n" +
                                      "UWAGA: Makra działają wyłącznie dla operacji narzędzia: 1, 2, 100, 101 i 200 tj. weryfikacji, konwersji TXT->JSON, wdrażania aktualizacji, zbiorczego wdrażania aktualizacji i tworzenia struktury lokalizacji.\n" +
                                      "UWAGA2: Pierwsza operacja makra musi zostać zdefiniowana jako: 1 (tj. weryfikacja).\n" +
                                      "UWAGA3: Zaleca się definiowanie makra w taki sposób aby w pierwszej kolejności wykonywać operacje weryfikacji (1) dla wszystkich plików po kolei, następnie konwersję (2) dla wszystkich plików, a na końcu wdrażanie aktualizacji (100) po kolei dla wszystkich plików lub na końcu zbiorcze wdrażanie aktualizacji (101).\n" +
                                      "UWAGA4: Można dodawać spacje w zdefiniowanym makrze. Jeśli makro jest długie, poprawi to czytelność i ułatwi jego edycję w przyszłości.");

                    Koniec();
                }
                else
                {
                    //Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Podano błędny numer operacji.");
                    Console.ResetColor();

                    Koniec();
                }
            }
            else
            {
                //Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Podano błedny numer operacji.");
                Console.ResetColor();

                Koniec();
            }



        }
        
        public static void Main2(string[] args)
        {
            int argumenty_ilosc = args.Length;
            g_args = args;
            
            //Console.Title =_PWR_naglowek;
            
            Process[] procesy_nazwa = Process.GetProcessesByName("pwrpl-converter");

            /*
            Console.WriteLine("Lista uruchomionych procesów:");
            for (int x = 0; x < procesy_nazwa.Length; x++)
            {
                Console.WriteLine("(" + x + ") " + procesy_nazwa[x].ToString());
            }
            */

            if (procesy_nazwa.Length <= 1)
            {
                konfiguracja.WygenerujDomyslnyPlikKonfiguracyjnyJesliNieIstnieje();

                cfg = konfiguracja.WczytajDaneKonfiguracyjne();

                string numer_operacji_string = "-1";

                Console.WriteLine(_PWR_naglowek);

                if (argumenty_ilosc == 0)
                {
                    Console.WriteLine("WAŻNE: Pliki poddawane operacjom muszą zostać skopiowane wcześniej do folderu z tym programem.");
                    Console.WriteLine("---[PWR_PL]:");
                    Console.WriteLine("0. Uruchomienie makra lub jego zdefiniowanie.");
                    Console.WriteLine("1. [1xstringsTransifexCOM] Weryfikacja identyfikatorów numerów linii na początku stringów w pliku TXT pochodzącego z Transifex.com.");
                    Console.WriteLine("2. [2xTransifex.com.TXT->1xJSON] Konwersja plików TXT z platformy Transifex.com do pliku JSON.");
                    Console.WriteLine("3. [JSON->JSON] Konwersja pliku JSON z polskimi znakami na plik bez polskich znakow.");
                    Console.WriteLine("100. [JSON+Metadane->JSON] Wdrażanie aktualizacji do pliku JSON.");
                    Console.WriteLine("101. [JSON+Metadane->JSON] Zbiorcze wdrażanie aktualizacji do pliku JSON.");
                    Console.WriteLine("200. [JSON+Metadane->Folder JSON] Tworzenie wieloplikowej struktury lokalizacji.");
                    Console.WriteLine("---------------------------------------");
                    Console.Write("Wpisz numer operacji, którą chcesz wykonać: ");
                    numer_operacji_string = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("[DEBUG] args:");
                    
                    argumenty_czyuwzgledniac = true;
                    
                    for (int i = 0; i < argumenty_ilosc; i++)
                    {
                        Console.WriteLine($"args[{i}] == {args[i]}");
                    }
                    
                    // -op=1 -np=[np] -in=[path] [opcjonalnie:]-event=[token_zdarzenia]
                    if (args[0] == "-op=1")
                    {
                        if (argumenty_ilosc >= 3)
                        {
                            numer_operacji_string = "1";
                        }
                        else
                        {
                            Blad("Błąd (#args:-op=1): Składnia podanych argumentów jest nieprawidłowa.");
                            
                            Informacja("Wzorzec:");
                            Informacja(" -op=1 -np=[np] -in=[path] [opcjonalnie:]-event=[token_zdarzenia]");
                            Informacja("gdzie [np] to: [numer_porzadkowy_bez_#_oraz_zera_na_poczatku] (przykładowo: 1 (dla #01_1.0.1c))");
                            Informacja("gdzie [path] to pełna ścieżka do pliku \"*.keysTransifexCOM.txt\"");
                            Informacja("gdzie [token_zdarzenia] to token, pod którym zostanie zapisane zdarzenie w katalogu \"API\" w pliku o nazwie \"[nazwa_skryptu_wysyłajacego]-[token_zdarzenia].event\" (UWAGA: jeśli jako argument nie zostanie zdefiniowany token zdarzenia, wtedy zostanie wygenerowany losowy)");
                        }
                    }
                    
                    // -op=2 -np=[np] -in1=[path1] -in2=[path2] -w1=[numer_opcji1] -w2=[numer_opcji2] -out=[path_out] [opcjonalnie:]-event=[token_zdarzenia]
                    else if (args[0] == "-op=2")
                    {
                        if (argumenty_ilosc >= 7)
                        {
                            numer_operacji_string = "2";
                        }
                        else
                        {
                            Blad("Błąd (#args:-op=2): Składnia podanych argumentów jest nieprawidłowa.");
                            
                            Informacja("Wzorzec:");
                            Informacja(" -op=2 -np=[np] -in1=[path1] -in2=[path2] -w1=[numer_opcji1] -w2=[numer_opcji2] -out=[path_out] [opcjonalnie:]-event=[token_zdarzenia]");
                            Informacja("gdzie [np] to: [numer_porzadkowy_bez_#_oraz_zera_na_poczatku] (przykładowo: 1 (dla #01_1.0.1c))");
                            Informacja("gdzie [path1] to pełna ścieżka do pliku \"*.keysTransifexCOM.txt\"");
                            Informacja("gdzie [path2] to pełna ścieżka do pliku \"*.stringsTransifexCOM.txt\"");
                            Informacja("gdzie [numer_opcji1] to numer opcji 1 (domyślnie 0)");
                            Informacja("gdzie [numer_opcji2] to numer opcji 2 (domyślnie 0)");
                            Informacja("gdzie [path_out] to pełna ścieżka do nowotworzonego pliku .json, który zostanie wygenerowany - plik powinien kończyć się rozszerzeniem \".json\"");
                            Informacja("gdzie [token_zdarzenia] to token, pod którym zostanie zapisane zdarzenie w katalogu \"API\" w pliku o nazwie \"[nazwa_skryptu_wysyłajacego]-[token_zdarzenia].event\" (UWAGA: jeśli jako argument nie zostanie zdefiniowany token zdarzenia, wtedy zostanie wygenerowany losowy)");

                        }
                    }
                    
                    else if (args[0] == "-op=3")
                    {
                        Informacja("Argumenty dla wskazanej operacji nie są zaimplementowane.");
                    }
                    
                    else if (args[0] == "-op=100")
                    {
                        Informacja("Argumenty dla wskazanej operacji nie są zaimplementowane.");
                    }
                    
                    // -op=101 -in_folder=[folderpath] -out_folder=[folderpath_out] [opcjonalnie:]-event=[token_zdarzenia]
                    else if (args[0] == "-op=101")
                    {
                        if (argumenty_ilosc >= 3)
                        {
                            numer_operacji_string = "101";
                        }
                        else
                        {
                            Blad("Błąd (#args:-op=2): Składnia podanych argumentów jest nieprawidłowa.");

                            Informacja("Wzorzec:");
                            Informacja(" -op=101 -in_folder=[folderpath] -out_folder=[folderpath_out] [opcjonalnie:]-event=[token_zdarzenia]");
                            Informacja("gdzie [folderpath] to pełna ścieżka do folderu zawierającego bazowy plik JSON oraz aktualizacje do wdrożenia. UWAGA: ścieżka musi być zakończona znakiem: " + sc);
                            Informacja("gdzie [folderpath_out] to pełna ścieżka do folderu, gdzie ma zostać zapisany wygenerowany plik lub pliki. UWAGA: ścieżka musi być zakończona znakiem: " + sc);
                            Informacja("gdzie [token_zdarzenia] to token, pod którym zostanie zapisane zdarzenie w katalogu \"API\" w pliku o nazwie \"[nazwa_skryptu_wysyłajacego]-[token_zdarzenia].event\" (UWAGA: jeśli jako argument nie zostanie zdefiniowany token zdarzenia, wtedy zostanie wygenerowany losowy)");

                        }
                    }
                    
                    else if (args[0] == "-op=200")
                    {
                        Informacja("Argumenty dla wskazanej operacji nie są zaimplementowane.");
                    }
                    
                    else
                    {
                        Blad("Błąd (#args:2): Składnia podanych argumentów jest nieprawidłowa.");
                        
                        Informacja("Pierwszym argumentem powinien być: -op=[numer_operacji]");
                    }
                }
                

                if (CzyParsowanieINTUdane(numer_operacji_string))
                {
                    int numer_operacji_int = int.Parse(numer_operacji_string);

                    if (numer_operacji_int == 0)
                    {
                        MenuMakr();
                    }
                    else if (numer_operacji_int == 1)
                    {
                        WeryfikacjaPlikowMetadanych("StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach");
                    }
                    else if (numer_operacji_int == 2)
                    {
                        WeryfikacjaPlikowMetadanych("TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON");
                    }
                    else if (numer_operacji_int == 3)
                    {
                        UsuwanieZnakowPL();
                    }
                    else if (numer_operacji_int == 100)
                    {
                        WdrazanieAktualizacji_WeryfikacjaPlikowMetadanych();
                    }
                    else if (numer_operacji_int == 101)
                    {
                        WdrazanieAktualizacji_WeryfikacjaPlikowMetadanych(true);
                    }
                    else if (numer_operacji_int == 200)
                    {
                        TworzenieStrukturyPlikowLokalizacji_WeryfikacjaPlikowUpdateLocStruct();
                    }

                    else
                    {
                        Blad("Podano błędny numer operacji.");

                        Koniec();
                    }
                }
                else
                {
                    Blad("Podano błedny numer operacji.");

                    Koniec();
                }

            }
            else
            {
                Blad("Aplikacja pwrpl-converter jest aktualnie uruchomiona w innym oknie lub nazwa pliku wykonywalnego jest nieprawidłowa.");

                Koniec();
            }
            

        }


        private static bool UtworzNaglowekJSON(string nazwaplikuJSON, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikJSON_fs;

            if (nazwafolderu == "")
            {
                plikJSON_fs = new FileStream(nazwaplikuJSON, FileMode.Create, FileAccess.Write);
            }
            else
            {
                plikJSON_fs = new FileStream(nazwafolderu + sc + nazwaplikuJSON, FileMode.Create, FileAccess.Write);
            }

            try
            {
                StreamWriter plikJSON_sw = new StreamWriter(plikJSON_fs);

                plikJSON_sw.WriteLine("{");
                plikJSON_sw.WriteLine("  \"$id\": \"1\",");
                plikJSON_sw.WriteLine("  \"strings\": {");

                plikJSON_sw.Close();

                rezultat = true;

            }
            catch
            {
                rezultat = false;
            }

            plikJSON_fs.Close();


            return rezultat;
        }

        private static bool UtworzStopkeJSON(string nazwaplikuJSON, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikJSON_fs;

            string sprawdzenie_istnienia;

            if (nazwafolderu == "")
            {
                sprawdzenie_istnienia = nazwaplikuJSON;
            }
            else
            {
                sprawdzenie_istnienia = nazwafolderu + sc + nazwaplikuJSON;
            }

            if (File.Exists(sprawdzenie_istnienia))
            {
                if (nazwafolderu == "")
                {
                    plikJSON_fs = new FileStream(nazwaplikuJSON, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    plikJSON_fs = new FileStream(nazwafolderu + sc + nazwaplikuJSON, FileMode.Append, FileAccess.Write);
                }

                try
                {
                    StreamWriter plikJSON_sw = new StreamWriter(plikJSON_fs);

                    plikJSON_sw.WriteLine("  }");
                    plikJSON_sw.Write("}");

                    plikJSON_sw.Close();

                    rezultat = true;

                }
                catch
                {
                    rezultat = false;
                }

                plikJSON_fs.Close();

            }
            else
            {
                rezultat = false;
            }

            return rezultat;
        }

        private static void UtworzPlikTXT_TMP(string nazwa_pliku, List<string> lista_danych, int index_od, int index_do)
        {
            //Console.WriteLine("UtworzPlikTXT_TMP(" + nazwa_pliku + ", " + lista_danych + "," + index_od + ", " + index_do + ")");

            if (Directory.Exists(nazwafolderutmp) == false)
            {
                Directory.CreateDirectory(nazwafolderutmp);
            }

            if (File.Exists(nazwafolderutmp + sc + nazwa_pliku) == true)
            {
                File.Delete(nazwafolderutmp + sc + nazwa_pliku);
            }

            FileStream plikTMP_fs = new FileStream(nazwafolderutmp + sc + nazwa_pliku, FileMode.Append, FileAccess.Write);

            try
            {
                StreamWriter plikTMP_sw = new StreamWriter(plikTMP_fs);

                for (int zd = index_od; zd <= index_do; zd++)
                {
                    plikTMP_sw.WriteLine(lista_danych[zd]);
                }



                plikTMP_sw.Close();
            }
            catch (Exception Error)
            {
                Blad("BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików w metodzie UtworzPlikTXT_TMP(" + nazwa_pliku + ", " + lista_danych + "," + index_od + ", " + index_do + ") - (Error: " + Error + ")!");
            }

            plikTMP_fs.Close();

        }

        private static void UsunPlikiTMP(List<string> lista_nazw_plikow)
        {
            if (Directory.Exists(nazwafolderutmp) == true)
            {
                for (int i = 0; i < lista_nazw_plikow.Count; i++)
                {
                    if (File.Exists(nazwafolderutmp + sc + lista_nazw_plikow[i]) == true)
                    {
                        File.Delete(nazwafolderutmp + sc + lista_nazw_plikow[i]);
                    }
                }

            }
        }

        public static void WeryfikacjaPlikowMetadanych(string nazwa_metody_ktora_ma_zostac_uruchomiona)
        {
            Console.WriteLine("1. #" + cfg["numerPorzadkowyPierwszegoProjektuZTransifexCOM"] + "_" + cfg["wersjaGryPierwszegoProjektuZTransifexCOM"]);

            string folderupdate = folderglownyprogramu + sc + "update";
            string przyrostek_UpdateLocStruct = ".UpdateLocStruct.json";
            string przyrostek_UpdateLog = ".UpdateLog.json";
            string przyrostek_UpdateSchema = ".UpdateSchema.json";

            List<string> lista_oznaczen_aktualizacji = new List<string>();

            if (Directory.Exists(folderupdate) == true)
            {
                List<string> istniejacenazwyplikowmetadanych = PobierzNazwyPlikowJSONzFolderu("update");

                int np = 2;
                for (int i = 0; i < istniejacenazwyplikowmetadanych.Count; i++)
                {
                    //Console.WriteLine("Nazwa pliku metadanych: " + istniejacenazwyplikowmetadanych[i]);

                    if (istniejacenazwyplikowmetadanych[i].Contains(przyrostek_UpdateSchema) == true)
                    {
                        string oznaczenie_aktualizacji = istniejacenazwyplikowmetadanych[i].Split(new string[] { ".Update" }, StringSplitOptions.None)[0];

                        //Console.WriteLine("Oznaczenie aktualizacji: " + oznaczenie_aktualizacji);

                        if (File.Exists(folderupdate + sc + oznaczenie_aktualizacji + przyrostek_UpdateLocStruct) == true
                            && File.Exists(folderupdate + sc + oznaczenie_aktualizacji + przyrostek_UpdateLog) == true
                            && File.Exists(folderupdate + sc + oznaczenie_aktualizacji + przyrostek_UpdateSchema) == true)
                        {
                            lista_oznaczen_aktualizacji.Add(oznaczenie_aktualizacji);

                            Console.WriteLine(np + ". " + oznaczenie_aktualizacji.Replace("-", "->"));

                            np++;
                        }

                    }

                }

            }



            string numer_pozycji_string = "";
            Console.Write("Wpisz numer pozycji, której konwersja ma dotyczyć: ");

            if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
            {
                makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                numer_pozycji_string = makro_operacje_lista[makro_aktualny_indeks_listy];

                Console.WriteLine(makro_operacje_lista[makro_aktualny_indeks_listy]);
            }
            else if (argumenty_czyuwzgledniac == true)
            {
                
                string numer_pozycji_string_tmp = g_args[1].Replace("-np=", "");
                
                if (CzyParsowanieINTUdane(numer_pozycji_string_tmp) == true)
                {
                    numer_pozycji_string = numer_pozycji_string_tmp;
                    
                    Console.Write(numer_pozycji_string + "\n");
                }
            }
            else
            {
                numer_pozycji_string = Console.ReadLine();
            }

            if (CzyParsowanieINTUdane(numer_pozycji_string))
            {
                string domyslna_nazwaplikukeysTransifexCOMTXT = "";
                string domyslna_nazwaplikustringsTransifexCOMTXT = "";


                int numer_pozycji_int = int.Parse(numer_pozycji_string);

                if (numer_pozycji_int == 1)
                {
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_numerporzadkowy = "#1";

                    //Console.WriteLine("DBG: Wybrano domyślny plik (czyli pozycję nr.: " + numer_pozycji_string + ").");

                    if (nazwa_metody_ktora_ma_zostac_uruchomiona == "StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach")
                    {
                        if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
                        {
                            domyslna_nazwaplikustringsTransifexCOMTXT = cfg["domyslnaNazwaPlikuTXTZTransifexCOM"].ToString();
                        }
                        else
                        {
                            domyslna_nazwaplikustringsTransifexCOMTXT = "";
                        }

                        StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach(domyslna_nazwaplikustringsTransifexCOMTXT);

                    }
                    else if (nazwa_metody_ktora_ma_zostac_uruchomiona == "TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON")
                    {
                        if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
                        {
                            domyslna_nazwaplikukeysTransifexCOMTXT = cfg["domyslnaNazwaPlikukeysTransifexCOMTXT"].ToString();
                            domyslna_nazwaplikustringsTransifexCOMTXT = cfg["domyslnaNazwaPlikuTXTZTransifexCOM"].ToString();
                        }
                        else
                        {
                            domyslna_nazwaplikukeysTransifexCOMTXT = "";
                            domyslna_nazwaplikustringsTransifexCOMTXT = "";
                        }

                        TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON(domyslna_nazwaplikukeysTransifexCOMTXT, domyslna_nazwaplikustringsTransifexCOMTXT);


                    }
                }
                else if (numer_pozycji_int > 1)
                {
                    int indeks_oznaczeniaaktualizacji = (numer_pozycji_int) - 2;

                    if ((indeks_oznaczeniaaktualizacji >= 0) && (lista_oznaczen_aktualizacji.Count - 1 >= indeks_oznaczeniaaktualizacji))
                    {
                        //Console.WriteLine("DBG: Wybrano plik aktualizacji (a konkretnie pozycję nr.: " + numer_pozycji_string + ").");

                        if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
                        {
                            string[] tmp1_loa = lista_oznaczen_aktualizacji[indeks_oznaczeniaaktualizacji].Split(new char[] { '_' });

                            if (tmp1_loa.Length >= 2)
                            {
                                string numerporzadkowy_aktualizacji = tmp1_loa[0];
                                tmpdlawatkow_2xtransifexCOMtxttoJSON_numerporzadkowy = tmp1_loa[0];
                                string oznaczenie_aktualizacji = tmp1_loa[1];

                                domyslna_nazwaplikukeysTransifexCOMTXT = cfg["domyslnaNazwaPlikuAktualizacjikeysTransifexCOMTXT"].ToString()
                                .Replace("%OZNACZENIE_AKTUALIZACJI%", oznaczenie_aktualizacji)
                                ;

                                domyslna_nazwaplikustringsTransifexCOMTXT = cfg["domyslnaNazwaPlikuTXTAktualizacjiZTransifexCOM"].ToString()
                                .Replace("%NUMER_PORZADKOWY_AKTUALIZACJI%", numerporzadkowy_aktualizacji.Replace("#", ""))
                                .Replace("%OZNACZENIE_AKTUALIZACJI%", oznaczenie_aktualizacji.Replace(".", ""))
                                ;
                            }
                            else
                            {
                                Blad("Wykryto przynajmniej jedną nieprawidłowość w nazwach plików metadanych aktualizacji.");

                                Koniec();
                            }
                        }
                        else
                        {
                            domyslna_nazwaplikukeysTransifexCOMTXT = "";
                            domyslna_nazwaplikustringsTransifexCOMTXT = "";
                        }

                        if (nazwa_metody_ktora_ma_zostac_uruchomiona == "StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach")
                        {
                            StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach(domyslna_nazwaplikustringsTransifexCOMTXT);
                        }
                        else if (nazwa_metody_ktora_ma_zostac_uruchomiona == "TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON")
                        {
                            TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON(domyslna_nazwaplikukeysTransifexCOMTXT, domyslna_nazwaplikustringsTransifexCOMTXT);
                        }

                    }
                    else
                    {

                        Blad("Podano błędny numer pozycji. (#3)");

                        Koniec();
                    }
                }
                else
                {
                    Blad("Podano błędny numer pozycji. (#2)");

                    Koniec();
                }

            }
            else
            {
                Blad("Podano błędny numer pozycji. (#1)");

                Koniec();
            }


        }

        private static void StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach(string domyslna_nazwaplikustringsTransifexCOMTXT)
        {
            string api_zapisywanatresczdarzenia = "Niepowodzenie operacji";
            
            bool nie_wyswietlaj_komunikatu_o_sukcesie = false;

            List<int> bledy = new List<int>();

            string nazwaplikustringsTransifexCOMTXT = "";
            
            if (argumenty_czyuwzgledniac == false)
            {
                if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
                {
                    nazwaplikustringsTransifexCOMTXT = domyslna_nazwaplikustringsTransifexCOMTXT;

                }
                else
                {
                    Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt: ");
                    nazwaplikustringsTransifexCOMTXT = Console.ReadLine();
                }
            }
            else if (argumenty_czyuwzgledniac == true)
            {
                string in_path = g_args[2].Replace("-in=", "");
                nazwaplikustringsTransifexCOMTXT = in_path; //w tym przypadku jest to scieżka do pliku
            }
            
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikustringsTransifexCOMTXT);
            if (File.Exists(nazwaplikustringsTransifexCOMTXT))
            {
                

                uint plik_stringsTransifexCOMTXT_liczbalinii = PoliczLiczbeLinii(nazwaplikustringsTransifexCOMTXT);

                if (wl_pasekpostepu == true)
                {
                    InicjalizacjaPaskaPostepu(Convert.ToInt32(plik_stringsTransifexCOMTXT_liczbalinii));
                }

                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_stringsTransifexCOMTXT_fs = new FileStream(nazwaplikustringsTransifexCOMTXT, FileMode.Open, FileAccess.Read);

                try
                {
                    string plik_stringsTransifexCOMTXT_trescaktualnejlinii;

                    StreamReader plik_stringsTransifexCOMTXT_sr = new StreamReader(plik_stringsTransifexCOMTXT_fs);

                    if (wylacz_calkowitepokazywaniepostepow == true) { Console.WriteLine("Trwa analizowanie linii. Może to chwilę zająć. Proszę czekać...");}
                    
                    int postep_w_procentach_ostatniawartosccalkowita = -1;
                    int plik_stringsTransifexCOMTXT_aktualnalinia = 1;
                    while (plik_stringsTransifexCOMTXT_sr.Peek() != -1)
                    {
                        plik_stringsTransifexCOMTXT_trescaktualnejlinii = plik_stringsTransifexCOMTXT_sr.ReadLine();

                        int plik_stringsTransifexCOMTXT_aktualnyidlinii = plik_stringsTransifexCOMTXT_aktualnalinia + 3;

                        string[] podzial1 = plik_stringsTransifexCOMTXT_trescaktualnejlinii.Split(new char[] { '>' });
                        int id_pobrane_z_tresci_pliku;

                        if (wylacz_calkowitepokazywaniepostepow == false)
                        {
                            if (wl_pasekpostepu == false)
                            {
                                int postep_w_procentach = int.Parse(PoliczPostepWProcentach(plik_stringsTransifexCOMTXT_aktualnalinia, plik_stringsTransifexCOMTXT_liczbalinii));
                                string komunikat_aktualnypostep = $"Trwa analizowanie linii. Aktualny postęp: {postep_w_procentach}% z {plik_stringsTransifexCOMTXT_liczbalinii}";

                                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                                {
                                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;
                                    komunikat_aktualnypostep = "[Operacja makra: " + makro_numeroperacjiwkolejnosci + "/" + makro_operacje_lista.Count + "] " + komunikat_aktualnypostep;
                                }

                                if (postep_w_procentach_ostatniawartosccalkowita != postep_w_procentach)
                                {
                                    //Console.WriteLine($"{postep_w_procentach_ostatniawartosccalkowita} != {postep_w_procentach}");
                            
                                    Console.WriteLine(komunikat_aktualnypostep);
                                    postep_w_procentach_ostatniawartosccalkowita = postep_w_procentach;

                                }
                            }
                            else if (wl_pasekpostepu == true)
                            {
                                pasek_postepu.Refresh(plik_stringsTransifexCOMTXT_aktualnalinia, "Trwa analizowanie linii...");
                            }
                        }

                        try
                        {

                            if (podzial1[0].Contains(' ') == false)
                            {
                                id_pobrane_z_tresci_pliku = int.Parse(podzial1[0].Replace("<", ""));

                                if (plik_stringsTransifexCOMTXT_aktualnyidlinii == id_pobrane_z_tresci_pliku)
                                {
                                    //Console.WriteLine("[Linia nr: " + plik_stringsTransifexCOMTXT_aktualnalinia + "] ID-ok");
                                }
                                else
                                {
                                    bledy.Add(plik_stringsTransifexCOMTXT_aktualnalinia);

                                    //Console.WriteLine("[Linia nr: " + plik_stringsTransifexCOMTXT_aktualnalinia + "] Błędna wartość ID w stringu! ID w tej linii powinien mieć treść <" + plik_stringsTransifexCOMTXT_aktualnyidlinii + ">");
                                }

                            }
                            else
                            {
                                bledy.Add(plik_stringsTransifexCOMTXT_aktualnalinia);
                            }


                        }
                        catch
                        {
                            bledy.Add(plik_stringsTransifexCOMTXT_aktualnalinia);

                            //Console.WriteLine("[Linia nr: " + plik_stringsTransifexCOMTXT_aktualnalinia + "] Błędna wartość id w stringu (zmienna nie może przyjąć wartości innej niż int).");
                        }


                        plik_stringsTransifexCOMTXT_aktualnalinia++;
                    }



                    plik_stringsTransifexCOMTXT_sr.Close();

                }
                catch
                {
                    nie_wyswietlaj_komunikatu_o_sukcesie = true;

                    string komunikat_obledzie;
                    komunikat_obledzie = "BŁĄD: Wystapil nieoczekiwany błąd w dostępie do pliku.";

                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }

                    Blad(komunikat_obledzie);
                    
                }


                plik_stringsTransifexCOMTXT_fs.Close();



            }
            else
            {
                nie_wyswietlaj_komunikatu_o_sukcesie = true;

                string komunikat_obledzie;
                komunikat_obledzie = "BŁĄD: Nie istnieje wskazany plik: \"" + nazwaplikustringsTransifexCOMTXT + "\".";

                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                {
                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                }

                Blad(komunikat_obledzie);

                
            }


            int bledy_iloscwykrytych = bledy.Count();
            if (bledy_iloscwykrytych == 0)
            {
                if (nie_wyswietlaj_komunikatu_o_sukcesie == false)
                {

                    string komunikat_osukcesie;
                    komunikat_osukcesie = "Nie znaleziono błędów w identyfikatorach linii na początku stringów.";

                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_sukcesy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_osukcesie);

                    }

                    api_zapisywanatresczdarzenia = "Powodzenie operacji";
                    
                    Sukces(komunikat_osukcesie);
                    
                }

            }
            else
            {
                

                Blad("Znaleziono błędów w pliku: " + bledy_iloscwykrytych);
                Blad("UWAGA: Jeśli identyfikator w danej linii się zgadza to należy skontrolować również czy nie ma nieprawidłowo wstawionych spacji (spacje przed <id> oraz w treści <id> są niedozwolone).");

                for (int ib = 0; ib < bledy_iloscwykrytych; ib++)
                {
                    int numer_linii = bledy[ib];
                    int poprawny_identyfikator_linii = numer_linii + 3;

                    string komunikat_obledzie;
                    komunikat_obledzie = "Wykryto błąd w linii nr: " + numer_linii + " (poprawny id powinien mieć treść: \"<" + poprawny_identyfikator_linii.ToString() + ">\")";

                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }


                    Blad(komunikat_obledzie);
                }

            }


            if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
            {
                Makro_UruchomienieKolejnejOperacji();
            }
            else
            {
                Console.ResetColor();

                string api_tokenzdarzenia = "";
                
                foreach (string? argument in g_args)
                {
                    if (argument.Contains("-event=") == true)
                    {
                        string wartosc_argumentu = argument.Replace("-event=", "");
                        if (wartosc_argumentu != "") { api_tokenzdarzenia = wartosc_argumentu; }
                    }
                }
                
                Koniec(api_tokenzdarzenia, api_zapisywanatresczdarzenia);
                
            }

        }

        public static void TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON(string domyslna_nazwaplikuaktualizacjikeysTransifexCOMTXT, string domyslna_nazwaplikuaktualizacjistringsTransifexCOMTXT)
        {
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - POCZĄTEK */
            if (Directory.Exists(nazwafolderutmp) == true)
            {
                Directory.Delete(nazwafolderutmp, true);
            }
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - KONIEC */

            string api_zapisywanatresczdarzenia = "Niepowodzenie operacji";

            string plikkeystxt_path = "";
            string plikstringstxt_path = "";
            string nowyplikJSON_path = "";
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const int ilosc_watkow = 20;
            List<string> plikkeystxt_trescilinii = new List<string>();
            List<string> plikstringstxt_trescilinii = new List<string>();

            if (argumenty_czyuwzgledniac == false)
            {
                if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
                {
                    plikkeystxt_path = domyslna_nazwaplikuaktualizacjikeysTransifexCOMTXT;

                    plikstringstxt_path = domyslna_nazwaplikuaktualizacjistringsTransifexCOMTXT;
                }
                else
                {
                    Console.Write("Podaj nazwę pliku .keysTransifexCOM.txt: ");
                    plikkeystxt_path = Console.ReadLine();

                    Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt: ");
                    plikstringstxt_path = Console.ReadLine();
                }
            }
            else if (argumenty_czyuwzgledniac == true)
            {
                plikkeystxt_path = g_args[2].Replace("-in1=", "");
                plikstringstxt_path = g_args[3].Replace("-in2=", "");
            }

            Console.WriteLine("Podano nazwę pliku .keysTransifexCOM.txt: " + plikkeystxt_path);
            Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM.txt: " + plikstringstxt_path);

            if (File.Exists(plikkeystxt_path) && File.Exists(plikstringstxt_path))
            {
                Console.WriteLine("Czy dołączyć numer porządkowy bazy tłumaczenia lub aktualizacji tłumaczenia do każdej linii? (Wybierz numer poniższej opcji i zatwierdź ENTEREM.)");
                Console.WriteLine("0. Nie dołączaj. (format: string)");
                Console.WriteLine("1. Dołącz numer porządkowy. (format: #numer_porządkowy:string)");

                string czydolaczycnumerporzadkowy_wybor;

                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                {
                    makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                    czydolaczycnumerporzadkowy_wybor = makro_operacje_lista[makro_aktualny_indeks_listy];
                }
                else if (argumenty_czyuwzgledniac == true)
                {
                    czydolaczycnumerporzadkowy_wybor = g_args[4].Replace("-w1=", "");
                }
                else
                {
                    czydolaczycnumerporzadkowy_wybor = Console.ReadLine();
                }

                if (czydolaczycnumerporzadkowy_wybor == "1")
                {
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumeryporzadkowe_wybor = 1;
                    Console.WriteLine("Numery porządkowe zostaną dołączone do lokalizacji i będą się wyświetlały w grze.");
                }




                Console.WriteLine("Czy dołączyć numery linii do lokalizacji, aby wyświetlały się w grze? (Wybierz numer poniższej opcji i zatwierdź ENTEREM.)");
                Console.WriteLine("0. Nie dołączaj. (format: string)");
                Console.WriteLine("1. Dołącz numery linii. (format: [numer_linii]string)");
                Console.WriteLine("2. Dołącz numery i identyfikatory linii. (format: [numer_linii]<id_linii>string)");

                string czydolaczycnumerylinii_wybor;

                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                {
                    makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                    czydolaczycnumerylinii_wybor = makro_operacje_lista[makro_aktualny_indeks_listy];

                }
                else if (argumenty_czyuwzgledniac == true)
                {
                    czydolaczycnumerylinii_wybor = g_args[5].Replace("-w2=", "");
                }
                else
                {
                    czydolaczycnumerylinii_wybor = Console.ReadLine();
                }


                if (czydolaczycnumerylinii_wybor == "1")
                {
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor = 1;
                    Console.WriteLine("Numery linii zostaną dołączone do lokalizacji i będą się wyświetlały w grze.");
                }
                else if (czydolaczycnumerylinii_wybor == "2")
                {
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor = 2;
                    Console.WriteLine("Numery wraz z identyfikatorami linii zostaną dołączone do lokalizacji i będą się wyświetlały w grze.");
                }

                if (argumenty_czyuwzgledniac == true)
                {
                    nowyplikJSON_path = g_args[6].Replace("-out=", "");
                }
                else
                {
                    nowyplikJSON_path = "NOWY_" + plikkeystxt_path.Replace(".keysTransifexCOM.txt", "");
                }

                Console.WriteLine("Nowy plik JSON to: " + nowyplikJSON_path);

                plikkeystxt_ilosclinii = PoliczLiczbeLinii(plikkeystxt_path);
                plikstringstxt_ilosclinii = PoliczLiczbeLinii(plikstringstxt_path);
                //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);

                tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP = plikkeystxt_ilosclinii;

                if (wl_pasekpostepu == true)
                {
                    InicjalizacjaPaskaPostepu(Convert.ToInt32(plikkeystxt_ilosclinii));
                }

                if (plikkeystxt_ilosclinii == plikstringstxt_ilosclinii)
                {


                    if (Directory.Exists(nazwafolderutmp) == false)
                    {
                        Directory.CreateDirectory(nazwafolderutmp);
                    }

                    decimal maksymalna_ilosc_linii_dla_1_watku = Math.Ceiling(Convert.ToDecimal(plikkeystxt_ilosclinii) / Convert.ToDecimal(ilosc_watkow));

                    //Console.WriteLine("plikkeystxt_ilosclinii: " + plikkeystxt_ilosclinii);
                    //Console.WriteLine("plikstringstxt_ilosclinii: " + plikstringstxt_ilosclinii);
                    //Console.WriteLine("ilosc_watkow: " + ilosc_watkow);
                    //Console.WriteLine("maksymalna_ilosc_linii_dla_1_watku: " + maksymalna_ilosc_linii_dla_1_watku);


                    FileStream plikkeystxt_fs = new FileStream(plikkeystxt_path, FileMode.Open, FileAccess.Read);
                    FileStream plikstringstxt_fs = new FileStream(plikstringstxt_path, FileMode.Open, FileAccess.Read);

                    StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);
                    StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);

                    
                    if (wylacz_calkowitepokazywaniepostepow == true) { Console.WriteLine("Trwa przygotowywanie linii. Może to chwilę zająć. Proszę czekać..");}


                    while (plikkeystxt_sr.Peek() != -1)
                    {

                        string plikkeystxt_trescaktualnejlinii = plikkeystxt_sr.ReadLine();

                        plikkeystxt_trescilinii.Add(plikkeystxt_trescaktualnejlinii);

                    }


                    while (plikstringstxt_sr.Peek() != -1)
                    {

                        string plikstringstxt_trescaktualnejlinii = plikstringstxt_sr.ReadLine();

                        plikstringstxt_trescilinii.Add(plikstringstxt_trescaktualnejlinii);

                    }


                    List<string> listaplikowkeystxtTMP = new List<string>();
                    List<string> listaplikowstringstxtTMP = new List<string>();
                    List<string> listaplikowjsonTMP = new List<string>();


                    for (int lw = 0; lw < ilosc_watkow; lw++)
                    {
                        int numer_pliku = lw + 1;

                        int index_od = lw * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku);
                        int index_do = ((lw + 1) * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku)) - 1;

                        if (index_do > Convert.ToInt32(plikkeystxt_ilosclinii) - 1)
                        {
                            index_do = Convert.ToInt32(plikkeystxt_ilosclinii) - 1;
                        }

                        if (argumenty_czyuwzgledniac == true)
                        {
                            string[] plikkeystxt_path_split = plikkeystxt_path.Split(sc);
                            string plikkeystxt_nazwa = plikkeystxt_path_split[plikkeystxt_path_split.Length - 1];
                            
                            string[] plikstringstxt_path_split = plikstringstxt_path.Split(sc);
                            string plikstringstxt_nazwa = plikstringstxt_path_split[plikstringstxt_path_split.Length - 1];

                            
                            UtworzPlikTXT_TMP(plikkeystxt_nazwa + "_" + numer_pliku + ".tmp", plikkeystxt_trescilinii, index_od, index_do);
                            UtworzPlikTXT_TMP(plikstringstxt_nazwa + "_" + numer_pliku + ".tmp", plikstringstxt_trescilinii, index_od, index_do);

                            listaplikowkeystxtTMP.Add(plikkeystxt_nazwa + "_" + numer_pliku + ".tmp");
                            listaplikowstringstxtTMP.Add(plikstringstxt_nazwa + "_" + numer_pliku + ".tmp");
                            listaplikowjsonTMP.Add(plikkeystxt_nazwa.Replace(".keysTransifexCOM.txt", "") + "_" + numer_pliku + ".tmp");
                        }
                        else
                        {
                            UtworzPlikTXT_TMP(plikkeystxt_path + "_" + numer_pliku + ".tmp", plikkeystxt_trescilinii, index_od, index_do);
                            UtworzPlikTXT_TMP(plikstringstxt_path + "_" + numer_pliku + ".tmp", plikstringstxt_trescilinii, index_od, index_do);

                            listaplikowkeystxtTMP.Add(plikkeystxt_path + "_" + numer_pliku + ".tmp");
                            listaplikowstringstxtTMP.Add(plikstringstxt_path + "_" + numer_pliku + ".tmp");
                            listaplikowjsonTMP.Add(plikkeystxt_path.Replace(".keysTransifexCOM.txt", "") + "_" + numer_pliku + ".tmp");
                        }
                    }

                    tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP = listaplikowkeystxtTMP;
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP = listaplikowstringstxtTMP;
                    tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP = listaplikowjsonTMP;


                    plikkeystxt_sr.Close();
                    plikstringstxt_sr.Close();

                    plikkeystxt_fs.Close();
                    plikstringstxt_fs.Close();




                    //TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje("test1.json.keysTransifexCOM.txt_1.tmp", "test1.json.stringsTransifexCOM.txt_1.tmp");

                    Thread watek1_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek1);
                    Thread watek2_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek2);
                    Thread watek3_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek3);
                    Thread watek4_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek4);
                    Thread watek5_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek5);
                    Thread watek6_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek6);
                    Thread watek7_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek7);
                    Thread watek8_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek8);
                    Thread watek9_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek9);
                    Thread watek10_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek10);
                    Thread watek11_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek11);
                    Thread watek12_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek12);
                    Thread watek13_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek13);
                    Thread watek14_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek14);
                    Thread watek15_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek15);
                    Thread watek16_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek16);
                    Thread watek17_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek17);
                    Thread watek18_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek18);
                    Thread watek19_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek19);
                    Thread watek20_transifexCOMtxttoJSON = new Thread(TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek20);

                    watek1_transifexCOMtxttoJSON.Start();
                    watek2_transifexCOMtxttoJSON.Start();
                    watek3_transifexCOMtxttoJSON.Start();
                    watek4_transifexCOMtxttoJSON.Start();
                    watek5_transifexCOMtxttoJSON.Start();
                    watek6_transifexCOMtxttoJSON.Start();
                    watek7_transifexCOMtxttoJSON.Start();
                    watek8_transifexCOMtxttoJSON.Start();
                    watek9_transifexCOMtxttoJSON.Start();
                    watek10_transifexCOMtxttoJSON.Start();
                    watek11_transifexCOMtxttoJSON.Start();
                    watek12_transifexCOMtxttoJSON.Start();
                    watek13_transifexCOMtxttoJSON.Start();
                    watek14_transifexCOMtxttoJSON.Start();
                    watek15_transifexCOMtxttoJSON.Start();
                    watek16_transifexCOMtxttoJSON.Start();
                    watek17_transifexCOMtxttoJSON.Start();
                    watek18_transifexCOMtxttoJSON.Start();
                    watek19_transifexCOMtxttoJSON.Start();
                    watek20_transifexCOMtxttoJSON.Start();


                    watek1_transifexCOMtxttoJSON.Join();
                    watek2_transifexCOMtxttoJSON.Join();
                    watek3_transifexCOMtxttoJSON.Join();
                    watek4_transifexCOMtxttoJSON.Join();
                    watek5_transifexCOMtxttoJSON.Join();
                    watek6_transifexCOMtxttoJSON.Join();
                    watek7_transifexCOMtxttoJSON.Join();
                    watek8_transifexCOMtxttoJSON.Join();
                    watek9_transifexCOMtxttoJSON.Join();
                    watek10_transifexCOMtxttoJSON.Join();
                    watek11_transifexCOMtxttoJSON.Join();
                    watek12_transifexCOMtxttoJSON.Join();
                    watek13_transifexCOMtxttoJSON.Join();
                    watek14_transifexCOMtxttoJSON.Join();
                    watek15_transifexCOMtxttoJSON.Join();
                    watek16_transifexCOMtxttoJSON.Join();
                    watek17_transifexCOMtxttoJSON.Join();
                    watek18_transifexCOMtxttoJSON.Join();
                    watek19_transifexCOMtxttoJSON.Join();
                    watek20_transifexCOMtxttoJSON.Join();

                    //Sukces("!!!Zaraportowano zakończenie wszystkich wątków!!!");



                    string finalnyplikJSON_path = "";

                    if (argumenty_czyuwzgledniac == true)
                    {
                        finalnyplikJSON_path = g_args[6].Replace("-out=", "");
                    }
                    else
                    {
                        finalnyplikJSON_path = "NOWY_" + plikkeystxt_path.Replace(".keysTransifexCOM.txt", "");
                    }
                    
                    if (File.Exists(finalnyplikJSON_path) == true) { File.Delete(finalnyplikJSON_path); }


                    UtworzNaglowekJSON(nowyplikJSON_path);


                    FileStream finalnyplikJSON_fs = new FileStream(finalnyplikJSON_path, FileMode.Append, FileAccess.Write);

                    try //#1
                    {
                        StreamWriter finalnyplikJSON_sw = new StreamWriter(finalnyplikJSON_fs);


                        for (int lpj = 0; lpj < tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP.Count; lpj++)
                        {
                            FileStream plikjsonTMP_fs = new FileStream(nazwafolderutmp + sc + tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP[lpj], FileMode.Open, FileAccess.Read);

                            try //#2
                            {
                                StreamReader plikjsonTMP_sr = new StreamReader(plikjsonTMP_fs);

                                finalnyplikJSON_sw.Write(plikjsonTMP_sr.ReadToEnd());

                                plikjsonTMP_sr.Close();
                            }
                            catch (Exception Error)
                            {

                                string komunikat_obledzie;
                                komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #2 (for-lpj: " + lpj + ", Error: " + Error + ")!";

                                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                                {
                                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                                }

                                Blad(komunikat_obledzie);

                            }

                            plikjsonTMP_fs.Close();


                        }

                        finalnyplikJSON_sw.Close();



                    }
                    catch (Exception Error)
                    {

                        string komunikat_obledzie;
                        komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #1 (Error: " + Error + ")!";

                        if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                            makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                        }

                        Blad(komunikat_obledzie);

                    }


                    finalnyplikJSON_fs.Close();

                    bool stopkaJSON_rezultat = UtworzStopkeJSON(nowyplikJSON_path);

                    if (stopkaJSON_rezultat == true)
                    {
                        if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
                        {
                            if (File.Exists(plikstringstxt_path) == true) { File.Delete(plikstringstxt_path); }
                            if (File.Exists(cfg["domyslnaNazwaPlikustringsTransifexCOMTXT"].ToString()) == true) { File.Delete(cfg["domyslnaNazwaPlikustringsTransifexCOMTXT"].ToString()); }
                        }

                        makro_pomyslnezakonczenieoperacjinr2 = true;

                        string komunikat_osukcesie;
                        komunikat_osukcesie = $"Plik JSON zostal wygenerowany: \"{nowyplikJSON_path}\"";

                        if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                            makro_sukcesy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_osukcesie);

                        }

                        api_zapisywanatresczdarzenia = "Powodzenie operacji";
                        
                        Sukces(komunikat_osukcesie);

                    }

                }
                else
                {
                    string komunikat_obledzie;
                    komunikat_obledzie = "BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!";

                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }

                    Blad(komunikat_obledzie);

                }


            }
            else
            {
                string komunikat_obledzie;
                komunikat_obledzie = "BŁĄD: W folderze z programem nie istnieje przynajmniej jeden plik, których nazwy wskazano.";

                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                {
                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                }

                Blad(komunikat_obledzie);
                
            }
            
            //czyszczenie pamięci
            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP);
            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP);
            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP);
            


            if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1 && makro_pomyslnezakonczenieoperacjinr2 == true)
            {
                //czyszczenie pamięci dodatkowej jeśli makro jest aktywowane
                tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP = 0;
                tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii = 1;
                tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP.Clear();
                tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP.Clear();
                tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP.Clear();


                Makro_UruchomienieKolejnejOperacji();
            }
            else
            {
                string api_tokenzdarzenia = "";
                
                foreach (string? argument in g_args)
                {
                    if (argument.Contains("-event=") == true)
                    {
                        string wartosc_argumentu = argument.Replace("-event=", "");
                        if (wartosc_argumentu != "") { api_tokenzdarzenia = wartosc_argumentu; }
                    }
                }
                
                Koniec(api_tokenzdarzenia, api_zapisywanatresczdarzenia);

            }

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(string nazwaplikukeystxt, string nazwaplikustringstxt, bool ostatni_watek = false)
        {
            int wielkiznakPL_rozmiar = int.Parse(cfg["narzucRozmiarWProcentachDlaWielkichZnakowPL"].ToString());
            //string nazwaplikukeystxt;
            //string nazwaplikustringstxt;
            string nazwanowegoplikuJSON;
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const char separator = ';';


            if (File.Exists(nazwafolderutmp + sc + nazwaplikukeystxt) && File.Exists(nazwafolderutmp + sc + nazwaplikustringstxt))
            {
                nazwanowegoplikuJSON = nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");


                //Console.WriteLine("Nazwa nowego pliku JSON to: " + nazwanowegoplikuJSON);

                plikkeystxt_ilosclinii = PoliczLiczbeLinii(nazwafolderutmp + sc + nazwaplikukeystxt);
                plikstringstxt_ilosclinii = PoliczLiczbeLinii(nazwafolderutmp + sc + nazwaplikustringstxt);
                //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);



                if (plikkeystxt_ilosclinii == plikstringstxt_ilosclinii)
                {
                    bool bledywplikuJSON = false;
                    FileStream nowyplikJSON_fs = new FileStream(nazwafolderutmp + sc + nazwanowegoplikuJSON, FileMode.Append, FileAccess.Write);
                    FileStream plikkeystxt_fs = new FileStream(nazwafolderutmp + sc + nazwaplikukeystxt, FileMode.Open, FileAccess.Read);

                    try //#1
                    {
                        string plikkeystxt_trescaktualnejlinii;
                        string plikstringstxt_trescaktualnejlinii;

                        StreamWriter nowyplikJSON_sw = new StreamWriter(nowyplikJSON_fs);
                        StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);
                        
                        int postep_w_procentach_ostatniawartosccalkowita = -1;
                        int plikkeystxt_sr_nraktualnejlinii = 1;
                        while (plikkeystxt_sr.Peek() != -1)
                        {
                            plikkeystxt_trescaktualnejlinii = plikkeystxt_sr.ReadLine();
                            string[] plikkeystxt_wartoscilinii = plikkeystxt_trescaktualnejlinii.Split(new char[] { separator }); //skladnia: plikkeystxt_wartoscilinii[0:key||0<:vars]

                            //Console.WriteLine("Pobrano KEY   z linii " + plikkeystxt_sr_nraktualnejlinii + " o tresci: " + plikkeystxt_trescaktualnejlinii);

                            FileStream plikstringstxt_fs = new FileStream(nazwafolderutmp + sc + nazwaplikustringstxt, FileMode.Open, FileAccess.Read);

                            try //#2
                            {
                                StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);

                                int plikstringstxt_sr_nraktualnejlinii = 1;
                                while (plikstringstxt_sr.Peek() != -1)
                                {
                                    string nraktualnejliniiwplikuJSON = "";

                                    plikstringstxt_trescaktualnejlinii = plikstringstxt_sr.ReadLine();


                                    if (plikstringstxt_sr_nraktualnejlinii == plikkeystxt_sr_nraktualnejlinii)
                                    {
                                        string plikstringstxt_trescaktualnejliniipofiltracjitmp;

                                        string[] tmp1 = plikstringstxt_trescaktualnejlinii.Split(new char[] { '>' });

                                        if (tmp1.Length > 0)
                                        {
                                            plikstringstxt_trescaktualnejliniipofiltracjitmp = plikstringstxt_trescaktualnejlinii;

                                            //Console.WriteLine("[DEBUG] tmp1[0]==" + tmp1[0]);

                                            string tmp2 = tmp1[0].TrimStart().Remove(0, 1);

                                            nraktualnejliniiwplikuJSON = tmp2;

                                            //Console.WriteLine("[DEBUG] tmp2==" + tmp2);

                                            //plikstringstxt_trescaktualnejliniipofiltracjitmp = plikstringstxt_trescaktualnejliniipofiltracjitmp.Replace("<" + tmp2 + ">", "");

                                            string tmp3 = "<" + tmp2 + ">";
                                            int tmp4 = tmp3.Length;

                                            plikstringstxt_trescaktualnejliniipofiltracjitmp = plikstringstxt_trescaktualnejliniipofiltracjitmp.Remove(0, tmp4);
                                        }
                                        else
                                        {
                                            plikstringstxt_trescaktualnejliniipofiltracjitmp = plikstringstxt_trescaktualnejlinii;
                                        }



                                        string plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescaktualnejliniipofiltracjitmp;

                                        //Console.WriteLine("!!!: Liczba key+vars w linii nr. " + plikkeystxt_sr_nraktualnejlinii + ": " + plikkeystxt_wartoscilinii.Length);

                                        List<string> lista_zmiennych_linii = new List<string>();

                                        int plikstringstxt_trescuaktualnionalinii_iloscznakow = plikstringstxt_trescuaktualnionalinii.Length;

                                        if ((plikstringstxt_trescuaktualnionalinii_iloscznakow <= 25)
                                            && (
                                                    plikstringstxt_trescuaktualnionalinii.Contains("Ś") == true
                                                    ||
                                                    plikstringstxt_trescuaktualnionalinii.Contains("Ż") == true
                                                    ||
                                                    plikstringstxt_trescuaktualnionalinii.Contains("Ź") == true
                                                    ||
                                                    plikstringstxt_trescuaktualnionalinii.Contains("Ć") == true
                                                    ||
                                                    plikstringstxt_trescuaktualnionalinii.Contains("Ń") == true
                                               )
                                           )
                                        {
                                            if (plikstringstxt_trescuaktualnionalinii.Contains("<size=") == false && (wielkiznakPL_rozmiar != 100))
                                            {
                                                plikstringstxt_trescuaktualnionalinii = "<size=" + wielkiznakPL_rozmiar + "%>" + plikstringstxt_trescuaktualnionalinii + "</size>";
                                            }
                                        }
                                        else
                                        {
                                            if (wielkiznakPL_rozmiar != 100)
                                            {
                                                plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii
                                                    .Replace("Ś", "<size=" + wielkiznakPL_rozmiar + "%>Ś</size>")
                                                    .Replace("Ż", "<size=" + wielkiznakPL_rozmiar + "%>Ż</size>")
                                                    .Replace("Ź", "<size=" + wielkiznakPL_rozmiar + "%>Ź</size>")
                                                    .Replace("Ć", "<size=" + wielkiznakPL_rozmiar + "%>Ć</size>")
                                                    .Replace("Ń", "<size=" + wielkiznakPL_rozmiar + "%>Ń</size>");

                                            }
                                        }


                                        plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii

                                        .Replace("\"", "<bs_n1>")
                                        .Replace("<br>", "\\n")
                                        .Replace("<bs_n1>", "\\\"")
                                        .Replace("<bs_br>", "<br>");



                                        if (plikkeystxt_wartoscilinii.Length > 1)
                                        {

                                            for (int ivw = 1; ivw < plikkeystxt_wartoscilinii.Length; ivw++)
                                            {
                                                int ivwminus1 = ivw - 1;

                                                lista_zmiennych_linii.Add("<kl" + ivwminus1 + ">" + ";" + plikkeystxt_wartoscilinii[ivw]);

                                            }

                                        }

                                        //Console.WriteLine("lista_zmiennych_linii.Count: " + lista_zmiennych_linii.Count);

                                        for (int it1 = 0; it1 < lista_zmiennych_linii.Count; it1++)
                                        {
                                            plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii.Replace(lista_zmiennych_linii[it1].Split(new char[] { ';' })[0], lista_zmiennych_linii[it1].Split(new char[] { ';' })[1]);

                                            //Console.WriteLine("Sparsowano zmienna w linii nr. " + plikstringstxt_sr_nraktualnejlinii + ": " + lista_zmiennych_linii[it1].Split(new char[] { ';' })[0] + "na " + lista_zmiennych_linii[it1].Split(new char[] { ';' })[1]);

                                        }


                                        //Console.WriteLine("MOMENT PRZED ZAPISEM: " + plikstringstxt_trescuaktualnionalinii);

                                        //Console.WriteLine("plikkeystxt_sr_nraktualnejlinii: " + plikkeystxt_sr_nraktualnejlinii);
                                        //Console.WriteLine("plikkeystxt_ilosclinii: " + plikkeystxt_ilosclinii);

                                        if (plikstringstxt_trescuaktualnionalinii == " ") { plikstringstxt_trescuaktualnionalinii = ""; }

                                        int rzeczywistynumer_aktualnejlinii = Convert.ToInt32(nraktualnejliniiwplikuJSON) - 3;

                                        if (tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor == 1)
                                        {
                                            plikstringstxt_trescuaktualnionalinii = "<size=65%>[" + rzeczywistynumer_aktualnejlinii + "]</size>" + plikstringstxt_trescuaktualnionalinii;
                                        }
                                        else if (tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumerylinii_wybor == 2)
                                        {
                                            plikstringstxt_trescuaktualnionalinii = "<size=65%>[" + rzeczywistynumer_aktualnejlinii + "]</size>" + "<size=50%>" + "<" + nraktualnejliniiwplikuJSON + ">" + "</size>" + plikstringstxt_trescuaktualnionalinii;
                                        }

                                        if (tmpdlawatkow_2xtransifexCOMtxttoJSON_czydolaczycnumeryporzadkowe_wybor == 1)
                                        {
                                            plikstringstxt_trescuaktualnionalinii = "<size=30%>" + tmpdlawatkow_2xtransifexCOMtxttoJSON_numerporzadkowy + ":" + "</size>" + plikstringstxt_trescuaktualnionalinii;
                                        }

                                        if (plikstringstxt_sr_nraktualnejlinii != plikkeystxt_ilosclinii)
                                        {
                                            nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\",");
                                        }
                                        else
                                        {

                                            if (ostatni_watek == false)
                                            {
                                                nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\",");
                                            }
                                            else
                                            {
                                                nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\"");
                                            }
                                        }


                                    }




                                    plikstringstxt_sr_nraktualnejlinii++;
                                }


                                plikstringstxt_sr.Close();

                            }
                            catch (Exception Error)
                            {

                                string komunikat_obledzie;
                                komunikat_obledzie = "BLAD: Wystapil nieoczekiwany blad w dostepie do plikow. (TRY #2) (Error: " + Error + ")";

                                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                                {
                                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                                }

                                Blad(komunikat_obledzie);

                            }

                            if (wylacz_calkowitepokazywaniepostepow == false)
                            {
                                if (wl_pasekpostepu == false)
                                {

                                    int postep_w_procentach = int.Parse(PoliczPostepWProcentach(tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii, tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP));
                                
                                    string komunikat_aktualnypostep = $"Trwa przygotowywanie linii. Aktualny postęp: {postep_w_procentach}% z {tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP}";

                                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                                    {
                                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;
                                        komunikat_aktualnypostep = "[Operacja makra: " + makro_numeroperacjiwkolejnosci + "/" + makro_operacje_lista.Count + "] " + komunikat_aktualnypostep;
                                    }

                                    if (postep_w_procentach_ostatniawartosccalkowita != postep_w_procentach)
                                    {
                                        //Console.WriteLine($"{postep_w_procentach_ostatniawartosccalkowita} != {postep_w_procentach}");
                                
                                        Console.WriteLine(komunikat_aktualnypostep);
                                        postep_w_procentach_ostatniawartosccalkowita = postep_w_procentach;

                                    }
                                }
                                else if (wl_pasekpostepu == true)
                                {
                                    pasek_postepu.Refresh(Convert.ToInt32(tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii), "Trwa przygotowywanie linii...");
                                }
                            }

                            tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii++;


                            plikkeystxt_sr_nraktualnejlinii++;

                            plikstringstxt_fs.Close();

                        }
                        plikkeystxt_sr.Close();
                        nowyplikJSON_sw.Close();




                    }
                    catch (Exception Error)
                    {
                        string komunikat_obledzie;
                        komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do plików. (TRY #1) (Error: " + Error + ")";

                        if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                            makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                        }

                        Blad(komunikat_obledzie);

                    }

                    plikkeystxt_fs.Close();
                    nowyplikJSON_fs.Close();

                    if (bledywplikuJSON == true && File.Exists(nazwanowegoplikuJSON))
                    {
                        File.Delete(nazwanowegoplikuJSON);

                        /*
                        //Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("bledywplikuJSON: true");
                        Console.ResetColor();
                        */
                    }
                    else
                    {
                        /*
                        //Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine("bledywplikuJSON: false");
                        Console.ResetColor();
                        */
                    }





                }
                else
                {

                    string komunikat_obledzie;
                    komunikat_obledzie = "BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!";

                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }

                    Blad(komunikat_obledzie);


                }


            }
            else
            {

                string komunikat_obledzie;
                komunikat_obledzie = "BŁĄD: Brak wskazanych plików.";

                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                {
                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                }

                Blad(komunikat_obledzie);

            }



        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek1()
        {
            int index = 0;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek2()
        {
            int index = 1;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek3()
        {
            int index = 2;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek4()
        {
            int index = 3;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek5()
        {
            int index = 4;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek6()
        {
            int index = 5;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek7()
        {
            int index = 6;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek8()
        {
            int index = 7;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek9()
        {
            int index = 8;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek10()
        {
            int index = 9;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek11()
        {
            int index = 10;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek12()
        {
            int index = 11;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek13()
        {
            int index = 12;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek14()
        {
            int index = 13;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek15()
        {
            int index = 14;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek16()
        {
            int index = 15;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek17()
        {
            int index = 16;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek18()
        {
            int index = 17;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek19()
        {
            int index = 18;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index]);

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_watek20()
        {
            int index = 19;
            TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP[index], tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP[index], true);

        }

        public static void UsuwanieZnakowPL()
        {
            string nazwaplikuzrodlowego;

            if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
            {
                nazwaplikuzrodlowego = cfg["domyslnaNazwaWygenerowanegoPlikuLokalizacjiJSON"].ToString();
            }
            else
            {
                Console.Write("Podaj nazwę pliku: ");
                nazwaplikuzrodlowego = Console.ReadLine();
            }

            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuzrodlowego);
            if (File.Exists(nazwaplikuzrodlowego))
            {
                uint plikzrodlowy_liczbalinii = PoliczLiczbeLinii(nazwaplikuzrodlowego);

                if (wl_pasekpostepu == true)
                {
                    InicjalizacjaPaskaPostepu(Convert.ToInt32(plikzrodlowy_liczbalinii));
                }

                if (plikzrodlowy_liczbalinii > 0)
                {
                    FileStream plikzrodlowy_fs = new FileStream(nazwaplikuzrodlowego, FileMode.Open, FileAccess.Read);
                    FileStream nowyplik_fs = new FileStream("BezZnakowPL_" + nazwaplikuzrodlowego, FileMode.Create, FileAccess.Write);

                    try
                    {
                        StreamReader plikzrodlowy_sr = new StreamReader(plikzrodlowy_fs);
                        StreamWriter nowyplik_sw = new StreamWriter(nowyplik_fs);

                        int plikzrodlowy_numerlinii = 1;
                        while (plikzrodlowy_sr.Peek() != -1)
                        {
                            string uaktualniona_linia = plikzrodlowy_sr.ReadLine()

                            .Replace('ę', 'e')
                            .Replace('Ę', 'E')
                            .Replace('ó', 'o')
                            .Replace('Ó', 'O')
                            .Replace('ą', 'a')
                            .Replace('Ą', 'A')
                            .Replace('ś', 's')
                            .Replace('Ś', 'S')
                            .Replace('ł', 'l')
                            .Replace('Ł', 'L')
                            .Replace('ż', 'z')
                            .Replace('Ż', 'Z')
                            .Replace('ź', 'z')
                            .Replace('Ź', 'Z')
                            .Replace('ć', 'c')
                            .Replace('Ć', 'C')
                            .Replace('ń', 'n')
                            .Replace('Ń', 'N');


                            if (plikzrodlowy_numerlinii != plikzrodlowy_liczbalinii)
                            {
                                nowyplik_sw.WriteLine(uaktualniona_linia);
                            }
                            else
                            {
                                nowyplik_sw.Write(uaktualniona_linia);
                            }

                            if (wl_pasekpostepu == false)
                            {
                                Console.WriteLine("Trwa zapisywanie linii nr.: " + plikzrodlowy_numerlinii + "/" + plikzrodlowy_liczbalinii + " [" + PoliczPostepWProcentach(plikzrodlowy_numerlinii, plikzrodlowy_liczbalinii) + "%]");
                            }
                            else if (wl_pasekpostepu == true)
                            {
                                pasek_postepu.Refresh(plikzrodlowy_numerlinii, "Trwa zapisywanie linii...");
                            }

                            plikzrodlowy_numerlinii++;

                        }

                        nowyplik_sw.Close();
                        plikzrodlowy_sr.Close();

                        //Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine("Utworzono nowy plik nie zawierajacy polskich znakow: " + "BezZnakowPL_" + nazwaplikuzrodlowego);
                        Console.ResetColor();

                    }
                    catch
                    {
                        //Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine("BLAD: Wystapil nieoczekiwany blad w dostepie do plikow.");
                        Console.ResetColor();
                    }

                    nowyplik_fs.Close();
                    plikzrodlowy_fs.Close();

                }
                else
                {
                    //Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("BLAD: Wystapil problem ze zliczeniem linii w podanym pliku!");
                    Console.ResetColor();
                }

            }
            else
            {
                //Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BLAD: Nie istnieje plik o nazwie \"" + nazwaplikuzrodlowego + "\" w folderze z programem.");
                Console.ResetColor();
            }

            Koniec();

        }

        public static void WdrazanieAktualizacji_WeryfikacjaPlikowMetadanych(bool zbiorcze_wdrazanie_aktualizacji = false)
        {
            string folderupdate = folderglownyprogramu + sc + "update";
            string przyrostek_UpdateLocStruct = ".UpdateLocStruct.json";
            string przyrostek_UpdateLog = ".UpdateLog.json";
            string przyrostek_UpdateSchema = ".UpdateSchema.json";

            if (Directory.Exists(folderupdate) == true)
            {
                List<string> istniejacenazwyplikowmetadanych = PobierzNazwyPlikowJSONzFolderu("update");

                List<string> lista_oznaczen_aktualizacji = new List<string>();

                int np = 1;
                for (int i = 0; i < istniejacenazwyplikowmetadanych.Count; i++)
                {
                    //Console.WriteLine("Nazwa pliku metadanych: " + istniejacenazwyplikowmetadanych[i]);

                    if (istniejacenazwyplikowmetadanych[i].Contains(przyrostek_UpdateSchema) == true)
                    {
                        string oznaczenie_aktualizacji = istniejacenazwyplikowmetadanych[i].Split(new string[] { ".Update" }, StringSplitOptions.None)[0];

                        //Console.WriteLine("Oznaczenie aktualizacji: " + oznaczenie_aktualizacji);

                        if (File.Exists(folderupdate + sc + oznaczenie_aktualizacji + przyrostek_UpdateLocStruct) == true
                            && File.Exists(folderupdate + sc + oznaczenie_aktualizacji + przyrostek_UpdateLog) == true
                            && File.Exists(folderupdate + sc + oznaczenie_aktualizacji + przyrostek_UpdateSchema) == true)
                        {
                            lista_oznaczen_aktualizacji.Add(oznaczenie_aktualizacji);

                            if (zbiorcze_wdrazanie_aktualizacji == false)
                            {
                                Console.WriteLine(np + ". " + oznaczenie_aktualizacji.Replace("-", "->"));
                            }

                            np++;
                        }

                    }

                }

                if (istniejacenazwyplikowmetadanych.Count > 0 && lista_oznaczen_aktualizacji.Count > 0)
                {
                    string numer_pozycji_string = "";

                    if (zbiorcze_wdrazanie_aktualizacji == false)
                    {
                        Console.Write("Wpisz numer pozycji aktualizacji, którą chcesz wdrożyć: ");

                        if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                        {
                            makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                            numer_pozycji_string = makro_operacje_lista[makro_aktualny_indeks_listy];
                            Console.WriteLine(makro_operacje_lista[makro_aktualny_indeks_listy]);
                        }
                        /*
                        else if (argumenty_czyuwzgledniac == true)
                        {
                            numer_pozycji_string = g_args[1].Replace("-np=", "");
                        }
                        */
                        else
                        {
                            numer_pozycji_string = Console.ReadLine();
                        }
                    }
                    else if (zbiorcze_wdrazanie_aktualizacji == true)
                    {
                        numer_pozycji_string = lista_oznaczen_aktualizacji.Count().ToString();
                    }


                    if (CzyParsowanieINTUdane(numer_pozycji_string))
                    {
                        int indeks_oznaczeniaaktualizacji = (int.Parse(numer_pozycji_string)) - 1;

                        if ((indeks_oznaczeniaaktualizacji >= 0) && (lista_oznaczen_aktualizacji.Count - 1 >= indeks_oznaczeniaaktualizacji))
                        {

                            string[] tmp2_loa = lista_oznaczen_aktualizacji[indeks_oznaczeniaaktualizacji].Split(new char[] { '_' });

                            if (tmp2_loa.Length >= 2)
                            {
                                string numerporzadkowy_aktualizacji = tmp2_loa[0];
                                string oznaczenie_aktualizacji = tmp2_loa[1];

                                if (zbiorcze_wdrazanie_aktualizacji == false)
                                {
                                    WdrazanieAktualizacji_Wielowatkowe(numerporzadkowy_aktualizacji, oznaczenie_aktualizacji);
                                }
                                else if (zbiorcze_wdrazanie_aktualizacji == true)
                                {
                                    //Console.WriteLine("[DEBUG] numer_pozycji_string == " + numer_pozycji_string);

                                    ZbiorczeWdrazanieAktualizacji_Wielowatkowe(lista_oznaczen_aktualizacji, numerporzadkowy_aktualizacji, oznaczenie_aktualizacji);
                                }
                            }
                            else
                            {
                                Blad("Wykryto przynajmniej jedną nieprawidłowość w nazwach plików metadanych aktualizacji.");

                                Koniec();
                            }

                        }
                        else
                        {
                            Blad("Podano błędny numer pozycji. (#2)");

                            Koniec();
                        }

                    }
                    else
                    {
                        Blad("Podano błędny numer pozycji aktualizacji. (#1)");

                        Koniec();
                    }


                }
                else
                {
                    Blad("Folder \"" + folderupdate + sc +"\" nie zawiera kompletnych metadanych o aktualizacjach.");

                    Koniec();

                }
            }
            else
            {
                Blad("Nie istnieje folder \"" + folderupdate + sc + " zawierający kompletne metadane o aktualizacjach.");

                Koniec();

            }

        }

        public static void WdrazanieAktualizacji_Wielowatkowe(string numerporzadkowy_aktualizacji, string oznaczenie_aktualizacji) //numerporzadkowy_aktualizacji np: #2 oznaczenie_aktualizacji np: 1.0.1c-1.1.7c
        /* 
         * WYMAGA PLIKÓW METADANYCH AKTUALIZACJI WYGENEROWANYCH W pwrpl-tools DO DZIAŁANIA. PLIKI TE MUSZĄ ZOSTAĆ UMIESZCZONE W FOLDERZE "pwrpl-converter/update/":
         */
        {
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - POCZĄTEK */
            if (Directory.Exists(nazwafolderutmp) == true)
            {
                Directory.Delete(nazwafolderutmp, true);
            }
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - KONIEC */


            const int ilosc_watkow = 20;

            string[] oa = oznaczenie_aktualizacji.Split(new char[] { '-' });
            string numer_starej_wersji = oa[0];
            string numer_nowej_wersji = oa[1];

            string folderupdate = folderglownyprogramu + sc + "update";

            if (Directory.Exists(folderupdate) == true)
            {

                string plikUpdateSchemaJSON_nazwa;
                string plikUpdateLocStructJSON_nazwa;
                string pliklokalizacjistarejwersji_nazwa;
                string pliklokalizacjizaktualizacjadonowejwersji_nazwa;

                string NOWYpliklokalizacjipoaktualizacji_nazwa = "NOWY_plPL-" + numer_nowej_wersji + ".json";

                if (File.Exists(NOWYpliklokalizacjipoaktualizacji_nazwa) == true) { File.Delete(NOWYpliklokalizacjipoaktualizacji_nazwa); }

                if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
                {
                    pliklokalizacjistarejwersji_nazwa = "NOWY_plPL-" + numer_starej_wersji + ".json";
                    pliklokalizacjizaktualizacjadonowejwersji_nazwa = "NOWY_plPL-update-" + oznaczenie_aktualizacji + ".json";
                }
                else
                {


                    Console.Write("Podaj nazwę pliku json lokalizacji w wersji " + numer_starej_wersji + ": ");
                    pliklokalizacjistarejwersji_nazwa = Console.ReadLine();
                    Console.Write("Podaj nazwę pliku json, który zawiera aktualizacje do wersji " + numer_nowej_wersji + ": ");
                    pliklokalizacjizaktualizacjadonowejwersji_nazwa = Console.ReadLine();

                }

                plikUpdateSchemaJSON_nazwa = numerporzadkowy_aktualizacji + "_" + oznaczenie_aktualizacji + ".UpdateSchema.json";
                plikUpdateLocStructJSON_nazwa = numerporzadkowy_aktualizacji + "_" + oznaczenie_aktualizacji + ".UpdateLocStruct.json";

                Console.WriteLine("Podano nazwę pliku .UpdateSchema.json dla aktualizacji " + oznaczenie_aktualizacji + ": " + plikUpdateSchemaJSON_nazwa);
                Console.WriteLine("Podano nazwę pliku .UpdateLocStruct.json dla aktualizacji " + oznaczenie_aktualizacji + ": " + plikUpdateLocStructJSON_nazwa);
                Console.WriteLine("Podano nazwę pliku json lokalizacji w wersji " + numer_starej_wersji + ": " + pliklokalizacjistarejwersji_nazwa);
                Console.WriteLine("Podano nazwę pliku json, który zawiera aktualizacje do wersji " + numer_nowej_wersji + ": " + pliklokalizacjizaktualizacjadonowejwersji_nazwa);

                if ((File.Exists(folderupdate + sc + plikUpdateSchemaJSON_nazwa) == true)
                   && (File.Exists(folderupdate + sc + plikUpdateLocStructJSON_nazwa) == true)
                   && (File.Exists(pliklokalizacjistarejwersji_nazwa) == true)
                   && (File.Exists(pliklokalizacjizaktualizacjadonowejwersji_nazwa) == true))
                {


                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji = oznaczenie_aktualizacji;

                    dynamic[] _Schemat_tablicalistdanych = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(folderupdate + sc + plikUpdateSchemaJSON_nazwa);
                    dynamic[] _StrukturaLokalizacji_tablicalistdanych = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(folderupdate + sc + plikUpdateLocStructJSON_nazwa);
                    dynamic[] _PlikLokalizacjiStarejWersji_tablicalistdanych = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(pliklokalizacjistarejwersji_nazwa);
                    dynamic[] _PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(pliklokalizacjizaktualizacjadonowejwersji_nazwa);

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_tablicalistdanych = _Schemat_tablicalistdanych;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_tablicalistdanych = _StrukturaLokalizacji_tablicalistdanych;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_tablicalistdanych = _PlikLokalizacjiStarejWersji_tablicalistdanych;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych = _PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych;

                    List<dynamic> _Schemat_listakluczy = _Schemat_tablicalistdanych[0];
                    List<dynamic> _StrukturaLokalizacji_listakluczy = _StrukturaLokalizacji_tablicalistdanych[0];
                    List<dynamic> _PlikLokalizacjiStarejWersji_listakluczy = _PlikLokalizacjiStarejWersji_tablicalistdanych[0];
                    List<dynamic> _PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy = _PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych[0];

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_listakluczy = _Schemat_listakluczy;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_listakluczy = _StrukturaLokalizacji_listakluczy;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listakluczy = _PlikLokalizacjiStarejWersji_listakluczy;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy = _PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy;

                    //List<List<dynamic>> _Schemat_listastringow = _Schemat_tablicalistdanych[1]; //lista stringów w pliku UpdateSchema.json jest zawsze pusta
                    List<List<dynamic>> _StrukturaLokalizacji = _StrukturaLokalizacji_tablicalistdanych[1];
                    List<List<dynamic>> _PlikLokalizacjiStarejWersji_listastringow = _PlikLokalizacjiStarejWersji_tablicalistdanych[1];
                    List<List<dynamic>> _PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow = _PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych[1];

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji = _StrukturaLokalizacji;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listastringow = _PlikLokalizacjiStarejWersji_listastringow;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow = _PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow;

                    //Console.WriteLine("_Schemat_listakluczy[0]:" + _Schemat_listakluczy[0].ToString());
                    //Console.WriteLine("_Schemat_listastringow[0][0]:" + _Schemat_listastringow[0][0].ToString()); //out of index
                    //Console.WriteLine("_Schemat_listakluczy[1]:" + _Schemat_listakluczy[1].ToString());
                    //Console.WriteLine("_Schemat_listastringow[1][0]:" + _Schemat_listastringow[1][0].ToString()); //out of index

                    int plikUpdateSchemaJSON_iloscwszystkichkluczy = _Schemat_listakluczy.Count;

                    if (wl_pasekpostepu == true)
                    {
                        InicjalizacjaPaskaPostepu(plikUpdateSchemaJSON_iloscwszystkichkluczy);
                    }

                    decimal maksymalna_ilosc_linii_dla_1_watku = Math.Ceiling(Convert.ToDecimal(plikUpdateSchemaJSON_iloscwszystkichkluczy) / Convert.ToDecimal(ilosc_watkow));

                    //Console.WriteLine("plikUpdateSchemaJSON_iloscwszystkichkluczy: " + plikUpdateSchemaJSON_iloscwszystkichkluczy);
                    //Console.WriteLine("ilosc_watkow: " + ilosc_watkow);
                    //Console.WriteLine("maksymalna_ilosc_linii_dla_1_watku: " + maksymalna_ilosc_linii_dla_1_watku);

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateSchemaJSONTMP = plikUpdateSchemaJSON_iloscwszystkichkluczy;


                    //bool t = CzyIstniejeDanyKluczWLiscieKluczy(_Schemat_listakluczy, "f0e56983-7bef-4f82-b95a-1c2e871f1319");
                    //Console.WriteLine("t: " + t);


                    List<string> listaplikowjsonTMP = new List<string>();
                    List<int> listazakresuindeksow_od = new List<int>();
                    List<int> listazakresuindeksow_do = new List<int>();


                    for (int lw = 0; lw < ilosc_watkow; lw++)
                    {
                        int numer_pliku = lw + 1;

                        int index_od = lw * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku);
                        int index_do = ((lw + 1) * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku)) - 1;

                        if (index_do > Convert.ToInt32(plikUpdateSchemaJSON_iloscwszystkichkluczy) - 1)
                        {
                            index_do = Convert.ToInt32(plikUpdateSchemaJSON_iloscwszystkichkluczy) - 1;
                        }

                        listaplikowjsonTMP.Add(NOWYpliklokalizacjipoaktualizacji_nazwa + "_" + numer_pliku + ".tmp");
                        listazakresuindeksow_od.Add(index_od);
                        listazakresuindeksow_do.Add(index_do);
                    }

                    /*
                    for (int test1 = 0; test1 < listazakresuindeksow_od.Count; test1++)
                    {
                        Console.WriteLine("test1[" + test1 + "] (zakres od): " + listazakresuindeksow_od[test1]);
                    }
                    for (int test2 = 0; test2 < listazakresuindeksow_do.Count; test2++)
                    {
                        Console.WriteLine("test2[" + test2 + "] (zakres do): " + listazakresuindeksow_do[test2]);
                    }
                    */

                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP = listaplikowjsonTMP;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD = listazakresuindeksow_od;
                    tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO = listazakresuindeksow_do;



                    Thread watek1 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek1);
                    Thread watek2 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek2);
                    Thread watek3 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek3);
                    Thread watek4 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek4);
                    Thread watek5 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek5);
                    Thread watek6 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek6);
                    Thread watek7 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek7);
                    Thread watek8 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek8);
                    Thread watek9 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek9);
                    Thread watek10 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek10);
                    Thread watek11 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek11);
                    Thread watek12 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek12);
                    Thread watek13 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek13);
                    Thread watek14 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek14);
                    Thread watek15 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek15);
                    Thread watek16 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek16);
                    Thread watek17 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek17);
                    Thread watek18 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek18);
                    Thread watek19 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek19);
                    Thread watek20 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek20);

                    watek1.Start();
                    watek2.Start();
                    watek3.Start();
                    watek4.Start();
                    watek5.Start();
                    watek6.Start();
                    watek7.Start();
                    watek8.Start();
                    watek9.Start();
                    watek10.Start();
                    watek11.Start();
                    watek12.Start();
                    watek13.Start();
                    watek14.Start();
                    watek15.Start();
                    watek16.Start();
                    watek17.Start();
                    watek18.Start();
                    watek19.Start();
                    watek20.Start();


                    watek1.Join();
                    watek2.Join();
                    watek3.Join();
                    watek4.Join();
                    watek5.Join();
                    watek6.Join();
                    watek7.Join();
                    watek8.Join();
                    watek9.Join();
                    watek10.Join();
                    watek11.Join();
                    watek12.Join();
                    watek13.Join();
                    watek14.Join();
                    watek15.Join();
                    watek16.Join();
                    watek17.Join();
                    watek18.Join();
                    watek19.Join();
                    watek20.Join();

                    //Sukces("!!!Zaraportowano zakończenie wszystkich wątków!!!");



                    string nazwafinalnegoplikuJSON = NOWYpliklokalizacjipoaktualizacji_nazwa;

                    if (File.Exists(nazwafinalnegoplikuJSON) == true) { File.Delete(nazwafinalnegoplikuJSON); }


                    UtworzNaglowekJSON(nazwafinalnegoplikuJSON);


                    FileStream finalnyplikJSON_fs = new FileStream(nazwafinalnegoplikuJSON, FileMode.Append, FileAccess.Write);

                    try //#1
                    {
                        StreamWriter finalnyplikJSON_sw = new StreamWriter(finalnyplikJSON_fs);


                        for (int lpj2 = 0; lpj2 < tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP.Count; lpj2++)
                        {
                            FileStream plikjsonTMP_fs = new FileStream(nazwafolderutmp + sc + tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[lpj2], FileMode.Open, FileAccess.Read);

                            try //#2
                            {
                                StreamReader plikjsonTMP_sr = new StreamReader(plikjsonTMP_fs);

                                finalnyplikJSON_sw.Write(plikjsonTMP_sr.ReadToEnd());

                                plikjsonTMP_sr.Close();
                            }
                            catch (Exception Error)
                            {

                                string komunikat_obledzie;
                                komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #2 (for-lpj2: " + lpj2 + ", Error: " + Error + ")!";

                                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                                {
                                    int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                                    makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                                }

                            }

                            plikjsonTMP_fs.Close();


                        }

                        finalnyplikJSON_sw.Close();



                    }
                    catch (Exception Error)
                    {

                        string komunikat_obledzie;
                        komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #1 (Error: " + Error + ")!";

                        if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                            makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                        }

                    }


                    finalnyplikJSON_fs.Close();

                    bool stopkaJSON_rezultat = UtworzStopkeJSON(nazwafinalnegoplikuJSON);

                    if (stopkaJSON_rezultat == true)
                    {
                        makro_pomyslnezakonczenieoperacjinr100 = true;



                        //Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine("Plik JSON o nazwie \"" + nazwafinalnegoplikuJSON + "\" zostal wygenerowany.");
                        Console.ResetColor();

                    }




                }
                else
                {
                    Blad("Nie istnieje przynajmniej jeden z podanych plików.");
                }

            }
            else
            {
                Blad("Nie istnieje folder \"" + folderupdate + "\" zawierający metadane o aktualizacji.");
            }

            //czyszczenie pamięci
            UsunPlikiTMP(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP);



            if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1 && makro_pomyslnezakonczenieoperacjinr100 == true)
            {
                //czyszczenie pamięci dodatkowej jeśli makro jest aktywne
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii = 1;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji = "";
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_tablicalistdanych = null;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_tablicalistdanych = null;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_tablicalistdanych = null;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych = null;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_listakluczy.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji_listakluczy.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listakluczy.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__StrukturaLokalizacji.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listastringow.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateSchemaJSONTMP = 0;
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD.Clear();
                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO.Clear();


                Makro_UruchomienieKolejnejOperacji();
            }
            else
            {

                Koniec();

            }



        }

        public static void WdrazanieAktualizacji_Wielowatkowe_Operacje(string nazwaplikuJSONTMP, int index_klucza_OD, int index_klucza_DO, bool ostatni_watek = false)
        {

            if (Directory.Exists(nazwafolderutmp) == false)
            {
                Directory.CreateDirectory(nazwafolderutmp);
            }

            if (File.Exists(nazwafolderutmp + sc + nazwaplikuJSONTMP))
            {
                File.Delete(nazwafolderutmp + sc + nazwaplikuJSONTMP);
            }


            List<string> lista_zaktualizowanychlinii_tmp = new List<string>();

            for (int i1 = index_klucza_OD; i1 <= index_klucza_DO; i1++)
            {
                int nr_ostatniej_linii = tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateSchemaJSONTMP - 5;

                if (wl_pasekpostepu == false)
                {

                    string komunikat_aktualnypostep = "Trwa wdrażanie aktualizacji " + tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji.Replace("-", "->") + " do linii nr.: " + tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii + "/" + nr_ostatniej_linii + " [" + PoliczPostepWProcentach(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii, nr_ostatniej_linii) + "%]";

                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;
                        komunikat_aktualnypostep = "[Operacja makra: " + makro_numeroperacjiwkolejnosci + "/" + makro_operacje_lista.Count + "] " + komunikat_aktualnypostep;
                    }

                    Console.WriteLine(komunikat_aktualnypostep);

                }
                else if (wl_pasekpostepu == true)
                {
                    pasek_postepu.Refresh(Convert.ToInt32(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii), "Trwa wdrażanie aktualizacji...");
                }

                    string _Schemat_aktualnyklucz = tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__Schemat_listakluczy[i1].ToString();

                if (_Schemat_aktualnyklucz != "$id" && _Schemat_aktualnyklucz != "strings")
                {

                    //sprci1 - sprawdzenie czy dany klucz znajduje się w pliku lokalizacji z aktualizacją do nowej wersji
                    //sprci2 - sprawdzenie czy dany klicz znajduje się w pliku lokalizacji starej wersji

                    bool sprci1 = CzyIstniejeDanyKluczWLiscieKluczy(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listakluczy, _Schemat_aktualnyklucz);

                    if (sprci1 == false)
                    {
                        bool sprci2 = CzyIstniejeDanyKluczWLiscieKluczy(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listakluczy, _Schemat_aktualnyklucz);

                        if (sprci2 == true)
                        {
                            int index = PobierzNumerIndeksuZListyKluczyIStringow(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_tablicalistdanych, _Schemat_aktualnyklucz);

                            string tresc_stringa = tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiStarejWersji_listastringow[index][0];

                            tresc_stringa = tresc_stringa
                                            .Replace("\n", "\\n")
                                            .Replace("\"", "\\\"")
                                            .Replace("\t", "\\t");

                            lista_zaktualizowanychlinii_tmp.Add(_Schemat_aktualnyklucz + "[[[---]]]" + tresc_stringa);


                        }
                        else
                        {
                            lista_zaktualizowanychlinii_tmp.Add(_Schemat_aktualnyklucz + "[[[---]]]" + "");



                        }

                    }
                    else
                    {
                        int index = PobierzNumerIndeksuZListyKluczyIStringow(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_tablicalistdanych, _Schemat_aktualnyklucz);

                        string tresc_stringa = tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe__PlikLokalizacjiZAktualizacjaDoNowejWersji_listastringow[index][0];

                        tresc_stringa = tresc_stringa
                                        .Replace("\n", "\\n")
                                        .Replace("\"", "\\\"")
                                        .Replace("\t", "\\t");

                        lista_zaktualizowanychlinii_tmp.Add(_Schemat_aktualnyklucz + "[[[---]]]" + tresc_stringa);



                    }

                }

                tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_numeraktualnejlinii++;

            }

            FileStream NOWYplikJSONTMPpoaktualizacji_fs = new FileStream("tmp" + sc + nazwaplikuJSONTMP, FileMode.Append, FileAccess.Write);
            StreamWriter NOWYplikJSONTMPpoaktualizacji_sw = new StreamWriter(NOWYplikJSONTMPpoaktualizacji_fs);


            for (int zzd1 = 0; zzd1 < lista_zaktualizowanychlinii_tmp.Count; zzd1++)
            {
                string[] dane = lista_zaktualizowanychlinii_tmp[zzd1].Split(new string[] { "[[[---]]]" }, StringSplitOptions.None);

                string _KLUCZ = dane[0];
                string _STRING = dane[1];

                NOWYplikJSONTMPpoaktualizacji_sw.Write("    \"" + _KLUCZ + "\": \"" + _STRING + "\"");

                if (zzd1 + 1 != lista_zaktualizowanychlinii_tmp.Count || ostatni_watek == false)
                {
                    NOWYplikJSONTMPpoaktualizacji_sw.Write(",");
                }

                NOWYplikJSONTMPpoaktualizacji_sw.Write("\n");

            }


            NOWYplikJSONTMPpoaktualizacji_sw.Close();
            NOWYplikJSONTMPpoaktualizacji_fs.Close();


            //Sukces("Plik JSON o nazwie \"" + nazwaplikuJSONTMP + "\" zostal wygenerowany.");



        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek1()
        {
            int index = 0;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek2()
        {
            int index = 1;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek3()
        {
            int index = 2;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek4()
        {
            int index = 3;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek5()
        {
            int index = 4;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek6()
        {
            int index = 5;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek7()
        {
            int index = 6;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek8()
        {
            int index = 7;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek9()
        {
            int index = 8;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek10()
        {
            int index = 9;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek11()
        {
            int index = 10;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek12()
        {
            int index = 11;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek13()
        {
            int index = 12;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek14()
        {
            int index = 13;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek15()
        {
            int index = 14;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek16()
        {
            int index = 15;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek17()
        {
            int index = 16;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek18()
        {
            int index = 17;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek19()
        {
            int index = 18;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void WdrazanieAktualizacji_Wielowatkowe_watek20()
        {
            int index = 19;
            WdrazanieAktualizacji_Wielowatkowe_Operacje(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index], true);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe(List<string> lista_oznaczen_aktualizacji, string numerporzadkowy_aktualizacji_docelowej, string oznaczenie_aktualizacji_docelowej)
        {
            string api_zapisywanatresczdarzenia = "Niepowodzenie operacji";
            
            const int ilosc_watkow = 20;

            tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__listawatkow_lista_rekordowplikudocelowego = new List<List<Rekord>>();

            string numerwersjidocelowej = oznaczenie_aktualizacji_docelowej.Split(new char[] { '-' })[1];

            tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_numerwersjidocelowej = numerwersjidocelowej;

            //Console.WriteLine("[DEBUG] Zbiorcze wdrażanie aktualizacji: " + cfg["wersjaGryPierwszegoProjektuZTransifexCOM"] + "----->" + numerwersjidocelowej);


            string folderupdate = folderglownyprogramu + sc + "update";
            string plikUpdateSchemaJSONwersjidocelowej_nazwa = numerporzadkowy_aktualizacji_docelowej + "_" + oznaczenie_aktualizacji_docelowej + ".UpdateSchema.json";

            List<string> lista_wymaganychplikowUPDATE = new List<string>();
            List<string> lista_brakujacychplikowUPDATE = new List<string>();

            string in_folder = "";
            if (argumenty_czyuwzgledniac == true)
            {
                in_folder = g_args[1].Replace("-in_folder=", "");
            }
            
            if (File.Exists(in_folder + "NOWY_plPL-" + cfg["wersjaGryPierwszegoProjektuZTransifexCOM"] + ".json") == true)
            {
                lista_wymaganychplikowUPDATE.Add(in_folder + "NOWY_plPL-" + cfg["wersjaGryPierwszegoProjektuZTransifexCOM"] + ".json");
            }
            else
            {
                lista_brakujacychplikowUPDATE.Add(in_folder + "NOWY_plPL-" + cfg["wersjaGryPierwszegoProjektuZTransifexCOM"] + ".json");
            }

            for (int i1 = 0; i1 < lista_oznaczen_aktualizacji.Count(); i1++)
            {
                string wymaganyplik_nazwa = in_folder + "NOWY_plPL-update-" + lista_oznaczen_aktualizacji[i1].Split(new char[] { '_' })[1] + ".json";
                
                if (File.Exists(wymaganyplik_nazwa) == true)
                {
                    lista_wymaganychplikowUPDATE.Add(wymaganyplik_nazwa);
                }
                else
                {
                    lista_brakujacychplikowUPDATE.Add(wymaganyplik_nazwa);
                }
            }


            if ((lista_wymaganychplikowUPDATE.Count() == lista_oznaczen_aktualizacji.Count() + 1) && lista_brakujacychplikowUPDATE.Count() == 0)
            {
                List<List<Rekord>> listaplikowUPDATE_listarekordow = new List<List<Rekord>>(); //listaplikowUPDATE_listarekordow[indeks_pliku][indeks_rekordu]

                for (int i2 = 0; i2 < lista_wymaganychplikowUPDATE.Count; i2++)
                {

                    List<Rekord> danezplikuJSON_listarekordow = new List<Rekord>();

                    dynamic[] danezplikuJSON_tablicalistdanych = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(lista_wymaganychplikowUPDATE[i2]);

                    List<dynamic> danezplikuJSON_listakluczy = danezplikuJSON_tablicalistdanych[0];
                    List<List<dynamic>> danezplikuJSON_listastringow = danezplikuJSON_tablicalistdanych[1];

                    for (int i2b = 0; i2b < danezplikuJSON_listakluczy.Count(); i2b++)
                    {

                        if (i2b != 0 && i2b != 1) //odfiltrowanie pierwszych dwóch rekordów zawierających słowa, wczytane z pliku JSON, takie jak: "$id", "string", "1"
                        {
                            for (int i2c = 0; i2c < danezplikuJSON_listastringow[i2b].Count(); i2c++)
                            {

                                int _ID = i2b + 2;
                                string _Plik = lista_wymaganychplikowUPDATE[i2].ToString();
                                string _Klucz = danezplikuJSON_listakluczy[i2b];
                                string _String = FiltrujString(danezplikuJSON_listastringow[i2b][i2c]);

                                //Console.WriteLine("[DEBUG] " + _ID + "|" + _Plik + "|" + _Klucz + "|" + _String);

                                danezplikuJSON_listarekordow.Add(new Rekord { ID = _ID, Plik = _Plik, Klucz = _Klucz, String = _String });

                            }

                        }


                    }

                    listaplikowUPDATE_listarekordow.Add(danezplikuJSON_listarekordow);

                }

                /*
                Console.WriteLine("[DEBUG] listaplikow_listarekordow.Count()==" + listaplikow_listarekordow.Count());
                for (int dbg1 = 0; dbg1 < listaplikow_listarekordow.Count(); dbg1++)
                {
                    Console.WriteLine("[DEBUG] listaplikow_listarekordow[" + dbg1 + "].Count()==" + listaplikow_listarekordow[dbg1].Count());
                }
                Console.WriteLine("[DEBUG] listaplikow_listarekordow[0][0] zawiera:\n" + listaplikow_listarekordow[0][0]);
                */

                tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_listaplikowUPDATE_listarekordow = listaplikowUPDATE_listarekordow;


                dynamic[] _SchematLokalizacjiWersjiDocelowej = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(folderupdate + sc + plikUpdateSchemaJSONwersjidocelowej_nazwa);
                List<dynamic> _SchematLokalizacjiWersjiDocelowej_listakluczy = _SchematLokalizacjiWersjiDocelowej[0];

                tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__SchematLokalizacjiWersjiDocelowej_listakluczy = _SchematLokalizacjiWersjiDocelowej_listakluczy;

                double nr_ostatniej_linii = _SchematLokalizacjiWersjiDocelowej_listakluczy.Count();


                decimal maksymalna_ilosc_linii_dla_1_watku = Math.Ceiling(Convert.ToDecimal(nr_ostatniej_linii) / Convert.ToDecimal(ilosc_watkow));

                List<int> listazakresuindeksow_od = new List<int>();
                List<int> listazakresuindeksow_do = new List<int>();


                for (int lw = 0; lw < ilosc_watkow; lw++)
                {
                    int index_od = lw * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku);
                    int index_do = ((lw + 1) * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku)) - 1;

                    if (index_do > Convert.ToInt32(nr_ostatniej_linii) - 1)
                    {
                        index_do = Convert.ToInt32(nr_ostatniej_linii) - 1;
                    }

                    listazakresuindeksow_od.Add(index_od);
                    listazakresuindeksow_do.Add(index_do);

                    //Console.WriteLine("[DEBUG] Zakres lw=" + lw + ": indeksy od " + index_od + " do " + index_do);
                }


                tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD = listazakresuindeksow_od;
                tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO = listazakresuindeksow_do;

                
                if (wylacz_calkowitepokazywaniepostepow == true) { Console.WriteLine($"Trwa zbiorcze wdrażanie aktualizacji {cfg["wersjaGryPierwszegoProjektuZTransifexCOM"]}----->{tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_numerwersjidocelowej}. Może to chwilę zająć. Proszę czekać...");}

                
                Thread watek1 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek1);
                Thread watek2 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek2);
                Thread watek3 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek3);
                Thread watek4 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek4);
                Thread watek5 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek5);
                Thread watek6 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek6);
                Thread watek7 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek7);
                Thread watek8 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek8);
                Thread watek9 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek9);
                Thread watek10 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek10);
                Thread watek11 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek11);
                Thread watek12 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek12);
                Thread watek13 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek13);
                Thread watek14 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek14);
                Thread watek15 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek15);
                Thread watek16 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek16);
                Thread watek17 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek17);
                Thread watek18 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek18);
                Thread watek19 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek19);
                Thread watek20 = new Thread(ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek20);

                watek1.Start();
                watek2.Start();
                watek3.Start();
                watek4.Start();
                watek5.Start();
                watek6.Start();
                watek7.Start();
                watek8.Start();
                watek9.Start();
                watek10.Start();
                watek11.Start();
                watek12.Start();
                watek13.Start();
                watek14.Start();
                watek15.Start();
                watek16.Start();
                watek17.Start();
                watek18.Start();
                watek19.Start();
                watek20.Start();


                watek1.Join();
                watek2.Join();
                watek3.Join();
                watek4.Join();
                watek5.Join();
                watek6.Join();
                watek7.Join();
                watek8.Join();
                watek9.Join();
                watek10.Join();
                watek11.Join();
                watek12.Join();
                watek13.Join();
                watek14.Join();
                watek15.Join();
                watek16.Join();
                watek17.Join();
                watek18.Join();
                watek19.Join();
                watek20.Join();

                //Sukces("!!!Zaraportowano zakończenie wszystkich wątków!!!");



                List<Rekord> lista_rekordowplikudocelowego = new List<Rekord>();
                for (int dww1 = 0; dww1 < tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__listawatkow_lista_rekordowplikudocelowego.Count(); dww1++)
                {
                    for (int dww2 = 0; dww2 < tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__listawatkow_lista_rekordowplikudocelowego[dww1].Count(); dww2++)
                    {
                        lista_rekordowplikudocelowego.Add(tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__listawatkow_lista_rekordowplikudocelowego[dww1][dww2]);
                    }

                }

                lista_rekordowplikudocelowego.Sort();

                string out_folder = "";
                
                if (argumenty_czyuwzgledniac == true) { out_folder = g_args[2].Replace("-out_folder=", ""); } 
                
                string finalnyplikJSON_path = out_folder + "NOWY_plPL-" + numerwersjidocelowej + ".json";

                if (File.Exists(finalnyplikJSON_path) == true) { File.Delete(finalnyplikJSON_path); }


                UtworzNaglowekJSON(finalnyplikJSON_path);

                

                FileStream finalnyplikJSON_fs = new FileStream(finalnyplikJSON_path, FileMode.Append, FileAccess.Write);

                try //#1
                {
                    StreamWriter finalnyplikJSON_sw = new StreamWriter(finalnyplikJSON_fs);

                    for (int zzd1 = 0; zzd1 < lista_rekordowplikudocelowego.Count; zzd1++)
                    {

                        string _KLUCZ = lista_rekordowplikudocelowego[zzd1].Klucz;
                        string _STRING = lista_rekordowplikudocelowego[zzd1].String;

                        finalnyplikJSON_sw.Write(/*"[DEBUG-ID: " + lista_rekordowplikudocelowego[zzd1].ID + "] " + */"    \"" + _KLUCZ + "\": \"" + _STRING + "\"");

                        if (zzd1 + 1 != lista_rekordowplikudocelowego.Count())
                        {
                            finalnyplikJSON_sw.Write(",");
                        }

                        finalnyplikJSON_sw.Write("\n");

                    }


                    finalnyplikJSON_sw.Close();



                }
                catch (Exception Error)
                {

                    string komunikat_obledzie;
                    komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #1 (Error: " + Error + ")!";

                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                    {
                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                    }

                }


                finalnyplikJSON_fs.Close();

                bool stopkaJSON_rezultat = UtworzStopkeJSON(finalnyplikJSON_path);

                if (stopkaJSON_rezultat == true)
                {
                    makro_pomyslnezakonczenieoperacjinr101 = true;
                    
                    api_zapisywanatresczdarzenia = "Powodzenie operacji";

                    Sukces($"Plik JSON zostal wygenerowany: \"{finalnyplikJSON_path}\"");

                }


                if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1 && makro_pomyslnezakonczenieoperacjinr101 == true)
                {
                    //czyszczenie pamięci dodatkowej jeśli makro jest aktywne
                    tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__listawatkow_lista_rekordowplikudocelowego.Clear();
                    tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_numerwersjidocelowej = null;
                    tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_listaplikowUPDATE_listarekordow.Clear();
                    tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__SchematLokalizacjiWersjiDocelowej_listakluczy.Clear();
                    tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD.Clear();
                    tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO.Clear();
                    tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_postep_aktualnalinia = 0;

                    Makro_UruchomienieKolejnejOperacji();
                }



            }
            else
            {
                for (int e1 = 0; e1 < lista_brakujacychplikowUPDATE.Count(); e1++)
                {
                    Blad("Brak wymaganego pliku o nazwie \"" + lista_brakujacychplikowUPDATE[e1] + "\".");

                }


            }

            string api_tokenzdarzenia = "";
                
            foreach (string? argument in g_args)
            {
                if (argument.Contains("-event=") == true)
                {
                    string wartosc_argumentu = argument.Replace("-event=", "");
                    if (wartosc_argumentu != "") { api_tokenzdarzenia = wartosc_argumentu; }
                }
            }
            
            
            Koniec(api_tokenzdarzenia, api_zapisywanatresczdarzenia);

            
        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(int indeks_watku, int zakres_indeksow_od, int zakres_indeksow_do, bool ostatni_watek = false)
        {
            List<Rekord> danywatek_lista_rekordowplikudocelowego = new List<Rekord>();

            int nr_ostatniej_linii = tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__SchematLokalizacjiWersjiDocelowej_listakluczy.Count();


            for (int i3 = zakres_indeksow_od; i3 <= zakres_indeksow_do; i3++)
            {
                int postep_w_procentach_ostatniawartosccalkowita = -1;
                double numeraktualnejlinii = i3 + 1;
                
                for (int i3b = tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_listaplikowUPDATE_listarekordow.Count() - 1; i3b >= 0; i3b--)
                {
                    //Console.WriteLine("[DEBUG] Szukam klucza: " + tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__SchematLokalizacjiWersjiDocelowej_listakluczy[i3]);

                    List<Rekord> lista_znalezionekluczewdanympliku = tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_listaplikowUPDATE_listarekordow[i3b].FindAll(x => x.Klucz == tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__SchematLokalizacjiWersjiDocelowej_listakluczy[i3]);

                    if (lista_znalezionekluczewdanympliku.Count() == 1)
                    {
                        danywatek_lista_rekordowplikudocelowego.Add(new Rekord
                        {
                            ID = i3 + 2,
                            Plik = lista_znalezionekluczewdanympliku[0].Plik,
                            Klucz = lista_znalezionekluczewdanympliku[0].Klucz,
                            String = lista_znalezionekluczewdanympliku[0].String
                        });

                        //Console.WriteLine("[DEBUG] Znaleziono klucz: " + tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__SchematLokalizacjiWersjiDocelowej_listakluczy[i3] + " w i3b==" + i3b);

                        if (wylacz_calkowitepokazywaniepostepow == false)
                        {
                            int postep_w_procentach = int.Parse(PoliczPostepWProcentach(tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_postep_aktualnalinia, nr_ostatniej_linii));
                            string komunikat_aktualnypostep = $"Trwa zbiorcze wdrażanie aktualizacji {cfg["wersjaGryPierwszegoProjektuZTransifexCOM"]}----->{tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_numerwersjidocelowej}. Aktualny postęp: {postep_w_procentach}% z {nr_ostatniej_linii}";

                            if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                            {
                                int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;
                                komunikat_aktualnypostep = "[Operacja makra: " + makro_numeroperacjiwkolejnosci + "/" + makro_operacje_lista.Count + "] " + komunikat_aktualnypostep;
                            }

                            if (postep_w_procentach_ostatniawartosccalkowita != postep_w_procentach)
                            {
                                //Console.WriteLine($"{postep_w_procentach_ostatniawartosccalkowita} != {postep_w_procentach}");
                                
                                Console.WriteLine(komunikat_aktualnypostep);
                                postep_w_procentach_ostatniawartosccalkowita = postep_w_procentach;

                            }
                        }
                        
                        break;
                    }
                    else if (lista_znalezionekluczewdanympliku.Count() == 0)
                    {
                        //kontynuowanie wyszukiwania klucza w innych plikach metodą wstecznej symulacji (zezwolenie na dalsze działanie pętli)
                    }
                    else
                    {
                        Blad("Krytyczny błąd: W pliku o przydzielonym indeksie wewnętrznym i3b=" + i3b + " występuje więcej niż 1 linia zawierająca ten sam klucz o wartości: \"" + lista_znalezionekluczewdanympliku[0].Klucz + "\".");
                    }

                }

                tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_postep_aktualnalinia++;

            }

            tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe__listawatkow_lista_rekordowplikudocelowego.Add(danywatek_lista_rekordowplikudocelowego);

            //Console.WriteLine("[DEBUG] danywatek_lista_rekordowplikudocelowego.Count()==" + danywatek_lista_rekordowplikudocelowego.Count());
            
        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek1()
        {
            int index = 0;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek2()
        {
            int index = 1;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek3()
        {
            int index = 2;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek4()
        {
            int index = 3;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek5()
        {
            int index = 4;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek6()
        {
            int index = 5;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek7()
        {
            int index = 6;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek8()
        {
            int index = 7;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek9()
        {
            int index = 8;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek10()
        {
            int index = 9;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek11()
        {
            int index = 10;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek12()
        {
            int index = 11;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek13()
        {
            int index = 12;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek14()
        {
            int index = 13;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek15()
        {
            int index = 14;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek16()
        {
            int index = 15;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek17()
        {
            int index = 16;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek18()
        {
            int index = 17;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek19()
        {
            int index = 18;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index]);

        }

        public static void ZbiorczeWdrazanieAktualizacji_Wielowatkowe_watek20()
        {
            int index = 19;
            ZbiorczeWdrazanieAktualizacji_Wielowatkowe_Operacje(index, tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD[index], tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO[index], true);

        }

        public static void TworzenieStrukturyPlikowLokalizacji_WeryfikacjaPlikowUpdateLocStruct()
        {
            string folderupdate = folderglownyprogramu + sc + "update";
            string przyrostek_UpdateLocStruct = ".UpdateLocStruct.json";

            if (Directory.Exists(folderupdate) == true)
            {
                List<string> istniejacenazwyplikowmetadanych = PobierzNazwyPlikowJSONzFolderu("update");

                List<string> lista_oznaczen_wersji = new List<string>();

                int np = 1;
                for (int i = 0; i < istniejacenazwyplikowmetadanych.Count; i++)
                {
                    //Console.WriteLine("Nazwa pliku metadanych: " + istniejacenazwyplikowmetadanych[i]);

                    if (istniejacenazwyplikowmetadanych[i].Contains(przyrostek_UpdateLocStruct) == true)
                    {
                        string oznaczenie_wersji = istniejacenazwyplikowmetadanych[i].Split(new string[] { ".Update" }, StringSplitOptions.None)[0];

                        //Console.WriteLine("Oznaczenie aktualizacji: " + oznaczenie_aktualizacji);

                        if (File.Exists(folderupdate + sc + oznaczenie_wersji + przyrostek_UpdateLocStruct) == true)
                        {
                            lista_oznaczen_wersji.Add(oznaczenie_wersji);

                            string ow = oznaczenie_wersji.Split(new char[] { '_' })[1];
                            string oznaczenie_nowej_wersji = ow.Split(new char[] { '-' })[1];

                            Console.WriteLine(np + ". " + oznaczenie_nowej_wersji);

                            np++;
                        }

                    }

                }

                if (istniejacenazwyplikowmetadanych.Count > 0 && lista_oznaczen_wersji.Count > 0)
                {
                    string numer_pozycji_string;
                    Console.Write("Wpisz numer pozycji wersji, dla której chcesz utworzyć strukturę lokalizacji: ");

                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                    {
                        makro_aktualny_indeks_listy = makro_aktualny_indeks_listy + 1;
                        numer_pozycji_string = makro_operacje_lista[makro_aktualny_indeks_listy];
                        Console.WriteLine(makro_operacje_lista[makro_aktualny_indeks_listy]);
                    }
                    else
                    {
                        numer_pozycji_string = Console.ReadLine();
                    }



                    if (CzyParsowanieINTUdane(numer_pozycji_string))
                    {
                        int indeks_oznaczeniawersji = (int.Parse(numer_pozycji_string)) - 1;

                        if ((indeks_oznaczeniawersji >= 0) && (lista_oznaczen_wersji.Count - 1 >= indeks_oznaczeniawersji))
                        {

                            string[] tmp2_loa = lista_oznaczen_wersji[indeks_oznaczeniawersji].Split(new char[] { '_' });

                            if (tmp2_loa.Length >= 2)
                            {
                                string numerporzadkowy_aktualizacji = tmp2_loa[0];
                                string oznaczenie_aktualizacji = tmp2_loa[1];

                                TworzenieStrukturyPlikowLokalizacji_Jednowatkowe(numerporzadkowy_aktualizacji, oznaczenie_aktualizacji);
                                
                            }
                            else
                            {
                                Blad("Wykryto przynajmniej jedną nieprawidłowość w nazwach plików UpdateLocStruct.");

                                Koniec();
                            }

                        }
                        else
                        {
                            Blad("Podano błędny numer pozycji. (#2)");

                            Koniec();
                        }

                    }
                    else
                    {
                        Blad("Podano błędny numer pozycji. (#1)");

                        Koniec();
                    }


                }
                else
                {
                    Blad("Folder \"" + folderupdate + "\" nie zawiera metadanych typu UpdateLocStruct.");

                    Koniec();

                }
            }
            else
            {
                Blad("Nie istnieje folder \"" + folderupdate + "\" zawierający metadane typu UpdateLocStruct.");

                Koniec();

            }

        }

        public static void TworzenieStrukturyPlikowLokalizacji_Jednowatkowe(string numerporzadkowy_wersji, string oznaczenie_wersji) //numerporzadkowy_aktualizacji np: #2 oznaczenie_aktualizacji np: 1.0.1c-1.1.7c
        /* 
         * WYMAGA PLIKÓW METADANYCH AKTUALIZACJI WYGENEROWANYCH W pwrpl-tools DO DZIAŁANIA. PLIKI TE MUSZĄ ZOSTAĆ UMIESZCZONE W FOLDERZE "pwrpl-converter/update/":
         */
        {
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - POCZĄTEK */
            if (Directory.Exists(nazwafolderutmp) == true)
            {
                Directory.Delete(nazwafolderutmp, true);
            }
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - KONIEC */


            //const int ilosc_watkow = 1;

            tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_oznaczeniewersji = oznaczenie_wersji;

            string[] ow = oznaczenie_wersji.Split(new char[] { '-' });
            string numer_starej_wersji = ow[0];
            string numer_nowej_wersji = ow[1];

            string folderupdate = folderglownyprogramu + sc + "update";

            if (Directory.Exists(folderupdate) == true)
            {

                string plikUpdateLocStructJSON_nazwa;
                string plikJSONlokalizacji_bazadoutworzeniastruktury_nazwa = "";

                string NOWYfolderlokalizacji_nazwa = "NOWY_plPL-" + numer_nowej_wersji;

                if (Directory.Exists(NOWYfolderlokalizacji_nazwa) == true)
                {
                    var losujliczbe = new Random();
                    var bity = new byte[5];
                    losujliczbe.NextBytes(bity);

                    Directory.Move(NOWYfolderlokalizacji_nazwa, NOWYfolderlokalizacji_nazwa.Replace("NOWY", "STARY") + "_" + losujliczbe.Next(1000, 9999));
                }

                Directory.CreateDirectory(NOWYfolderlokalizacji_nazwa);

                if ((makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1) || int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                {
                    
                    plikJSONlokalizacji_bazadoutworzeniastruktury_nazwa = "NOWY_plPL-" + numer_nowej_wersji + ".json";
                }
                else
                {

                    Console.Write("Podaj nazwę pliku json lokalizacji w wersji " + numer_nowej_wersji + ", który chcesz wykorzystać do utworzenia struktury lokalizacji: ");
                    plikJSONlokalizacji_bazadoutworzeniastruktury_nazwa = Console.ReadLine();

                }

                plikUpdateLocStructJSON_nazwa = numerporzadkowy_wersji + "_" + oznaczenie_wersji + ".UpdateLocStruct.json";

                Console.WriteLine("Podano nazwę pliku .UpdateLocStruct.json dla aktualizacji " + oznaczenie_wersji + ": " + plikUpdateLocStructJSON_nazwa);
                Console.WriteLine("Podano nazwę pliku json lokalizacji w wersji " + numer_nowej_wersji + ", który zostanie wykorzystany do utworzenia struktury lokalizacji: " + plikJSONlokalizacji_bazadoutworzeniastruktury_nazwa);

                if (
                      (File.Exists(folderupdate + sc + plikUpdateLocStructJSON_nazwa) == true)
                      && (File.Exists(plikJSONlokalizacji_bazadoutworzeniastruktury_nazwa) == true)
                   )
                {
                    if (PoliczLiczbeLinii(folderupdate + sc + plikUpdateLocStructJSON_nazwa) == PoliczLiczbeLinii(plikJSONlokalizacji_bazadoutworzeniastruktury_nazwa))
                    {
                        Console.WriteLine("Trwa segregowanie danych przed właściwym tworzeniem struktury lokalizacji...");
                        Console.WriteLine("Proszę czekać...");

                        dynamic[] _StrukturaLokalizacji_tablicalistdanych = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(folderupdate + sc + plikUpdateLocStructJSON_nazwa);
                        dynamic[] _BazaDlaStruktury_tablicalistdanych = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(plikJSONlokalizacji_bazadoutworzeniastruktury_nazwa);

                        List<dynamic> _StrukturaLokalizacji_listakluczy = _StrukturaLokalizacji_tablicalistdanych[0];
                        List<dynamic> _BazaDlaStruktury_listakluczy = _BazaDlaStruktury_tablicalistdanych[0];

                        List<List<dynamic>> _StrukturaLokalizacji = _StrukturaLokalizacji_tablicalistdanych[1];
                        List<List<dynamic>> _BazaDlaStruktury = _BazaDlaStruktury_tablicalistdanych[1];


                        /*
                        //[DEBUG] test wyświetlania _StrukturaLokalizacji
                        Console.WriteLine("_StrukturaLokalizacji_listakluczy[0]:" + _StrukturaLokalizacji_listakluczy[0].ToString());
                        Console.WriteLine("_StrukturaLokalizacji_listakluczy[1]:" + _StrukturaLokalizacji_listakluczy[1].ToString());
                        JSON.WyswietlWszystkieStaleIIchWartosci_v1(_StrukturaLokalizacji_tablicalistdanych[0], _StrukturaLokalizacji_tablicalistdanych[1]);
                        Console.WriteLine("[_StrukturaLokalizacji] klucz o indeksie 5: " + _StrukturaLokalizacji_listakluczy[5]);
                        Console.WriteLine("[_StrukturaLokalizacji] nazwa pliku o indeksie 5: " + _StrukturaLokalizacji[5][0]);
                        */

                        /*
                        //[DEBUG] test wyświetlania _BazaDlaStruktury
                        Console.WriteLine("_BazaDlaStruktury_listakluczy[0]:" + _BazaDlaStruktury_listakluczy[0].ToString());
                        Console.WriteLine("_BazaDlaStruktury_listakluczy[1]:" + _BazaDlaStruktury_listakluczy[1].ToString());
                        JSON.WyswietlWszystkieStaleIIchWartosci_v1(_BazaDlaStruktury_tablicalistdanych[0], _BazaDlaStruktury_tablicalistdanych[1]);
                        Console.WriteLine("[_BazaDlaStruktury] klucz o indeksie 5: " + _BazaDlaStruktury_listakluczy[5]);
                        Console.WriteLine("[_BazaDlaStruktury] nazwa pliku o indeksie 5: " + _BazaDlaStruktury[5][0]);
                        */


                        List<Rekord> struktura_dane = new List<Rekord>();

                        //Informacja("_BazaDlaStruktury_listakluczy.Count(): " + _BazaDlaStruktury_listakluczy.Count());

                        for (int i1 = 0; i1 < _BazaDlaStruktury_listakluczy.Count(); i1++)
                        {
                            //Informacja("_BazaDlaStruktury[i1].Count(): " + _BazaDlaStruktury[i1].Count());

                            if (i1 != 0 && i1 != 1) //odfiltrowanie pierwszych dwóch rekordów zawierających słowa, wczytane z pliku JSON, takie jak: "$id", "string", "1"
                            {
                                for (int i2 = 0; i2 < _BazaDlaStruktury[i1].Count(); i2++)
                                {
                                    int _ID = i1;
                                    string _Plik = _StrukturaLokalizacji[i1][i2].ToString();
                                    string _Klucz = _BazaDlaStruktury_listakluczy[i1];
                                    string _String = _BazaDlaStruktury[i1][i2];

                                    //Console.WriteLine(_ID + "|" + _Plik + "|" + _Klucz + "|" + _String);

                                    struktura_dane.Add(new Rekord() { ID = _ID, Plik = _Plik, Klucz = _Klucz, String = _String });
                                }

                            }
                        }



                        //Informacja(struktura_dane.Count().ToString());

                        TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_Operacje(NOWYfolderlokalizacji_nazwa, struktura_dane);

                        Sukces("Struktura lokalizacji została pomyślnie zapisana w Folderze JSON o nazwie \"" + NOWYfolderlokalizacji_nazwa + "\".");

                        makro_pomyslnezakonczenieoperacjinr200 = true;

                        /*
                        // test dodawania danych do listy i sortowania - POCZĄTEK
                        struktura_dane.Add(new Rekord() { ID = 1, Plik = "h", Klucz = "klucz8", String = "string8" });
                        struktura_dane.Add(new Rekord() { ID = 2, Plik = "i", Klucz = "klucz9", String = "string9" });
                        struktura_dane.Add(new Rekord() { ID = 3, Plik = "g", Klucz = "klucz7", String = "string7" });
                        struktura_dane.Add(new Rekord() { ID = 4, Plik = "b", Klucz = "klucz2", String = "string2" });
                        struktura_dane.Add(new Rekord() { ID = 5, Plik = "a", Klucz = "klucz1", String = "string1" });
                        struktura_dane.Add(new Rekord() { ID = 6, Plik = "c", Klucz = "klucz3", String = "string3" });
                        struktura_dane.Add(new Rekord() { ID = 7, Plik = "e", Klucz = "klucz5", String = "string5" });
                        struktura_dane.Add(new Rekord() { ID = 8, Plik = "d", Klucz = "klucz4", String = "string4" });
                        struktura_dane.Add(new Rekord() { ID = 9, Plik = "f", Klucz = "klucz6", String = "string6" });

                        Console.WriteLine("\nPrzed sortowaniem:");
                        foreach (Rekord _rekord in struktura_dane)
                        {
                            Console.WriteLine(_rekord);
                        }

                        struktura_dane.Sort(delegate (Rekord x, Rekord y)
                        {
                            if (x.Plik == null && y.Plik == null) return 0;
                            else if (x.Plik == null) return -1;
                            else if (y.Plik == null) return 1;
                            else return x.Plik.CompareTo(y.Plik);
                        });

                        Console.WriteLine("\nPo sortowaniu rosnąco według nazwy plików:");
                        foreach (Rekord _rekord in struktura_dane)
                        {
                            Console.WriteLine(_rekord);
                        }
                        // test dodawania danych do listy i sortowania - KONIEC
                        */




                        /*
                        int plikUpdateLocStructJSON_iloscwszystkichkluczy = _StrukturaLokalizacji_listakluczy.Count;

                        if (wl_pasekpostepu == true)
                        {
                            InicjalizacjaPaskaPostepu(plikUpdateLocStructJSON_iloscwszystkichkluczy);
                        }

                        decimal maksymalna_ilosc_linii_dla_1_watku = Math.Ceiling(Convert.ToDecimal(plikUpdateLocStructJSON_iloscwszystkichkluczy) / Convert.ToDecimal(ilosc_watkow));

                        Console.WriteLine("plikUpdateLocStructJSON_iloscwszystkichkluczy: " + plikUpdateLocStructJSON_iloscwszystkichkluczy);
                        Console.WriteLine("ilosc_watkow: " + ilosc_watkow);
                        Console.WriteLine("maksymalna_ilosc_linii_dla_1_watku: " + maksymalna_ilosc_linii_dla_1_watku);

                        //tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_iloscwszystkichkluczyplikuUpdateLocStructJSONTMP = plikUpdateLocStructJSON_iloscwszystkichkluczy;
                        */

                        //bool t = CzyIstniejeDanyKluczWLiscieKluczy(_StrukturaLokalizacji_listakluczy, "11111111-1111-1111-1111-111111111111");
                        //Console.WriteLine("t: " + t);

                        /*
                        List<string> listaplikowjsonTMP = new List<string>();
                        List<int> listazakresuindeksow_od = new List<int>();
                        List<int> listazakresuindeksow_do = new List<int>();


                        for (int lw = 0; lw < ilosc_watkow; lw++)
                        {
                            int numer_pliku = lw + 1;

                            int index_od = lw * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku);
                            int index_do = ((lw + 1) * Convert.ToInt32(maksymalna_ilosc_linii_dla_1_watku)) - 1;

                            if (index_do > Convert.ToInt32(plikUpdateLocStructJSON_iloscwszystkichkluczy) - 1)
                            {
                                index_do = Convert.ToInt32(plikUpdateLocStructJSON_iloscwszystkichkluczy) - 1;
                            }

                            listaplikowjsonTMP.Add(NOWYfolderlokalizacji_nazwa + "_" + numer_pliku + ".tmp");
                            listazakresuindeksow_od.Add(index_od);
                            listazakresuindeksow_do.Add(index_do);
                        }
                        */

                        /*
                        for (int test1 = 0; test1 < listazakresuindeksow_od.Count; test1++)
                        {
                            Console.WriteLine("test1[" + test1 + "] (zakres od): " + listazakresuindeksow_od[test1]);
                        }
                        for (int test2 = 0; test2 < listazakresuindeksow_do.Count; test2++)
                        {
                            Console.WriteLine("test2[" + test2 + "] (zakres do): " + listazakresuindeksow_do[test2]);
                        }
                        */

                        //tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP = listaplikowjsonTMP;
                        //tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowOD = listazakresuindeksow_od;
                        //tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_zakresindeksowDO = listazakresuindeksow_do;


                        /*

                        Thread watek1 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek1);
                        Thread watek2 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek2);
                        Thread watek3 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek3);
                        Thread watek4 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek4);
                        Thread watek5 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek5);
                        Thread watek6 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek6);
                        Thread watek7 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek7);
                        Thread watek8 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek8);
                        Thread watek9 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek9);
                        Thread watek10 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek10);
                        Thread watek11 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek11);
                        Thread watek12 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek12);
                        Thread watek13 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek13);
                        Thread watek14 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek14);
                        Thread watek15 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek15);
                        Thread watek16 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek16);
                        Thread watek17 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek17);
                        Thread watek18 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek18);
                        Thread watek19 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek19);
                        Thread watek20 = new Thread(WdrazanieAktualizacji_Wielowatkowe_watek20);

                        watek1.Start();
                        watek2.Start();
                        watek3.Start();
                        watek4.Start();
                        watek5.Start();
                        watek6.Start();
                        watek7.Start();
                        watek8.Start();
                        watek9.Start();
                        watek10.Start();
                        watek11.Start();
                        watek12.Start();
                        watek13.Start();
                        watek14.Start();
                        watek15.Start();
                        watek16.Start();
                        watek17.Start();
                        watek18.Start();
                        watek19.Start();
                        watek20.Start();


                        watek1.Join();
                        watek2.Join();
                        watek3.Join();
                        watek4.Join();
                        watek5.Join();
                        watek6.Join();
                        watek7.Join();
                        watek8.Join();
                        watek9.Join();
                        watek10.Join();
                        watek11.Join();
                        watek12.Join();
                        watek13.Join();
                        watek14.Join();
                        watek15.Join();
                        watek16.Join();
                        watek17.Join();
                        watek18.Join();
                        watek19.Join();
                        watek20.Join();

                        //Sukces("!!!Zaraportowano zakończenie wszystkich wątków!!!");

                        */


                        /*

                        string nazwafinalnegoplikuJSON = NOWYfolderlokalizacji_nazwa;

                        if (File.Exists(nazwafinalnegoplikuJSON) == true) { File.Delete(nazwafinalnegoplikuJSON); }


                        UtworzNaglowekJSON(nazwafinalnegoplikuJSON);


                        FileStream finalnyplikJSON_fs = new FileStream(nazwafinalnegoplikuJSON, FileMode.Append, FileAccess.Write);

                        try //#1
                        {
                            StreamWriter finalnyplikJSON_sw = new StreamWriter(finalnyplikJSON_fs);


                            for (int lpj2 = 0; lpj2 < tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP.Count; lpj2++)
                            {
                                FileStream plikjsonTMP_fs = new FileStream(nazwafolderutmp + sc + tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP[lpj2], FileMode.Open, FileAccess.Read);

                                try //#2
                                {
                                    StreamReader plikjsonTMP_sr = new StreamReader(plikjsonTMP_fs);

                                    finalnyplikJSON_sw.Write(plikjsonTMP_sr.ReadToEnd());

                                    plikjsonTMP_sr.Close();
                                }
                                catch (Exception Error)
                                {

                                    string komunikat_obledzie;
                                    komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #2 (for-lpj2: " + lpj2 + ", Error: " + Error + ")!";

                                    if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                                    {
                                        int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                                        makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                                    }

                                }

                                plikjsonTMP_fs.Close();


                            }

                            finalnyplikJSON_sw.Close();



                        }
                        catch (Exception Error)
                        {

                            string komunikat_obledzie;
                            komunikat_obledzie = "BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #1 (Error: " + Error + ")!";

                            if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                            {
                                int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;

                                makro_bledy_lista.Add(makro_numeroperacjiwkolejnosci + ";" + komunikat_obledzie);

                            }

                        }


                        finalnyplikJSON_fs.Close();

                        bool stopkaJSON_rezultat = UtworzStopkeJSON(nazwafinalnegoplikuJSON);

                        if (stopkaJSON_rezultat == true)
                        {
                            makro_pomyslnezakonczenieoperacjinr100 = true;



                            //Console.BackgroundColor = ConsoleColor.Green;
                            Console.WriteLine("Plik JSON o nazwie \"" + nazwafinalnegoplikuJSON + "\" zostal wygenerowany.");
                            Console.ResetColor();

                        }

                        */



                    }
                    else
                    {
                        Blad("Wystąpiła niezgodność kompatybilności wskazanego pliku lokalizacyjnego z wybraną wersją gry. Upewnij się, że wskazałeś właśiwy plik i wybrałeś dla niego właściwą wersję gry w celu utworzenia struktury lokalizacji.");
                    }

                }
                else
                {
                    Blad("Nie istnieje przynajmniej jeden z podanych plików.");
                }

            }
            else
            {
                Blad("Nie istnieje folder \"" + folderupdate + "\" zawierający metadane UpdateLocStruct.");
            }

            //czyszczenie pamięci
            //UsunPlikiTMP(tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_listaplikowjsonTMP);



            if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1 && makro_pomyslnezakonczenieoperacjinr200 == true)
            {
                
                //czyszczenie pamięci dodatkowej jeśli makro jest aktywne
                tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_numeraktualnejlinii = 1;
                tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_oznaczeniewersji = "";
                

                Makro_UruchomienieKolejnejOperacji();
            }
            else
            {

                Koniec();

            }



        }

        public static void TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_Operacje(string NOWYfolderlokalizacji_nazwa, List<Rekord> struktura_dane)
        {

            //string lancuch_tmpnazwysegregowanychplikow = ""; //nazwa_pliku1;nazwa_pliku2;nazwa_pliku3 itd.
            List<List<Rekord>> lista_list_danych_1_konkretnego_pliku = new List<List<Rekord>>();
            string lancuch_wszystkichplikow = "";

            int nr_ostatniej_linii = 1;
            //tworzenie łańcucha zawierającego wszystkie nazwy plikow w struktura_danych
            for (int tl1 = 0; tl1 < struktura_dane.Count(); tl1++)
            {
                if (lancuch_wszystkichplikow.Contains(struktura_dane[tl1].Plik) == false)
                {
                    lancuch_wszystkichplikow = lancuch_wszystkichplikow + struktura_dane[tl1].Plik + ";";
                }

                nr_ostatniej_linii++;
            }

            string[] lancuch_wszystkichplikow_Split = lancuch_wszystkichplikow.TrimEnd(new char[] { ';' }).Split(';');
            //Informacja("lancuch_wszystkichplikow_Split.Length:" + lancuch_wszystkichplikow_Split.Length);
            for (int tl2 = 0; tl2 < lancuch_wszystkichplikow_Split.Length; tl2++)
            {
                //Console.WriteLine(lancuch_wszystkichplikow_Split[tl2]);

                List<Rekord> lista_danych_1_konkretnego_pliku = struktura_dane.FindAll(x => x.Plik == lancuch_wszystkichplikow_Split[tl2]);

                lista_list_danych_1_konkretnego_pliku.Add(lista_danych_1_konkretnego_pliku);

                /*
                Console.WriteLine("---");
                foreach (Rekord poz in wynik)
                {
                    Console.WriteLine(poz);
                }
                */

            }



            string lista_tmpnazwotwartychplikow = ""; //nazwa_pliku1;nazwa_pliku2;nazwa_pliku3 itd.
            List<string> lista_nazwotwartychplikow = new List<string>();
            List<FileStream> lista_otwartychplikow_fs = new List<FileStream>();
            List<StreamWriter> lista_otwartychstrumienizapisudoplikow_sw = new List<StreamWriter>();
            List<List<Linia>> lista_list_liniidozapisuwpliku = new List<List<Linia>>();


            //Informacja("lista_list_danych_1_konkretnego_pliku.Count(): " + lista_list_danych_1_konkretnego_pliku.Count());

            for (int tl3 = 0; tl3 < lista_list_danych_1_konkretnego_pliku.Count(); tl3++)
            {
                List<Linia> lista_liniidozapisuwpliku = new List<Linia>();

                //Informacja("lista_list_danych_1_konkretnego_pliku[tl3].Count(): " + lista_list_danych_1_konkretnego_pliku[tl3].Count());

                for (int tl4 = 0; tl4 < lista_list_danych_1_konkretnego_pliku[tl3].Count(); tl4++)
                {


                    int _ID = lista_list_danych_1_konkretnego_pliku[tl3][tl4].ID;
                    string _Plik = lista_list_danych_1_konkretnego_pliku[tl3][tl4].Plik;
                    string _Klucz = lista_list_danych_1_konkretnego_pliku[tl3][tl4].Klucz;
                    string _String = lista_list_danych_1_konkretnego_pliku[tl3][tl4].String;

                    if (lista_tmpnazwotwartychplikow.Contains(_Plik) == true)
                    {

                        lista_liniidozapisuwpliku.Add(new Linia { OtwartyStrumienZapisuDoPliku = lista_otwartychstrumienizapisudoplikow_sw.Last(), Plik = _Plik, Klucz = _Klucz, String = _String });

                    }
                    else if (lista_tmpnazwotwartychplikow.Contains(_Plik) == false)
                    {

                        UtworzNaglowekJSON(NOWYfolderlokalizacji_nazwa + sc + _Plik);

                        FileStream danypliklokalizacji_fs = new FileStream(NOWYfolderlokalizacji_nazwa + sc + _Plik, FileMode.Append, FileAccess.Write);
                        lista_otwartychplikow_fs.Add(danypliklokalizacji_fs);

                        try
                        {
                            StreamWriter danypliklokalizacji_sw = new StreamWriter(danypliklokalizacji_fs);

                            lista_liniidozapisuwpliku.Add(new Linia { OtwartyStrumienZapisuDoPliku = danypliklokalizacji_sw, Plik = _Plik, Klucz = _Klucz, String = _String });

                            lista_otwartychstrumienizapisudoplikow_sw.Add(danypliklokalizacji_sw);
                        }
                        catch
                        {
                            Blad("Wystąpił nieoczekiwany błąd w strumieniu zapisu do pliku: " + _Plik + " (tl3: " + tl3 +", tl4: " + tl4 + ")");
                        }

                        lista_tmpnazwotwartychplikow = lista_tmpnazwotwartychplikow + _Plik + ";";
                        lista_nazwotwartychplikow.Add(_Plik);

                    }


                }

                lista_list_liniidozapisuwpliku.Add(lista_liniidozapisuwpliku);



            }


            //zapisywanie danych w plikach
            string tmp_nazwaplikuzostatniejlinii = "";

            //Informacja("lista_list_liniidozapisuwpliku.Count(): " + lista_list_liniidozapisuwpliku.Count());

            for (int isd2 = 0; isd2 < lista_list_liniidozapisuwpliku.Count(); isd2++)
            {
                for (int isd2b = 0; isd2b < lista_list_liniidozapisuwpliku[isd2].Count(); isd2b++)
                {

                    if (wl_pasekpostepu == false)
                    {

                        string komunikat_aktualnypostep = "Trwa tworzenie struktury lokalizacji dla wersji " + tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_oznaczeniewersji.Split("-")[1] + ": " + tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_numeraktualnejlinii + "/" + nr_ostatniej_linii + " [" + PoliczPostepWProcentach(tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_numeraktualnejlinii, nr_ostatniej_linii) + "%]";

                        if (makro_aktywowane == true && int.Parse(cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString()) == 1)
                        {
                            int makro_numeroperacjiwkolejnosci = makro_aktualny_indeks_listy + 1;
                            komunikat_aktualnypostep = "[Operacja makra: " + makro_numeroperacjiwkolejnosci + "/" + makro_operacje_lista.Count + "] " + komunikat_aktualnypostep;
                        }

                        Console.WriteLine(komunikat_aktualnypostep);

                    }
                    else if (wl_pasekpostepu == true)
                    {
                        pasek_postepu.Refresh(Convert.ToInt32(tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_numeraktualnejlinii), "Trwa tworzenie struktury lokalizacji...");
                    }


                    StreamWriter OtwartyStrumienZapisuDoPliku = lista_list_liniidozapisuwpliku[isd2][isd2b].OtwartyStrumienZapisuDoPliku;
                    string _Plik_zapis = lista_list_liniidozapisuwpliku[isd2][isd2b].Plik;
                    string _Klucz_zapis = lista_list_liniidozapisuwpliku[isd2][isd2b].Klucz;
                    string _String_zapis = FiltrujString(lista_list_liniidozapisuwpliku[isd2][isd2b].String);



                    OtwartyStrumienZapisuDoPliku.Write("    \"" + _Klucz_zapis + "\": \"" + _String_zapis + "\"");


                    if (isd2b + 1 != lista_list_liniidozapisuwpliku[isd2].Count())
                    {
                        OtwartyStrumienZapisuDoPliku.Write(",");
                    }

                    OtwartyStrumienZapisuDoPliku.Write("\n");


                    tmp_nazwaplikuzostatniejlinii = _Plik_zapis;



                    tmpdlawatkow_TworzenieStrukturyPlikowLokalizacji_Jednowatkowe_numeraktualnejlinii++;

                }

            }
            



            //zamykanie strumieni zapisów do plików, plików i tworzenie stopek
            for (int isd3 = 0; isd3 < lista_otwartychstrumienizapisudoplikow_sw.Count(); isd3++)
            {
                lista_otwartychstrumienizapisudoplikow_sw[isd3].Close();
                lista_otwartychplikow_fs[isd3].Close();

                UtworzStopkeJSON(NOWYfolderlokalizacji_nazwa + sc + lista_nazwotwartychplikow[isd3]);
            }

            //czyszczenie danych
            lista_tmpnazwotwartychplikow = "";
            lista_nazwotwartychplikow.Clear();
            lista_otwartychplikow_fs.Clear();
            lista_otwartychstrumienizapisudoplikow_sw.Clear();
            lista_list_liniidozapisuwpliku.Clear();


        }


    }
}
