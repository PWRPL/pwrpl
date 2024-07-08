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

namespace pwrpl_converter;

partial class pwrpl_converter
{
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