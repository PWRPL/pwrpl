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
    private static readonly bool _alternatywne_cudzyslowy_czyuzywac = true; //jeśli true: zamienia wszystkie tagi <bs_n1> na alternatywne cudzysłowy „”
    private static readonly string _alternatywny_cudzyslow_otwierajacy = "”"; // 
    private static readonly string _alternatywny_cudzyslow_zamykajacy = "”"; // UWAGA: Zalecane jest stosowanie tych samych znaków otwierajacych i zamykających, ponieważ jeśli są różne (np. „”), występują błędy w wyświetlaniu cudzysłowów wewnątrz innych cudzysłowów - np. „tekst 1 ”tekst2„”
    
    public static string ZamienCudzyslowyZDomyslnychNaAlternatywne(string tresc_stringa)
    {
        string zamieniane = "\\\"";
        int liczbawystapien = 0;

        string rezultat = Regex.Replace(tresc_stringa, zamieniane, match =>
        {
            liczbawystapien++;
            if (liczbawystapien % 2 == 1)
            {
                return _alternatywny_cudzyslow_otwierajacy;
            }
            else
            {
                return _alternatywny_cudzyslow_zamykajacy;
            }
        });

        return rezultat;
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

                
                if (wylacz_calkowitepokazywaniepostepow == true) { Console.WriteLine("Trwa przygotowywanie linii. Może to chwilę zająć. Proszę czekać...");}


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

                                    if (_alternatywne_cudzyslowy_czyuzywac == true)
                                    {
                                        plikstringstxt_trescuaktualnionalinii = ZamienCudzyslowyZDomyslnychNaAlternatywne(plikstringstxt_trescuaktualnionalinii)
                                                
                                                //usuwanie backslahy sprzed alternatywnych cudzysłowów
                                                .Replace("\\" + _alternatywny_cudzyslow_otwierajacy, _alternatywny_cudzyslow_otwierajacy)
                                                .Replace("\\" + _alternatywny_cudzyslow_zamykajacy, _alternatywny_cudzyslow_zamykajacy)
                                            
                                                //naprawienie cudzyslowow w tagach <link=""> <sprite name=""> itp (muszą zawierać domyślne cudzysłowy)
                                                .Replace("=" + _alternatywny_cudzyslow_otwierajacy, "=\\\"")
                                                .Replace(_alternatywny_cudzyslow_zamykajacy + ">", "\\\">")
                                                ;
                                    }

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

}