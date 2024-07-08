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

            
            if (wylacz_calkowitepokazywaniepostepow == true) { Console.WriteLine($"Trwa zbiorcze wdrażanie aktualizacji {cfg["wersjaGryPierwszegoProjektuZTransifexCOM"]}----->{tmpdlawatkow_ZbiorczeWdrazanieAktualizacji_Wielowatkowe_numerwersjidocelowej}. Może to chwilę zająć. Proszę czekać..."); }

            
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

}