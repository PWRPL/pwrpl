using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;

using System.Text.RegularExpressions;

using System.Reflection.Metadata;
using static System.Net.WebRequestMethods;
using static System.Net.Mime.MediaTypeNames;
using File = System.IO.File;
using System.Data.Common;

using static pwrpl_tools.pwrpltools;
using System.Security.Cryptography;

namespace pwrpl_tools
{
    internal class ToyBox_PL
    {
        //TOYBOX_PL

        public static List<string> PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(string wersjaibuild_Toyboxa) // wersjaibuild_Toyboxa np.: 1.4.17(202210040215)
        {
            List<string> listaplikow = new List<string>();

            //struktura folderu lokalizacji dla wersji ToyBoxa: 1.4.17(202210040215)
            if (wersjaibuild_Toyboxa == "1.4.17(202210040215)")
            {
                List<string> lp = new List<string>(new string[]
                {
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintAction.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintBrowser.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintListUI.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}Editor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}EditorActions.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}FactsEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}ArmiesEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}CrusadeEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}EventEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}SettlementsEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeChildrenDrawer.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeInfo.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudesEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeTreeModel.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}ReferenceGraph.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// $"ToyBox{sc}classes{sc}MainUI{sc}ActionButtons.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Actions.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}BagOfTricks.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}BlueprintLoader.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}CharacterPicker.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}EnchantmentEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}FeaturesTreeEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}LevelUp.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Main.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}MulticlassPicker.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}PartyEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}PhatLoot.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Playground.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}QuestEditor.cs"
                });
                listaplikow = lp;
            }

            //struktura folderu lokalizacji dla wersji ToyBoxa: 1.4.19(202212180342)
            else if (wersjaibuild_Toyboxa == "1.4.19(202212180342)")
            {
                List<string> lp = new List<string>(new string[]
                {
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BuffExclusionEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintAction.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintBrowser.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintListUI.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}Editor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}EditorActions.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}FactsEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}ArmiesEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}CrusadeEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}EventEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}SettlementsEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeChildrenDrawer.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeInfo.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudesEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeTreeModel.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}ReferenceGraph.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// $"ToyBox{sc}classes{sc}MainUI{sc}ActionButtons.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Actions.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}BagOfTricks.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}BlueprintLoader.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}CharacterPicker.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}EnchantmentEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}FeaturesTreeEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}LevelUp.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Main.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}MulticlassPicker.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}PartyEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}PhatLoot.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Playground.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}QuestEditor.cs"
                });
                listaplikow = lp;
            }
            //struktura folderu lokalizacji dla wersji ToyBoxa: 1.4.20(202301021625)
            else if (wersjaibuild_Toyboxa == "1.4.20(202301021625)")
            {
                List<string> lp = new List<string>(new string[]
                {
                    $"ToyBox{sc}classes{sc}Models{sc}Settings+UI.cs",

                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BuffExclusionEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintAction.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintBrowser.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}BlueprintListUI.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// "ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}Editor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}EditorActions.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Browser{sc}FactsEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}ArmiesEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}CrusadeEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}EventEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Crusade{sc}SettlementsEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeChildrenDrawer.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// "ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeInfo.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudesEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}EtudeTreeModel.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Etudes{sc}ReferenceGraph.cs",
                    //w tej wersji brak wykrytych stringów w tym pliku// "ToyBox{sc}classes{sc}MainUI{sc}ActionButtons.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Actions.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}BagOfTricks.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}BlueprintLoader.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}CharacterPicker.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}EnchantmentEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}FeaturesTreeEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}LevelUp.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Main.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}MulticlassPicker.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}PartyEditor.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}PhatLoot.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}Playground.cs",
                    $"ToyBox{sc}classes{sc}MainUI{sc}QuestEditor.cs"
                });
                listaplikow = lp;
            }


            //jeśli struktura folderu lokalizacji dla podanej wersji ToyBoxa nie jest zdefiniowana
            else
            {
                Blad("BŁĄD: pwrpl-tools nie zawiera zdefiniowanej struktury folderu lokalizacji dla podanej wersji Toyboxa: " + wersjaibuild_Toyboxa);
                PotwierdzEnterem();
            }

            return listaplikow;

        }

        public static List<ToyBox_Stala> PobierzListeStalychKtoreMuszaZostacPodmienione(string wersjaibuild_Toyboxa)
        {
            List<ToyBox_Stala> listastalych = new List<ToyBox_Stala>();

            //lista stałych, które muszą zostać podmienione dla wersji ToyBoxa: 1.4.20(202301021625)
            if (wersjaibuild_Toyboxa == "1.4.20(202301021625)")
            {
                listastalych.Add(new ToyBox_Stala
                {
                    Index = 0,
                    PlikCS = $"ModKit{sc}Utility{sc}Logging.cs",
                    String_EN = "    public enum LogLevel : int {\n        Error,\n        Warning,\n        Info,\n        Debug,\n        Trace\n    }\n",
                    String_PL = "    public enum LogLevel : int {\n        Błąd,\n        Ostrzeżenie,\n        Informacja,\n        Debugowanie,\n        Śledzenie\n    }\n"
                });

                listastalych.Add(new ToyBox_Stala
                {
                    Index = 1,
                    PlikCS = $"ModKit{sc}Utility{sc}Logging.cs",
                    String_EN = "LogLevel.Error",
                    String_PL = "LogLevel.Błąd"
                });

                listastalych.Add(new ToyBox_Stala
                {
                    Index = 2,
                    PlikCS = $"ModKit{sc}Utility{sc}Logging.cs",
                    String_EN = "LogLevel.Warning",
                    String_PL = "LogLevel.Ostrzeżenie"
                });

                listastalych.Add(new ToyBox_Stala
                {
                    Index = 3,
                    PlikCS = $"ModKit{sc}Utility{sc}Logging.cs",
                    String_EN = "LogLevel.Info",
                    String_PL = "LogLevel.Informacja"
                });

                listastalych.Add(new ToyBox_Stala
                {
                    Index = 4,
                    PlikCS = $"ModKit{sc}Utility{sc}Logging.cs",
                    String_EN = "LogLevel.Debug",
                    String_PL = "LogLevel.Debugowanie"
                });

                listastalych.Add(new ToyBox_Stala
                {
                    Index = 5,
                    PlikCS = $"ModKit{sc}Utility{sc}Logging.cs",
                    String_EN = "LogLevel.Trace",
                    String_PL = "LogLevel.Śledzenie"
                });

                listastalych.Add(new ToyBox_Stala
                {
                    Index = 5,
                    PlikCS = $"ToyBox{sc}classes{sc}Models{sc}Settings.cs",
                    String_EN = "LogLevel.Info",
                    String_PL = "LogLevel.Informacja"
                });


            }



            //jeśli struktura folderu lokalizacji dla podanej wersji ToyBoxa nie jest zdefiniowana
            else
            {
                Blad("BŁĄD: pwrpl-tools nie zawiera zdefiniowanej struktury stałych, które muszą zostać podmienione dla podanej wersji Toyboxa: " + wersjaibuild_Toyboxa);
                PotwierdzEnterem();
            }

            return listastalych;
        }

        public static List<string> PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStalych(string wersjaibuild_Toyboxa)
        {
            List<string> listaplikow = new List<string>();

            List<ToyBox_Stala> listastalych = PobierzListeStalychKtoreMuszaZostacPodmienione(wersjaibuild_Toyboxa);

            for (int ls = 0; ls < listastalych.Count; ls++)
            {
                List<string> znalezionyplik_wliscie = listaplikow.FindAll(x => x == listastalych[ls].PlikCS);

                if (znalezionyplik_wliscie.Count == 0)
                {
                    listaplikow.Add(listastalych[ls].PlikCS);
                }

            }

            return listaplikow;
        }

        public static List<dynamic> PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(string wersjaibuild_Toyboxa, string typ_folderu, string nazwa_folderu) // wersjaibuild_Toyboxa np.: 1.4.17(202210040215)
        {
            List<dynamic> rezultat = new List<dynamic>(); //metoda zwraca rezultat[0]==(bool)czystrukturajestzgodna, rezultat[1]==(int)iloscplikowwstrukturze
            bool czystrukturajestzgodna = true;
            int iloscplikowwstrukturze;

            List<string> zdefiniowanastrukturalokalizacji_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(wersjaibuild_Toyboxa);
            iloscplikowwstrukturze = zdefiniowanastrukturalokalizacji_listaplikow.Count();

            for (int spr = 0; spr < zdefiniowanastrukturalokalizacji_listaplikow.Count(); spr++)
            {
                if (typ_folderu == "CS")
                {
                    if (File.Exists(nazwa_folderu + sc + zdefiniowanastrukturalokalizacji_listaplikow[spr]) == false)
                    {
                        czystrukturajestzgodna = false;
                        iloscplikowwstrukturze--;
                    }
                }
                else if (typ_folderu == "StringsTransifexcomTXT")
                {
                    if (File.Exists(nazwa_folderu + sc + zdefiniowanastrukturalokalizacji_listaplikow[spr] + ".StringsTransifexCOM.txt") == false)
                    {
                        czystrukturajestzgodna = false;
                        iloscplikowwstrukturze--;
                    }
                }
                else
                {
                    czystrukturajestzgodna = false;
                    iloscplikowwstrukturze--;
                }
            }

            rezultat.Add(czystrukturajestzgodna);
            rezultat.Add(iloscplikowwstrukturze);
            return rezultat;
        }

        public static void UtworzStruktureFolderuLokalizacji(string wersjaibuild_Toyboxa, string nazwa_folderu_lokalizacji, bool czy_uwzglednic_strukture_dla_stalych = false) // wersjaibuild_Toyboxa np.: 1.4.17(202210040215)
        {

            List<string> zdefiniowanastrukturalokalizacjidlastringow_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(wersjaibuild_Toyboxa);

            for (int i1 = 0; i1 < zdefiniowanastrukturalokalizacjidlastringow_listaplikow.Count(); i1++)
            {
                string[] nazwa_folderu = zdefiniowanastrukturalokalizacjidlastringow_listaplikow[i1].Split(sc);

                string ostatniasciezkadoglebokoscifolderu_tmp = nazwa_folderu_lokalizacji;
                for (int i2 = 0; i2 < nazwa_folderu.Length; i2++)
                {
                    if (nazwa_folderu[i2].Contains(".cs") == false)
                    {
                        ostatniasciezkadoglebokoscifolderu_tmp = ostatniasciezkadoglebokoscifolderu_tmp + sc + nazwa_folderu[i2];

                        if (Directory.Exists(ostatniasciezkadoglebokoscifolderu_tmp) == false)
                        {
                            Directory.CreateDirectory(ostatniasciezkadoglebokoscifolderu_tmp);
                        }
                    }
                }
                ostatniasciezkadoglebokoscifolderu_tmp = "";

            }



            if (czy_uwzglednic_strukture_dla_stalych == true)
            {
                List<string> zdefiniowanastrukturalokalizacjidlastalych_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStalych(wersjaibuild_Toyboxa);

                for (int i3 = 0; i3 < zdefiniowanastrukturalokalizacjidlastalych_listaplikow.Count(); i3++)
                {
                    string[] nazwa_folderu = zdefiniowanastrukturalokalizacjidlastalych_listaplikow[i3].Split(sc);

                    string ostatniasciezkadoglebokoscifolderu_tmp = nazwa_folderu_lokalizacji;
                    for (int i4 = 0; i4 < nazwa_folderu.Length; i4++)
                    {
                        if (nazwa_folderu[i4].Contains(".cs") == false)
                        {
                            ostatniasciezkadoglebokoscifolderu_tmp = ostatniasciezkadoglebokoscifolderu_tmp + sc + nazwa_folderu[i4];

                            if (Directory.Exists(ostatniasciezkadoglebokoscifolderu_tmp) == false)
                            {
                                Directory.CreateDirectory(ostatniasciezkadoglebokoscifolderu_tmp);
                            }
                        }
                    }
                    ostatniasciezkadoglebokoscifolderu_tmp = "";

                }

            }
        }

        public static List<RekordTXT> WczytajDaneZPlikuDoListy(string typ_pliku, string sciezka_do_pliku)
        {
            List<RekordTXT> plik_zawartosclinii = new List<RekordTXT>();

            if (File.Exists(sciezka_do_pliku) == true)
            {

                FileStream plik_fs = new FileStream(sciezka_do_pliku, FileMode.Open, FileAccess.Read);

                try
                {
                    StreamReader plik_sr = new StreamReader(plik_fs);

                    int id = 0;
                    int plik_numerlinii;
                    while (plik_sr.Peek() != -1)
                    {
                        plik_numerlinii = id + 1;

                        string zawartosc_linii = plik_sr.ReadLine();

                        if (typ_pliku == "CS")
                        {
                            /* MANUALNA "NAPRAWA" TREŚCI LINII - POCZĄTEK */
                            zawartosc_linii = zawartosc_linii.Replace("{\"LEEEROOOOOOOYYY JEEEENKINS!!!\".color(RGBA.maroon)}", "LEEEROOOOOOOYYY JEEEENKINS!!!");
                            /* MANUALNA "NAPRAWA" TREŚCI LINII - KONIEC */

                            plik_zawartosclinii.Add(new RekordTXT() { ID = id, NumerLinii = plik_numerlinii, IndeksStringaWDanejLinii = 0, String = zawartosc_linii });
                        }
                        else if (typ_pliku == "stringsTransifexcomTXT")
                        {
                            string[] podzial1 = zawartosc_linii.Split('>');
                            if (podzial1.Length >= 2)
                            {
                                string[] podzial2 = podzial1[0].Replace("<", "").Split(':');

                                if (podzial2.Length == 2)
                                {
                                    int _NumerLinii_wedlugplikuCS = int.Parse(podzial2[0]);
                                    int _IndeksStringaWDanejLinii = int.Parse(podzial2[1]);
                                    string _String = podzial1[1].Replace("\"", "'"); ;

                                    plik_zawartosclinii.Add(new RekordTXT() { ID = id, NumerLinii = _NumerLinii_wedlugplikuCS, IndeksStringaWDanejLinii = _IndeksStringaWDanejLinii, String = _String });

                                }

                            }

                        }


                        id++;
                    }

                    plik_sr.Close();

                }
                catch
                {
                    Blad("BŁĄD: Wystapił nieoczekiwany błąd w dostępie do pliku we wskazanej ścieżce: \"" + sciezka_do_pliku + "\".");
                }

                plik_fs.Close();

            }
            else
            {
                Blad("BŁĄD: Nie istnieje taki plik we wskazanej ścieżce: \"" + sciezka_do_pliku + "\"");
            }

            return plik_zawartosclinii;

        }

        public static void WyswietlWlasciwaStruktureFolderuLokalizacji()
        {
            string podana_wersjaibuild;

            Console.Write("Podaj numer wersji ToyBoxa z numerem buildu w nawiasie, np. 1.4.17(202210040215): ");
            podana_wersjaibuild = Console.ReadLine();
            Console.WriteLine("Podano jako wersję(build): " + podana_wersjaibuild);

            Console.WriteLine("a) Lista plików dla podanej wersji, w których modyfikowane są String'i:");

            List<string> zdefiniowanastrukturalokalizacjidlaStringow_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(podana_wersjaibuild);

            for (int zlp = 0; zlp < zdefiniowanastrukturalokalizacjidlaStringow_listaplikow.Count(); zlp++)
            {
                Console.WriteLine($"<Nazwa_Folderu>{sc}" + zdefiniowanastrukturalokalizacjidlaStringow_listaplikow[zlp]);
            }

            Console.WriteLine("\nb) Lista plików dla podanej wersji, w których modyfikowane są Stałe:");

            List<string> zdefiniowanastrukturalokalizacjidlaStalych_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStalych(podana_wersjaibuild);

            for (int zlp2 = 0; zlp2 < zdefiniowanastrukturalokalizacjidlaStalych_listaplikow.Count(); zlp2++)
            {
                Console.WriteLine($"<Nazwa_Folderu>{sc}" + zdefiniowanastrukturalokalizacjidlaStalych_listaplikow[zlp2]);
            }

            Informacja("\nZalecane jest umieszczanie całych folderów \"ModKit\" oraz \"ToyBox\" z kodami źródłowymi ToyBox'a w celach konwersji.");

        }

        public static void FolderCStoFolderTXTTransifexCOM_ZNumeramiLiniiZPlikuCS()
        {

            string nazwafolderuCS;
            string nazwaNOWEGOfolderuStringsTransifexcomTXT;
            string podana_wersjaibuild;

            Console.Write("Podaj nazwę folderu CS: ");
            nazwafolderuCS = Console.ReadLine();
            if (nazwafolderuCS == "") { nazwafolderuCS = "test1"; }
            Console.WriteLine("Podano nazwę folderu: " + nazwafolderuCS);
            nazwaNOWEGOfolderuStringsTransifexcomTXT = "NOWY_StringsTransifexcomTXT_" + nazwafolderuCS;

            Console.Write("Podaj numer wersji ToyBoxa z numerem buildu w nawiasie, np. 1.4.17(202210040215): ");
            podana_wersjaibuild = Console.ReadLine();
            Console.WriteLine("Podano jako wersję(build): " + podana_wersjaibuild);

            List<string> zdefiniowanastrukturalokalizacji_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(podana_wersjaibuild);


            if (Directory.Exists(nazwafolderuCS) == true)
            {

                if (Convert.ToBoolean(PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild, "CS", nazwafolderuCS)[0]) == true)
                {

                    if (Directory.Exists(nazwaNOWEGOfolderuStringsTransifexcomTXT) == true)
                    {
                        var losujliczbe = new Random();
                        var bity = new byte[5];
                        losujliczbe.NextBytes(bity);

                        Directory.Move(nazwaNOWEGOfolderuStringsTransifexcomTXT, nazwaNOWEGOfolderuStringsTransifexcomTXT.Replace("NOWY", "STARY") + "_" + losujliczbe.Next(1000, 9999));
                    }

                    UtworzStruktureFolderuLokalizacji(podana_wersjaibuild, nazwaNOWEGOfolderuStringsTransifexcomTXT);


                    for (int og = 0; og < zdefiniowanastrukturalokalizacji_listaplikow.Count(); og++)
                    {
                        string sciezkadodanegoplikuCS = zdefiniowanastrukturalokalizacji_listaplikow[og];
                        string sciezkadodanegoplikustringsTransifexcomTXT = sciezkadodanegoplikuCS + ".stringsTransifexCOM.txt";
                        string nazwaplikuCS = "";
                        string[] sciezka_podzial = sciezkadodanegoplikuCS.Split(sc);
                        for (int np = 0; np < sciezka_podzial.Length; np++)
                        {
                            if (sciezka_podzial[np].Contains(".cs") == true)
                            {
                                nazwaplikuCS = sciezka_podzial[np];
                            }
                        }

                        if (File.Exists(nazwafolderuCS + sc + sciezkadodanegoplikuCS) == true)
                        {
                            uint plik_CS_liczbalinii = PoliczLiczbeLinii(nazwafolderuCS + sc + sciezkadodanegoplikuCS);

                            //Console.WriteLine("Istnieje podany plik.");
                            FileStream plik_CS_fs = new FileStream(nazwafolderuCS + sc + sciezkadodanegoplikuCS, FileMode.Open, FileAccess.Read);
                            //if (File.Exists(nazwafolderuCS + sc + sciezkadodanegoplikuCS) == true) { File.Delete(nazwafolderuCS + sc + sciezkadodanegoplikuCS); }
                            FileStream nowy_plik_transifexCOMstringstxt_fs = new FileStream(nazwaNOWEGOfolderuStringsTransifexcomTXT + sc + sciezkadodanegoplikustringsTransifexcomTXT, FileMode.Create, FileAccess.ReadWrite);

                            try
                            {

                                StreamReader plik_CS_sr = new StreamReader(plik_CS_fs);
                                StreamWriter nowy_plik_transifexCOMstringstxt_sw = new StreamWriter(nowy_plik_transifexCOMstringstxt_fs);

                                int plik_CS_linia = 1;
                                while (plik_CS_sr.Peek() != -1)
                                {
                                    int aktualny_indeks = 1;

                                    string tresc_linii_CS = plik_CS_sr.ReadLine();

                                    /* MANUALNA "NAPRAWA" TREŚCI LINII - POCZĄTEK */
                                    tresc_linii_CS = tresc_linii_CS.Replace("{\"LEEEROOOOOOOYYY JEEEENKINS!!!\".color(RGBA.maroon)}", "LEEEROOOOOOOYYY JEEEENKINS!!!");
                                    /* MANUALNA "NAPRAWA" TREŚCI LINII - KONIEC */


                                    if (tresc_linii_CS.Contains('"') == true)
                                    {
                                        /*
                                        if
                                        (
                                            tresc_linii_CS.Contains("string") == true ||
                                            tresc_linii_CS.Contains("Label(") == true ||
                                            tresc_linii_CS.Contains("HStack(") == true ||
                                            tresc_linii_CS.Contains("ActionButton(") == true ||
                                            tresc_linii_CS.Contains("Toggle(") == true ||
                                            tresc_linii_CS.Contains("KeyBindPicker(") == true || //!!!POMINĄĆ 1 PARAMETR (tj. indeks==1)
                                            tresc_linii_CS.Contains("NonBindableActionButton(") == true ||
                                            tresc_linii_CS.Contains("ModifierPicker(") == true || //!!!POMINĄĆ 1 PARAMETR (tj. indeks==1)
                                            tresc_linii_CS.Contains("Mod.Warn(") == true ||
                                            tresc_linii_CS.Contains("var") == true ||
                                            tresc_linii_CS.Contains("LogSlider(") == true ||
                                            tresc_linii_CS.Contains("Slider(") == true ||
                                            tresc_linii_CS.Contains("ToggleCallback(") == true ||
                                            tresc_linii_CS.Contains("EnumGrid(") == true

                                        )
                                        */
                                        {
                                            string[] linia_podzial_1 = tresc_linii_CS.Split(new char[] { '"' });

                                            int linia_podzial_1_Length = linia_podzial_1.Length;


                                            if (linia_podzial_1_Length > 1)
                                            {

                                                for (int iz1 = 0; iz1 < linia_podzial_1_Length; iz1++)
                                                {
                                                    if (iz1 > 0 && (iz1 % 2 != 0))
                                                    {
                                                        string zapisz_tresc_linii = "<" + plik_CS_linia + ":" + aktualny_indeks + ">" + linia_podzial_1[iz1];

                                                        zapisz_tresc_linii = zapisz_tresc_linii.Replace("\"", "'");

                                                        if ((tresc_linii_CS.Contains("KeyBindPicker(") == true && aktualny_indeks == 1)
                                                            ||
                                                            (tresc_linii_CS.Contains("ModifierPicker(") == true && aktualny_indeks == 1))
                                                        {
                                                            //NIE ZAPISUJ
                                                        }
                                                        else
                                                        {
                                                            nowy_plik_transifexCOMstringstxt_sw.WriteLine(zapisz_tresc_linii);
                                                        }


                                                        aktualny_indeks = aktualny_indeks + 2;
                                                    }
                                                }



                                            }

                                        }

                                    }


                                    Console.WriteLine("[" + sciezkadodanegoplikustringsTransifexcomTXT + "] Trwa konwertowanie linii nr. " + plik_CS_linia + "/" + plik_CS_liczbalinii + " [" + PoliczPostepWProcentach(plik_CS_linia, plik_CS_liczbalinii) + "%]");

                                    plik_CS_linia++;
                                }




                                nowy_plik_transifexCOMstringstxt_sw.Close();
                                plik_CS_sr.Close();


                            }
                            catch
                            {
                                Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do plików (og==" + og + ").");
                            }

                            nowy_plik_transifexCOMstringstxt_fs.Close();
                            plik_CS_fs.Close();

                        }
                        else
                        {
                            Blad("BŁĄD: Nie istnieje plik: " + nazwafolderuCS + sc + sciezkadodanegoplikuCS + " (og==" + og + ")");
                        }

                    }

                }
                else
                {
                    Blad("Błąd: Struktura wskazanego folderu lokalizacji o nazwie \"" + nazwafolderuCS + "\" różni się od zdefiniowanej struktury folderu lokalizacji ToyBox lub nie jest zdefiniowana w tej wersji narzędzia.");
                }

            }
            else
            {
                Blad("BŁĄD: Nie istnieje wskazany folder o nazwie: " + nazwafolderuCS);
            }

            bool nieutworzonowszystkichplikow = false;
            int ilosc_utworzonychplikowStringsTransifexcomTXT = 0;
            for (int sip = 0; sip < zdefiniowanastrukturalokalizacji_listaplikow.Count(); sip++)
            {
                if (File.Exists(nazwaNOWEGOfolderuStringsTransifexcomTXT + sc + zdefiniowanastrukturalokalizacji_listaplikow[sip] + ".stringsTransifexCOM.txt") == false)
                {
                    nieutworzonowszystkichplikow = true;

                }
                else
                {
                    ilosc_utworzonychplikowStringsTransifexcomTXT = ilosc_utworzonychplikowStringsTransifexcomTXT + 1;
                }
            }

            if (nieutworzonowszystkichplikow == false)
            {
                Sukces("Utworzono nowy folder StringsTransifex.com.TXT o nazwie \"" + nazwaNOWEGOfolderuStringsTransifexcomTXT + "\" i utworzono " + ilosc_utworzonychplikowStringsTransifexcomTXT + " z " + zdefiniowanastrukturalokalizacji_listaplikow.Count() + " plików StringsTransifexCOM.txt");
            }
            else
            {
                Blad("Utworzono nowy folder StringsTransifex.com.TXT o nazwie \"" + nazwaNOWEGOfolderuStringsTransifexcomTXT + "\", ale nie utworzono wszystkich wymaganych plików StringsTransifexCOM.txt (" + ilosc_utworzonychplikowStringsTransifexcomTXT + " z " + zdefiniowanastrukturalokalizacji_listaplikow.Count() + ")!");
            }


        }

        public static void ENplusPL_2FolderyTXTTransifexCOMtoFolderCS_ZNumeramiLiniiZPlikuCS()
        {

            string folderCS_oryginalnyEN_nazwa;
            string folderstringsTransifexcomTXTEN_nazwa;
            string folderstringsTransifexcomTXTPL_nazwa;
            string podana_wersjaibuild;


            Console.Write("Podaj nazwę oryginalnego folderu CS ToyBoxa w języku EN: ");
            folderCS_oryginalnyEN_nazwa = Console.ReadLine();
            Console.Write("Podaj nazwę folderu TXT przeznaczonego dla Transifex jako język źródłowy EN: ");
            folderstringsTransifexcomTXTEN_nazwa = Console.ReadLine();
            Console.Write("Podaj nazwę folderu TXT pochodzącego z Transifex w języku PL: ");
            folderstringsTransifexcomTXTPL_nazwa = Console.ReadLine();
            Console.Write("Podaj numer wersji ToyBoxa z numerem buildu w nawiasie, np. 1.4.17(202210040215): ");
            podana_wersjaibuild = Console.ReadLine();

            List<string> zdefiniowanastrukturalokalizacjidlaStringow_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(podana_wersjaibuild);
            List<string> zdefiniowanastrukturalokalizacjidlaStalych_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStalych(podana_wersjaibuild);

            string nowyfolderCS_nazwa = "NOWY_CS_" + folderCS_oryginalnyEN_nazwa.Replace("EN", "PL");

            if
            (
                Directory.Exists(folderCS_oryginalnyEN_nazwa) == true &&
                Directory.Exists(folderstringsTransifexcomTXTEN_nazwa) == true &&
                Directory.Exists(folderstringsTransifexcomTXTPL_nazwa) == true
            )
            {

                if
                (
                    Convert.ToBoolean(PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild, "CS", folderCS_oryginalnyEN_nazwa)[0]) == true
                    && Convert.ToBoolean(PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild, "StringsTransifexcomTXT", folderstringsTransifexcomTXTEN_nazwa)[0]) == true
                    && Convert.ToBoolean(PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild, "StringsTransifexcomTXT", folderstringsTransifexcomTXTPL_nazwa)[0]) == true
                )
                {
                    //----------------1----------------
                    //PODMIENIANIE STRING'ÓW - POCZĄTEK
                    //----------------1----------------

                    if (Directory.Exists(nowyfolderCS_nazwa) == true)
                    {
                        var losujliczbe = new Random();
                        var bity = new byte[5];
                        losujliczbe.NextBytes(bity);

                        Directory.Move(nowyfolderCS_nazwa, nowyfolderCS_nazwa.Replace("NOWY", "STARY") + "_" + losujliczbe.Next(1000, 9999));
                    }

                    UtworzStruktureFolderuLokalizacji(podana_wersjaibuild, nowyfolderCS_nazwa, true);


                    for (int og = 0; og < zdefiniowanastrukturalokalizacjidlaStringow_listaplikow.Count(); og++)
                    {
                        string sciezkadodanegoplikuCS = zdefiniowanastrukturalokalizacjidlaStringow_listaplikow[og];
                        string sciezkadodanegoplikustringsTransifexcomTXT = sciezkadodanegoplikuCS + ".stringsTransifexCOM.txt";
                        string nazwaplikuCS = "";
                        string nazwaplikuStringsTransifexcomTXT = "";
                        string[] sciezka_podzial = sciezkadodanegoplikuCS.Split(sc);
                        for (int np = 0; np < sciezka_podzial.Length; np++)
                        {
                            if (sciezka_podzial[np].Contains(".cs") == true)
                            {
                                nazwaplikuCS = sciezka_podzial[np];
                                nazwaplikuStringsTransifexcomTXT = sciezka_podzial[np] + ".stringsTransifexCOM.txt";

                            }
                        }


                        List<RekordTXT> plikCS_oryginalnyEN_zawartosclinii = WczytajDaneZPlikuDoListy("CS", folderCS_oryginalnyEN_nazwa + sc + sciezkadodanegoplikuCS);
                        List<RekordTXT> plikstringsTransifexcomTXTEN_zawartosclinii = WczytajDaneZPlikuDoListy("stringsTransifexcomTXT", folderstringsTransifexcomTXTEN_nazwa + sc + sciezkadodanegoplikustringsTransifexcomTXT);
                        List<RekordTXT> plikstringsTransifexcomTXTPL_zawartosclinii = WczytajDaneZPlikuDoListy("stringsTransifexcomTXT", folderstringsTransifexcomTXTPL_nazwa + sc + sciezkadodanegoplikustringsTransifexcomTXT);


                        if (plikstringsTransifexcomTXTEN_zawartosclinii.Count() == plikstringsTransifexcomTXTPL_zawartosclinii.Count())
                        {
                            FileStream nowyplikCS_fs = new FileStream(nowyfolderCS_nazwa + sc + sciezkadodanegoplikuCS, FileMode.Create, FileAccess.ReadWrite);
                            StreamWriter nowyplikCS_sw = new StreamWriter(nowyplikCS_fs);

                            int numer_aktualnejlinii;

                            for (int op1 = 0; op1 < plikCS_oryginalnyEN_zawartosclinii.Count(); op1++)
                            {
                                numer_aktualnejlinii = op1 + 1;
                                string aktualnalinia_zawartosc = plikCS_oryginalnyEN_zawartosclinii[op1].String;


                                List<RekordTXT> znalezionyrekord_EN = plikstringsTransifexcomTXTEN_zawartosclinii.FindAll(x => x.NumerLinii == numer_aktualnejlinii);
                                List<RekordTXT> znalezionyrekord_PL = plikstringsTransifexcomTXTPL_zawartosclinii.FindAll(x => x.NumerLinii == numer_aktualnejlinii);

                                for (int opd = 0; opd < znalezionyrekord_EN.Count(); opd++)
                                {
                                    if (znalezionyrekord_EN[opd].String != "" && znalezionyrekord_PL[opd].String != "")
                                    {
                                        aktualnalinia_zawartosc = aktualnalinia_zawartosc.Replace("\"" + znalezionyrekord_EN[opd].String + "\"", "\"" + znalezionyrekord_PL[opd].String + "\"");
                                    }
                                }

                                nowyplikCS_sw.WriteLine(aktualnalinia_zawartosc);

                            }

                            nowyplikCS_sw.Close();
                            nowyplikCS_fs.Close();

                            //Sukces("Utworzono nowy plik kodu źródłowego ToyBox o nazwie \"" + nazwaplikuCS + "\" z zaimplementowanym językiem PL.");


                        }
                        else
                        {
                            Blad("BŁĄD: Ilość linii w plikach \"" + folderstringsTransifexcomTXTEN_nazwa + sc + sciezkadodanegoplikustringsTransifexcomTXT + "\" i \"" + folderstringsTransifexcomTXTPL_nazwa + sc + sciezkadodanegoplikustringsTransifexcomTXT + "\" + plikstringsTransifexcomTXTPL_nazwa + \"\" nie zgadza się.");
                        }

                    }
                    //----------------1----------------
                    //PODMIENIANIE STRING'ÓW - KONIEC
                    //----------------1----------------


                    
                    
                    //----------------2----------------
                    //PODMIENIANIE STAŁYCH - POCZĄTEK
                    //----------------2----------------
                    List<ToyBox_Stala> listastalych = PobierzListeStalychKtoreMuszaZostacPodmienione(podana_wersjaibuild);

                    for (int ls1 = 0; ls1 < listastalych.Count; ls1++)
                    {
                        if (File.Exists(nowyfolderCS_nazwa + sc + listastalych[ls1].PlikCS) == false)
                        {
                            if (File.Exists(folderCS_oryginalnyEN_nazwa + sc + listastalych[ls1].PlikCS) == true)
                            {
                                FileStream plikCSorig_fs = new FileStream(folderCS_oryginalnyEN_nazwa + sc + listastalych[ls1].PlikCS, FileMode.Open, FileAccess.Read);
                                StreamReader plikCSorig_sr = new StreamReader(plikCSorig_fs);

                                string zrodloplikuCS = plikCSorig_sr.ReadToEnd().Replace(listastalych[ls1].String_EN, listastalych[ls1].String_PL);

                                //Console.WriteLine("[DEBUG] zrodloplikuCS: " + zrodloplikuCS);
                                //Informacja("[DEBUG] listastalych[ls1].String_EN:" + listastalych[ls1].String_EN);

                                plikCSorig_sr.Close();
                                plikCSorig_fs.Close();


                                FileStream plikCS_fs = new FileStream(nowyfolderCS_nazwa + sc + listastalych[ls1].PlikCS, FileMode.Create, FileAccess.Write);
                                StreamWriter plikCS_sw = new StreamWriter(plikCS_fs);

                                plikCS_sw.Write(zrodloplikuCS);

                                plikCS_sw.Close();
                                plikCS_fs.Close();

                            }

                        }
                        else if (File.Exists(nowyfolderCS_nazwa + sc + listastalych[ls1].PlikCS) == true)
                        {
                            FileStream plikCSprzedzmiana_fs = new FileStream(nowyfolderCS_nazwa + sc + listastalych[ls1].PlikCS, FileMode.Open, FileAccess.Read);
                            StreamReader plikCSprzedzmiana_sr = new StreamReader(plikCSprzedzmiana_fs);

                            string zrodloplikuCS = plikCSprzedzmiana_sr.ReadToEnd().Replace(listastalych[ls1].String_EN, listastalych[ls1].String_PL);

                            plikCSprzedzmiana_sr.Close();
                            plikCSprzedzmiana_fs.Close();


                            FileStream plikCSpozmianie_fs = new FileStream(nowyfolderCS_nazwa + sc + listastalych[ls1].PlikCS, FileMode.Open, FileAccess.Write);
                            StreamWriter plikCSpozmianie_sw = new StreamWriter(plikCSpozmianie_fs);

                            plikCSpozmianie_sw.Write(zrodloplikuCS);

                            plikCSpozmianie_sw.Close();
                            plikCSpozmianie_fs.Close();


                        }


                    }

                    listastalych.Clear();
                    //----------------2----------------
                    //PODMIENIANIE STAŁYCH - KONIEC
                    //----------------2----------------
                    


                }
                else
                {
                    Blad("Błąd: Struktura przynajmniej jednego ze wskazanych folderów lokalizacji (o nazwach \"" + folderCS_oryginalnyEN_nazwa + "\", \"" + folderstringsTransifexcomTXTEN_nazwa + "\" i \"" + folderstringsTransifexcomTXTPL_nazwa + "\") różni się od zdefiniowanej struktury folderu lokalizacji ToyBox lub nie jest ona zdefiniowana w narzędziu.");
                }


            }
            else
            {
                Blad("BŁĄD: Nie istnieje przynajmniej jeden z podanych folderów.");
            }


            bool nieutworzonowszystkichplikow = false;
            int ilosc_utworzonychplikowCSdlaStringow = 0;
            for (int sip = 0; sip < zdefiniowanastrukturalokalizacjidlaStringow_listaplikow.Count(); sip++)
            {
                if (File.Exists(nowyfolderCS_nazwa + sc + zdefiniowanastrukturalokalizacjidlaStringow_listaplikow[sip]) == false)
                {
                    nieutworzonowszystkichplikow = true;

                }
                else
                {
                    ilosc_utworzonychplikowCSdlaStringow = ilosc_utworzonychplikowCSdlaStringow + 1;
                }
            }
            int ilosc_utworzonychplikowCSdlaStalych = 0;
            for (int sip2 = 0; sip2 < zdefiniowanastrukturalokalizacjidlaStalych_listaplikow.Count(); sip2++)
            {
                if (File.Exists(nowyfolderCS_nazwa + sc + zdefiniowanastrukturalokalizacjidlaStalych_listaplikow[sip2]) == false)
                {
                    nieutworzonowszystkichplikow = true;

                }
                else
                {
                    ilosc_utworzonychplikowCSdlaStalych = ilosc_utworzonychplikowCSdlaStalych + 1;
                }
            }


            if (nieutworzonowszystkichplikow == false)
            {
                Sukces("Utworzono nowy folder CS o nazwie \"" + nowyfolderCS_nazwa + "\" i utworzono " + ilosc_utworzonychplikowCSdlaStringow + " z " + zdefiniowanastrukturalokalizacjidlaStringow_listaplikow.Count() + " plików CS dla String'ów oraz utworzono lub zmodyfikowano " + ilosc_utworzonychplikowCSdlaStalych + " z " + zdefiniowanastrukturalokalizacjidlaStalych_listaplikow.Count() + " plików CS dla Stałych.");
            }
            else
            {
                Blad("Próbowano utworzyć folder CS o nazwie \"" + nowyfolderCS_nazwa + "\", ale nie utworzono wszystkich wymaganych plików CS dla Stringów (" + ilosc_utworzonychplikowCSdlaStringow + " z " + zdefiniowanastrukturalokalizacjidlaStringow_listaplikow.Count() + ") lub nie utworzono/nie zmodyfikowano wszystkich wymaganych plików CS dla Stałych (" + ilosc_utworzonychplikowCSdlaStalych + " z " + zdefiniowanastrukturalokalizacjidlaStalych_listaplikow.Count() + ")!");
            }

        }

        public static void WygenerujPlikKonfiguracyjnyDlaNarzedziaTX()
        {
            string organizacja_nazwa;
            string projekt_nazwa;
            string podana_wersjaibuild;

            Console.Write("Podaj nazwę organizacji: ");
            organizacja_nazwa = Console.ReadLine();
            Console.Write("Podaj nazwę projektu: ");
            projekt_nazwa = Console.ReadLine();
            Console.Write("Podaj numer wersji ToyBoxa z numerem buildu w nawiasie, np. 1.4.17(202210040215): ");
            podana_wersjaibuild = Console.ReadLine();


            if (organizacja_nazwa != "" && projekt_nazwa != "")
            {
                string TXplikkonf_nazwa = "tx-ToyBox_PL" + podana_wersjaibuild + "-config";
                List<string> zdefiniowanastrukturalokalizacji_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(podana_wersjaibuild);

                if (File.Exists(TXplikkonf_nazwa) == true) { File.Delete(TXplikkonf_nazwa); }

                FileStream TXplikkonf_fs = new FileStream(TXplikkonf_nazwa, FileMode.Create, FileAccess.Write);

                try
                {
                    StreamWriter TXplikconf_sw = new StreamWriter(TXplikkonf_fs);

                    TXplikconf_sw.WriteLine("[main]");
                    TXplikconf_sw.WriteLine("host = https://www.transifex.com");
                    TXplikconf_sw.WriteLine("");


                    for (int zd = 0; zd < zdefiniowanastrukturalokalizacji_listaplikow.Count; zd++)
                    {
                        string[] podzial1 = zdefiniowanastrukturalokalizacji_listaplikow[zd].Split(sc);

                        string zasob_nazwa;
                        if (podzial1.Length > 1)
                        {
                            zasob_nazwa = podzial1[podzial1.Length - 1];
                        }
                        else
                        {
                            zasob_nazwa = podzial1[0];
                        }
                        zasob_nazwa = zasob_nazwa + "stringstransifexcomtxt";

                        TXplikconf_sw.WriteLine("[o:" + organizacja_nazwa + ":p:" + projekt_nazwa + ":r:" + zasob_nazwa.ToLower().Replace(".", "").Replace("+", "") + "]");
                        TXplikconf_sw.WriteLine("file_filter = <lang>/" + zdefiniowanastrukturalokalizacji_listaplikow[zd].Replace(sc.ToString(), "/") + ".stringsTransifexCOM.txt");
                        TXplikconf_sw.WriteLine("source_file = ");
                        TXplikconf_sw.WriteLine("source_lang = en");
                        TXplikconf_sw.WriteLine("type = TXT");
                        TXplikconf_sw.WriteLine("minimum_perc = 0");
                        TXplikconf_sw.WriteLine("");

                    }

                    TXplikconf_sw.Close();

                    Sukces("Wygenerowano plik konfiguracyjny o nazwie " + TXplikkonf_nazwa + " dla TX potrzebny do zautomatyzowanego pobierania zasobów lokalizacji ToyBox z serwerów Transifex.com.");

                }
                catch
                {
                    Blad("BŁĄD: Wystapił nieoczekiwany błąd w dostępie do pliku: \"" + TXplikkonf_nazwa + "\".");
                }

                TXplikkonf_fs.Close();


            }
            else
            {
                Blad("Nie podano nazwy organizacji lub nazwy projektu.");
            }


        }

        public static void PrzygotowywanieNowszejWersjiWrazZPrzeniesieniemDoNiejPrzetlumaczonychLiniiZPoprzedniejWersji()
        {

            string folderstringsTransifexcomTXToldEN_nazwa;
            string folderstringsTransifexcomTXTnewEN_nazwa;
            string folderstringsTransifexcomTXToldPL_nazwa;

            string podana_wersjaibuild_starejwersji;
            string podana_wersjaibuild_nowejwersji;

            Console.Write("Podaj nazwę folderu TXT przeznaczonego dla Transifex jako język źródłowy EN starej wersji: ");
            folderstringsTransifexcomTXToldEN_nazwa = Console.ReadLine();
            Console.Write("Podaj nazwę folderu TXT przeznaczonego dla Transifex jako język źródłowy EN nowej wersji: ");
            folderstringsTransifexcomTXTnewEN_nazwa = Console.ReadLine();
            Console.Write("Podaj nazwę folderu TXT pochodzącego z Transifex w języku PL starej wersji: ");
            folderstringsTransifexcomTXToldPL_nazwa = Console.ReadLine();

            Console.Write("Podaj numer starej wersji ToyBoxa z numerem buildu w nawiasie, np. 1.4.17(202210040215): ");
            podana_wersjaibuild_starejwersji = Console.ReadLine();
            Console.Write("Podaj numer nowej wersji ToyBoxa z numerem buildu w nawiasie, np. 1.4.17(202210040215): ");
            podana_wersjaibuild_nowejwersji = Console.ReadLine();

            List<string> zdefiniowanastrukturalokalizacji_starejwersji_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(podana_wersjaibuild_starejwersji); ;
            List<string> zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow = PobierzZdefiniowanaStruktureFolderuCSLokalizacjiDlaStringow(podana_wersjaibuild_nowejwersji); ;

            string nowyfolderstringsTransifexcomTXTnewPL_nazwa = "NOWY_TXT_" + folderstringsTransifexcomTXTnewEN_nazwa.Replace("EN", "PL");
            string nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa = "NOWY_TXT_Z_OZNACZONYMI_NOWYMILINIAMINOWEJWERSJI_" + folderstringsTransifexcomTXTnewEN_nazwa.Replace("EN", "PL");

            string komunikat_OZNACZENIAnowychliniinowejwersji = "***TE LINIĘ DODANO W WERSJI TOYBOXA: " + podana_wersjaibuild_nowejwersji + "***";

            if
            (
                Directory.Exists(folderstringsTransifexcomTXToldEN_nazwa) == true &&
                Directory.Exists(folderstringsTransifexcomTXTnewEN_nazwa) == true &&
                Directory.Exists(folderstringsTransifexcomTXToldPL_nazwa) == true
            )
            {
                if
                (
                    Convert.ToBoolean(PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild_starejwersji, "StringsTransifexcomTXT", folderstringsTransifexcomTXToldEN_nazwa)[0]) == true
                    && Convert.ToBoolean(PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild_nowejwersji, "StringsTransifexcomTXT", folderstringsTransifexcomTXTnewEN_nazwa)[0]) == true
                    && Convert.ToBoolean(PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild_starejwersji, "StringsTransifexcomTXT", folderstringsTransifexcomTXToldPL_nazwa)[0]) == true
                )
                {

                    if (Directory.Exists(nowyfolderstringsTransifexcomTXTnewPL_nazwa) == true)
                    {
                        var losujliczbe = new Random();
                        var bity = new byte[5];
                        losujliczbe.NextBytes(bity);

                        Directory.Move(nowyfolderstringsTransifexcomTXTnewPL_nazwa, nowyfolderstringsTransifexcomTXTnewPL_nazwa.Replace("NOWY", "STARY") + "_" + losujliczbe.Next(1000, 9999));
                    }

                    if (Directory.Exists(nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa) == true)
                    {
                        var losujliczbe = new Random();
                        var bity = new byte[5];
                        losujliczbe.NextBytes(bity);

                        Directory.Move(nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa, nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa.Replace("NOWY", "STARY") + "_" + losujliczbe.Next(1000, 9999));
                    }

                    UtworzStruktureFolderuLokalizacji(podana_wersjaibuild_nowejwersji, nowyfolderstringsTransifexcomTXTnewPL_nazwa);
                    UtworzStruktureFolderuLokalizacji(podana_wersjaibuild_nowejwersji, nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa);

                    //Console.WriteLine("[DEBUG] zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow.Count()==" + zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow.Count());

                    for (int og = 0; og < zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow.Count(); og++)
                    {
                        string sciezkaplikustringsTransifexcomTXT_nowejwersji = zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow[og].Replace(".cs", ".cs.stringsTransifexCOM.txt");
                        string nazwaplikuStringsTransifexcomTXT = "";
                        string[] sciezka_podzial = sciezkaplikustringsTransifexcomTXT_nowejwersji.Split(sc);
                        for (int np = 0; np < sciezka_podzial.Length; np++)
                        {
                            if (sciezka_podzial[np].Contains(".cs.stringsTransifexCOM.txt") == true)
                            {
                                nazwaplikuStringsTransifexcomTXT = sciezka_podzial[np];
                            }
                        }
                        
                        FileStream nowyplikStringsTransifexcomTXTnewPL_fs = new FileStream(nowyfolderstringsTransifexcomTXTnewPL_nazwa + sc + sciezkaplikustringsTransifexcomTXT_nowejwersji, FileMode.Create, FileAccess.Write);
                        StreamWriter nowyplikStringsTransifexcomTXTnewPL_sw = new StreamWriter(nowyplikStringsTransifexcomTXTnewPL_fs);

                        FileStream nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_fs = new FileStream(nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa + sc + sciezkaplikustringsTransifexcomTXT_nowejwersji, FileMode.Create, FileAccess.Write);
                        StreamWriter nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_sw = new StreamWriter(nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_fs);


                        List<RekordTXT> plikstringsTransifexcomTXTnewEN_zawartoscilinii = WczytajDaneZPlikuDoListy("stringsTransifexcomTXT", folderstringsTransifexcomTXTnewEN_nazwa + sc + sciezkaplikustringsTransifexcomTXT_nowejwersji);


                        if (File.Exists(folderstringsTransifexcomTXToldEN_nazwa + sc + sciezkaplikustringsTransifexcomTXT_nowejwersji) == true)
                        {
                            //Console.WriteLine("[DEBUG] folderstringsTransifexcomTXToldEN_nazwa{sc}sciezkaplikustringsTransifexcomTXT_nowejwersji==" + folderstringsTransifexcomTXToldEN_nazwa + sc + sciezkaplikustringsTransifexcomTXT_nowejwersji);

                            string sciezkaplikustringsTransifexcomTXT_starejwersji;
                            List<string> wyszukanasciezkadodanegoplikulokalizacji_starejwersji = zdefiniowanastrukturalokalizacji_starejwersji_listaplikow.FindAll(x => x == zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow[og]);

                            if (wyszukanasciezkadodanegoplikulokalizacji_starejwersji.Count() == 1)
                            {
                                sciezkaplikustringsTransifexcomTXT_starejwersji = sciezkaplikustringsTransifexcomTXT_nowejwersji;

                                List<RekordTXT> plikstringsTransifexcomTXToldEN_zawartoscilinii = WczytajDaneZPlikuDoListy("stringsTransifexcomTXT", folderstringsTransifexcomTXToldEN_nazwa + sc + sciezkaplikustringsTransifexcomTXT_starejwersji);
                                List<RekordTXT> plikstringsTransifexcomTXToldPL_zawartoscilinii = WczytajDaneZPlikuDoListy("stringsTransifexcomTXT", folderstringsTransifexcomTXToldPL_nazwa + sc + sciezkaplikustringsTransifexcomTXT_starejwersji);

                                if (plikstringsTransifexcomTXToldEN_zawartoscilinii.Count() == plikstringsTransifexcomTXToldPL_zawartoscilinii.Count())
                                {
                                    int ilosc_wszystkichlinii = plikstringsTransifexcomTXTnewEN_zawartoscilinii.Count();
                                    int numer_aktualnejlinii;

                                    //Console.WriteLine("[DEBUG] plikstringsTransifexcomTXTnewEN_zawartoscilinii.Count()==" + plikstringsTransifexcomTXTnewEN_zawartoscilinii.Count() + " (w: og==" + og + " tj. w pliku \"" + folderstringsTransifexcomTXTnewEN_nazwa + sc + sciezkaplikustringsTransifexcomTXT_nowejwersji + "\")");

                                    for (int op1 = 0; op1 < ilosc_wszystkichlinii; op1++)
                                    {
                                        numer_aktualnejlinii = op1 + 1;
                                        string aktualnalinianewEN_zawartosc = plikstringsTransifexcomTXTnewEN_zawartoscilinii[op1].String;

                                        List<RekordTXT> znalezionyrekord_oldEN = plikstringsTransifexcomTXToldEN_zawartoscilinii.FindAll(x => x.String == aktualnalinianewEN_zawartosc);

                                        bool ta_liniazostaladodanawnowejwersji = false;

                                        if (znalezionyrekord_oldEN.Count() == 0)
                                        {
                                            //nic nie rób - tj. nie nadpisuj wartości zmiennej aktualnalinianewEN_zawartosc

                                            //oznacz tę linię jako nowododaną w nowej wersji ToyBoxa dla pliku nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa
                                            ta_liniazostaladodanawnowejwersji = true;
                                        }
                                        else if (znalezionyrekord_oldEN.Count() > 0)
                                        {
                                            List<RekordTXT> znalezionyrekord_oldPL = plikstringsTransifexcomTXToldPL_zawartoscilinii.FindAll(x => (x.NumerLinii == znalezionyrekord_oldEN[0].NumerLinii) && (x.IndeksStringaWDanejLinii == znalezionyrekord_oldEN[0].IndeksStringaWDanejLinii));

                                            /*
                                            for (int dbg1 = 0; dbg1 < znalezionyrekord_oldPL.Count(); dbg1++)
                                            {
                                                Console.WriteLine("[dbg1" + dbg1 + "] znalezionyrekord_oldPL[dbg1]==" + znalezionyrekord_oldPL[dbg1]);
                                            }
                                            */

                                            if (znalezionyrekord_oldPL.Count() == 1)
                                            {
                                                aktualnalinianewEN_zawartosc = znalezionyrekord_oldPL[0].String;

                                                if (znalezionyrekord_oldEN.Count() > 1)
                                                {
                                                    //Informacja("[DEBUG] W pliku \"" + folderstringsTransifexcomTXToldEN_nazwa + sc + sciezkaplikustringsTransifexcomTXT_starejwersji + "\" znaleziono więcej niż 1 string==" + znalezionyrekord_oldEN[0].String);
                                                    //Informacja("[DEBUG] Szczegóły powyższego komunikatu: W nowym pliku w tej linii zapisana zostanie pierwsza znaleziona treść stringa przeniesiona z odpowiadającej linii pliku starej wersji.");
                                                }
                                            }
                                            else
                                            {
                                                Blad("Wystąpił krytyczny wyjątek #3: Znaleziono więcej niż 1 jedną linię o tej samej wartości <NumerLinii:IndeksStringawDanejLinii> w pliku \"" + folderstringsTransifexcomTXToldPL_nazwa + sc + sciezkaplikustringsTransifexcomTXT_starejwersji + "\".");
                                                Blad("Szczegóły powyższego błędu: znalezionyrekord_oldPL.Count()==" + znalezionyrekord_oldPL.Count() + ", og==" + og + ", op1==" + op1);
                                                PotwierdzEnterem();
                                            }
                                        }

                                        string zapisywana_tresclinii = "<" + plikstringsTransifexcomTXTnewEN_zawartoscilinii[op1].NumerLinii + ":" + plikstringsTransifexcomTXTnewEN_zawartoscilinii[op1].IndeksStringaWDanejLinii + ">" + aktualnalinianewEN_zawartosc;

                                        nowyplikStringsTransifexcomTXTnewPL_sw.WriteLine(zapisywana_tresclinii);

                                        if (ta_liniazostaladodanawnowejwersji == false)
                                        {
                                            nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_sw.WriteLine(zapisywana_tresclinii);
                                        }
                                        else if (ta_liniazostaladodanawnowejwersji == true)
                                        {
                                            nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_sw.WriteLine(komunikat_OZNACZENIAnowychliniinowejwersji + zapisywana_tresclinii);
                                        }

                                        Console.WriteLine("[" + sciezkaplikustringsTransifexcomTXT_nowejwersji + "] [" + podana_wersjaibuild_starejwersji + "->" + podana_wersjaibuild_nowejwersji + "] Trwa zapisywanie linii nr. " + numer_aktualnejlinii + "/" + ilosc_wszystkichlinii + " [" + PoliczPostepWProcentach(numer_aktualnejlinii, ilosc_wszystkichlinii) + "%]");

                                    }

                                }
                                else
                                {
                                    Blad("BŁĄD: Ilość linii w plikach \"" + folderstringsTransifexcomTXToldEN_nazwa + sc + sciezkaplikustringsTransifexcomTXT_starejwersji + "\" i \"" + folderstringsTransifexcomTXToldPL_nazwa + sc + sciezkaplikustringsTransifexcomTXT_starejwersji + "\" nie zgadza się.");
                                    PotwierdzEnterem();
                                }

                            }
                            else if (wyszukanasciezkadodanegoplikulokalizacji_starejwersji.Count() > 1)
                            {
                                Blad("Wystąpił krytyczny wyjątek #1: Wykryto więcej niż jedną ścieżkę \"" + folderstringsTransifexcomTXToldPL_nazwa + sc + wyszukanasciezkadodanegoplikulokalizacji_starejwersji[0] + "\" w strukturze folderu lokalizacji.");
                                Blad("Szczegóły powyższego błędu: og==" + og + ", wyszukanasciezkadodanegoplikulokalizacji_starejwersji.Count()==" + wyszukanasciezkadodanegoplikulokalizacji_starejwersji.Count());
                                PotwierdzEnterem();
                            }
                            else
                            {
                                Blad("Wystąpił krytyczny wyjątek #2: Błąd podczas wyszukiwania ścieżki do pliku lokalizacji \"" + folderstringsTransifexcomTXToldPL_nazwa + sc + sciezkaplikustringsTransifexcomTXT_nowejwersji + "\" w strukturze folderu lokalizacji.");
                                Blad("Szczegóły powyższego błędu: og==" + og + ", sciezkaplikustringsTransifexcomTXT_nowejwersji.Count()==" + sciezkaplikustringsTransifexcomTXT_nowejwersji.Count());
                                PotwierdzEnterem();
                            }

                        }
                        else
                        {
                            int ilosc_wszystkichlinii = plikstringsTransifexcomTXTnewEN_zawartoscilinii.Count();
                            int numer_aktualnejlinii;

                            for (int op2 = 0; op2 < plikstringsTransifexcomTXTnewEN_zawartoscilinii.Count(); op2++)
                            {
                                numer_aktualnejlinii = op2 + 1;
                                string aktualnalinianewEN_zawartosc = plikstringsTransifexcomTXTnewEN_zawartoscilinii[op2].String;

                                string zapisywana_tresclinii = "<" + plikstringsTransifexcomTXTnewEN_zawartoscilinii[op2].NumerLinii + ":" + plikstringsTransifexcomTXTnewEN_zawartoscilinii[op2].IndeksStringaWDanejLinii + ">" + aktualnalinianewEN_zawartosc;

                                nowyplikStringsTransifexcomTXTnewPL_sw.WriteLine(zapisywana_tresclinii);
                                nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_sw.WriteLine(komunikat_OZNACZENIAnowychliniinowejwersji + zapisywana_tresclinii);

                                Console.WriteLine("[" + sciezkaplikustringsTransifexcomTXT_nowejwersji + "] [" + podana_wersjaibuild_starejwersji + "->" + podana_wersjaibuild_nowejwersji + "] Trwa zapisywanie linii nr. " + numer_aktualnejlinii + "/" + ilosc_wszystkichlinii + " [" + PoliczPostepWProcentach(numer_aktualnejlinii, ilosc_wszystkichlinii) + "%]");
                            }


                        }

                        nowyplikStringsTransifexcomTXTnewPL_sw.Close();
                        nowyplikStringsTransifexcomTXTnewPL_fs.Close();

                        nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_sw.Close();
                        nowyplikStringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_fs.Close();

                        //Sukces("Utworzono nowy plik .cs.stringsTransifexCOM.txt ToyBox o nazwie \"" + nazwaplikuStringsTransifexcomTXT);

                    }


                }
                else
                {
                    Blad("Błąd: Struktura przynajmniej jednego ze wskazanych folderów (o nazwach \"" + folderstringsTransifexcomTXToldEN_nazwa + "\", \"" + folderstringsTransifexcomTXTnewEN_nazwa + "\" i \"" + folderstringsTransifexcomTXToldPL_nazwa + "\") różni się od zdefiniowanej struktury folderu lokalizacji w stosunku do odpowiadającej wersji ToyBox.");
                }

            }
            else
            {
                Blad("BŁĄD: Nie istnieje przynajmniej jeden z podanych folderów.");
            }


            List<dynamic> spr1 = PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild_nowejwersji, "StringsTransifexcomTXT", nowyfolderstringsTransifexcomTXTnewPL_nazwa);
            bool spr1_czystrukturajestzgodna = Convert.ToBoolean(spr1[0]);
            int spr1_ilosczapisanychplikow = Convert.ToInt32(spr1[1]);

            if (spr1_czystrukturajestzgodna == true)
            {
                Sukces("Utworzono nowy folder StringsTransifex.com.TXT o nazwie \"" + nowyfolderstringsTransifexcomTXTnewPL_nazwa + "\" i zapisano " + spr1_ilosczapisanychplikow + " z " + zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow.Count() + " plików StringsTransifexCOM.txt");
            }
            else
            {
                Blad("Utworzono nowy folder StringsTransifex.com.TXT o nazwie \"" + nowyfolderstringsTransifexcomTXTnewPL_nazwa + "\", ale nie zapisano wszystkich wymaganych plików StringsTransifexCOM.txt (" + spr1_ilosczapisanychplikow + " z " + zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow.Count() + ")!");
            }



            List<dynamic> spr2 = PorownajStruktureFolderuCSZeZdefiniowanaStrukturaLokalizacjiCS(podana_wersjaibuild_nowejwersji, "StringsTransifexcomTXT", nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa);
            bool spr2_czystrukturajestzgodna = Convert.ToBoolean(spr2[0]);
            int spr2_ilosczapisanychplikow = Convert.ToInt32(spr2[1]);

            if (spr2_czystrukturajestzgodna == true)
            {
                Sukces("Utworzono nowy folder StringsTransifex.com.TXT o nazwie \"" + nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa + "\" i zapisano " + spr2_ilosczapisanychplikow + " z " + zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow.Count() + " plików StringsTransifexCOM.txt");
            }
            else
            {
                Blad("Utworzono nowy folder StringsTransifex.com.TXT o nazwie \"" + nowyfolderstringsTransifexcomTXTnewPL_zOZNACZONYMInowymiliniaminowejwersji_nazwa + "\", ale nie zapisano wszystkich wymaganych plików StringsTransifexCOM.txt (" + spr2_ilosczapisanychplikow + " z " + zdefiniowanastrukturalokalizacji_nowejwersji_listaplikow.Count() + ")!");
            }


        }

    }
}
