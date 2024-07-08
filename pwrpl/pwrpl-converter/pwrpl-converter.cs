/*
UWAGA: NIE ZMIENIAĆ NUMERÓW OPERACJI, ZE WZGLĘDU NA:
-ZAIMPLEMENTOWANE MAKRA OD WERSJI v.1.7
-ZAIMPLEMENTOWANĄ OBSŁUGĘ ARGUMENTÓW ORAZ API OD WERSJI V.3.01
*/

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
    

    partial class pwrpl_converter
    {
        public const string _PWR_nazwaprogramu = "pwrpl-converter";
        public const string _PWR_wersjaprogramu = "v.3.05";
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
        
        //wbudowując w aplikację "pwrpl" zmienić nazwę metody głównej na Main2
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
                    else if (numer_operacji_int == 150)
                    {
                        WeryfikacjaPlikowMetadanych("JSONtoJSON_KonwersjaCudzysłowowWTresciachLiniiNaAlternatywne");
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
        

    }
}
