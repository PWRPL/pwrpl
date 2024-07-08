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


                if (wylacz_calkowitepokazywaniepostepow == true) { Console.WriteLine($"Trwa wdrażanie aktualizacji {tmpdlawatkow_WdrazanieAktualizacji_Wielowatkowe_oznaczenieaktualizacji.Replace("-", "->")}. Może to chwilę zająć. Proszę czekać..."); }
                    

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

            if (wylacz_calkowitepokazywaniepostepow == false)
            {
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

}