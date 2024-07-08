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
using System.Runtime.InteropServices;
using static pwrpl_tools.pwrpltools;

namespace pwrpl_tools
{

    internal class PWR_PL
    {
        const string nazwafolderutmp = "tmp";


        //V0 (METODY UNIKATOWE)
        public static bool UtworzNaglowekJSON(string nazwaplikuJSON, string nazwafolderu = "")
        {
            bool rezultat;

            FileStream plikJSON_fs;

            if (nazwafolderu == "")
            {
                plikJSON_fs = new FileStream(folderglownyprogramu + nazwaplikuJSON, FileMode.Create, FileAccess.Write);
            }
            else
            {
                plikJSON_fs = new FileStream(folderglownyprogramu + nazwafolderu + sc + nazwaplikuJSON, FileMode.Create, FileAccess.Write);
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

        public static bool UtworzStopkeJSON(string nazwaplikuJSON, string nazwafolderu = "")
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

            if (File.Exists(folderglownyprogramu + sprawdzenie_istnienia))
            {
                if (nazwafolderu == "")
                {
                    plikJSON_fs = new FileStream(folderglownyprogramu + nazwaplikuJSON, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    plikJSON_fs = new FileStream(folderglownyprogramu + nazwafolderu + sc + nazwaplikuJSON, FileMode.Append, FileAccess.Write);
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

        public static void UtworzPlikTXT_TMP(string nazwa_pliku, List<string> lista_danych, int index_od, int index_do)
        {
            //Console.WriteLine("UtworzPlikTXT_TMP(" + nazwa_pliku + ", " + lista_danych + "," + index_od + ", " + index_do + ")");

            if (Directory.Exists(folderglownyprogramu + nazwafolderutmp) == false)
            {
                Directory.CreateDirectory(nazwafolderutmp);
            }

            if (File.Exists(folderglownyprogramu + nazwafolderutmp + sc + nazwa_pliku) == true)
            {
                File.Delete(folderglownyprogramu + nazwafolderutmp + sc + nazwa_pliku);
            }

            FileStream plikTMP_fs = new FileStream(folderglownyprogramu + nazwafolderutmp + sc + nazwa_pliku, FileMode.Append, FileAccess.Write);

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

        public static void UsunPlikiTMP(List<string> lista_nazw_plikow)
        {
            if (Directory.Exists(folderglownyprogramu + nazwafolderutmp) == true)
            {
                for (int i = 0; i < lista_nazw_plikow.Count; i++)
                {
                    if (File.Exists(folderglownyprogramu + nazwafolderutmp + sc + lista_nazw_plikow[i]) == true)
                    {
                        File.Delete(folderglownyprogramu + nazwafolderutmp + sc + lista_nazw_plikow[i]);
                    }
                }

            }
        }

        public static void ZnajdzIndeksKonkretnegoKluczaWTablicyListKluczyIStringow(dynamic[] tablica_list_kluczy_i_stringow, string szukany_klucz)
        {
            //tablica_list_kluczy_i_stringow[0] to lista kluczy
            //tablica_list_kluczy_i_stringow[1] to lista stringow

            if (tablica_list_kluczy_i_stringow.Length == 2)
            {
                List<dynamic> lista_kluczy = tablica_list_kluczy_i_stringow[0];
                List<List<dynamic>> lista_stringow = tablica_list_kluczy_i_stringow[1];

                int znaleziony_index = lista_kluczy.IndexOf(szukany_klucz);

                Console.WriteLine("znaleziony_index: " + znaleziony_index.ToString());
            }
        }

        public static List<Linia_stringsTransifexCOMTXT> UtworzListeLiniiZPlikuTXTStringsTransifexCOM(string nazwapliku_stringsTransifexCOMtxt)
        {
            List<Linia_stringsTransifexCOMTXT> lista_danych = new List<Linia_stringsTransifexCOMTXT>();

            if (File.Exists(nazwapliku_stringsTransifexCOMtxt) == true)
            {
                FileStream plikstringsTransifexCOMtxt_fs = new FileStream(nazwapliku_stringsTransifexCOMtxt, FileMode.Open, FileAccess.Read);
                try
                {
                    StreamReader plikstringsTransifexCOMtxt_sr = new StreamReader(plikstringsTransifexCOMtxt_fs);

                    int numeraktualnejlinii = 1;
                    while (plikstringsTransifexCOMtxt_sr.Peek() != -1)
                    {
                        int _Index = numeraktualnejlinii - 1;
                        string trescaktualnejlinii = plikstringsTransifexCOMtxt_sr.ReadLine();

                        string plikstringstxt_trescaktualnejlinii_ID;
                        string plikstringstxt_trescaktualnejlinii_STRING;

                        string[] tmp1 = trescaktualnejlinii.Split(new char[] { '>' });

                        if (tmp1.Length > 0)
                        {
                            plikstringstxt_trescaktualnejlinii_STRING = trescaktualnejlinii;

                            //Console.WriteLine("[DEBUG] tmp1[0]==" + tmp1[0]);

                            string tmp2 = tmp1[0].TrimStart().Remove(0, 1);

                            //Console.WriteLine("[DEBUG] tmp2==" + tmp2);

                            string tmp3 = "<" + tmp2 + ">";
                            int tmp4 = tmp3.Length;

                            plikstringstxt_trescaktualnejlinii_ID = tmp2;
                            plikstringstxt_trescaktualnejlinii_STRING = plikstringstxt_trescaktualnejlinii_STRING.Remove(0, tmp4);

                            lista_danych.Add(new Linia_stringsTransifexCOMTXT() { Index = _Index, ID = int.Parse(plikstringstxt_trescaktualnejlinii_ID), String = plikstringstxt_trescaktualnejlinii_STRING });
                        }
                        else
                        {
                            Blad("BŁĄD: Nie wykryto identyfikatora linii w pliku \"" + nazwapliku_stringsTransifexCOMtxt + "\" w linii nr.: " + numeraktualnejlinii);
                        }

                        numeraktualnejlinii++;
                    }

                    plikstringsTransifexCOMtxt_sr.Close();
                }
                catch
                {
                    Blad("BŁĄD: Wystąpił nieoczekiwany problem w dostępie do pliku: " + nazwapliku_stringsTransifexCOMtxt);
                }
                plikstringsTransifexCOMtxt_fs.Close();

            }
            else
            {
                Blad("BŁĄD: Nie istnieje plik o nazwie \"" + nazwapliku_stringsTransifexCOMtxt + "\".");
            }

            return lista_danych;
        }

        public static List<RekordJSON> WczytajDaneZPlikuJSONdoListyRekordow(string nazwa_pliku_JSON)
        {
            List<RekordJSON> danezplikuJSON_listarekordow = new List<RekordJSON>();

            if (File.Exists(nazwa_pliku_JSON))
            {

                dynamic[] danezplikuJSON_tablicalistdanych = JSON.NET6.WczytajStaleIIchWartosciZPlikuJSON(nazwa_pliku_JSON);

                List<dynamic> danezplikuJSON_listakluczy = danezplikuJSON_tablicalistdanych[0];
                List<List<dynamic>> danezplikuJSON_listastringow = danezplikuJSON_tablicalistdanych[1];

                for (int i2b = 0; i2b < danezplikuJSON_listakluczy.Count(); i2b++)
                {

                    if (i2b != 0 && i2b != 1) //odfiltrowanie pierwszych dwóch rekordów zawierających słowa, wczytane z pliku JSON, takie jak: "$id", "string", "1"
                    {
                        for (int i2c = 0; i2c < danezplikuJSON_listastringow[i2b].Count(); i2c++)
                        {

                            int _ID = i2b + 2;
                            string _Plik = nazwa_pliku_JSON;
                            string _Klucz = danezplikuJSON_listakluczy[i2b];
                            string _String = FiltrujString(danezplikuJSON_listastringow[i2b][i2c]);

                            //Console.WriteLine("[DEBUG] " + _ID + "|" + _Plik + "|" + _Klucz + "|" + _String);

                            danezplikuJSON_listarekordow.Add(new RekordJSON { ID = _ID, Plik = _Plik, Klucz = _Klucz, String = _String });

                        }

                    }


                }

            }

            return danezplikuJSON_listarekordow;
        }

        //OPERACJE WEWNĘTRZNE NA PLIKACH LOKALIZACJI

        public static void WeryfikacjaIstnieniaParNawiasowKlamrowych()
        {

            Console.Write("Podaj nazwę pliku JSON: ");
            string plikJSON_nazwa = Console.ReadLine();
            if (plikJSON_nazwa == "") { plikJSON_nazwa = "test1.json"; }
            Console.WriteLine("Podano nazwę pliku: " + plikJSON_nazwa);
            if (File.Exists(folderglownyprogramu + plikJSON_nazwa))
            {

                Console.WriteLine("Trwa werfikacja - ewentualnie wykryte błędy zostaną wyświetlone poniżej:");

                uint plik_liczbalinii = PoliczLiczbeLinii(folderglownyprogramu + plikJSON_nazwa);

                FileStream plik_fs = new FileStream(folderglownyprogramu + plikJSON_nazwa, FileMode.Open, FileAccess.Read);

                StreamReader plik_sr = new StreamReader(plik_fs);

                string tmp_ostatnia_odczytana_tresc_KEY = "";

                uint linia = 1;
                while (plik_sr.Peek() != -1)
                {


                    string tresc_linii_JSON = plik_sr.ReadLine();

                    string tresclinii_ciagzmiennych = "";


                    string[] linia_podzial_1 = tresc_linii_JSON.Split(new string[] { "\": \"" }, StringSplitOptions.None);

                    /*
                    for (int a1 = 0; a1 < linia_podzial_1.Length; a1++)
                    {

                        //Console.WriteLine("linia_podzial_1[" + a1 + "]: " + linia_podzial_1[a1]);
                    }
                    */

                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_1.Length: " + linia_podzial_1.Length);

                    if (linia_podzial_1.Length <= 2)
                    {
                        string KEYt1 = linia_podzial_1[0].Trim();
                        int KEYt1_iloscznakow = KEYt1.Length;

                        if (KEYt1_iloscznakow >= 2)
                        {

                            string[] linia_2_separatory = { KEYt1 + "\": \"" };

                            string[] linia_podzial_2 = tresc_linii_JSON.Split(linia_2_separatory, StringSplitOptions.None);

                            /*
                            for (int a2 = 0; a2 < linia_podzial_2.Length; a2++)
                            {

                                Console.WriteLine("linia_podzial_2[" + a2 + "]: " + linia_podzial_2[a2]);
                            }
                            */

                            //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_2.Length: " + linia_podzial_2.Length);

                            if (linia_podzial_2.Length >= 2)
                            {

                                string STRINGt1 = linia_podzial_2[1].TrimEnd();
                                int STRINGt1_iloscznakow = STRINGt1.Length;


                                //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEYt1_iloscznakow: " + KEYt1_iloscznakow);
                                //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRINGt1_iloscznakow: " + STRINGt1_iloscznakow);


                                if (KEYt1_iloscznakow >= 2 && STRINGt1_iloscznakow >= 1)
                                {
                                    string KEY = KEYt1.Remove(0, 1);

                                    int cofniecie_wskaznika = STRINGt1_iloscznakow - 1;
                                    int usunac_znakow = 1;
                                    if (linia != plik_liczbalinii - 2)
                                    {
                                        cofniecie_wskaznika = STRINGt1_iloscznakow - 2;
                                        usunac_znakow = 2;
                                    }

                                    string STRINGt2 = STRINGt1.Remove(cofniecie_wskaznika, usunac_znakow);
                                    string STRING = STRINGt2;



                                    //Console.WriteLine("[linia:" + plik_linia + "] KEY:" + KEY);
                                    //Console.WriteLine("[linia:" + plik_linia + "] STRING:" + STRING);


                                    if (KEY != "$id")
                                    {

                                        string tresc_KEY = KEY;



                                        string tresc_STRING = STRING;


                                        string rodzajenawiasow = "{|}";
                                        int iloscnawiasowwlinii = 0;
                                        Regex regex = new Regex(rodzajenawiasow);
                                        MatchCollection matchCollection = regex.Matches(tresc_STRING);
                                        foreach (var match in matchCollection)
                                        {
                                            iloscnawiasowwlinii++;
                                        }
                                        if (iloscnawiasowwlinii % 2 != 0)
                                        {
                                            Blad("UWAGA: Linia nr." + linia + " ma błędną ilość nawiasów {}!");
                                        }




                                    }

                                }

                            }

                        }


                    }






                    linia++;
                }

            }
            else
            {
                Blad("BŁĄD: Brak takiego pliku.");
            }

        }

        public static void OznaczenieLiniiWPlikustringsTransifexCOMtxt(string tresc_oznaczenia)
        {
            Console.Write("Podaj nazwę pliku stringsTransifexCOM.txt: ");
            string plikstringsTransifexCOMtxt_nazwa = Console.ReadLine();
            if (plikstringsTransifexCOMtxt_nazwa == "") { plikstringsTransifexCOMtxt_nazwa = "test1.json.stringsTransifexCOM.txt"; }
            string nowyplikstringsTransifexCOMtxt_nazwa = "ZAWIERAJACY_OZNACZONE_ZAKRESY_LINII_" + plikstringsTransifexCOMtxt_nazwa;
            if (File.Exists(nowyplikstringsTransifexCOMtxt_nazwa)) { File.Delete(nowyplikstringsTransifexCOMtxt_nazwa); }

            Console.Write("Podaj zakresy identyfikatorów linii, które mają zostać oznaczone (np: 100, 200, 300-400): ");
            string zakresyidentyfikatorowlinii_wprowadzonystring = Console.ReadLine();

            if (zakresyidentyfikatorowlinii_wprowadzonystring != null)
            {
                string[] zil_tmp1 = zakresyidentyfikatorowlinii_wprowadzonystring.Split(", ", StringSplitOptions.None);

                bool blad_parsowaniaint = false;

                for (int zil = 0; zil < zil_tmp1.Length; zil++)
                {
                    string[] zil_tmp2 = zil_tmp1[zil].Split(new char[] { '-' });

                    if (zil_tmp2.Length == 1)
                    {
                        if (CzyParsowanieINTUdane(zil_tmp2[0]) == false)
                        {
                            //Console.WriteLine("[DEBUG]: CzyParsowanieINTUdane(zil_tmp2[0]) == false (zil==" + zil + ")");

                            blad_parsowaniaint = true;
                        }
                    }
                    else if (zil_tmp2.Length == 2)
                    {
                        int poprzedniidentyfikatorlinii = -1;

                        for (int zil2 = 0; zil2 < zil_tmp2.Length; zil2++)
                        {
                            if (CzyParsowanieINTUdane(zil_tmp2[zil2]) == false)
                            {
                                //Console.WriteLine("[DEBUG]: CzyParsowanieINTUdane(zil_tmp2[zil2]) == false (zil==" + zil + ", zil2==" + zil2 + ", zil_tmp2[zil2]==" + zil_tmp2[zil2] + ")");

                                blad_parsowaniaint = true;
                            }
                            else
                            {
                                if ((poprzedniidentyfikatorlinii != -1) && (poprzedniidentyfikatorlinii >= int.Parse(zil_tmp2[zil2])))
                                {
                                    //Console.WriteLine("[DEBUG]: (poprzedniidentyfikatorlinii != -1) && (poprzedniidentyfikatorlinii >= int.Parse(zil_tmp2[zil2])) (zil==" + zil + ", zil2==" + zil2 + ")");

                                    blad_parsowaniaint = true;

                                    Blad("Wystąpił błąd parsowania zakresu (w zil=" + zil + " i zil2=" + zil2 + "). Wartość pierwszego identyfikatora zakresu nie może być większa lub równa wartości drugiego (" + poprzedniidentyfikatorlinii + " >= " + zil_tmp2[zil2] + ").");
                                }

                                poprzedniidentyfikatorlinii = int.Parse(zil_tmp2[zil2]);
                            }


                        }
                    }
                    else
                    {
                        Blad("Wystąpił błąd parsowania zakresu (zil_tmp2.Length > 2 w zil==" + zil + ").");
                    }
                }

                if (blad_parsowaniaint == false)
                {
                    if (File.Exists(plikstringsTransifexCOMtxt_nazwa))
                    {
                        int iloscoznaczonychlinii = 0;

                        FileStream plik_fs = new FileStream(plikstringsTransifexCOMtxt_nazwa, FileMode.Open, FileAccess.Read);
                        FileStream nowyplik_fs = new FileStream(nowyplikstringsTransifexCOMtxt_nazwa, FileMode.Create, FileAccess.Write);

                        StreamReader plik_sr = new StreamReader(plik_fs);
                        StreamWriter nowyplik_sw = new StreamWriter(nowyplik_fs);

                        uint numeraktualnejlinii = 1;
                        while (plik_sr.Peek() != -1)
                        {
                            string plikstringstxt_trescaktualnejlinii = plik_sr.ReadLine();

                            string[] tmp1 = plikstringstxt_trescaktualnejlinii.Split(new char[] { '>' });
                            //Console.WriteLine("[DEBUG] tmp1[0]==" + tmp1[0]);
                            string tmp2 = tmp1[0].TrimStart().Remove(0, 1);

                            if (CzyParsowanieINTUdane(tmp2))
                            {
                                bool czy_oznaczyc_aktualna_linie = false;

                                int identyfikatoraktualnejlinii = int.Parse(tmp2);
                                //Console.WriteLine("[DEBUG] tmp2==" + tmp2);



                                for (int zil3 = 0; zil3 < zil_tmp1.Length; zil3++)
                                {
                                    string[] zil_tmp3 = zil_tmp1[zil3].Split(new char[] { '-' });

                                    if (zil_tmp3.Length == 1)
                                    {

                                        if (identyfikatoraktualnejlinii == int.Parse(zil_tmp3[0]))
                                        {
                                            czy_oznaczyc_aktualna_linie = true;
                                        }

                                    }
                                    else if (zil_tmp3.Length == 2)
                                    {
                                        if ((identyfikatoraktualnejlinii >= int.Parse(zil_tmp3[0])) && (identyfikatoraktualnejlinii <= int.Parse(zil_tmp3[1])))
                                        {
                                            czy_oznaczyc_aktualna_linie = true;
                                        }
                                    }

                                }


                                if (czy_oznaczyc_aktualna_linie == false)
                                {
                                    nowyplik_sw.WriteLine(plikstringstxt_trescaktualnejlinii);
                                }
                                else if (czy_oznaczyc_aktualna_linie == true)
                                {
                                    nowyplik_sw.WriteLine(tresc_oznaczenia + plikstringstxt_trescaktualnejlinii);

                                    iloscoznaczonychlinii++;
                                }


                            }


                            numeraktualnejlinii++;
                        }

                        plik_sr.Close();
                        nowyplik_sw.Close();

                        plik_fs.Close();
                        nowyplik_fs.Close();


                        Sukces("Utworzono plik TXT: \"" + nowyplikstringsTransifexCOMtxt_nazwa + "\" zawierający oznaczone linie na podstawie wskazanych zakresów ich identyfikatorów.\nW pliku oznaczono następującą ilość linii: " + iloscoznaczonychlinii);


                    }
                    else
                    {
                        Blad("BŁĄD: Brak takiego pliku.");
                    }

                }
                else
                {
                    Blad("Podano nieprawidłowy zakres identyfikatorów linii do oznaczenia (wystąpił błąd parsowania).");
                }

            }
            else
            {
                Blad("Nie podano wymaganego zakresu identyfikatorów linii do oznaczenia.");
            }
        }

        public static void JSONplusJSONtoJSON_PrzeniesienieStringowWedlugSzablonu()
        {
            Console.Write("Podaj nazwę źródłowego pliku JSON, z którego ma zostać przeniesiona treść stringów: ");
            string plikJSONzrodlowy_nazwa = Console.ReadLine();
            if (plikJSONzrodlowy_nazwa == "") { plikJSONzrodlowy_nazwa = "test1.json"; }

            Console.Write("Podaj nazwę szablonowego pliku JSON, według którego ma zostać utworzony nowy plik: ");
            string plikJSONszablonowy_nazwa = Console.ReadLine();
            if (plikJSONszablonowy_nazwa == "") { plikJSONszablonowy_nazwa = "test2.json"; }

            Console.WriteLine("Jaka treść ma zostać wstawiona do linii, których nie odnaleziono w pliku źródłowym, a istnieją w pliku szablonowym?");
            Console.WriteLine("1. Pusta treść linii.");
            Console.WriteLine("2. Chcę zdefiniować treść linii.");
            Console.Write("Wpisz numer opcji: ");
            string tresclinii_wyboropcji_string = Console.ReadLine();
            string tresclinii_zdefiniowana = "";

            if (tresclinii_wyboropcji_string == "2")
            {
                Console.Write("Wpisz treść, która zostanie wstawiona do linii, których nie odnaleziono w pliku źródłowym, a istnieją w pliku szablonowym: ");
                tresclinii_zdefiniowana = Console.ReadLine();
            }
            
            string plikJSONdocelowy_nazwa = "NOWY_" + plikJSONszablonowy_nazwa;
            if (File.Exists(plikJSONdocelowy_nazwa)) { File.Delete(plikJSONdocelowy_nazwa); }

            if (File.Exists(plikJSONzrodlowy_nazwa) && File.Exists(plikJSONszablonowy_nazwa))
            {
                List<RekordJSON> plikJSONzrodlowy_listarekordow = WczytajDaneZPlikuJSONdoListyRekordow(plikJSONzrodlowy_nazwa);
                List<RekordJSON> plikJSONszablonowy_listarekordow = WczytajDaneZPlikuJSONdoListyRekordow(plikJSONszablonowy_nazwa);
                List<RekordJSON> plikJSONdocelowy_listarekordow = new List<RekordJSON>();

                //int aktualnyID_dlaplikudocelowegoJSON = 0 + 2;

                for (int wr = 0; wr < plikJSONszablonowy_listarekordow.Count(); wr++)
                {
                    List<RekordJSON> lista_znalezioneklucze_wplikuJSONzrodlowym = plikJSONzrodlowy_listarekordow.FindAll(x => x.Klucz == plikJSONszablonowy_listarekordow[wr].Klucz);

                    if (lista_znalezioneklucze_wplikuJSONzrodlowym.Count() == 1)
                    {

                        //Console.WriteLine("[DEBUG] Znaleziono klucz: " + lista_znalezioneklucze_wplikuJSONzrodlowym[0].Klucz);

                        plikJSONdocelowy_listarekordow.Add(new RekordJSON { ID = plikJSONszablonowy_listarekordow[wr].ID, Plik = plikJSONdocelowy_nazwa, Klucz = lista_znalezioneklucze_wplikuJSONzrodlowym[0].Klucz, String = lista_znalezioneklucze_wplikuJSONzrodlowym[0].String });

                        //aktualnyID_dlaplikudocelowegoJSON++;
                        
                    }
                    else if (lista_znalezioneklucze_wplikuJSONzrodlowym.Count() == 0)
                    {
                        plikJSONdocelowy_listarekordow.Add(new RekordJSON { ID = plikJSONszablonowy_listarekordow[wr].ID, Plik = plikJSONdocelowy_nazwa, Klucz = plikJSONszablonowy_listarekordow[wr].Klucz, String = tresclinii_zdefiniowana });
                    }
                    else
                    {
                        Blad("Krytyczny błąd: W pliku źródłowym JSON występuje więcej niż 1 string zawierający ten sam klucz o wartości: \"" + lista_znalezioneklucze_wplikuJSONzrodlowym[wr].Klucz + "\".");
                    }

                }

                bool czy_utworzono_naglowek = UtworzNaglowekJSON(plikJSONdocelowy_nazwa);

                if (czy_utworzono_naglowek == true)
                {
                    FileStream plikJSONdocelowy_fs = new FileStream(plikJSONdocelowy_nazwa, FileMode.Append, FileAccess.Write);

                    try
                    {
                        StreamWriter plikJSONdocelowy_sw = new StreamWriter(plikJSONdocelowy_fs);

                        for (int zd = 0; zd < plikJSONdocelowy_listarekordow.Count(); zd++)
                        {

                            string _KLUCZ = plikJSONdocelowy_listarekordow[zd].Klucz;
                            string _STRING = plikJSONdocelowy_listarekordow[zd].String;

                            plikJSONdocelowy_sw.Write(/*"[DEBUG-ID: " + plikJSONdocelowy_listarekordow[zd].ID + "] " + */"    \"" + _KLUCZ + "\": \"" + _STRING + "\"");

                            if (zd + 1 != plikJSONdocelowy_listarekordow.Count())
                            {
                                plikJSONdocelowy_sw.Write(",");
                            }

                            plikJSONdocelowy_sw.Write("\n");

                        }

                        plikJSONdocelowy_sw.Close();
                    }
                    catch
                    {
                        Blad("Wystąpił problem z zapisem do nowogenerowanego pliku JSON: " + plikJSONdocelowy_nazwa);
                    }

                    plikJSONdocelowy_fs.Close();

                    bool czy_utworzono_stopke = UtworzStopkeJSON(plikJSONdocelowy_nazwa);

                    if (czy_utworzono_stopke == true)
                    {
                        Sukces("Plik JSON o nazwie \"" + plikJSONdocelowy_nazwa + "\" został wygenerowany.");
                    }
                    else
                    {
                        Blad("BŁĄD: Wystąpił problem z utworzeniem stopki w nowogenerowanym pliku JSON: " + plikJSONdocelowy_nazwa);
                    }

                }
                else
                {
                    Blad("BŁĄD: Wystąpił problem z utworzeniem nagłówka w nowogenerowanym pliku JSON: " + plikJSONdocelowy_nazwa);
                }

            }
            else
            {
                Blad("BŁĄD: Nie istnieje przynajmniej jeden ze wskazanych plików.");
            }

        }


        public static void stringsTransifexCOMTXT_PorownanieEnORIGzCzesciowoPrzetlumaczonymNaPLorazUsuniecieTresciNieprzetlumaczonychStringow()
        {
            Console.Write("Podaj nazwę pliku stringsTransifexCOMTXT, który zawiera oryginalną angielską lokalizację: ");
            string txtORIGen_nazwa = Console.ReadLine();
            
            Console.WriteLine("Podaj nazwę pliku stringsTransifexCOMTXT, który jest częściowo przetłumaczony na język polski: ");
            string txtCzesciowoPrzetlumaczonyPL_nazwa = Console.ReadLine();

            string txtNowyPlik_nazwa = "NOWY_" + txtCzesciowoPrzetlumaczonyPL_nazwa;
            
            if (File.Exists(txtORIGen_nazwa) == true && File.Exists(txtCzesciowoPrzetlumaczonyPL_nazwa) == true)
            {
                List<string> linie_txtORIGen = new List<string>();
                List<string> linie_txtCzesciowoPrzetlumaczonyPL = new List<string>();
                List<string> linie_txtNowyPlik = new List<string>();

                FileStream txtORIGen_fs = new FileStream(txtORIGen_nazwa, FileMode.Open, FileAccess.Read);

                try
                {
                    StreamReader txtORIGen_sr = new StreamReader(txtORIGen_fs);

                    while (txtORIGen_sr.Peek() != -1)
                    {
                        string txtORIGen_trescaktualnejlinii = txtORIGen_sr.ReadLine();
                        
                        linie_txtORIGen.Add(txtORIGen_trescaktualnejlinii);
                    }
                    
                    txtORIGen_sr.Close();
                    
                }
                catch
                {
                    Blad("BŁĄD: Wystąpił nieoczekiwany problem z dostępem do pliku: " + txtORIGen_nazwa);
                }

                txtORIGen_fs.Close();

                
                

                FileStream txtCzesciowoPrzetlumaczonyPL_fs = new FileStream(txtCzesciowoPrzetlumaczonyPL_nazwa, FileMode.Open, FileAccess.Read);
                
                try
                {
                    StreamReader txtCzesciowoPrzetlumaczonyPL_sr = new StreamReader(txtCzesciowoPrzetlumaczonyPL_fs);

                    while (txtCzesciowoPrzetlumaczonyPL_sr.Peek() != -1)
                    {
                        string txtCzesciowoPrzetlumaczonyPL_trescaktualnejlinii = txtCzesciowoPrzetlumaczonyPL_sr.ReadLine();
                        
                        linie_txtCzesciowoPrzetlumaczonyPL.Add(txtCzesciowoPrzetlumaczonyPL_trescaktualnejlinii);
                    }
                    
                    txtCzesciowoPrzetlumaczonyPL_sr.Close();
                    
                }
                catch
                {
                    Blad("BŁĄD: Wystąpił nieoczekiwany problem z dostępem do pliku: " + txtORIGen_nazwa);
                }

                txtORIGen_fs.Close();



                if (linie_txtORIGen.Count == linie_txtCzesciowoPrzetlumaczonyPL.Count)
                {
                    if (File.Exists(txtNowyPlik_nazwa) == true) { File.Delete(txtNowyPlik_nazwa); }
                    
                    FileStream txtNowyPlik_fs = new FileStream(txtNowyPlik_nazwa, FileMode.CreateNew, FileAccess.Write);

                    try
                    {
                        StreamWriter txtNowyPlik_sw = new StreamWriter(txtNowyPlik_fs);

                        for (int il = 0; il < linie_txtORIGen.Count; il++)
                        {
                            if (linie_txtORIGen[il] != linie_txtCzesciowoPrzetlumaczonyPL[il])
                            {
                                txtNowyPlik_sw.WriteLine(linie_txtCzesciowoPrzetlumaczonyPL[il]);
                            }
                            else
                            {
                                txtNowyPlik_sw.WriteLine("TŁUMACZENIE_DO_USUNIĘCIA"); //pusta linia
                            }

                        }

                        txtNowyPlik_sw.Close();
                    }
                    catch
                    {
                        Blad("BŁĄD: Wystąpił nieoczekiwany problem z dostępem do pliku: " + txtNowyPlik_nazwa);
                    }

                    txtNowyPlik_fs.Close();


                    if (File.Exists(txtNowyPlik_nazwa) == true)
                    {
                        Sukces("Utworzono nowy plik: " + txtNowyPlik_nazwa);
                    }

                }
                else
                {
                    Blad($"BŁĄD: Pliki \"{txtORIGen_nazwa}\" i \"{txtCzesciowoPrzetlumaczonyPL_nazwa}\" mają różną ilość linii");
                }
                

            }
            else
            {
                Blad("Nie istnieje przynajmniej jeden z podanych plików.");
            }
        }
        
        
        
        //V1

        public static void JSONtoTXT()
        {
            string nazwaplikuJSON;

            Console.Write("Podaj nazwę pliku JSON: ");
            nazwaplikuJSON = Console.ReadLine();
            if (nazwaplikuJSON == "") { nazwaplikuJSON = "test1.json"; }
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuJSON);
            if (File.Exists(folderglownyprogramu + nazwaplikuJSON))
            {
                uint plik_JSON_liczbalinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikuJSON);

                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_JSON_fs = new FileStream(nazwaplikuJSON, FileMode.Open, FileAccess.Read);
                FileStream nowy_plik_keystxt_fs = new FileStream(nazwaplikuJSON + ".keys.txt", FileMode.Create, FileAccess.ReadWrite);
                FileStream nowy_plik_stringstxt_fs = new FileStream(nazwaplikuJSON + ".strings.txt", FileMode.Create, FileAccess.ReadWrite);

                try
                {
                    int ilosc_wykrytych_STRINGS = 0;
                    int ilosc_wykrytych_VARS = 0;
                    List<List<string>> vars_tmp = new List<List<string>>(); //skladnia vars_tmp[numer_linii][0:key||1:ciag_zmiennych]
                    const char separator = ';';


                    StreamReader plik_JSON_sr = new StreamReader(plik_JSON_fs);
                    StreamWriter nowy_plik_keystxt_sr = new StreamWriter(nowy_plik_keystxt_fs);
                    StreamWriter nowy_plik_stringstxt_sr = new StreamWriter(nowy_plik_stringstxt_fs);

                    int plik_JSON_linia = 1;
                    while (plik_JSON_sr.Peek() != -1)
                    {
                        string tresc_linii_JSON = plik_JSON_sr.ReadLine();

                        string tresclinii_ciagzmiennych = "";

                        vars_tmp.Add(new List<string>());


                        string[] linia_podzial_1 = tresc_linii_JSON.Split(new string[] { "\": \"" }, StringSplitOptions.None);

                        /*
                        for (int a1 = 0; a1 < linia_podzial_1.Length; a1++)
                        {

                            //Console.WriteLine("linia_podzial_1[" + a1 + "]: " + linia_podzial_1[a1]);
                        }
                        */

                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_1.Length: " + linia_podzial_1.Length);

                        if (linia_podzial_1.Length <= 2)
                        {
                            string KEYt1 = linia_podzial_1[0].Trim();
                            int KEYt1_iloscznakow = KEYt1.Length;

                            if (KEYt1_iloscznakow >= 2)
                            {

                                string[] linia_2_separatory = { KEYt1 + "\": \"" };

                                string[] linia_podzial_2 = tresc_linii_JSON.Split(linia_2_separatory, StringSplitOptions.None);

                                /*
                                for (int a2 = 0; a2 < linia_podzial_2.Length; a2++)
                                {

                                    Console.WriteLine("linia_podzial_2[" + a2 + "]: " + linia_podzial_2[a2]);
                                }
                                */

                                //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_2.Length: " + linia_podzial_2.Length);

                                if (linia_podzial_2.Length >= 2)
                                {

                                    string STRINGt1 = linia_podzial_2[1].TrimEnd();
                                    int STRINGt1_iloscznakow = STRINGt1.Length;


                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEYt1_iloscznakow: " + KEYt1_iloscznakow);
                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRINGt1_iloscznakow: " + STRINGt1_iloscznakow);


                                    if (KEYt1_iloscznakow >= 2 && STRINGt1_iloscznakow >= 1)
                                    {
                                        string KEY = KEYt1.Remove(0, 1);

                                        int cofniecie_wskaznika = STRINGt1_iloscznakow - 1;
                                        int usunac_znakow = 1;
                                        if (plik_JSON_linia != plik_JSON_liczbalinii - 2)
                                        {
                                            cofniecie_wskaznika = STRINGt1_iloscznakow - 2;
                                            usunac_znakow = 2;
                                        }

                                        string STRINGt2 = STRINGt1.Remove(cofniecie_wskaznika, usunac_znakow);
                                        string STRING = STRINGt2;



                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEY:" + KEY);
                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRING:" + STRING);


                                        if (KEY != "$id")
                                        {

                                            string tresc_KEY = KEY;

                                            try
                                            {
                                                //Console.WriteLine("indeks wykrytego KEY'a: " + ilosc_wykrytych_VARS);

                                                vars_tmp[ilosc_wykrytych_VARS].Add(tresc_KEY);
                                            }
                                            catch
                                            {
                                                Blad("BLAD: vars_tmp #1!");
                                            }

                                            //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja klucza o treści: " + tresc_KEY);

                                            ilosc_wykrytych_VARS++;




                                            string tresc_STRING = STRING

                                            /*
                                            .Replace("\\n"  , " |__BR__| "   )
                                            .Replace("\\\"" , " |__BS_N1__| ")
                                            .Replace("<b>"  , " |__B1__| "   )
                                            .Replace("</b>" , " |__B2__| "   )
                                            .Replace("<i>"  , " |__I1__| "   )
                                            .Replace("</i>" , " |__I2__| "   )
                                            .Replace("<u>"  , " |__U1__| "   )
                                            .Replace("</u>" , " |__U2__| "   )
                                            .Replace("<"    , " |__NT1__| ")
                                            .Replace(">"    , " |__NT2__| ")
                                            */

                                            .Replace("\\n", " %__BR__% ")
                                            .Replace("\\\"", " %__BS_N1__% ")
                                            .Replace("<b>", " %__B1__% ")
                                            .Replace("</b>", " %__B2__% ")
                                            .Replace("<i>", " %__I1__% ")
                                            .Replace("</i>", " %__I2__% ")
                                            .Replace("<u>", " %__U1__% ")
                                            .Replace("</u>", " %__U2__% ")
                                            .Replace("<", " %__NT1__% ")
                                            .Replace(">", " %__NT2__% ")

                                            ;

                                            if (tresc_STRING == "")
                                            {
                                                tresc_STRING = " ";
                                            }

                                            if (tresc_STRING.Contains('{') || tresc_STRING.Contains('}'))
                                            {
                                                string rodzajenawiasow = "{|}";
                                                int iloscnawiasowwlinii = 0;
                                                Regex regex = new Regex(rodzajenawiasow);
                                                MatchCollection matchCollection = regex.Matches(tresc_STRING);
                                                foreach (var match in matchCollection)
                                                {
                                                    iloscnawiasowwlinii++;
                                                }
                                                if (iloscnawiasowwlinii % 2 == 0)
                                                {
                                                    //Console.WriteLine("Linia nr." + plik_JSON_linia + " posiada pary nawiasów {}.");

                                                    if (tresc_STRING.Contains('{') && tresc_STRING.Contains('}'))
                                                    {
                                                        string[] tresclinii_nawklamrowy_podzial1 = tresc_STRING.Split(new char[] { '{' });

                                                        for (int i1 = 0; i1 < tresclinii_nawklamrowy_podzial1.Length; i1++)
                                                        {
                                                            //Console.WriteLine("tresclinii_nawklamrowy_podzial1[" + i1.ToString() + "]: " + tresclinii_nawklamrowy_podzial1[i1]);

                                                            if (tresclinii_nawklamrowy_podzial1[i1].Contains('}'))
                                                            {
                                                                int kl_index = i1 - 1;
                                                                string tresczwnetrzanawiasuklamrowego = tresclinii_nawklamrowy_podzial1[i1].Split(new char[] { '}' })[0];
                                                                string nazwazmiennej_w_stringstxt = " %KL__" + kl_index + "__E% "; //np. " |KL__0__E| ", " |KL__1__E| ", " |KL__2__E| " itd.

                                                                //Console.WriteLine("tresczwnetrzanawiasuklamrowego (" + i1.ToString() + "): " + tresczwnetrzanawiasuklamrowego);

                                                                tresclinii_ciagzmiennych += "{" + tresczwnetrzanawiasuklamrowego + "}";

                                                                if (i1 + 1 != tresclinii_nawklamrowy_podzial1.Length) { tresclinii_ciagzmiennych += separator; }

                                                                tresc_STRING = tresc_STRING.Replace("{" + tresczwnetrzanawiasuklamrowego + "}", nazwazmiennej_w_stringstxt);

                                                                //Console.WriteLine("nazwazmiennej_w_stringstxt: " + nazwazmiennej_w_stringstxt);
                                                                //Console.WriteLine("tresc_STRING: " + tresc_STRING);


                                                            }


                                                        }

                                                    }



                                                }
                                                else
                                                {
                                                    Blad("BŁĄD: Linia nr." + plik_JSON_linia + " ma błędną ilość nawiasów {}!");
                                                }



                                            }
                                            else
                                            {
                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " NIE posiada pary nawiasów {}.");

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja string'a o tresci: " + tresc_STRING);

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " zawiera VARS: " + tresclinii_ciagzmiennych);

                                            }


                                            nowy_plik_stringstxt_sr.WriteLine(tresc_STRING);

                                            vars_tmp[ilosc_wykrytych_STRINGS].Add(tresclinii_ciagzmiennych);

                                            ilosc_wykrytych_STRINGS++;


                                        }

                                    }

                                }

                            }


                        }


                        Console.WriteLine("Trwa konwertowanie linii nr. " + plik_JSON_linia + "/" + plik_JSON_liczbalinii + " [" + PoliczPostepWProcentach(plik_JSON_linia, plik_JSON_liczbalinii) + "%]");

                        plik_JSON_linia++;
                    }

                    //Console.WriteLine("ilosc_wykrytych_vars:" + ilosc_wykrytych_VARS);
                    //Console.WriteLine("vars_tmp[0][0]: " + vars_tmp[0][0]);

                    for (int iv1 = 0; iv1 < vars_tmp.Count; iv1++)
                    {
                        for (int iv2 = 0; iv2 < vars_tmp[iv1].Count; iv2++)
                        {
                            //Console.WriteLine("vars_tmp[" + iv1 + "][" + iv2 + "]: " + vars_tmp[iv1][iv2]);

                            if (iv2 == 0)
                            {
                                nowy_plik_keystxt_sr.Write(vars_tmp[iv1][iv2]);
                            }
                            else if (iv2 == 1)
                            {
                                if (vars_tmp[iv1][iv2] != "")
                                {
                                    nowy_plik_keystxt_sr.Write(separator + vars_tmp[iv1][iv2] + "\n");
                                }
                                else
                                {
                                    nowy_plik_keystxt_sr.Write("\n");
                                }
                            }
                        }
                    }




                    nowy_plik_keystxt_sr.Close();
                    nowy_plik_stringstxt_sr.Close();
                    plik_JSON_sr.Close();


                }
                catch
                {
                    Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do plików.");
                }

                nowy_plik_keystxt_fs.Close();
                nowy_plik_stringstxt_fs.Close();
                plik_JSON_fs.Close();


            }
            else
            {
                Blad("BŁĄD: Brak takiego pliku.");
            }

            if (File.Exists(nazwaplikuJSON + ".keys.txt") && File.Exists(nazwaplikuJSON + ".strings.txt"))
            {
                Console.WriteLine("----------------------------------");
                Sukces("Utworzono 2 pliki TXT: \"" + nazwaplikuJSON + ".keys.txt\" oraz \"" + nazwaplikuJSON + ".strings.txt\"");

            }




        }

        public static void TXTtoJSON()
        {

            string nazwaplikukeystxt;
            string nazwaplikustringstxt;
            string walidacjapoAutoTranslatorze;
            string nazwanowegoplikuJSON;
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const char separator = ';';

            Console.Write("Podaj nazwę pliku .keys.txt: ");
            nazwaplikukeystxt = Console.ReadLine();
            if (nazwaplikukeystxt == "") { nazwaplikukeystxt = "test1.json.keys.txt"; }
            Console.WriteLine("Podano nazwę pliku .keys.txt: " + nazwaplikukeystxt);

            Console.Write("Podaj nazwę pliku .strings.txt: ");
            nazwaplikustringstxt = Console.ReadLine();
            if (nazwaplikustringstxt == "") { nazwaplikustringstxt = "test1.json.strings.txt"; }
            Console.WriteLine("Podano nazwę pliku .strings.txt: " + nazwaplikustringstxt);

            Console.Write("Włączyć walidację treści po autotranslatorze? [t/n]: ");
            walidacjapoAutoTranslatorze = Console.ReadLine();

            if (walidacjapoAutoTranslatorze == "t" || walidacjapoAutoTranslatorze == "n")
            {
                if (File.Exists(nazwaplikukeystxt) && File.Exists(nazwaplikustringstxt))
                {
                    if (walidacjapoAutoTranslatorze == "t")
                    {
                        nazwanowegoplikuJSON = "NOWY_WTPA_" + nazwaplikukeystxt.Replace(".keys.txt", "");
                    }
                    else
                    {
                        nazwanowegoplikuJSON = "NOWY_" + nazwaplikukeystxt.Replace(".keys.txt", "");
                    }

                    Console.WriteLine("Nazwa nowego pliku JSON to: " + nazwanowegoplikuJSON);

                    plikkeystxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikukeystxt);
                    plikstringstxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikustringstxt);
                    //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                    //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);



                    if (plikkeystxt_ilosclinii == plikstringstxt_ilosclinii)
                    {
                        bool naglowekJSON_rezultat = UtworzNaglowekJSON(nazwanowegoplikuJSON);
                        if (naglowekJSON_rezultat == true)
                        {
                            bool bledywplikuJSON = false;
                            FileStream nowyplikJSON_fs = new FileStream(nazwanowegoplikuJSON, FileMode.Append, FileAccess.Write);
                            FileStream plikkeystxt_fs = new FileStream(nazwaplikukeystxt, FileMode.Open, FileAccess.Read);

                            try //#1
                            {
                                string plikkeystxt_trescaktualnejlinii;
                                string plikstringstxt_trescaktualnejlinii;

                                StreamWriter nowyplikJSON_sw = new StreamWriter(nowyplikJSON_fs);
                                StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);

                                int plikkeystxt_sr_nraktualnejlinii = 1;
                                while (plikkeystxt_sr.Peek() != -1)
                                {
                                    plikkeystxt_trescaktualnejlinii = plikkeystxt_sr.ReadLine();
                                    string[] plikkeystxt_wartoscilinii = plikkeystxt_trescaktualnejlinii.Split(new char[] { separator }); //skladnia: plikkeystxt_wartoscilinii[0:key||0<:vars]

                                    //Console.WriteLine("Pobrano KEY   z linii " + plikkeystxt_sr_nraktualnejlinii + " o tresci: " + plikkeystxt_trescaktualnejlinii);

                                    FileStream plikstringstxt_fs = new FileStream(nazwaplikustringstxt, FileMode.Open, FileAccess.Read);

                                    try //#2
                                    {
                                        StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);

                                        int plikstringstxt_sr_nraktualnejlinii = 1;
                                        while (plikstringstxt_sr.Peek() != -1)
                                        {
                                            plikstringstxt_trescaktualnejlinii = plikstringstxt_sr.ReadLine();

                                            if (plikstringstxt_sr_nraktualnejlinii == plikkeystxt_sr_nraktualnejlinii)
                                            {

                                                string plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescaktualnejlinii;

                                                //Console.WriteLine("!!!: Liczba key+vars w linii nr. " + plikkeystxt_sr_nraktualnejlinii + ": " + plikkeystxt_wartoscilinii.Length);

                                                List<string> lista_zmiennych_linii = new List<string>();


                                                if (walidacjapoAutoTranslatorze == "t")
                                                {
                                                    plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii


                                                    /* POCZATEK - AUTOTRANSLATOR FIX - naprawa uszkodzonych zmiennych i tagow przez autotranslator */

                                                    //dodatkowa korekcja " dodana 2022.06.12, zaktualizowana 2022.10.20
                                                    .Replace("\"", "'")

                                                    //stale
                                                    .Replace("< ", "<")
                                                    .Replace(" >", ">")
                                                    .Replace("</ ", "</")
                                                    .Replace(" = ", "=")
                                                    .Replace("# ", "#")

                                                    .Replace("  %__BS_N1__% ", "\\\"")
                                                    .Replace(" %__BS_N1__%  ", "\\\"")

                                                    //dodatkowa korekcja nr.2 dodana 2022.06.12 , zaktualizowana 2022.10.20
                                                    .Replace(".  %__BS_N1__% .", ". %__BS_N1__% ")
                                                    .Replace("!  %__BS_N1__% .", "! %__BS_N1__% ")
                                                    .Replace("?  %__BS_N1__% .", "? %__BS_N1__% ")
                                                    .Replace("> %__BS_N1__%  ", "> %__BS_N1__% ")
                                                    .Replace(". %__BS_N1__% .", ". %__BS_N1__% ")
                                                    .Replace("! %__BS_N1__% .", "! %__BS_N1__% ")
                                                    .Replace("? %__BS_N1__% .", "? %__BS_N1__% ")
                                                    .Replace(".  %__BS_N1__% ", ". %__BS_N1__% ")
                                                    .Replace("!  %__BS_N1__% ", "! %__BS_N1__% ")
                                                    .Replace("?  %__BS_N1__% ", "? %__BS_N1__% ")

                                                    //dodatkowa korekcja nr.3 dodana 2022.06.12
                                                    .Replace("komandor", "dowódc{mf|a|zyni}")
                                                    .Replace("Komandor", "Dowódc{mf|a|zyni}")
                                                    .Replace("komandora", "dowódc{mf|ę|zynię}")
                                                    .Replace("Komandora", "Dowódc{mf|ę|zynię}")
                                                    .Replace("komandorowi", "dowódc{mf|y|zyni}")
                                                    .Replace("Komandorowi", "Dowódc{mf|y|zyni}")
                                                    .Replace("komandorem", "dowódc{mf|ą|zynią}")
                                                    .Replace("Komandorem", "Dowódc{mf|ą|zynią}")
                                                    .Replace("komandorze", "dowódc{mf|o|zyni}")
                                                    .Replace("Komandorze", "Dowódc{mf|o|zyni}")
                                                    .Replace("AC", "KP")
                                                    .Replace("HD", "KW")
                                                    .Replace("DC", "KT")
                                                    .Replace("DR", "RO")

                                                    //dodatkowa korekcja nr.4 dodana 2022.06.12 (oznaczenie rzutów kośmi)
                                                    .Replace("1d2", "1k2")
                                                    .Replace("2d2", "2k2")
                                                    .Replace("1d3", "1k3")
                                                    .Replace("2d3", "2k3")
                                                    .Replace("10d4", "10k4")
                                                    .Replace("20d4", "20k4")
                                                    .Replace("1d4", "1k4")
                                                    .Replace("2d4", "2k4")
                                                    .Replace("3d4", "3k4")
                                                    .Replace("5d4", "5k4")
                                                    .Replace("10d6", "10k6")
                                                    .Replace("12d6", "12k6")
                                                    .Replace("15d6", "15k6")
                                                    .Replace("20d6", "20k6")
                                                    .Replace("1d6", "1k6")
                                                    .Replace("2d6", "2k6")
                                                    .Replace("3d6", "3k6")
                                                    .Replace("4d6", "4k6")
                                                    .Replace("5d6", "5k6")
                                                    .Replace("6d6", "6k6")
                                                    .Replace("7d6", "7k6")
                                                    .Replace("8d6", "8k6")
                                                    .Replace("10d6", "10k6")
                                                    .Replace("12d6", "12k6")
                                                    .Replace("15d6", "15k6")
                                                    .Replace("20d6", "20k6")
                                                    .Replace("10d8", "10k8")
                                                    .Replace("12d8", "12k8")
                                                    .Replace("20d8", "20k8")
                                                    .Replace("1d8", "1k8")
                                                    .Replace("2d8", "2k8")
                                                    .Replace("3d8", "3k8")
                                                    .Replace("4d8", "4k8")
                                                    .Replace("5d8", "5k8")
                                                    .Replace("6d8", "6k8")
                                                    .Replace("7d8", "7k8")
                                                    .Replace("8d8", "8k8")
                                                    .Replace("10d10", "10k10")
                                                    .Replace("1d10", "1k10")
                                                    .Replace("2d10", "2k10")
                                                    .Replace("3d10", "3k10")
                                                    .Replace("5d10", "5k10")
                                                    .Replace("8d10", "8k10")
                                                    .Replace("1d12", "1k12")
                                                    .Replace("2d12", "2k12")
                                                    .Replace("15d20", "15k20")
                                                    .Replace("1d20", "1k20")

                                                    //dodatkowa korekcja nr.5 dodana 2022.06.30
                                                    .Replace("Konstytucja", "Kondycja")
                                                    .Replace("Konstytucji", "Kondycji")
                                                    .Replace("Konstytucją", "Kondycją")
                                                    .Replace("konstytucja", "Kondycja")
                                                    .Replace("konstytucji", "Kondycji")
                                                    .Replace("konstytucją", "Kondycją")
                                                    .Replace("partia", "drużyna")
                                                    .Replace("partii", "drużyny")

                                                    //dodatkowo niedomkniete LONG'i mogą powodować zawieszanie się, a nawet awarie gry
                                                    .Replace("[ LONGSTART]", "[LONGSTART]")
                                                    .Replace("[LONGSTART ]", "[LONGSTART]")
                                                    .Replace("[ LONGSTART ]", "[LONGSTART]")
                                                    .Replace("[ LONGEND]", "[LONGEND]")
                                                    .Replace("[LONGEND ]", "[LONGEND]")
                                                    .Replace("[ LONGEND ]", "[LONGEND]")





                                                    //chwilowe

                                                    ;

                                                    /* KONIEC - AUTOTRANSLATOR FIX - naprawa uszkodzonych zmiennych i tagow przez autotranslator */

                                                }


                                                plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii

                                                .Replace(" %__BR__% ", "\\n")
                                                .Replace(" %__BS_N1__% ", "\\\"")
                                                .Replace(" %__B1__% ", "<b>")
                                                .Replace(" %__B2__% ", "</b>")
                                                .Replace(" %__I1__% ", "<i>")
                                                .Replace(" %__I2__% ", "</i>")
                                                .Replace(" %__U1__% ", "<u>")
                                                .Replace(" %__U2__% ", "</u>")
                                                .Replace(" %__NT1__% ", "<")
                                                .Replace(" %__NT2__% ", ">")

                                                //dodatkowa korekcja nr.6 dodana 2022.10.20
                                                .Replace("%__BR__%", "\\n")
                                                .Replace("%__BS_N1__%", "\\\"")
                                                .Replace("%__B1__%", "<b>")
                                                .Replace("%__B2__%", "</b>")
                                                .Replace("%__I1__%", "<i>")
                                                .Replace("%__I2__%", "</i>")
                                                .Replace("%__U1__%", "<u>")
                                                .Replace("%__U2__%", "</u>")
                                                .Replace("%__NT1__%", "<")
                                                .Replace("%__NT2__%", ">")

                                                ;


                                                if (plikkeystxt_wartoscilinii.Length > 1)
                                                {

                                                    for (int ivw = 1; ivw < plikkeystxt_wartoscilinii.Length; ivw++)
                                                    {
                                                        int ivwminus1 = ivw - 1;

                                                        lista_zmiennych_linii.Add(" %KL__" + ivwminus1 + "__E% " + ";" + plikkeystxt_wartoscilinii[ivw]);

                                                    }

                                                }

                                                //Console.WriteLine("lista_zmiennych_linii.Count: " + lista_zmiennych_linii.Count);

                                                for (int it1 = 0; it1 < lista_zmiennych_linii.Count; it1++)
                                                {
                                                    plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii

                                                    .Replace(lista_zmiennych_linii[it1].Split(new char[] { ';' })[0], lista_zmiennych_linii[it1].Split(new char[] { ';' })[1])

                                                    // dodatkowa korekcja dodana 2022.10.21
                                                    .Replace(lista_zmiennych_linii[it1].Split(new char[] { ';' })[0].Trim(), " " + lista_zmiennych_linii[it1].Split(new char[] { ';' })[1] + " ")
                                                    ;

                                                    //Console.WriteLine("Sparsowano zmienna w linii nr. " + plikstringstxt_sr_nraktualnejlinii + ": " + lista_zmiennych_linii[it1].Split(new char[] { ';' })[0] + "na " + lista_zmiennych_linii[it1].Split(new char[] { ';' })[1]);

                                                }



                                                //Console.WriteLine("MOMENT PRZED ZAPISEM: " + plikstringstxt_trescuaktualnionalinii);

                                                //Console.WriteLine("plikkeystxt_sr_nraktualnejlinii: " + plikkeystxt_sr_nraktualnejlinii);
                                                //Console.WriteLine("plikkeystxt_ilosclinii: " + plikkeystxt_ilosclinii);

                                                if (plikstringstxt_trescuaktualnionalinii == " ") { plikstringstxt_trescuaktualnionalinii = ""; }

                                                if (plikstringstxt_sr_nraktualnejlinii != plikkeystxt_ilosclinii)
                                                {
                                                    nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\",");
                                                }
                                                else
                                                {
                                                    nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\"");
                                                }


                                            }


                                            plikstringstxt_sr_nraktualnejlinii++;
                                        }


                                        plikstringstxt_sr.Close();

                                    }
                                    catch
                                    {
                                        Blad("BLAD: Wystapil nieoczekiwany blad w dostepie do plikow. (TRY #2, plikkeystxt_sr_nraktualnejlinii: " + plikkeystxt_sr_nraktualnejlinii + ")");
                                    }

                                    Console.WriteLine("Trwa konwertowanie linii nr.: " + plikkeystxt_sr_nraktualnejlinii + "/" + plikkeystxt_ilosclinii + " [" + PoliczPostepWProcentach(plikkeystxt_sr_nraktualnejlinii, plikkeystxt_ilosclinii) + "%]");

                                    plikkeystxt_sr_nraktualnejlinii++;

                                    plikstringstxt_fs.Close();

                                }
                                plikkeystxt_sr.Close();
                                nowyplikJSON_sw.Close();


                                bool stopkaJSON_rezultat = UtworzStopkeJSON(nazwanowegoplikuJSON);

                                if (stopkaJSON_rezultat == true)
                                {
                                    //Console.WriteLine("Pomyślnie utworzono stopkę w pliku JSON.");
                                }
                                else
                                {
                                    Blad("BŁĄD: Wystąpil problem z utworzeniem stopki w pliku JSON.");
                                }


                                if (naglowekJSON_rezultat == true && stopkaJSON_rezultat == true)
                                {
                                    Sukces("Plik JSON o nazwie \"" + nazwanowegoplikuJSON + "\" zostal wygenerowany.");
                                }
                                else
                                {
                                    bledywplikuJSON = true;

                                    Blad("BŁĄD: Plik JSON nie został wygenerowany (patrz: błędy powyżej)!");
                                }

                            }
                            catch
                            {
                                Blad("BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do plików. (TRY #1)");
                            }

                            plikkeystxt_fs.Close();
                            nowyplikJSON_fs.Close();

                            if (bledywplikuJSON == true && File.Exists(nazwanowegoplikuJSON))
                            {
                                File.Delete(nazwanowegoplikuJSON);

                                //Blad("bledywplikuJSON: true");
                            }
                            else
                            {
                                //Sukces("bledywplikuJSON: false");
                            }

                        }
                        else
                        {
                            Blad("BŁĄD: Wystapil problem z utworzeniem nagłówka w pliku JSON!");
                        }

                    }
                    else
                    {
                        Blad("BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!");
                    }


                }
                else
                {
                    Blad("BŁĄD: Brak wskazanych plików.");
                }

            }
            else
            {
                Blad("BŁĄD: Podano nieprawidłową wartość. Prawidłowa wartość to t lub n.");
            }


        }

        public static void UsuwanieZnakowPL()
        {
            string nazwaplikuzrodlowego;

            Console.Write("Podaj nazwę pliku: ");
            nazwaplikuzrodlowego = Console.ReadLine();
            if (nazwaplikuzrodlowego == "") { nazwaplikuzrodlowego = "test.json"; }
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuzrodlowego);
            if (File.Exists(folderglownyprogramu + nazwaplikuzrodlowego))
            {
                uint plikzrodlowy_liczbalinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikuzrodlowego);

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

                            Console.WriteLine("Trwa zapisywanie linii nr.: " + plikzrodlowy_numerlinii + "/" + plikzrodlowy_liczbalinii + " [" + PoliczPostepWProcentach(plikzrodlowy_numerlinii, plikzrodlowy_liczbalinii) + "%]");

                            plikzrodlowy_numerlinii++;

                        }

                        nowyplik_sw.Close();
                        plikzrodlowy_sr.Close();

                        Sukces("Utworzono nowy plik nie zawierajacy polskich znakow: " + "BezZnakowPL_" + nazwaplikuzrodlowego);

                    }
                    catch
                    {
                        Blad("BLAD: Wystapil nieoczekiwany blad w dostepie do plikow.");
                    }

                    nowyplik_fs.Close();
                    plikzrodlowy_fs.Close();

                }
                else
                {
                    Blad("BLAD: Wystapil problem ze zliczeniem linii w podanym pliku!");
                }

            }
            else
            {
                Blad("BLAD: Brak takiego pliku.");
            }


        }

        public static void JSONtoTXTTransifexCOM()
        {
            string nazwaplikuJSON;

            Console.Write("Podaj nazwę pliku JSON: ");
            nazwaplikuJSON = Console.ReadLine();
            if (nazwaplikuJSON == "") { nazwaplikuJSON = "test1.json"; }
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuJSON);
            if (File.Exists(folderglownyprogramu + nazwaplikuJSON))
            {
                uint plik_JSON_liczbalinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikuJSON);

                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_JSON_fs = new FileStream(nazwaplikuJSON, FileMode.Open, FileAccess.Read);
                FileStream nowy_plik_transifexCOMkeystxt_fs = new FileStream(nazwaplikuJSON + ".keysTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);
                FileStream nowy_plik_transifexCOMstringstxt_fs = new FileStream(nazwaplikuJSON + ".stringsTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);

                try
                {
                    int ilosc_wykrytych_STRINGS = 0;
                    int ilosc_wykrytych_VARS = 0;
                    List<List<string>> vars_tmp = new List<List<string>>(); //skladnia vars_tmp[numer_linii][0:key||1:ciag_zmiennych]
                    const char separator = ';';


                    StreamReader plik_JSON_sr = new StreamReader(plik_JSON_fs);
                    StreamWriter nowy_plik_transifexCOMkeystxt_sr = new StreamWriter(nowy_plik_transifexCOMkeystxt_fs);
                    StreamWriter nowy_plik_transifexCOMstringstxt_sr = new StreamWriter(nowy_plik_transifexCOMstringstxt_fs);

                    int plik_JSON_linia = 1;
                    while (plik_JSON_sr.Peek() != -1)
                    {
                        string tresc_linii_JSON = plik_JSON_sr.ReadLine();

                        string tresclinii_ciagzmiennych = "";

                        vars_tmp.Add(new List<string>());


                        string[] linia_podzial_1 = tresc_linii_JSON.Split(new string[] { "\": \"" }, StringSplitOptions.None);

                        /*
                        for (int a1 = 0; a1 < linia_podzial_1.Length; a1++)
                        {

                            //Console.WriteLine("linia_podzial_1[" + a1 + "]: " + linia_podzial_1[a1]);
                        }
                        */

                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_1.Length: " + linia_podzial_1.Length);

                        if (linia_podzial_1.Length <= 2)
                        {
                            string KEYt1 = linia_podzial_1[0].Trim();
                            int KEYt1_iloscznakow = KEYt1.Length;

                            if (KEYt1_iloscznakow >= 2)
                            {

                                string[] linia_2_separatory = { KEYt1 + "\": \"" };

                                string[] linia_podzial_2 = tresc_linii_JSON.Split(linia_2_separatory, StringSplitOptions.None);

                                /*
                                for (int a2 = 0; a2 < linia_podzial_2.Length; a2++)
                                {

                                    Console.WriteLine("linia_podzial_2[" + a2 + "]: " + linia_podzial_2[a2]);
                                }
                                */

                                //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_2.Length: " + linia_podzial_2.Length);

                                if (linia_podzial_2.Length >= 2)
                                {

                                    string STRINGt1 = linia_podzial_2[1].TrimEnd();
                                    int STRINGt1_iloscznakow = STRINGt1.Length;


                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEYt1_iloscznakow: " + KEYt1_iloscznakow);
                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRINGt1_iloscznakow: " + STRINGt1_iloscznakow);


                                    if (KEYt1_iloscznakow >= 2 && STRINGt1_iloscznakow >= 1)
                                    {
                                        string KEY = KEYt1.Remove(0, 1);

                                        int cofniecie_wskaznika = STRINGt1_iloscznakow - 1;
                                        int usunac_znakow = 1;
                                        if (plik_JSON_linia != plik_JSON_liczbalinii - 2)
                                        {
                                            cofniecie_wskaznika = STRINGt1_iloscznakow - 2;
                                            usunac_znakow = 2;
                                        }

                                        string STRINGt2 = STRINGt1.Remove(cofniecie_wskaznika, usunac_znakow);
                                        string STRING = STRINGt2;



                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEY:" + KEY);
                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRING:" + STRING);


                                        if (KEY != "$id")
                                        {

                                            string tresc_KEY = KEY;

                                            try
                                            {
                                                //Console.WriteLine("indeks wykrytego KEY'a: " + ilosc_wykrytych_VARS);

                                                vars_tmp[ilosc_wykrytych_VARS].Add(tresc_KEY);
                                            }
                                            catch
                                            {
                                                Blad("BLAD: vars_tmp #1!");
                                            }

                                            //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja klucza o treści: " + tresc_KEY);

                                            ilosc_wykrytych_VARS++;


                                            //string tresc_STRING = STRING;


                                            string tresc_STRING = STRING

                                            .Replace("\\n", "<br>")
                                            .Replace("\\\"", "<bs_n1>")

                                            ;

                                            if (tresc_STRING == "")
                                            {
                                                tresc_STRING = " ";
                                            }

                                            /*
                                            if (tresc_STRING.Contains('{') || tresc_STRING.Contains('}'))
                                            {
                                                string rodzajenawiasow = "{|}";
                                                int iloscnawiasowwlinii = 0;
                                                Regex regex = new Regex(rodzajenawiasow);
                                                MatchCollection matchCollection = regex.Matches(tresc_STRING);
                                                foreach (var match in matchCollection)
                                                {
                                                    iloscnawiasowwlinii++;
                                                }
                                                if (iloscnawiasowwlinii % 2 == 0)
                                                {
                                                    //Console.WriteLine("Linia nr." + plik_JSON_linia + " posiada pary nawiasów {}.");

                                                    if (tresc_STRING.Contains('{') && tresc_STRING.Contains('}'))
                                                    {
                                                        string[] tresclinii_nawklamrowy_podzial1 = tresc_STRING.Split(new char[] { '{' });

                                                        for (int i1 = 0; i1 < tresclinii_nawklamrowy_podzial1.Length; i1++)
                                                        {
                                                            //Console.WriteLine("tresclinii_nawklamrowy_podzial1[" + i1.ToString() + "]: " + tresclinii_nawklamrowy_podzial1[i1]);

                                                            if (tresclinii_nawklamrowy_podzial1[i1].Contains('}'))
                                                            {
                                                                int kl_index = i1 - 1;
                                                                string tresczwnetrzanawiasuklamrowego = tresclinii_nawklamrowy_podzial1[i1].Split(new char[] { '}' })[0];
                                                                string nazwazmiennej_w_stringstxt = "<kl" + kl_index + ">"; //np. <kl0>, <kl1>, <kl2> itd.

                                                                //Console.WriteLine("tresczwnetrzanawiasuklamrowego (" + i1.ToString() + "): " + tresczwnetrzanawiasuklamrowego);

                                                                tresclinii_ciagzmiennych += "{" + tresczwnetrzanawiasuklamrowego + "}";

                                                                if (i1 + 1 != tresclinii_nawklamrowy_podzial1.Length) { tresclinii_ciagzmiennych += separator; }

                                                                tresc_STRING = tresc_STRING.Replace("{" + tresczwnetrzanawiasuklamrowego + "}", nazwazmiennej_w_stringstxt);

                                                                //Console.WriteLine("nazwazmiennej_w_stringstxt: " + nazwazmiennej_w_stringstxt);
                                                                //Console.WriteLine("tresc_STRING: " + tresc_STRING);


                                                            }


                                                        }

                                                    }



                                                }
                                                else
                                                {
                                                    Blad("BŁĄD: Linia nr." + plik_JSON_linia + " ma błędną ilość nawiasów {}!");
                                                }



                                            }
                                            else
                                            {
                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " NIE posiada pary nawiasów {}.");

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja string'a o tresci: " + tresc_STRING);

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " zawiera VARS: " + tresclinii_ciagzmiennych);

                                            }
                                            */


                                            nowy_plik_transifexCOMstringstxt_sr.WriteLine(tresc_STRING);

                                            //vars_tmp[ilosc_wykrytych_STRINGS].Add(tresclinii_ciagzmiennych);

                                            ilosc_wykrytych_STRINGS++;


                                        }

                                    }

                                }

                            }


                        }


                        Console.WriteLine("Trwa konwertowanie linii nr. " + plik_JSON_linia + "/" + plik_JSON_liczbalinii + " [" + PoliczPostepWProcentach(plik_JSON_linia, plik_JSON_liczbalinii) + "%]");

                        plik_JSON_linia++;
                    }

                    //Console.WriteLine("ilosc_wykrytych_vars:" + ilosc_wykrytych_VARS);
                    //Console.WriteLine("vars_tmp[0][0]: " + vars_tmp[0][0]);


                    for (int iv1 = 0; iv1 < vars_tmp.Count; iv1++)
                    {
                        for (int iv2 = 0; iv2 < vars_tmp[iv1].Count; iv2++)
                        {
                            //Console.WriteLine("vars_tmp[" + iv1 + "][" + iv2 + "]: " + vars_tmp[iv1][iv2]);

                            if (iv2 == 0)
                            {
                                nowy_plik_transifexCOMkeystxt_sr.Write(vars_tmp[iv1][iv2]);
                            }
                            else if (iv2 == 1)
                            {
                                if (vars_tmp[iv1][iv2] != "")
                                {
                                    nowy_plik_transifexCOMkeystxt_sr.Write(separator + vars_tmp[iv1][iv2] + "\n");
                                }
                                else
                                {
                                    nowy_plik_transifexCOMkeystxt_sr.Write("\n");
                                }
                            }

                            nowy_plik_transifexCOMkeystxt_sr.Write("\n");

                        }
                    }





                    nowy_plik_transifexCOMkeystxt_sr.Close();
                    nowy_plik_transifexCOMstringstxt_sr.Close();
                    plik_JSON_sr.Close();


                }
                catch
                {
                    Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do plików.");
                }

                nowy_plik_transifexCOMkeystxt_fs.Close();
                nowy_plik_transifexCOMstringstxt_fs.Close();
                plik_JSON_fs.Close();


            }
            else
            {
                Blad("BŁĄD: Brak takiego pliku.");
            }

            if (File.Exists(nazwaplikuJSON + ".keys.txt") && File.Exists(nazwaplikuJSON + ".strings.txt"))
            {
                Console.WriteLine("----------------------------------");
                Sukces("Utworzono 2 pliki TXT: \"" + nazwaplikuJSON + ".keysTransifexCOM.txt\" oraz \"" + nazwaplikuJSON + ".stringsTransifexCOM.txt\"");

            }

        }

        public static void TXTTransifexCOMtoJSON()
        {

            string nazwaplikukeystxt;
            string nazwaplikustringstxt;
            string walidacjapoAutoTranslatorze;
            string nazwanowegoplikuJSON;
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const char separator = ';';

            Console.Write("Podaj nazwę pliku .keysTransifexCOM.txt: ");
            nazwaplikukeystxt = Console.ReadLine();
            if (nazwaplikukeystxt == "") { nazwaplikukeystxt = "test1.json.keys.txt"; }
            Console.WriteLine("Podano nazwę pliku .keysTransifexCOM.txt: " + nazwaplikukeystxt);

            Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt: ");
            nazwaplikustringstxt = Console.ReadLine();
            if (nazwaplikustringstxt == "") { nazwaplikustringstxt = "test1.json.strings.txt"; }
            Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM.txt: " + nazwaplikustringstxt);

            Console.Write("Włączyć walidację treści po autotranslatorze? [t/n]: ");
            walidacjapoAutoTranslatorze = Console.ReadLine();

            if (walidacjapoAutoTranslatorze == "t" || walidacjapoAutoTranslatorze == "n")
            {
                if (File.Exists(nazwaplikukeystxt) && File.Exists(nazwaplikustringstxt))
                {
                    if (walidacjapoAutoTranslatorze == "t")
                    {
                        nazwanowegoplikuJSON = "NOWY_WTPA_" + nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");
                    }
                    else
                    {
                        nazwanowegoplikuJSON = "NOWY_" + nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");
                    }

                    Console.WriteLine("Nazwa nowego pliku JSON to: " + nazwanowegoplikuJSON);

                    plikkeystxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikukeystxt);
                    plikstringstxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikustringstxt);
                    //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                    //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);



                    if (plikkeystxt_ilosclinii == plikstringstxt_ilosclinii)
                    {
                        bool naglowekJSON_rezultat = UtworzNaglowekJSON(nazwanowegoplikuJSON);
                        if (naglowekJSON_rezultat == true)
                        {
                            bool bledywplikuJSON = false;
                            FileStream nowyplikJSON_fs = new FileStream(nazwanowegoplikuJSON, FileMode.Append, FileAccess.Write);
                            FileStream plikkeystxt_fs = new FileStream(nazwaplikukeystxt, FileMode.Open, FileAccess.Read);

                            try //#1
                            {
                                string plikkeystxt_trescaktualnejlinii;
                                string plikstringstxt_trescaktualnejlinii;

                                StreamWriter nowyplikJSON_sw = new StreamWriter(nowyplikJSON_fs);
                                StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);

                                int plikkeystxt_sr_nraktualnejlinii = 1;
                                while (plikkeystxt_sr.Peek() != -1)
                                {
                                    plikkeystxt_trescaktualnejlinii = plikkeystxt_sr.ReadLine();
                                    string[] plikkeystxt_wartoscilinii = plikkeystxt_trescaktualnejlinii.Split(new char[] { separator }); //skladnia: plikkeystxt_wartoscilinii[0:key||0<:vars]

                                    //Console.WriteLine("Pobrano KEY   z linii " + plikkeystxt_sr_nraktualnejlinii + " o tresci: " + plikkeystxt_trescaktualnejlinii);

                                    FileStream plikstringstxt_fs = new FileStream(nazwaplikustringstxt, FileMode.Open, FileAccess.Read);

                                    try //#2
                                    {
                                        StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);

                                        int plikstringstxt_sr_nraktualnejlinii = 1;
                                        while (plikstringstxt_sr.Peek() != -1)
                                        {
                                            plikstringstxt_trescaktualnejlinii = plikstringstxt_sr.ReadLine();

                                            if (plikstringstxt_sr_nraktualnejlinii == plikkeystxt_sr_nraktualnejlinii)
                                            {

                                                string plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescaktualnejlinii;

                                                //Console.WriteLine("!!!: Liczba key+vars w linii nr. " + plikkeystxt_sr_nraktualnejlinii + ": " + plikkeystxt_wartoscilinii.Length);

                                                List<string> lista_zmiennych_linii = new List<string>();


                                                if (walidacjapoAutoTranslatorze == "t")
                                                {
                                                    plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii


                                                    /* POCZATEK - AUTOTRANSLATOR FIX - naprawa uszkodzonych zmiennych i tagow przez autotranslator */
                                                    //stale
                                                    .Replace("< ", "<")
                                                    .Replace(" >", ">")
                                                    .Replace("</ ", "</")
                                                    .Replace(" = ", "=")
                                                    .Replace("# ", "#")

                                                    .Replace(" <bs_n1>", "\\\"")
                                                    .Replace("<bs_n1> ", "\\\"")


                                                    //dodatkowo niedomkniete LONG'i mogą powodować zawieszanie sie i awarie gry
                                                    .Replace("[ LONGSTART]", "[LONGSTART]")
                                                    .Replace("[LONGSTART ]", "[LONGSTART]")
                                                    .Replace("[ LONGSTART ]", "[LONGSTART]")
                                                    .Replace("[ LONGEND]", "[LONGEND]")
                                                    .Replace("[LONGEND ]", "[LONGEND]")
                                                    .Replace("[ LONGEND ]", "[LONGEND]");


                                                    //chwilowe

                                                    /* KONIEC - AUTOTRANSLATOR FIX - naprawa uszkodzonych zmiennych i tagow przez autotranslator */

                                                }


                                                plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii

                                                .Replace("<br>", "\\n")
                                                .Replace("<bs_n1>", "\\\"");


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

                                                if (plikstringstxt_sr_nraktualnejlinii != plikkeystxt_ilosclinii)
                                                {
                                                    nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\",");
                                                }
                                                else
                                                {
                                                    nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\"");
                                                }


                                            }


                                            plikstringstxt_sr_nraktualnejlinii++;
                                        }


                                        plikstringstxt_sr.Close();

                                    }
                                    catch
                                    {
                                        Blad("BLAD: Wystapil nieoczekiwany blad w dostepie do plikow. (TRY #2, plikkeystxt_sr_nraktualnejlinii: " + plikkeystxt_sr_nraktualnejlinii + ")");
                                    }

                                    Console.WriteLine("Trwa konwertowanie linii nr.: " + plikkeystxt_sr_nraktualnejlinii + "/" + plikkeystxt_ilosclinii + " [" + PoliczPostepWProcentach(plikkeystxt_sr_nraktualnejlinii, plikkeystxt_ilosclinii) + "%]");

                                    plikkeystxt_sr_nraktualnejlinii++;

                                    plikstringstxt_fs.Close();

                                }
                                plikkeystxt_sr.Close();
                                nowyplikJSON_sw.Close();


                                bool stopkaJSON_rezultat = UtworzStopkeJSON(nazwanowegoplikuJSON);

                                if (stopkaJSON_rezultat == true)
                                {
                                    //Console.WriteLine("Pomyślnie utworzono stopkę w pliku JSON.");
                                }
                                else
                                {
                                    Blad("BŁĄD: Wystąpil problem z utworzeniem stopki w pliku JSON.");
                                }


                                if (naglowekJSON_rezultat == true && stopkaJSON_rezultat == true)
                                {
                                    Sukces("Plik JSON o nazwie \"" + nazwanowegoplikuJSON + "\" zostal wygenerowany.");
                                }
                                else
                                {
                                    bledywplikuJSON = true;

                                    Blad("BŁĄD: Plik JSON nie został wygenerowany (patrz: błędy powyżej)!");
                                }

                            }
                            catch
                            {
                                Blad("BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do plików. (TRY #1)");
                            }

                            plikkeystxt_fs.Close();
                            nowyplikJSON_fs.Close();

                            if (bledywplikuJSON == true && File.Exists(nazwanowegoplikuJSON))
                            {
                                File.Delete(nazwanowegoplikuJSON);

                                //Blad("bledywplikuJSON: true");
                            }
                            else
                            {
                                //Sukces("bledywplikuJSON: false");
                            }

                        }
                        else
                        {
                            Blad("BŁĄD: Wystapil problem z utworzeniem nagłówka w pliku JSON!");
                        }

                    }
                    else
                    {
                        Blad("BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!");
                    }


                }
                else
                {
                    Blad("BŁĄD: Brak wskazanych plików.");
                }

            }
            else
            {
                Blad("BŁĄD: Podano nieprawidłową wartość. Prawidłowa wartość to t lub n.");
            }


        }

        public static void TXTTransifexCOM_laczenieStringow2Lokalizacji()
        {

            string nazwaPIERWSZEGOplikustringstxt;
            string nazwaDRUGIEGOplikustringstxt;
            string nazwanowegoplikustringstxt;
            uint plikstringstxt_nr1_ilosclinii;
            uint plikstringstxt_nr2_ilosclinii;

            Console.Write("Podaj nazwę pliku oryginalnej angielksiej lokalizacji .stringsTransifexCOM.txt (EN): ");
            nazwaPIERWSZEGOplikustringstxt = Console.ReadLine();
            if (nazwaPIERWSZEGOplikustringstxt == "") { nazwaPIERWSZEGOplikustringstxt = ".stringsTransifexCOM.txt"; }
            Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM: " + nazwaPIERWSZEGOplikustringstxt);

            Console.Write("Podaj nazwę pliku polskiej lokalizacji .stringsTransifexCOM.txt (PL): ");
            nazwaDRUGIEGOplikustringstxt = Console.ReadLine();
            if (nazwaDRUGIEGOplikustringstxt == "") { nazwaDRUGIEGOplikustringstxt = ".stringsTransifexCOM.txt"; }
            Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM: " + nazwaDRUGIEGOplikustringstxt);


            if (File.Exists(folderglownyprogramu + nazwaPIERWSZEGOplikustringstxt) && File.Exists(folderglownyprogramu + nazwaDRUGIEGOplikustringstxt))
            {
                plikstringstxt_nr1_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaPIERWSZEGOplikustringstxt);
                plikstringstxt_nr2_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaDRUGIEGOplikustringstxt);

                FileStream plik1stringstxt_fs = new FileStream(nazwaPIERWSZEGOplikustringstxt, FileMode.Open, FileAccess.Read);
                FileStream plik2stringstxt_fs = new FileStream(nazwaDRUGIEGOplikustringstxt, FileMode.Open, FileAccess.Read);

                FileStream nowyplikstringstxt_fs = new FileStream(nazwaPIERWSZEGOplikustringstxt + "___I___" + nazwaDRUGIEGOplikustringstxt, FileMode.Append, FileAccess.Write);




                if (plikstringstxt_nr1_ilosclinii == plikstringstxt_nr2_ilosclinii)
                {

                    try
                    {
                        StreamReader plik1stringstxt_sr = new StreamReader(plik1stringstxt_fs);
                        StreamReader plik2stringstxt_sr = new StreamReader(plik2stringstxt_fs);

                        StreamWriter nowyplikstringstxt_sw = new StreamWriter(nowyplikstringstxt_fs);

                        uint plik1stringstxt_aktualnynumerlinii = 1;

                        while (plik1stringstxt_sr.Peek() != -1)
                        {

                            string plik1stringstxt_tresclinii = plik1stringstxt_sr.ReadLine();
                            string plik2stringstxt_tresclinii = plik2stringstxt_sr.ReadLine();

                            nowyplikstringstxt_sw.WriteLine("[[[---" + plik1stringstxt_tresclinii + "---]]] " + plik2stringstxt_tresclinii);


                            Console.WriteLine("Trwa konwertowanie linii nr. " + plik1stringstxt_aktualnynumerlinii + "/" + plikstringstxt_nr1_ilosclinii + " [" + PoliczPostepWProcentach(plik1stringstxt_aktualnynumerlinii, plikstringstxt_nr1_ilosclinii) + "%]");


                            plik1stringstxt_aktualnynumerlinii++;
                        }

                        plik1stringstxt_sr.Close();
                        plik2stringstxt_sr.Close();

                        nowyplikstringstxt_sw.Close();



                    }
                    catch
                    {
                        Blad("BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do plików. (TRY #1)");
                    }



                }
                else
                {
                    Blad("BŁĄD: Ilość linii w obydwu plikach nie jest identyczna!");
                }

                plik1stringstxt_fs.Close();
                plik2stringstxt_fs.Close();

                nowyplikstringstxt_fs.Close();


            }
            else
            {

                Blad("BŁĄD: Brak wskazanych plików.");

            }


        }

        public static void JSONtoTXTTransifexCOM_ZNumeramiLiniiZPlikuJSON()
        {
            string nazwaplikuJSON;

            Console.Write("Podaj nazwę pliku JSON: ");
            nazwaplikuJSON = Console.ReadLine();
            if (nazwaplikuJSON == "") { nazwaplikuJSON = "test1.json"; }
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuJSON);
            if (File.Exists(folderglownyprogramu + nazwaplikuJSON))
            {
                uint plik_JSON_liczbalinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikuJSON);

                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_JSON_fs = new FileStream(nazwaplikuJSON, FileMode.Open, FileAccess.Read);
                FileStream nowy_plik_transifexCOMkeystxt_fs = new FileStream(nazwaplikuJSON + ".keysTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);
                FileStream nowy_plik_transifexCOMstringstxt_fs = new FileStream(nazwaplikuJSON + ".stringsTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);

                try
                {
                    int ilosc_wykrytych_STRINGS = 0;
                    int ilosc_wykrytych_VARS = 0;
                    List<List<string>> vars_tmp = new List<List<string>>(); //skladnia vars_tmp[numer_linii][0:key||1:ciag_zmiennych]
                    const char separator = ';';


                    StreamReader plik_JSON_sr = new StreamReader(plik_JSON_fs);
                    StreamWriter nowy_plik_transifexCOMkeystxt_sr = new StreamWriter(nowy_plik_transifexCOMkeystxt_fs);
                    StreamWriter nowy_plik_transifexCOMstringstxt_sr = new StreamWriter(nowy_plik_transifexCOMstringstxt_fs);

                    if (wylacz_calkowitepokazywaniepostepow == true)
                    {
                        Console.WriteLine("Trwa konwertowanie linii. Może to chwilę zająć. Proszę czekać...");
                    }
                    
                    int plik_JSON_linia = 1;
                    while (plik_JSON_sr.Peek() != -1)
                    {
                        string tresc_linii_JSON = plik_JSON_sr.ReadLine();

                        string tresclinii_ciagzmiennych = "";

                        vars_tmp.Add(new List<string>());


                        string[] linia_podzial_1 = tresc_linii_JSON.Split(new string[] { "\": \"" }, StringSplitOptions.None);

                        /*
                        for (int a1 = 0; a1 < linia_podzial_1.Length; a1++)
                        {

                            //Console.WriteLine("linia_podzial_1[" + a1 + "]: " + linia_podzial_1[a1]);
                        }
                        */

                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_1.Length: " + linia_podzial_1.Length);

                        if (linia_podzial_1.Length <= 2)
                        {
                            string KEYt1 = linia_podzial_1[0].Trim();
                            int KEYt1_iloscznakow = KEYt1.Length;

                            if (KEYt1_iloscznakow >= 2)
                            {

                                string[] linia_2_separatory = { KEYt1 + "\": \"" };

                                string[] linia_podzial_2 = tresc_linii_JSON.Split(linia_2_separatory, StringSplitOptions.None);

                                /*
                                for (int a2 = 0; a2 < linia_podzial_2.Length; a2++)
                                {

                                    Console.WriteLine("linia_podzial_2[" + a2 + "]: " + linia_podzial_2[a2]);
                                }
                                */

                                //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_2.Length: " + linia_podzial_2.Length);

                                if (linia_podzial_2.Length >= 2)
                                {

                                    string STRINGt1 = linia_podzial_2[1].TrimEnd();
                                    int STRINGt1_iloscznakow = STRINGt1.Length;


                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEYt1_iloscznakow: " + KEYt1_iloscznakow);
                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRINGt1_iloscznakow: " + STRINGt1_iloscznakow);


                                    if (KEYt1_iloscznakow >= 2 && STRINGt1_iloscznakow >= 1)
                                    {
                                        string KEY = KEYt1.Remove(0, 1);

                                        int cofniecie_wskaznika = STRINGt1_iloscznakow - 1;
                                        int usunac_znakow = 1;
                                        if (plik_JSON_linia != plik_JSON_liczbalinii - 2)
                                        {
                                            cofniecie_wskaznika = STRINGt1_iloscznakow - 2;
                                            usunac_znakow = 2;
                                        }

                                        string STRINGt2 = STRINGt1.Remove(cofniecie_wskaznika, usunac_znakow);
                                        string STRING = STRINGt2;



                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEY:" + KEY);
                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRING:" + STRING);


                                        if (KEY != "$id")
                                        {

                                            string tresc_KEY = KEY;

                                            try
                                            {
                                                //Console.WriteLine("indeks wykrytego KEY'a: " + ilosc_wykrytych_VARS);

                                                vars_tmp[ilosc_wykrytych_VARS].Add(tresc_KEY);
                                            }
                                            catch
                                            {
                                                Blad("BLAD: vars_tmp #1!");
                                            }

                                            //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja klucza o treści: " + tresc_KEY);

                                            ilosc_wykrytych_VARS++;


                                            //string tresc_STRING = STRING;


                                            string tresc_STRING = STRING

                                            .Replace("\\n", "<br>")
                                            .Replace("\\\"", "<bs_n1>")
                                            .Replace("\\\\", "/") // tę linię dodano w v.1.64 - wykasować ją, jeśli będą występować problemy z parsowaniem pliku JSON wygenerowanego w pwrpl-converter w wersji v.2.03 lub nowszej
                                            ;

                                            if (tresc_STRING == "")
                                            {
                                                tresc_STRING = " ";
                                            }

                                            /*
                                            if (tresc_STRING.Contains('{') || tresc_STRING.Contains('}'))
                                            {
                                                string rodzajenawiasow = "{|}";
                                                int iloscnawiasowwlinii = 0;
                                                Regex regex = new Regex(rodzajenawiasow);
                                                MatchCollection matchCollection = regex.Matches(tresc_STRING);
                                                foreach (var match in matchCollection)
                                                {
                                                    iloscnawiasowwlinii++;
                                                }
                                                if (iloscnawiasowwlinii % 2 == 0)
                                                {
                                                    //Console.WriteLine("Linia nr." + plik_JSON_linia + " posiada pary nawiasów {}.");

                                                    if (tresc_STRING.Contains('{') && tresc_STRING.Contains('}'))
                                                    {
                                                        string[] tresclinii_nawklamrowy_podzial1 = tresc_STRING.Split(new char[] { '{' });

                                                        for (int i1 = 0; i1 < tresclinii_nawklamrowy_podzial1.Length; i1++)
                                                        {
                                                            //Console.WriteLine("tresclinii_nawklamrowy_podzial1[" + i1.ToString() + "]: " + tresclinii_nawklamrowy_podzial1[i1]);

                                                            if (tresclinii_nawklamrowy_podzial1[i1].Contains('}'))
                                                            {
                                                                int kl_index = i1 - 1;
                                                                string tresczwnetrzanawiasuklamrowego = tresclinii_nawklamrowy_podzial1[i1].Split(new char[] { '}' })[0];
                                                                string nazwazmiennej_w_stringstxt = "<kl" + kl_index + ">"; //np. <kl0>, <kl1>, <kl2> itd.

                                                                //Console.WriteLine("tresczwnetrzanawiasuklamrowego (" + i1.ToString() + "): " + tresczwnetrzanawiasuklamrowego);

                                                                tresclinii_ciagzmiennych += "{" + tresczwnetrzanawiasuklamrowego + "}";

                                                                if (i1 + 1 != tresclinii_nawklamrowy_podzial1.Length) { tresclinii_ciagzmiennych += separator; }

                                                                tresc_STRING = tresc_STRING.Replace("{" + tresczwnetrzanawiasuklamrowego + "}", nazwazmiennej_w_stringstxt);

                                                                //Console.WriteLine("nazwazmiennej_w_stringstxt: " + nazwazmiennej_w_stringstxt);
                                                                //Console.WriteLine("tresc_STRING: " + tresc_STRING);


                                                            }


                                                        }

                                                    }



                                                }
                                                else
                                                {
                                                    Blad("BŁĄD: Linia nr." + plik_JSON_linia + " ma błędną ilość nawiasów {}!");
                                                }



                                            }
                                            else
                                            {
                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " NIE posiada pary nawiasów {}.");

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja string'a o tresci: " + tresc_STRING);

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " zawiera VARS: " + tresclinii_ciagzmiennych);

                                            }
                                            */


                                            nowy_plik_transifexCOMstringstxt_sr.WriteLine("<" + plik_JSON_linia + ">" + tresc_STRING);

                                            //vars_tmp[ilosc_wykrytych_STRINGS].Add(tresclinii_ciagzmiennych);

                                            ilosc_wykrytych_STRINGS++;


                                        }

                                    }

                                }

                            }


                        }


                        if (wylacz_calkowitepokazywaniepostepow == false)
                        {
                            Console.WriteLine("Trwa konwertowanie linii nr. " + plik_JSON_linia + "/" + plik_JSON_liczbalinii + " [" + PoliczPostepWProcentach(plik_JSON_linia, plik_JSON_liczbalinii) + "%]");
                        }

                        plik_JSON_linia++;
                    }

                    //Console.WriteLine("ilosc_wykrytych_vars:" + ilosc_wykrytych_VARS);
                    //Console.WriteLine("vars_tmp[0][0]: " + vars_tmp[0][0]);


                    for (int iv1 = 0; iv1 < vars_tmp.Count; iv1++)
                    {
                        for (int iv2 = 0; iv2 < vars_tmp[iv1].Count; iv2++)
                        {
                            //Console.WriteLine("vars_tmp[" + iv1 + "][" + iv2 + "]: " + vars_tmp[iv1][iv2]);

                            if (iv2 == 0)
                            {
                                nowy_plik_transifexCOMkeystxt_sr.Write(vars_tmp[iv1][iv2]);
                            }
                            else if (iv2 == 1)
                            {
                                if (vars_tmp[iv1][iv2] != "")
                                {
                                    nowy_plik_transifexCOMkeystxt_sr.Write(separator + vars_tmp[iv1][iv2] + "\n");
                                }
                                else
                                {
                                    nowy_plik_transifexCOMkeystxt_sr.Write("\n");
                                }
                            }

                            nowy_plik_transifexCOMkeystxt_sr.Write("\n");

                        }
                    }





                    nowy_plik_transifexCOMkeystxt_sr.Close();
                    nowy_plik_transifexCOMstringstxt_sr.Close();
                    plik_JSON_sr.Close();

                    Sukces("Utworzono 2 pliki:");
                    Sukces("-\"" + nazwaplikuJSON + ".keysTransifexCOM.txt\": przeznaczony dla narzędzia pwrpl-converter.");
                    Sukces("-\"" + nazwaplikuJSON + ".stringsTransifexCOM.txt\": przeznaczony dla platformy Transifex.");


                }
                catch
                {
                    Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do plików.");
                }

                nowy_plik_transifexCOMkeystxt_fs.Close();
                nowy_plik_transifexCOMstringstxt_fs.Close();
                plik_JSON_fs.Close();


            }
            else
            {
                Blad("BŁĄD: Brak takiego pliku.");
            }

            if (File.Exists(nazwaplikuJSON + ".keys.txt") && File.Exists(nazwaplikuJSON + ".strings.txt"))
            {
                Console.WriteLine("----------------------------------");
                Sukces("Utworzono 2 pliki TXT: \"" + nazwaplikuJSON + ".keysTransifexCOM.txt\" oraz \"" + nazwaplikuJSON + ".stringsTransifexCOM.txt\"");

            }





        }

        public static void StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach()
        {
            List<int> bledy = new List<int>();

            string nazwaplikustringsTransifexCOMTXT;

            Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt: ");
            nazwaplikustringsTransifexCOMTXT = Console.ReadLine();
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikustringsTransifexCOMTXT);
            if (File.Exists(folderglownyprogramu + nazwaplikustringsTransifexCOMTXT))
            {
                uint plik_stringsTransifexCOMTXT_liczbalinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikustringsTransifexCOMTXT);

                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_stringsTransifexCOMTXT_fs = new FileStream(nazwaplikustringsTransifexCOMTXT, FileMode.Open, FileAccess.Read);

                try
                {
                    string plik_stringsTransifexCOMTXT_trescaktualnejlinii;

                    StreamReader plik_stringsTransifexCOMTXT_sr = new StreamReader(plik_stringsTransifexCOMTXT_fs);

                    int plik_stringsTransifexCOMTXT_aktualnalinia = 1;
                    while (plik_stringsTransifexCOMTXT_sr.Peek() != -1)
                    {
                        plik_stringsTransifexCOMTXT_trescaktualnejlinii = plik_stringsTransifexCOMTXT_sr.ReadLine();

                        int plik_stringsTransifexCOMTXT_aktualnyidlinii = plik_stringsTransifexCOMTXT_aktualnalinia + 3;

                        string[] podzial1 = plik_stringsTransifexCOMTXT_trescaktualnejlinii.Split(new char[] { '>' });
                        int id_pobrane_z_tresci_pliku;

                        Console.WriteLine("Trwa analizowanie linii nr: " + plik_stringsTransifexCOMTXT_aktualnalinia + "/" + plik_stringsTransifexCOMTXT_liczbalinii + " [" + PoliczPostepWProcentach(plik_stringsTransifexCOMTXT_aktualnalinia, plik_stringsTransifexCOMTXT_liczbalinii) + "%]");

                        try
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
                    Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do pliku.");
                }


                plik_stringsTransifexCOMTXT_fs.Close();



            }
            else
            {
                Blad("BŁĄD: Brak takiego pliku.");
            }


            int bledy_iloscwykrytych = bledy.Count();
            if (bledy_iloscwykrytych == 0)
            {
                Sukces("Nie znaleziono błędów w identyfikatorach linii na początku stringów.");
            }
            else
            {
                Blad("Znaleziono błędów w pliku: " + bledy_iloscwykrytych);

                for (int ib = 0; ib < bledy_iloscwykrytych; ib++)
                {
                    int numer_linii = bledy[ib];
                    int poprawny_identyfikator_linii = numer_linii + 3;

                    Blad("Wykryto błąd w linii nr: " + numer_linii + " (poprawny id powinien mieć treść: <" + poprawny_identyfikator_linii.ToString() + ">)");
                }

            }


        }
        public static void TXTTransifexCOMtoJSON_JednoWatkowyZNumeramiLiniiZPlikuJSON()
        {

            string nazwaplikukeystxt;
            string nazwaplikustringstxt;
            string nazwanowegoplikuJSON;
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const char separator = ';';

            Console.Write("Podaj nazwę pliku .keysTransifexCOM.txt: ");
            nazwaplikukeystxt = Console.ReadLine();
            if (nazwaplikukeystxt == "") { nazwaplikukeystxt = "test1.json.keys.txt"; }
            Console.WriteLine("Podano nazwę pliku .keysTransifexCOM.txt: " + nazwaplikukeystxt);

            Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt: ");
            nazwaplikustringstxt = Console.ReadLine();
            if (nazwaplikustringstxt == "") { nazwaplikustringstxt = "test1.json.strings.txt"; }
            Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM.txt: " + nazwaplikustringstxt);


            if (File.Exists(nazwaplikukeystxt) && File.Exists(nazwaplikustringstxt))
            {
                nazwanowegoplikuJSON = "NOWY_" + nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");

                Console.WriteLine("Nazwa nowego pliku JSON to: " + nazwanowegoplikuJSON);

                plikkeystxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikukeystxt);
                plikstringstxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikustringstxt);
                //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);



                if (plikkeystxt_ilosclinii == plikstringstxt_ilosclinii)
                {
                    bool naglowekJSON_rezultat = UtworzNaglowekJSON(nazwanowegoplikuJSON);
                    if (naglowekJSON_rezultat == true)
                    {
                        bool bledywplikuJSON = false;
                        FileStream nowyplikJSON_fs = new FileStream(nazwanowegoplikuJSON, FileMode.Append, FileAccess.Write);
                        FileStream plikkeystxt_fs = new FileStream(nazwaplikukeystxt, FileMode.Open, FileAccess.Read);

                        try //#1
                        {
                            string plikkeystxt_trescaktualnejlinii;
                            string plikstringstxt_trescaktualnejlinii;

                            StreamWriter nowyplikJSON_sw = new StreamWriter(nowyplikJSON_fs);
                            StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);

                            int plikkeystxt_sr_nraktualnejlinii = 1;
                            while (plikkeystxt_sr.Peek() != -1)
                            {
                                plikkeystxt_trescaktualnejlinii = plikkeystxt_sr.ReadLine();
                                string[] plikkeystxt_wartoscilinii = plikkeystxt_trescaktualnejlinii.Split(new char[] { separator }); //skladnia: plikkeystxt_wartoscilinii[0:key||0<:vars]

                                //Console.WriteLine("Pobrano KEY   z linii " + plikkeystxt_sr_nraktualnejlinii + " o tresci: " + plikkeystxt_trescaktualnejlinii);

                                FileStream plikstringstxt_fs = new FileStream(nazwaplikustringstxt, FileMode.Open, FileAccess.Read);

                                try //#2
                                {
                                    StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);

                                    int plikstringstxt_sr_nraktualnejlinii = 1;
                                    while (plikstringstxt_sr.Peek() != -1)
                                    {
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



                                            plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii

                                            .Replace("<br>", "\\n")
                                            .Replace("<bs_n1>", "\\\"");


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

                                            if (plikstringstxt_sr_nraktualnejlinii != plikkeystxt_ilosclinii)
                                            {
                                                nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\",");
                                            }
                                            else
                                            {
                                                nowyplikJSON_sw.WriteLine("    \"" + plikkeystxt_wartoscilinii[0] + "\": \"" + plikstringstxt_trescuaktualnionalinii + "\"");
                                            }


                                        }


                                        plikstringstxt_sr_nraktualnejlinii++;
                                    }


                                    plikstringstxt_sr.Close();

                                }
                                catch
                                {
                                    Blad("BLAD: Wystapil nieoczekiwany blad w dostepie do plikow. (TRY #2, plikkeystxt_sr_nraktualnejlinii: " + plikkeystxt_sr_nraktualnejlinii + ")");
                                }

                                Console.WriteLine("Trwa konwertowanie linii nr.: " + plikkeystxt_sr_nraktualnejlinii + "/" + plikkeystxt_ilosclinii + " [" + PoliczPostepWProcentach(plikkeystxt_sr_nraktualnejlinii, plikkeystxt_ilosclinii) + "%]");

                                plikkeystxt_sr_nraktualnejlinii++;

                                plikstringstxt_fs.Close();

                            }
                            plikkeystxt_sr.Close();
                            nowyplikJSON_sw.Close();


                            bool stopkaJSON_rezultat = UtworzStopkeJSON(nazwanowegoplikuJSON);

                            if (stopkaJSON_rezultat == true)
                            {
                                //Console.WriteLine("Pomyślnie utworzono stopkę w pliku JSON.");
                            }
                            else
                            {
                                Blad("BŁĄD: Wystąpil problem z utworzeniem stopki w pliku JSON.");
                            }


                            if (naglowekJSON_rezultat == true && stopkaJSON_rezultat == true)
                            {
                                Sukces("Plik JSON o nazwie \"" + nazwanowegoplikuJSON + "\" zostal wygenerowany.");
                            }
                            else
                            {
                                bledywplikuJSON = true;

                                Blad("BŁĄD: Plik JSON nie został wygenerowany (patrz: błędy powyżej)!");
                            }

                        }
                        catch
                        {
                            Blad("BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do plików. (TRY #1)");
                        }

                        plikkeystxt_fs.Close();
                        nowyplikJSON_fs.Close();

                        if (bledywplikuJSON == true && File.Exists(nazwanowegoplikuJSON))
                        {
                            File.Delete(nazwanowegoplikuJSON);

                            //Blad("bledywplikuJSON: true");
                        }
                        else
                        {
                            //Sukces("bledywplikuJSON: false");
                        }

                    }
                    else
                    {
                        Blad("BŁĄD: Wystapil problem z utworzeniem nagłówka w pliku JSON!");
                    }

                }
                else
                {
                    Blad("BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!");
                }


            }
            else
            {
                Blad("BŁĄD: Brak wskazanych plików.");
            }

            //}
            //else
            //{
            //Blad("BŁĄD: Podano nieprawidłową wartość. Prawidłowa wartość to t lub n.");
            //}


        }

        public static void TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON()
        {
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - POCZĄTEK */
            if (Directory.Exists(nazwafolderutmp) == true)
            {
                Directory.Delete(nazwafolderutmp, true);
            }
            /* USUWANIE FOLDERU TMP WRAZ Z ZAWARTOŚCIĄ (JEŚLI ISTNIEJE) - KONIEC */


            string nazwaplikukeystxt;
            string nazwaplikustringstxt;
            string nazwanowegoplikuJSON;
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const int ilosc_watkow = 20;
            List<string> plikkeystxt_trescilinii = new List<string>();
            List<string> plikstringstxt_trescilinii = new List<string>();


            Console.Write("Podaj nazwę pliku .keysTransifexCOM.txt: ");
            nazwaplikukeystxt = Console.ReadLine();
            Console.WriteLine("Podano nazwę pliku .keysTransifexCOM.txt: " + nazwaplikukeystxt);

            Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt: ");
            nazwaplikustringstxt = Console.ReadLine();
            Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM.txt: " + nazwaplikustringstxt);


            if (File.Exists(folderglownyprogramu + nazwaplikukeystxt) && File.Exists(folderglownyprogramu + nazwaplikustringstxt))
            {
                nazwanowegoplikuJSON = "NOWY_" + nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");

                Console.WriteLine("Nazwa nowego pliku JSON to: " + nazwanowegoplikuJSON);

                plikkeystxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikukeystxt);
                plikstringstxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikustringstxt);
                //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);

                tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP = plikkeystxt_ilosclinii;




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


                    FileStream plikkeystxt_fs = new FileStream(nazwaplikukeystxt, FileMode.Open, FileAccess.Read);
                    FileStream plikstringstxt_fs = new FileStream(nazwaplikustringstxt, FileMode.Open, FileAccess.Read);

                    StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);
                    StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);



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

                        UtworzPlikTXT_TMP(nazwaplikukeystxt + "_" + numer_pliku + ".tmp", plikkeystxt_trescilinii, index_od, index_do);
                        UtworzPlikTXT_TMP(nazwaplikustringstxt + "_" + numer_pliku + ".tmp", plikstringstxt_trescilinii, index_od, index_do);

                        listaplikowkeystxtTMP.Add(nazwaplikukeystxt + "_" + numer_pliku + ".tmp");
                        listaplikowstringstxtTMP.Add(nazwaplikustringstxt + "_" + numer_pliku + ".tmp");
                        listaplikowjsonTMP.Add(nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "") + "_" + numer_pliku + ".tmp");
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



                    string nazwafinalnegoplikuJSON = "NOWY_" + nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");

                    if (File.Exists(folderglownyprogramu + nazwafinalnegoplikuJSON) == true) { File.Delete(folderglownyprogramu + nazwafinalnegoplikuJSON); }


                    UtworzNaglowekJSON(nazwanowegoplikuJSON);


                    FileStream finalnyplikJSON_fs = new FileStream(folderglownyprogramu + nazwafinalnegoplikuJSON, FileMode.Append, FileAccess.Write);

                    try //#1
                    {
                        StreamWriter finalnyplikJSON_sw = new StreamWriter(finalnyplikJSON_fs);


                        for (int lpj = 0; lpj < tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP.Count; lpj++)
                        {
                            FileStream plikjsonTMP_fs = new FileStream(folderglownyprogramu + nazwafolderutmp + sc + tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP[lpj], FileMode.Open, FileAccess.Read);

                            try //#2
                            {
                                StreamReader plikjsonTMP_sr = new StreamReader(plikjsonTMP_fs);

                                finalnyplikJSON_sw.Write(plikjsonTMP_sr.ReadToEnd());

                                plikjsonTMP_sr.Close();
                            }
                            catch (Exception Error)
                            {
                                Blad("BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #2 (for-lpj: " + lpj + ", Error: " + Error + ")!");
                            }

                            plikjsonTMP_fs.Close();


                        }

                        finalnyplikJSON_sw.Close();



                    }
                    catch (Exception Error)
                    {
                        Blad("BŁĄD: Wystąpił nieoczekiwany wyjątek w dostępie do plików #1 (Error: " + Error + ")!");
                    }


                    finalnyplikJSON_fs.Close();

                    bool stopkaJSON_rezultat = UtworzStopkeJSON(nazwanowegoplikuJSON);

                    if (stopkaJSON_rezultat == true)
                    {
                        Sukces("Plik JSON o nazwie \"" + nazwanowegoplikuJSON + "\" zostal wygenerowany.");
                    }

                }
                else
                {
                    Blad("BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!");
                }


            }
            else
            {
                Blad("BŁĄD: Brak wskazanych plików.");
            }


            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP);
            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP);
            UsunPlikiTMP(tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP);



        }

        public static void TXTstringsTransifexCOM_ZastapienieWylacznieNieprzetłumaczonychLinii()
        {
            string nazwapliku_stringsTransifexCOMtxtORIGEN;
            string nazwapliku_stringsTransifexCOMtxtNajaktualniejszyPLiEN;
            string nazwapliku_stringsTransifexCOMtxtZawierajacyWszystkieLiniePL;
            string nazwanowegopliku_stringsTransifexCOMtxt;


            uint plikstringsTransifexCOMtxtORIGEN_ilosclinii;
            uint plikstringsTransifexCOMtxtNajaktualniejszyPLiEN_ilosclinii;
            uint plikstringsTransifexCOMtxtZawierajacyWszystkieLiniePL_ilosclinii;

            Console.WriteLine("UWAGA: Tylko linie nieprzetłumaczone zostaną zastąpione!");

            Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt oryginalnej angielskiej lokalizacji: ");
            nazwapliku_stringsTransifexCOMtxtORIGEN = Console.ReadLine();
            //Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM.txt oryginalnej angielskiej lokalizacji: " + nazwapliku_stringsTransifexCOMtxtORIGEN);

            Console.Write("Podaj nazwę najaktualniejszego pliku .stringsTransifexCOM.txt zawierającego część linii przetłumaczonych na PL, a część linii nieprzetłumaczonych: ");
            nazwapliku_stringsTransifexCOMtxtNajaktualniejszyPLiEN = Console.ReadLine();
            //Console.WriteLine("Podano nazwę najaktualniejszego pliku .stringsTransifexCOM.txt zawierającego część linii przetłumaczonych na PL, a część linii nieprzetłumaczonych: " + nazwapliku_stringsTransifexCOMtxtNajaktualniejszyPLiEN);

            Console.Write("Podaj nazwę pliku .stringsTransifexCOM.txt zawierającego wszystkie przetłumaczone na PL linie, z którego mają zostać przeniesione tłumaczenia do pliku z częścią nieprzetłumaczonych linii: ");
            nazwapliku_stringsTransifexCOMtxtZawierajacyWszystkieLiniePL = Console.ReadLine();
            //Console.WriteLine("Podano nazwę pliku .stringsTransifexCOM.txt zawierającego wszystkie przetłumaczone na PL linie, z którego mają zostać przeniesione tłumaczenia do pliku z częścią nieprzetłumaczonych linii: " + nazwapliku_stringsTransifexCOMtxtZawierajacyWszystkieLiniePL);

            nazwanowegopliku_stringsTransifexCOMtxt = "NOWY_" + nazwapliku_stringsTransifexCOMtxtNajaktualniejszyPLiEN;

            if (File.Exists(nazwapliku_stringsTransifexCOMtxtORIGEN) == true && File.Exists(nazwapliku_stringsTransifexCOMtxtNajaktualniejszyPLiEN) == true && File.Exists(nazwapliku_stringsTransifexCOMtxtZawierajacyWszystkieLiniePL) == true)
            {
                plikstringsTransifexCOMtxtORIGEN_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwapliku_stringsTransifexCOMtxtORIGEN);
                plikstringsTransifexCOMtxtNajaktualniejszyPLiEN_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwapliku_stringsTransifexCOMtxtNajaktualniejszyPLiEN);
                plikstringsTransifexCOMtxtZawierajacyWszystkieLiniePL_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwapliku_stringsTransifexCOMtxtZawierajacyWszystkieLiniePL);

                if (plikstringsTransifexCOMtxtORIGEN_ilosclinii == plikstringsTransifexCOMtxtNajaktualniejszyPLiEN_ilosclinii && plikstringsTransifexCOMtxtNajaktualniejszyPLiEN_ilosclinii == plikstringsTransifexCOMtxtZawierajacyWszystkieLiniePL_ilosclinii)
                {
                    int liczbazaktualizowanychlinii = 0;
                    bool blad_stopujacy = false;

                    List<Linia_stringsTransifexCOMTXT> stringsTransifexCOMtxtORIGEN_listalinii = UtworzListeLiniiZPlikuTXTStringsTransifexCOM(nazwapliku_stringsTransifexCOMtxtORIGEN);
                    List<Linia_stringsTransifexCOMTXT> stringsTransifexCOMtxtNajaktualniejszyPLiEN_listalinii = UtworzListeLiniiZPlikuTXTStringsTransifexCOM(nazwapliku_stringsTransifexCOMtxtNajaktualniejszyPLiEN);
                    List<Linia_stringsTransifexCOMTXT> stringsTransifexCOMtxtZawierajacyWszystkieLiniePL_listalinii = UtworzListeLiniiZPlikuTXTStringsTransifexCOM(nazwapliku_stringsTransifexCOMtxtZawierajacyWszystkieLiniePL);

                    List<Linia_stringsTransifexCOMTXT> NOWY_stringsTransifexCOMtxt_listalinii = new List<Linia_stringsTransifexCOMTXT>();

                    Console.WriteLine("DEBUG: " + stringsTransifexCOMtxtORIGEN_listalinii[0].String);
                    Console.WriteLine("DEBUG: " + stringsTransifexCOMtxtNajaktualniejszyPLiEN_listalinii[0].String);
                    Console.WriteLine("DEBUG: " + stringsTransifexCOMtxtZawierajacyWszystkieLiniePL_listalinii[0].String);

                    for (int op1 = 0; op1 < stringsTransifexCOMtxtORIGEN_listalinii.Count; op1++)
                    {
                        int numeraktualnejliniiwpliku = op1 + 1;

                        Linia_stringsTransifexCOMTXT aktualnalinia_z_ORIGEN = stringsTransifexCOMtxtORIGEN_listalinii[op1];
                        Linia_stringsTransifexCOMTXT aktualnalinia_z_NajaktualniejszyPLiEN = stringsTransifexCOMtxtNajaktualniejszyPLiEN_listalinii[op1];
                        Linia_stringsTransifexCOMTXT aktualnalinia_z_ZawierajacyWszystkieLiniePL = stringsTransifexCOMtxtZawierajacyWszystkieLiniePL_listalinii[op1];

                        if (aktualnalinia_z_ORIGEN.ID == aktualnalinia_z_NajaktualniejszyPLiEN.ID && aktualnalinia_z_NajaktualniejszyPLiEN.ID == aktualnalinia_z_ZawierajacyWszystkieLiniePL.ID)
                        {
                            if (aktualnalinia_z_NajaktualniejszyPLiEN.String == aktualnalinia_z_ORIGEN.String)
                            {
                                NOWY_stringsTransifexCOMtxt_listalinii.Add(new Linia_stringsTransifexCOMTXT()
                                {
                                    Index = aktualnalinia_z_ORIGEN.Index,
                                    ID = aktualnalinia_z_ORIGEN.ID,
                                    String = aktualnalinia_z_ZawierajacyWszystkieLiniePL.String
                                });

                                liczbazaktualizowanychlinii++;
                            }
                            else
                            {
                                NOWY_stringsTransifexCOMtxt_listalinii.Add(new Linia_stringsTransifexCOMTXT()
                                {
                                    Index = aktualnalinia_z_ORIGEN.Index,
                                    ID = aktualnalinia_z_ORIGEN.ID,
                                    String = aktualnalinia_z_NajaktualniejszyPLiEN.String
                                });
                            }

                        }
                        else
                        {
                            blad_stopujacy = true;

                            Blad("BŁĄD: Linie nr.: " + numeraktualnejliniiwpliku + " w trzech wskazanych plikach zawierają różne identyfikatory linii (<id>).");
                        }

                    }

                    if (blad_stopujacy == false)
                    {
                        if (File.Exists(nazwanowegopliku_stringsTransifexCOMtxt) == true) { File.Delete(nazwanowegopliku_stringsTransifexCOMtxt); }

                        FileStream nowyplikustringsTXT_fs = new FileStream(nazwanowegopliku_stringsTransifexCOMtxt, FileMode.Append, FileAccess.Write);

                        try
                        {
                            StreamWriter nowyplikustringsTXT_sr = new StreamWriter(nowyplikustringsTXT_fs);

                            for (int op2 = 0; op2 < NOWY_stringsTransifexCOMtxt_listalinii.Count; op2++)
                            {
                                int numeraktualnejlinii = op2 + 1;

                                Console.WriteLine("Trwa zapisywanie linii nr. " + numeraktualnejlinii + "/" + plikstringsTransifexCOMtxtORIGEN_ilosclinii);

                                Linia_stringsTransifexCOMTXT aktualnalinia = NOWY_stringsTransifexCOMtxt_listalinii[op2];

                                nowyplikustringsTXT_sr.WriteLine("<" + aktualnalinia.ID + ">" + aktualnalinia.String);
                            }

                            nowyplikustringsTXT_sr.Close();

                            Sukces("Został utworzony nowy plik o nazwie \"" + nazwanowegopliku_stringsTransifexCOMtxt + "\".");
                            Sukces("Łącznie zaktualizowano/zastąpiono liczbę linii: " + liczbazaktualizowanychlinii);
                        }
                        catch
                        {
                            Blad("BŁĄD: Wystąpił nieoczkiwany problem z dostępem do nowotworzonego pliku o nazwie \"" + nazwanowegopliku_stringsTransifexCOMtxt + "\".");
                        }

                        nowyplikustringsTXT_fs.Close();

                    }
                }
                else
                {
                    Blad("BŁĄD: Liczba linii w trzech podanych plikach nie jest identyczna!");
                }

            }
            else
            {
                Blad("BŁĄD: Brak przynajmniej jednego ze wskazanych plików.");

            }

        }

        public static void TXTTransifexCOMtoJSON_ZNumeramiLiniiZPlikuJSON_Operacje(string nazwaplikukeystxt, string nazwaplikustringstxt, bool ostatni_watek = false)
        {

            //string nazwaplikukeystxt;
            //string nazwaplikustringstxt;
            string nazwanowegoplikuJSON;
            uint plikkeystxt_ilosclinii;
            uint plikstringstxt_ilosclinii;
            const char separator = ';';


            if (File.Exists(folderglownyprogramu + nazwafolderutmp + sc + nazwaplikukeystxt) && File.Exists(folderglownyprogramu + nazwafolderutmp + sc + nazwaplikustringstxt))
            {
                nazwanowegoplikuJSON = nazwaplikukeystxt.Replace(".keysTransifexCOM.txt", "");


                //Console.WriteLine("Nazwa nowego pliku JSON to: " + nazwanowegoplikuJSON);

                plikkeystxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwafolderutmp + sc + nazwaplikukeystxt);
                plikstringstxt_ilosclinii = PoliczLiczbeLinii(folderglownyprogramu + nazwafolderutmp + sc + nazwaplikustringstxt);
                //Console.WriteLine("plik keys zawiera linii: " + plikkeystxt_ilosclinii);
                //Console.WriteLine("plik strings zawiera linii: " + plikstringstxt_ilosclinii);



                if (plikkeystxt_ilosclinii == plikstringstxt_ilosclinii)
                {
                    bool bledywplikuJSON = false;
                    FileStream nowyplikJSON_fs = new FileStream(folderglownyprogramu + nazwafolderutmp + sc + nazwanowegoplikuJSON, FileMode.Append, FileAccess.Write);
                    FileStream plikkeystxt_fs = new FileStream(folderglownyprogramu + nazwafolderutmp + sc + nazwaplikukeystxt, FileMode.Open, FileAccess.Read);

                    try //#1
                    {
                        string plikkeystxt_trescaktualnejlinii;
                        string plikstringstxt_trescaktualnejlinii;

                        StreamWriter nowyplikJSON_sw = new StreamWriter(nowyplikJSON_fs);
                        StreamReader plikkeystxt_sr = new StreamReader(plikkeystxt_fs);

                        int plikkeystxt_sr_nraktualnejlinii = 1;
                        while (plikkeystxt_sr.Peek() != -1)
                        {
                            plikkeystxt_trescaktualnejlinii = plikkeystxt_sr.ReadLine();
                            string[] plikkeystxt_wartoscilinii = plikkeystxt_trescaktualnejlinii.Split(new char[] { separator }); //skladnia: plikkeystxt_wartoscilinii[0:key||0<:vars]

                            //Console.WriteLine("Pobrano KEY   z linii " + plikkeystxt_sr_nraktualnejlinii + " o tresci: " + plikkeystxt_trescaktualnejlinii);

                            FileStream plikstringstxt_fs = new FileStream(folderglownyprogramu + nazwafolderutmp + sc + nazwaplikustringstxt, FileMode.Open, FileAccess.Read);

                            try //#2
                            {
                                StreamReader plikstringstxt_sr = new StreamReader(plikstringstxt_fs);

                                int plikstringstxt_sr_nraktualnejlinii = 1;
                                while (plikstringstxt_sr.Peek() != -1)
                                {
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



                                        plikstringstxt_trescuaktualnionalinii = plikstringstxt_trescuaktualnionalinii

                                        .Replace("\"", "<bs_n1>")
                                        .Replace("<br>", "\\n")
                                        .Replace("<bs_n1>", "\\\"");


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
                            catch
                            {
                                Blad("BLAD: Wystapil nieoczekiwany blad w dostepie do plikow. (TRY #2, plikkeystxt_sr_nraktualnejlinii: " + plikkeystxt_sr_nraktualnejlinii + ")");
                            }

                            Console.WriteLine("Trwa przygotowywanie linii nr.: " + tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii + "/" + tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP + " [" + PoliczPostepWProcentach(tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii, tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP) + "%]");

                            tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii++;


                            plikkeystxt_sr_nraktualnejlinii++;

                            plikstringstxt_fs.Close();

                        }
                        plikkeystxt_sr.Close();
                        nowyplikJSON_sw.Close();




                    }
                    catch (Exception Error)
                    {
                        Blad("BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do plików. (TRY #1) (Error: " + Error + ")");
                    }

                    plikkeystxt_fs.Close();
                    nowyplikJSON_fs.Close();

                    if (bledywplikuJSON == true && File.Exists(nazwanowegoplikuJSON))
                    {
                        File.Delete(nazwanowegoplikuJSON);

                        //Blad("bledywplikuJSON: true");
                    }
                    else
                    {
                        //Sukces("bledywplikuJSON: false");
                    }





                }
                else
                {
                    Blad("BŁĄD: Liczba linii w 2 plikach TXT jest nieidentyczna!");
                }


            }
            else
            {
                Blad("BŁĄD: Brak wskazanych plików.");
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


        //V2

        public static void UtworzTabele(string nazwa_tabeli)
        {
            MySql.MySql_Complex(
                "CREATE TABLE `" + nazwa_tabeli + "` (`Linia` int(6) NOT NULL, `Klucz` char(50) NOT NULL, `String` text NOT NULL) ENGINE = InnoDB DEFAULT CHARSET = utf8; ALTER TABLE `" + nazwa_tabeli + "` ADD PRIMARY KEY(`Linia`); ALTER TABLE `" + nazwa_tabeli + "` MODIFY `Linia` int(6) NOT NULL AUTO_INCREMENT; ALTER TABLE `" + nazwa_tabeli + "` ADD INDEX(`Klucz`);",
                skrypt,
                "UtworzTabele1");
        }

        public static void SkopiujTabeleWrazZDanymi(string nazwa_kopiowanej_tabeli, string nazwa_docelowa_nowej_tabeli)
        {
            MySql.MySql_Complex
            (
                "CREATE TABLE `" + nazwa_docelowa_nowej_tabeli + "` LIKE `" + nazwa_kopiowanej_tabeli + "`; INSERT INTO `" + nazwa_docelowa_nowej_tabeli + "` SELECT * FROM `" + nazwa_kopiowanej_tabeli + "`;",
                skrypt,
                "SkopiujTabeleWrazZDanymi"
            );
        }

        public static string PobierzCiagTokenowIstniejacychTabel()
        {
            string ciag_istniejacych_tokenow = "";

            List<List<dynamic>> istniejacetabele = MySql.MySql_ComplexWithResult("SHOW TABLES;", skrypt, "PobierzCiagTokenowIstniejacychTabel/pobierz_nazwy_tabel");

            int istniejacetabele_liczbawierszy = istniejacetabele.Count;

            for (int i1 = 0; i1 < istniejacetabele_liczbawierszy; i1++)
            {
                //Console.WriteLine("[DEBUG] istniejacetabele[" + i1 + "][0]: " + istniejacetabele[i1][0]);

                string nazwa_tabeli = istniejacetabele[i1][0].ToString();
                string token_tabeli = nazwa_tabeli.Split('_')[0];

                if (ciag_istniejacych_tokenow.Contains(token_tabeli) == false)
                {
                    ciag_istniejacych_tokenow += token_tabeli + ";";
                }

            }

            ciag_istniejacych_tokenow = ciag_istniejacych_tokenow.TrimEnd(new char[] { ';' });

            return ciag_istniejacych_tokenow;

        }

        public static uint PoliczLiczbeLinii_v2(string sciezka_do_folderu_pliku, string nazwa_pliku)
        {
            //Console.WriteLine("[DEBUG]" + sciezka_do_folderu_pliku + sc + nazwa_pliku);
            
            uint liczbalinii = 0;

            if (File.Exists(sciezka_do_folderu_pliku + sc + nazwa_pliku))
            {
                FileStream plik_fs = new FileStream(sciezka_do_folderu_pliku + sc + nazwa_pliku, FileMode.Open, FileAccess.Read);

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
                    Blad("BLAD: Wystapil nieoczekiwany blad w dostepie do pliku (metoda: PoliczLiczbeLinii_v2).");
                }

                plik_fs.Close();

            }
            else
            {
                Blad("BLAD: Nie istnieje wskazany plik (metoda: PoliczLiczbeLinii).");
            }

            return liczbalinii;
        }

        public static void WyswietlNazwyPlikowWFolderze()
        {
            string folderglowny = Directory.GetCurrentDirectory();

            string[] plikiJSONwfolderze_nazwy = Directory.GetFiles(folderglowny + "\\testJSON", "*.json");

            foreach (string s in plikiJSONwfolderze_nazwy)
            {
                FileInfo plik_fileinfo = null;

                try
                {
                    plik_fileinfo = new FileInfo(s);
                }
                catch (FileNotFoundException e)
                {
                    Blad("Blad: " + e.Message);
                    continue;
                }

                Console.WriteLine("Nazwa pliku: " + plik_fileinfo.Name);
            }

        }

        public static List<string> PobierzNazwyPlikowJSONzFolderu(string sciezka_do_folderu)
        {
            List<string> nazwy_plikow_JSON = new List<string>();
            
            if (Directory.Exists(sciezka_do_folderu) == true)
            {
                string[] plikiJSONwfolderze_nazwy = Directory.GetFiles(sciezka_do_folderu, "*.json");

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
                Blad("Blad: Nie istnieje folder: " + sciezka_do_folderu);
            }

            return nazwy_plikow_JSON;


        }

        public static List<string> PobierzNazwyPlikowJSONzMySql(string nazwa_bazy_danych)
        {
            List<string> nazwyplikowJSON = new List<string>();

            List<List<dynamic>> dane = MySql.MySql_ComplexWithResult
                                       (
                                           "SELECT `Plik` FROM `" + nazwa_bazy_danych + "` ORDER BY `ID` ASC;",
                                           skrypt,
                                           "PobierzNazwyPlikowJSONzMySql"
                                       );

            for (int r = 0; r < dane.Count; r++)
            {
                nazwyplikowJSON.Add(dane[r][0]);

                //Console.WriteLine("dane[" + r + "][0]: " + dane[r][0]);

            }

            return nazwyplikowJSON;

        }

        public static void UtworzTabele_v2(string nazwa_tabeli, bool tabela_listy_plikow = false)
        {
            if (tabela_listy_plikow == false)
            {
                MySql.MySql_Complex(
                    "CREATE TABLE `" + nazwa_tabeli + "` (`ID` int(8), `Plik` varchar(60) NOT NULL, `Linia` int(6) NOT NULL, `Klucz` char(50) NOT NULL, `String` text NOT NULL) ENGINE = InnoDB DEFAULT CHARSET = utf8; ALTER TABLE `" + nazwa_tabeli + "` ADD PRIMARY KEY(`ID`); ALTER TABLE `" + nazwa_tabeli + "` MODIFY `ID` int(8) NOT NULL AUTO_INCREMENT; ALTER TABLE `" + nazwa_tabeli + "` ADD INDEX(`Klucz`);",
                    skrypt,
                    "UtworzTabele_v2 (tabela_listy_plikow = false)");
            }
            else if (tabela_listy_plikow == true)
            {

                MySql.MySql_Complex(
                    "CREATE TABLE `" + nazwa_tabeli + "` (`ID` int(4), `Plik` varchar(60) NOT NULL) ENGINE = InnoDB DEFAULT CHARSET = utf8; ALTER TABLE `" + nazwa_tabeli + "` ADD PRIMARY KEY(`ID`); ALTER TABLE `" + nazwa_tabeli + "` MODIFY `ID` int(4) NOT NULL AUTO_INCREMENT; ALTER TABLE `" + nazwa_tabeli + "` ADD INDEX(`Plik`);",
                    skrypt,
                    "UtworzTabele_v2 (tabela_listy_plikow = true)");
            }
        }

        public static void ZmienOznaczenieJezykaDlaNazwPlikowLokalizacji(string nazwa_tabeli, string poprzednie_oznaczenie_jezyka, string aktualne_oznaczenie_jezyka)
        {
            string polaczenie_mysql1_id = "polaczenie_mysql1 (metoda: ZmienOznaczenieJezykaDlaNazwPlikowLokalizacji)";
            MySqlConnection polaczenie_mysql1 = MySql.MySql_Connect(skrypt, polaczenie_mysql1_id);

            List<List<dynamic>> dane_z_tabeli1 = MySql.MySql_ComplexWithResult
            (
                "SELECT * FROM `" + nazwa_tabeli + "_pliki` ORDER BY `ID` ASC;",
                skrypt,
                "ZmienOznaczenieJezykaDlaNazwPlikowLokalizacji/pobierz_dane_z_tabeli"
            );

            for (int ipd1 = 0; ipd1 < dane_z_tabeli1.Count; ipd1++)
            {
                int nr_rekordu = ipd1 + 1;
                Console.WriteLine("Trwa aktualizacja oznaczenia jezyka (z \"" + poprzednie_oznaczenie_jezyka + "\" na \"" + aktualne_oznaczenie_jezyka + "\") w rekordzie nr." + nr_rekordu + " w tabeli: " + nazwa_tabeli + "_pliki");

                string _ID = dane_z_tabeli1[ipd1][0].ToString();
                string _nazwa_pliku = dane_z_tabeli1[ipd1][1].ToString();

                string _nazwa_pliku_po_zmianie = _nazwa_pliku.Replace(poprzednie_oznaczenie_jezyka, aktualne_oznaczenie_jezyka);

                MySql.MySql_Query
                (
                    polaczenie_mysql1,
                    "UPDATE `" + nazwa_tabeli + "_pliki` SET `Plik` = '" + _nazwa_pliku_po_zmianie + "' WHERE `ID` = '" + _ID + "' AND `Plik` = '" + _nazwa_pliku + "';",
                    skrypt,
                    "ZmienOznaczenieJezykaDlaNazwPlikowLokalizacji/zmien_w_tabeli_z_lista_plikow"
                );

            }


            List<List<dynamic>> dane_z_tabeli2 = MySql.MySql_ComplexWithResult
            (
                "SELECT * FROM `" + nazwa_tabeli + "` ORDER BY `ID` ASC;",
                skrypt,
                "ZmienOznaczenieJezykaDlaNazwPlikowLokalizacji/pobierz_dane_z_tabeli"
            );

            for (int ipd2 = 0; ipd2 < dane_z_tabeli2.Count; ipd2++)
            {
                int nr_rekordu = ipd2 + 1;
                Console.WriteLine("Trwa aktualizacja oznaczenia jezyka (z \"" + poprzednie_oznaczenie_jezyka + "\" na \"" + aktualne_oznaczenie_jezyka + "\") w rekordzie nr." + nr_rekordu + " w tabeli: " + nazwa_tabeli);

                string _ID = dane_z_tabeli2[ipd2][0].ToString();
                string _klucz = dane_z_tabeli2[ipd2][3].ToString();
                string _string = dane_z_tabeli2[ipd2][4].ToString();
                string _nazwa_pliku = dane_z_tabeli2[ipd2][1].ToString();

                string _nazwa_pliku_po_zmianie = _nazwa_pliku.Replace(poprzednie_oznaczenie_jezyka, aktualne_oznaczenie_jezyka);

                MySql.MySql_Query
                (
                    polaczenie_mysql1,
                    "UPDATE `" + nazwa_tabeli + "` SET `Plik` = '" + _nazwa_pliku_po_zmianie + "' WHERE `ID` = '" + _ID + "' AND `Klucz` = '" + _klucz + "' AND `String` = '" + _string + "' AND `Plik` = '" + _nazwa_pliku + "';",
                    skrypt,
                    "ZmienOznaczenieJezykaDlaNazwPlikowLokalizacji/zmien_w_tabeli_z_liniami"
                );

            }

            MySql.MySql_Disconnect(polaczenie_mysql1, skrypt, polaczenie_mysql1_id);

        }

        public static void ImportujDaneZFolderuJSONDoTabeli(string odbierz_dane_czasu = "", string odbierz_nazwe_folderu = "", string odbierz_nazwe_nowotworzonej_tabeli = "")
        {
            string aktualny_czas_string;

            if (odbierz_dane_czasu == "")
            {
                aktualny_czas_string = PobierzTimestamp(aktualny_czas);
            }
            else
            {
                aktualny_czas_string = odbierz_dane_czasu;
            }

            string nazwa_folderu;

            if (odbierz_nazwe_folderu == "")
            {
                Console.Write("Podaj nazwę folderu do zaimportowania: ");
                nazwa_folderu = Console.ReadLine();
            }
            else
            {
                nazwa_folderu = odbierz_nazwe_folderu;
            }

            if (nazwa_folderu != "")
            {
                if (Directory.Exists(folderglownyprogramu + nazwa_folderu) == true)
                {

                    string nazwa_tabeli;
                    string nazwa_tabeli_listy_plikow;

                    if (odbierz_nazwe_nowotworzonej_tabeli == "")
                    {
                        nazwa_tabeli = aktualny_czas_string + nazwa_folderu;
                    }
                    else
                    {
                        nazwa_tabeli = aktualny_czas_string + odbierz_nazwe_nowotworzonej_tabeli;
                    }
                    nazwa_tabeli_listy_plikow = nazwa_tabeli + "_pliki";


                    List<string> plikiJSON_nazwy = PobierzNazwyPlikowJSONzFolderu(folderglownyprogramu + nazwa_folderu);

                    if (plikiJSON_nazwy.Count > 0)
                    {
                        UtworzTabele_v2(nazwa_tabeli_listy_plikow, true);
                        UtworzTabele_v2(nazwa_tabeli);

                        string polaczenie_mysql_sf_id = "polaczenie_mysql_sf";
                        MySqlConnection polaczenie_mysql_sf = MySql.MySql_Connect(skrypt, polaczenie_mysql_sf_id);

                        foreach (string nazwaplikuJSON in plikiJSON_nazwy)
                        {
                            string nazwaplikuJSON_pozmianieenGBnadeDE = PodmienOznaczenieJezyka(nazwaplikuJSON);

                            MySql.MySql_ComplexCommonConnection
                            (
                                polaczenie_mysql_sf,
                                "INSERT INTO `" + nazwa_tabeli_listy_plikow + "` (`ID`, `Plik`) VALUES(NULL, '" + nazwaplikuJSON_pozmianieenGBnadeDE + "');",
                                skrypt,
                                polaczenie_mysql_sf_id
                            );


                            ImportujDaneZPlikuJSONDoTabeli_v2(nazwa_folderu, nazwaplikuJSON, nazwa_tabeli);
                        }

                        MySql.MySql_Disconnect(polaczenie_mysql_sf, skrypt, polaczenie_mysql_sf_id);


                        Sukces("Dane z plikow JSON znajdujacych sie w folderze \"" + nazwa_folderu + "\" zostaly zaimportowane do bazy danych MySQL do tabeli o nazwie: \"" + nazwa_tabeli + "\"");
                    }
                    else
                    {
                        Blad("Blad: Wskazany folder nie zawiera plikow JSON.");
                    }


                }
                else
                {
                    Blad("Blad: Nie istnieje folder o podanej nazwie.");
                }
            }
            else
            {
                Blad("Blad: Nie podano nazwy folderu do zaimportowania.");
            }

            ImportujDaneZFolderuJSONDoTabeli_RaportujZakonczeniePracyWatku();

        }

        public static void ImportujDaneZPlikuJSONDoTabeli_v2(string folderJSON_nazwa, string plikJSON_nazwa, string tabela_nazwa)
        {
            uint plik_liczbalinii = PoliczLiczbeLinii_v2(folderglownyprogramu + folderJSON_nazwa, plikJSON_nazwa);

            FileStream plik_fs = new FileStream(folderglownyprogramu + folderJSON_nazwa + sc + plikJSON_nazwa, FileMode.Open, FileAccess.Read);

            StreamReader plik_sr = new StreamReader(plik_fs);

            string polaczenie_mysql1_id = "polaczenie_mysql1 (metoda: ImportujDaneZPlikuJSONDoTabeli_v2)";
            MySqlConnection polaczenie_mysql1 = MySql.MySql_Connect(skrypt, polaczenie_mysql1_id);

            string tmp_ostatnia_odczytana_tresc_KEY = "";

            uint linia = 1;
            while (plik_sr.Peek() != -1)
            {
                /*
                string plik_tresclinii = plik_sr.ReadLine();

                Console.WriteLine(plikJSON_nazwa + ": Analizowanie linii nr.: " + linia + "/" + plik_liczbalinii);

                if (plik_tresclinii.Contains("\"Key\": "))
                {
                    string tresc_KEY = plik_tresclinii.TrimStart().Split(new char[] { '\"' })[3];
                    tmp_ostatnia_odczytana_tresc_KEY = tresc_KEY;

                    string dodajtrescKEY_zapytanie = "INSERT INTO `" + tabela_nazwa + "` (`ID`, `Plik`, `Linia`, `Klucz`, `String`) VALUES(NULL, '" + plikJSON_nazwa + "', '" + linia + "', '" + tresc_KEY + "', '');";

                    MySql.MySql_Query
                    (
                      polaczenie_mysql1,
                      dodajtrescKEY_zapytanie,
                      skrypt,
                      polaczenie_mysql1_id
                    );

                    //Console.WriteLine("Próba wykonania zapytania: " + dodajtrescKEY_zapytanie);
                    //Console.WriteLine(plikJSON_nazwa + ": Zapisano KEY: " + tresc_KEY);


                }
                else if (plik_tresclinii.Contains("\"Value\": "))
                {
                    string tresc_STRING = plik_tresclinii.TrimStart()

                        .Replace("\\n", "<br>")
                        .Replace("\\\"", "<bs_n1>")
                        .Replace("'", "<apostrof>")
                        .Replace("\\t", "<t>")
                        .Replace("\\\\", "<bs_n2>")


                        .Split(new char[] { '\"' })[3];

                    string dodajtrescSTRING_zapytanie = "UPDATE `" + tabela_nazwa + "` SET `String` = '" + tresc_STRING + "' WHERE `Plik` = '" + plikJSON_nazwa + "' AND `Klucz` = '" + tmp_ostatnia_odczytana_tresc_KEY + "';";

                    MySql.MySql_Query
                    (
                      polaczenie_mysql1,
                      dodajtrescSTRING_zapytanie,
                      skrypt,
                      polaczenie_mysql1_id
                    );

                    //Console.WriteLine("Próba wykonania zapytania: " + dodajtrescSTRING_zapytanie);
                    //Console.WriteLine(plikJSON_nazwa + ": Zapisano VALUE: " + tresc_STRING);

                }

                linia++;

                */


                string tresc_linii_JSON = plik_sr.ReadLine();

                string tresclinii_ciagzmiennych = "";


                string[] linia_podzial_1 = tresc_linii_JSON.Split(new string[] { "\": \"" }, StringSplitOptions.None);

                /*
                for (int a1 = 0; a1 < linia_podzial_1.Length; a1++)
                {

                    //Console.WriteLine("linia_podzial_1[" + a1 + "]: " + linia_podzial_1[a1]);
                }
                */

                //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_1.Length: " + linia_podzial_1.Length);

                if (linia_podzial_1.Length <= 2)
                {
                    string KEYt1 = linia_podzial_1[0].Trim();
                    int KEYt1_iloscznakow = KEYt1.Length;

                    if (KEYt1_iloscznakow >= 2)
                    {

                        string[] linia_2_separatory = { KEYt1 + "\": \"" };

                        string[] linia_podzial_2 = tresc_linii_JSON.Split(linia_2_separatory, StringSplitOptions.None);

                        /*
                        for (int a2 = 0; a2 < linia_podzial_2.Length; a2++)
                        {

                            Console.WriteLine("linia_podzial_2[" + a2 + "]: " + linia_podzial_2[a2]);
                        }
                        */

                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_2.Length: " + linia_podzial_2.Length);

                        if (linia_podzial_2.Length >= 2)
                        {

                            string STRINGt1 = linia_podzial_2[1].TrimEnd();
                            int STRINGt1_iloscznakow = STRINGt1.Length;


                            //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEYt1_iloscznakow: " + KEYt1_iloscznakow);
                            //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRINGt1_iloscznakow: " + STRINGt1_iloscznakow);


                            if (KEYt1_iloscznakow >= 2 && STRINGt1_iloscznakow >= 1)
                            {
                                string KEY = KEYt1.Remove(0, 1);

                                int cofniecie_wskaznika = STRINGt1_iloscznakow - 1;
                                int usunac_znakow = 1;
                                if (linia != plik_liczbalinii - 2)
                                {
                                    cofniecie_wskaznika = STRINGt1_iloscznakow - 2;
                                    usunac_znakow = 2;
                                }

                                string STRINGt2 = STRINGt1.Remove(cofniecie_wskaznika, usunac_znakow);
                                string STRING = STRINGt2;



                                //Console.WriteLine("[linia:" + plik_linia + "] KEY:" + KEY);
                                //Console.WriteLine("[linia:" + plik_linia + "] STRING:" + STRING);


                                if (KEY != "$id")
                                {

                                    string tresc_KEY = KEY;



                                    string tresc_STRING = STRING
                                           .Replace("\\n", "<br>")
                                           .Replace("\\\"", "<bs_n1>")
                                           .Replace("'", "<apostrof>")
                                           .Replace("\\t", "<t>")
                                           .Replace("\\\\", "<bs_n2>"); ;


                                    string rodzajenawiasow = "{|}";
                                    int iloscnawiasowwlinii = 0;
                                    Regex regex = new Regex(rodzajenawiasow);
                                    MatchCollection matchCollection = regex.Matches(tresc_STRING);
                                    foreach (var match in matchCollection)
                                    {
                                        iloscnawiasowwlinii++;
                                    }
                                    if (iloscnawiasowwlinii % 2 != 0)
                                    {
                                        Blad("UWAGA: Linia nr." + linia + " ma błędną ilość nawiasów {}!: Treść linii: " + tresc_STRING);
                                    }


                                    string plikJSON_nazwa_popodmianieenGBnadeDE = PodmienOznaczenieJezyka(plikJSON_nazwa);

                                    string dodajtrescSTRING_zapytanie = "INSERT INTO `" + tabela_nazwa + "` (`ID`, `Plik`, `Linia`, `Klucz`, `String`) VALUES(NULL, '" + plikJSON_nazwa_popodmianieenGBnadeDE + "', '" + linia + "', '" + tresc_KEY + "', '" + tresc_STRING + "');";

                                    MySql.MySql_Query
                                    (
                                      polaczenie_mysql1,
                                      dodajtrescSTRING_zapytanie,
                                      skrypt,
                                      polaczenie_mysql1_id
                                    );




                                }

                            }

                        }

                    }


                }


                if (wylacz_calkowitepokazywaniepostepow == false)
                {
                    Console.WriteLine("Trwa importowanie do bazy danych MySQL linii nr. " + linia + "/" + plik_liczbalinii + " [" + PoliczPostepWProcentach(linia, plik_liczbalinii) + "%]");
                }

                linia++;



            }


            MySql.MySql_Disconnect(polaczenie_mysql1, skrypt, polaczenie_mysql1_id);

            //ImportujDaneZPlikuJSONDoTabeli_RaportujZakonczeniePracyWatku();

        }

        public static void WyeksportujZMySqlDoPlikowJSON(string przyrostek_nazwy_tabeli = "")
        {
            //-string nowyplikJSON_nazwa = "";

            string ciag_istniejacych_tokenow = PobierzCiagTokenowIstniejacychTabel();

            if (ciag_istniejacych_tokenow != "")
            {

                Console.WriteLine("Aktualnie istniejace tabele projektow (tokeny): " + ciag_istniejacych_tokenow);

                Console.Write("Wpisz numer projektu (token), który chcesz wyeksportować: ");
                string podany_token_projektu = Console.ReadLine();

                if (ciag_istniejacych_tokenow.Contains(podany_token_projektu) && podany_token_projektu != "")
                {
                    string podany_przyrostek_nazwy_tabeli = "";

                    if (przyrostek_nazwy_tabeli == "")
                    {
                        Console.Write("Podaj przyrostek nazwy tabeli (bez tokenu): ");
                        podany_przyrostek_nazwy_tabeli = Console.ReadLine();
                    }
                    else
                    {
                        podany_przyrostek_nazwy_tabeli = przyrostek_nazwy_tabeli;
                    }

                    string folder_nazwa = "WYEKSPORTOWANY_" + podany_token_projektu + podany_przyrostek_nazwy_tabeli;

                    if (Directory.Exists(folder_nazwa) == false)
                    {
                        Directory.CreateDirectory(folder_nazwa);

                        List<string> nazwyplikowJSONzMySql = PobierzNazwyPlikowJSONzMySql(podany_token_projektu + podany_przyrostek_nazwy_tabeli + "_pliki");

                        if (nazwyplikowJSONzMySql.Count > 0)
                        {
                            for (int i1 = 0; i1 < nazwyplikowJSONzMySql.Count; i1++)
                            {
                                string nazwaplikuJSON_tmp = nazwyplikowJSONzMySql[i1];

                                StreamWriter nazwaplikuJSON_tmp_sw = File.CreateText(folderglownyprogramu + folder_nazwa + sc + nazwaplikuJSON_tmp);

                                nazwaplikuJSON_tmp_sw.Close();

                                WyeksportujDaneZTabeliDoPlikuJSON_v2(podany_token_projektu + podany_przyrostek_nazwy_tabeli, folder_nazwa, nazwaplikuJSON_tmp);

                            }

                            Sukces("Wyeksportowano z bazy danych MySQL projekt o nazwie \"" + podany_token_projektu + "\", jego wszystkie pliki JSON wraz ze struktura folderu i zapisano w folderze o nazwie: \"" + folder_nazwa + "\"");

                        }
                        else
                        {
                            Blad("Blad: Blad podczas pobierania nazw plikow JSON z bazy danych MySql.");
                        }
                    }
                    else
                    {
                        Blad("Blad: Istnieje juz folder o wskazanej nazwie.");
                    }


                }
                else
                {
                    Blad("BLAD: Nie istnieje projekt o podanym tokenie!");
                }

            }
            else
            {
                Blad("Nie istnie żaden projekt zaimportowany do MySql.");
            }

            //-Sukces("Zapisano przetłumaczony plik nowej wersji o nazwie \"" + nowyplikJSON_nazwa + "\".");

        }

        public static void WyeksportujDaneZTabeliDoPlikuJSON_v2(string tabela_nazwa, string folder_nazwa, string plikJSON_nazwa)
        {
            bool czyutworzonynaglowek = UtworzNaglowekJSON(plikJSON_nazwa, folder_nazwa);

            if (czyutworzonynaglowek == true)
            {
                FileStream plikJSON_fs = new FileStream(folderglownyprogramu + folder_nazwa + sc + plikJSON_nazwa, FileMode.Append, FileAccess.Write);

                try
                {
                    StreamWriter plikJSON_sw = new StreamWriter(plikJSON_fs);



                    List<List<dynamic>> dane_z_tabeli = MySql.MySql_ComplexWithResult
                    (
                        "SELECT * FROM `" + tabela_nazwa + "` WHERE `Plik` = '" + plikJSON_nazwa + "' ORDER BY `ID` ASC;",
                        skrypt,
                        "WyeksportujDaneZTabeliDoPlikuJSON_v2/pobierz_dane_z_tabeli"
                    );

                    int tmp_nrostatwiersza = dane_z_tabeli.Count - 1;

                    Console.WriteLine("dane_z_tabeli[tmp_nrostatwiersza][0]: " + dane_z_tabeli[tmp_nrostatwiersza][0]);

                    int przewidywanaliczbawszystkichlinii = dane_z_tabeli.Count;
                    uint plikJSON_nraktualnejlinii = 1;
                    for (int ipd1 = 0; ipd1 < dane_z_tabeli.Count; ipd1++)
                    {
                        string wartosc_KEY = dane_z_tabeli[ipd1][3].ToString();
                        string wartosc_STRING = dane_z_tabeli[ipd1][4].ToString();

                        plikJSON_sw.Write("    \"" + wartosc_KEY + "\": \"" + wartosc_STRING

                            .Replace("<br>", "\\n")
                            .Replace("<bs_n1>", "\\\"")
                            .Replace("<apostrof>", "'")
                            .Replace("<t>", "\\t")
                            .Replace("<bs_n2>", "\\\\")


                            + "\"");




                        if (plikJSON_nraktualnejlinii != przewidywanaliczbawszystkichlinii)
                        {
                            plikJSON_sw.Write(",");
                        }

                        plikJSON_sw.Write("\n");

                        Console.WriteLine(plikJSON_nazwa + ": Trwa zapisywanie linii nr. " + plikJSON_nraktualnejlinii + "/" + przewidywanaliczbawszystkichlinii);


                        plikJSON_nraktualnejlinii++;
                    }


                    plikJSON_sw.Close();
                }
                catch
                {
                    Blad("BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do pliku.");
                }

                plikJSON_fs.Close();



                bool czyutworzonastopka = UtworzStopkeJSON(plikJSON_nazwa, folder_nazwa);

                if (czyutworzonastopka == false)
                {
                    Blad("Blad: Stopka w pliku \"" + plikJSON_nazwa + "\" nie zostala utworzona.");
                }
            }
            else
            {
                Blad("BŁĄD: Wystąpił nieoczekiwany błąd podczas tworzenia nagłówka pliku JSON.");
            }



        }

        public static void Importuj3folderyplikowJSONDoMySql(DateTime aktualny_czas)
        {
            string aktualny_czas_string = PobierzTimestamp(aktualny_czas);

            string folderORIGstarejwersji_nazwa = "";
            string folderORIGnowejwersji_nazwa = "";
            string folderPRZETLUMACZONYstarejwersji_nazwa = "";

            Console.WriteLine("UWAGA: W nazwach plików przedrostek \"enGB\" zostanie automatycznie zamieniony na przedrostek \"deDE\" w trakcie zapisywania do bazy danych.");

            Console.Write("Podaj nazwę folderu plików JSON oryginalnej lokalizacji starej wersji (ENTER=DEBUG): ");
            folderORIGstarejwersji_nazwa = Console.ReadLine();
            Console.Write("Podaj nazwę folderu plików JSON oryginalnej lokalizacji nowej wersji (ENTER=DEBUG): ");
            folderORIGnowejwersji_nazwa = Console.ReadLine();
            Console.Write("Podaj nazwę przetłumaczonego folderu plików JSON starej wersji (ENTER=DEBUG): ");
            folderPRZETLUMACZONYstarejwersji_nazwa = Console.ReadLine();

            if (folderORIGstarejwersji_nazwa == "") { folderORIGstarejwersji_nazwa = "enGB-2.0.5p"; }
            if (folderORIGnowejwersji_nazwa == "") { folderORIGnowejwersji_nazwa = "enGB-2.2.4p"; }
            if (folderPRZETLUMACZONYstarejwersji_nazwa == "") { folderPRZETLUMACZONYstarejwersji_nazwa = "plPL"; }

            //Console.WriteLine($"[DEBUG] Folder glowny programu: {folderglownyprogramu}");
            
            if (Directory.Exists(folderglownyprogramu +  folderORIGstarejwersji_nazwa) && Directory.Exists(folderglownyprogramu + folderORIGnowejwersji_nazwa) && Directory.Exists(folderglownyprogramu + folderPRZETLUMACZONYstarejwersji_nazwa))
            {

                //ImportujDaneZFolderuJSONDoTabeli(aktualny_czas_string, folderORIGstarejwersji_nazwa, "op2_origfolderstarejwersji");
                //ImportujDaneZFolderuJSONDoTabeli(aktualny_czas_string, folderORIGnowejwersji_nazwa, "op2_origfoldernowejwersji");
                //ImportujDaneZFolderuJSONDoTabeli(aktualny_czas_string, folderPRZETLUMACZONYstarejwersji_nazwa, "op2_przetlumaczonyfolderstarejwersji");

                tmpdlawatkow_op2_odbierzdaneczasu = aktualny_czas_string;
                tmpdlawatkow_op2_folderORIGstarejwersji_nazwa = folderORIGstarejwersji_nazwa;
                tmpdlawatkow_op2_folderORIGnowejwersji_nazwa = folderORIGnowejwersji_nazwa;
                tmpdlawatkow_op2_folderPRZETLUMACZONYstarejwersji_nazwa = folderPRZETLUMACZONYstarejwersji_nazwa;
                tmpdlawatkow_op2_tabelaORIGstarejwersji_nazwa = "op2_origfolderstarejwersji";
                tmpdlawatkow_op2_tabelaORIGnowejwersji_nazwa = "op2_origfoldernowejwersji";
                tmpdlawatkow_op2_tabelaPRZETLUMACZONAstarejwersji_nazwa = "op2_przetlumaczonyfolderstarejwersji";

                if (wylacz_calkowitepokazywaniepostepow == true)
                {
                    Console.WriteLine("Trwa importowanie do bazy danych MySQL zawartości trzech folderów z plikami JSON. Może to chwilę zająć. Proszę czekać...");
                }

                Thread watek1_op2 = new Thread(ImportujDaneZFolderuJSONDoTabeli_watek1);
                Thread watek2_op2 = new Thread(ImportujDaneZFolderuJSONDoTabeli_watek2);
                Thread watek3_op2 = new Thread(ImportujDaneZFolderuJSONDoTabeli_watek3);
                watek1_op2.Start();
                watek2_op2.Start();
                watek3_op2.Start();


            }
            else
            {
                Blad("BŁĄD: Nie istnieje przynajmnej jeden z podanych folderów z plikami JSON!");
            }


        }

        public static void ImportujDaneZFolderuJSONDoTabeli_watek1()
        {
            ImportujDaneZFolderuJSONDoTabeli(tmpdlawatkow_op2_odbierzdaneczasu, tmpdlawatkow_op2_folderORIGstarejwersji_nazwa, tmpdlawatkow_op2_tabelaORIGstarejwersji_nazwa);
        }

        public static void ImportujDaneZFolderuJSONDoTabeli_watek2()
        {
            ImportujDaneZFolderuJSONDoTabeli(tmpdlawatkow_op2_odbierzdaneczasu, tmpdlawatkow_op2_folderORIGnowejwersji_nazwa, tmpdlawatkow_op2_tabelaORIGnowejwersji_nazwa);
        }

        public static void ImportujDaneZFolderuJSONDoTabeli_watek3()
        {
            ImportujDaneZFolderuJSONDoTabeli(tmpdlawatkow_op2_odbierzdaneczasu, tmpdlawatkow_op2_folderPRZETLUMACZONYstarejwersji_nazwa, tmpdlawatkow_op2_tabelaPRZETLUMACZONAstarejwersji_nazwa);
        }

        public static void ImportujDaneZFolderuJSONDoTabeli_RaportujZakonczeniePracyWatku()
        {
            tmpdlawatkow_op2_iloscukonczonychwatkow++;

            if (tmpdlawatkow_op2_iloscukonczonychwatkow == 3)
            {
                Sukces("Zaimportowano projekt do bazy danych MySql. Ukończono pracę wątków: 3/3.");

                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();

            }
        }

        public static void WyciagnijDoNowegoPlikuJSONDodaneIZmienionePozycje_StaryORIGLangKontraNowyORIGLang_v2()
        {
            string ciag_istniejacych_tokenow = PobierzCiagTokenowIstniejacychTabel();

            if (ciag_istniejacych_tokenow != "")
            {
                string nowyplikJSONzawierajacyzmiany_nazwa;
                string nowyplikUpdateLogJSONzawierajacyinfoozmianach_nazwa;
                string nowyplikUpdateSchemaJSONzawierajacyschematpliku_nazwa;
                string nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_nazwa;

                string nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_nazwa;
                string nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_nazwa;

                Console.WriteLine("---");
                Console.WriteLine("Lista tokenów aktualnie istniejacych tabel projektów w bazie danych MySQL:");

                string[] lista_istniejacych_tokenow = ciag_istniejacych_tokenow.Split(';');

                for (int sp1 = 0; sp1 < lista_istniejacych_tokenow.Length; sp1++)
                {
                    Console.WriteLine(lista_istniejacych_tokenow[sp1]);
                }
                Console.WriteLine("---");

                Console.Write("Podaj token projektu: ");
                string podany_token_projektu = Console.ReadLine();

                if (ciag_istniejacych_tokenow.Contains(podany_token_projektu) && podany_token_projektu != "")
                {
                    Console.Write("Podaj dwucyfrowy #numer_projektu dla nowych plików metadanych (bez znaku # na początku): ");
                    string podany_numerporzadkowy = Console.ReadLine();

                    Console.Write("Podaj numer starej (poprzedniej) wersji gry dla nowych plików metadanych: ");
                    string podany_numerstarejwersjigry = Console.ReadLine();

                    Console.Write("Podaj numer nowej (aktualnej) wersji gry dla nowych plików metadanych: ");
                    string podany_numernowejwersjigry = Console.ReadLine();

                    if (podany_numerporzadkowy != "" && podany_numerstarejwersjigry != "" && podany_numernowejwersjigry != "")
                    {
                        if (wylacz_calkowitepokazywaniepostepow == true)
                        {
                            Console.WriteLine("Trwa analizowanie danych w bazie danych MySQL (tabela nowej wersji ORIGlang). Może to chwilę zająć. Proszę czekać...");
                        }

                        //string przedrostek_nazwy_metadanych = "ZAWIERAJACY_ZMIANY_" + podany_token_projektu;
                        string przedrostek_nazwy_metadanych = "#" + podany_numerporzadkowy + "_" + podany_numerstarejwersjigry + "-" + podany_numernowejwersjigry;

                        nowyplikJSONzawierajacyzmiany_nazwa = "plPL-update-" + podany_numerstarejwersjigry + "-" + podany_numernowejwersjigry + ".json";
                        nowyplikUpdateLogJSONzawierajacyinfoozmianach_nazwa = przedrostek_nazwy_metadanych + ".UpdateLog.json";
                        nowyplikUpdateSchemaJSONzawierajacyschematpliku_nazwa = przedrostek_nazwy_metadanych + ".UpdateSchema.json";
                        nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_nazwa = przedrostek_nazwy_metadanych + ".UpdateLocStruct.json";
                        nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_nazwa = przedrostek_nazwy_metadanych + ".UpdateOldModStrings.json";
                        nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_nazwa = przedrostek_nazwy_metadanych + ".UpdateNewModStrings.json";



                        UtworzNaglowekJSON(nowyplikJSONzawierajacyzmiany_nazwa);
                        UtworzNaglowekJSON(nowyplikUpdateLogJSONzawierajacyinfoozmianach_nazwa);
                        UtworzNaglowekJSON(nowyplikUpdateSchemaJSONzawierajacyschematpliku_nazwa);
                        UtworzNaglowekJSON(nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_nazwa);
                        UtworzNaglowekJSON(nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_nazwa);
                        UtworzNaglowekJSON(nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_nazwa);


                        FileStream nowyplikJSONzawierajacyzmiany_fs = new FileStream(nowyplikJSONzawierajacyzmiany_nazwa, FileMode.Append, FileAccess.Write);
                        FileStream nowyplikUpdateLogJSONzawierajacyinfoozmianach_fs = new FileStream(nowyplikUpdateLogJSONzawierajacyinfoozmianach_nazwa, FileMode.Append, FileAccess.Write);
                        FileStream nowyplikUpdateSchemaJSONzawierajacyschematpliku_fs = new FileStream(nowyplikUpdateSchemaJSONzawierajacyschematpliku_nazwa, FileMode.Append, FileAccess.Write);
                        FileStream nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_fs = new FileStream(nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_nazwa, FileMode.Append, FileAccess.Write);
                        FileStream nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_fs = new FileStream(nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_nazwa, FileMode.Append, FileAccess.Write);
                        FileStream nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_fs = new FileStream(nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_nazwa, FileMode.Append, FileAccess.Write);

                        //try
                        //{
                        StreamWriter nowyplikJSONzawierajacyzmiany_sw = new StreamWriter(nowyplikJSONzawierajacyzmiany_fs);
                        StreamWriter nowyplikUpdateLogJSONzawierajacyinfoozmianach_sw = new StreamWriter(nowyplikUpdateLogJSONzawierajacyinfoozmianach_fs);
                        StreamWriter nowyplikUpdateSchemaJSONzawierajacyschematpliku_sw = new StreamWriter(nowyplikUpdateSchemaJSONzawierajacyschematpliku_fs);
                        StreamWriter nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_sw = new StreamWriter(nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_fs);
                        StreamWriter nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_sw = new StreamWriter(nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_fs);
                        StreamWriter nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_sw = new StreamWriter(nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_fs);


                        List<List<dynamic>> tabelanowejwersjiORIGlang_danetabel = MySql.MySql_ComplexWithResult
                        (
                            "SELECT * FROM `" + podany_token_projektu + "_origfoldernowejwersji" + "` ORDER BY `ID` ASC;",
                            skrypt,
                            "WyciagnijDoNowegoPlikuJSONDodaneIZmienionePozycje_StaryORIGLangKontraNowyORIGLang_v2/pobierz_dane_z_tabeli(tabela_nowej_wersji_ORIGLang)"
                        );

                        // listazmian_tmp=KEY;STRING;RODZAJ_ZMIANY
                        List<string> listazmian_tmp = new List<string>();
                        List<string> listadanychplikuschematu_tmp = new List<string>();

                        //int nowyplikJSONzawierajacyzmiany_aktualnalinia = 1;
                        for (int ilx1 = 0; ilx1 < tabelanowejwersjiORIGlang_danetabel.Count; ilx1++)
                        {
                            int tabelanowejwersjiORIGlang_danetabel_numerrekordu = ilx1 + 1;


                            if (wylacz_calkowitepokazywaniepostepow == false)
                            {
                                Console.WriteLine("Trwa analizowanie danych w bazie danych MySQL (tabela nowej wersji ORIGlang) - rekord: " + tabelanowejwersjiORIGlang_danetabel_numerrekordu.ToString() + "/" + tabelanowejwersjiORIGlang_danetabel.Count);
                            }

                            
                            string tabelanowejwersjiORIGlang_aktualnaNAZWAPLIKU = tabelanowejwersjiORIGlang_danetabel[ilx1][1].ToString();
                            string tabelanowejwersjiORIGlang_aktualnyKLUCZ = tabelanowejwersjiORIGlang_danetabel[ilx1][3].ToString();
                            string tabelanowejwersjiORIGlang_aktualnySTRING = tabelanowejwersjiORIGlang_danetabel[ilx1][4].ToString();


                            List<List<dynamic>> tabelastarejwersjiORIGlang_dane1tabeli = MySql.MySql_ComplexWithResult
                            (
                                "SELECT * FROM `" + podany_token_projektu + "_origfolderstarejwersji" + "` WHERE `Klucz` = '" + tabelanowejwersjiORIGlang_aktualnyKLUCZ + "';",
                                skrypt,
                                "WyciagnijDoNowegoPlikuJSONDodaneIZmienionePozycje_StaryORIGLangKontraPlikORIGLang_v2/pobierz_dane_z_tabeli(tabela_starej_wersji_ORIGLang)"
                            );

                            if (tabelastarejwersjiORIGlang_dane1tabeli.Count == 0)
                            {
                                string wartosc_KEY = tabelanowejwersjiORIGlang_aktualnyKLUCZ;
                                string wartosc_STRING = tabelanowejwersjiORIGlang_aktualnySTRING;


                                listazmian_tmp.Add
                                (
                                    wartosc_KEY +
                                    "[[[---]]]" +
                                    wartosc_STRING
                                                  .Replace("<br>", "\\n")
                                                  .Replace("<bs_n1>", "\\\"")
                                                  .Replace("<apostrof>", "'")
                                                  .Replace("<t>", "\\t")
                                                  .Replace("<bs_n2>", "\\\\") +
                                    "[[[---]]]" +
                                    "NOWY_STRING" +
                                    "[[[---]]]" +
                                    "" // wartosc_STARY_STRING (w tym przypadku nie dotyczy)
                                );



                            }
                            else if (tabelastarejwersjiORIGlang_dane1tabeli.Count == 1)
                            {

                                string tabelastarejwersjiORIGlang_aktualnyKLUCZ = tabelastarejwersjiORIGlang_dane1tabeli[0][3].ToString();
                                string tabelastarejwersjiORIGlang_aktualnySTRING = tabelastarejwersjiORIGlang_dane1tabeli[0][4].ToString();

                                /* WAŻNE: jeśli występuje tylko zmiana wielkości liter w danym stringu między starą, a nową wersją lokalizacji to nie odnotowuj tego jako zmiany */

                                if (tabelastarejwersjiORIGlang_aktualnySTRING.ToLower() != tabelanowejwersjiORIGlang_aktualnySTRING.ToLower())
                                {

                                    string wartosc_KEY = tabelanowejwersjiORIGlang_aktualnyKLUCZ;
                                    string wartosc_STRING = tabelanowejwersjiORIGlang_aktualnySTRING;
                                    string wartosc_STARY_STRING = tabelastarejwersjiORIGlang_aktualnySTRING;

                                    /*
                                    nowyplikJSONzawierajacyzmiany_sw.Write("    \"" + wartosc_KEY + "\": \"" + wartosc_STRING + "\"");

                                    nowyplikJSONzawierajacyzmiany_sw.Write(",");

                                    nowyplikJSONzawierajacyzmiany_sw.Write("\n");


                                    nowyplikUpdateLogJSONzawierajacyinfoozmianach_sw.Write("    \"" + wartosc_KEY + "\": \"" +

                                    "MODYFIKOWANY_STRING\"");

                                    nowyplikUpdateLogJSONzawierajacyinfoozmianach_sw.Write(",");

                                    nowyplikUpdateLogJSONzawierajacyinfoozmianach_sw.Write("\n");
                                    */

                                    listazmian_tmp.Add
                                    (
                                        wartosc_KEY +
                                        "[[[---]]]" +
                                        wartosc_STRING
                                                      .Replace("<br>", "\\n")
                                                      .Replace("<bs_n1>", "\\\"")
                                                      .Replace("<apostrof>", "'")
                                                      .Replace("<t>", "\\t")
                                                      .Replace("<bs_n2>", "\\\\") +
                                        "[[[---]]]" +
                                        "MODYFIKOWANY_STRING" +
                                        "[[[---]]]" +
                                        wartosc_STARY_STRING
                                                  .Replace("<br>", "\\n")
                                                  .Replace("<bs_n1>", "\\\"")
                                                  .Replace("<apostrof>", "'")
                                                  .Replace("<t>", "\\t")
                                                  .Replace("<bs_n2>", "\\\\")


                                    );


                                }

                            }
                            else
                            {
                                Blad("BŁĄD: Klucz o wartości " + tabelanowejwersjiORIGlang_danetabel[ilx1][1] + " jest prawdopodobnie zdublowany w tabeli! (tabelastarejwersjiORIGlang_dane1tabeli.Count: " + tabelastarejwersjiORIGlang_dane1tabeli.Count + ")");
                            }


                            //tworzenie schematu pliku .UpdateSchema.json
                            listadanychplikuschematu_tmp.Add
                            (
                                tabelanowejwersjiORIGlang_aktualnyKLUCZ +
                                "[[[---]]]" +
                                tabelanowejwersjiORIGlang_aktualnaNAZWAPLIKU
                            );



                        }


                        for (int ilz = 0; ilz < listazmian_tmp.Count; ilz++)
                        {
                            string[] dane_zmiany = listazmian_tmp[ilz].Split(new string[] { "[[[---]]]" }, StringSplitOptions.None);
                            string _KEY = dane_zmiany[0];
                            string _STRING = dane_zmiany[1];
                            string _RODZAJZMIANY = dane_zmiany[2];
                            string _STARY_STRING = dane_zmiany[3];


                            nowyplikJSONzawierajacyzmiany_sw.Write("    \"" + _KEY + "\": \"" + _STRING + "\"");

                            if (ilz + 1 != listazmian_tmp.Count)
                            {
                                nowyplikJSONzawierajacyzmiany_sw.Write(",");
                            }

                            nowyplikJSONzawierajacyzmiany_sw.Write("\n");



                            nowyplikUpdateLogJSONzawierajacyinfoozmianach_sw.Write("    \"" + _KEY + "\": \"" +

                            _RODZAJZMIANY + "\"");

                            if (ilz + 1 != listazmian_tmp.Count)
                            {
                                nowyplikUpdateLogJSONzawierajacyinfoozmianach_sw.Write(",");
                            }

                            nowyplikUpdateLogJSONzawierajacyinfoozmianach_sw.Write("\n");



                            if (_RODZAJZMIANY == "MODYFIKOWANY_STRING")
                            {
                                nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_sw.Write("    \"" + _KEY + "\": \"" +

                                _STARY_STRING + "\"");
                            }
                            else
                            {
                                nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_sw.Write("    \"" + _KEY + "\": \"" +

                                "BRAK_ZMIAN" + "\"");
                            }

                            if (ilz + 1 != listazmian_tmp.Count)
                            {
                                nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_sw.Write(",");
                            }

                            nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_sw.Write("\n");




                            if (_RODZAJZMIANY == "MODYFIKOWANY_STRING")
                            {
                                nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_sw.Write("    \"" + _KEY + "\": \"" +

                                _STRING + "\"");
                            }
                            else
                            {
                                nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_sw.Write("    \"" + _KEY + "\": \"" +

                                "BRAK_ZMIAN" + "\"");
                            }

                            if (ilz + 1 != listazmian_tmp.Count)
                            {
                                nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_sw.Write(",");
                            }

                            nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_sw.Write("\n");


                        }



                        for (int ilz2 = 0; ilz2 < listadanychplikuschematu_tmp.Count; ilz2++)
                        {
                            string[] dane = listadanychplikuschematu_tmp[ilz2].Split(new string[] { "[[[---]]]" }, StringSplitOptions.None);
                            string _KEY = dane[0];
                            string _NAZWAPLIKU = dane[1];

                            //wpis w pliku .UpdateSchema.json
                            nowyplikUpdateSchemaJSONzawierajacyschematpliku_sw.Write("    \"" + _KEY + "\": \"\"");

                            if (ilz2 + 1 != listadanychplikuschematu_tmp.Count)
                            {
                                nowyplikUpdateSchemaJSONzawierajacyschematpliku_sw.Write(",");
                            }

                            nowyplikUpdateSchemaJSONzawierajacyschematpliku_sw.Write("\n");


                            //wpis w pliku .UpdateLocStruct.json
                            nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_sw.Write("    \"" + _KEY + "\": \"" + _NAZWAPLIKU + "\"");

                            if (ilz2 + 1 != listadanychplikuschematu_tmp.Count)
                            {
                                nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_sw.Write(",");
                            }

                            nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_sw.Write("\n");

                        }



                        nowyplikJSONzawierajacyzmiany_sw.Close();
                        nowyplikUpdateLogJSONzawierajacyinfoozmianach_sw.Close();
                        nowyplikUpdateSchemaJSONzawierajacyschematpliku_sw.Close();
                        nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_sw.Close();
                        nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_sw.Close();
                        nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_sw.Close();

                        //}
                        //catch
                        //{
                        //Blad("BŁĄD: Wystąpił nieoczekiwany błąd w dostępie do pliku.");
                        //}

                        nowyplikJSONzawierajacyzmiany_fs.Close();
                        nowyplikUpdateLogJSONzawierajacyinfoozmianach_fs.Close();
                        nowyplikUpdateSchemaJSONzawierajacyschematpliku_fs.Close();
                        nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_fs.Close();
                        nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_fs.Close();
                        nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_fs.Close();

                        UtworzStopkeJSON(nowyplikJSONzawierajacyzmiany_nazwa);
                        UtworzStopkeJSON(nowyplikUpdateLogJSONzawierajacyinfoozmianach_nazwa);
                        UtworzStopkeJSON(nowyplikUpdateSchemaJSONzawierajacyschematpliku_nazwa);
                        UtworzStopkeJSON(nowyplikUpdateLocStructJSONzawierajacystrukturelokalizacji_nazwa);
                        UtworzStopkeJSON(nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_nazwa);
                        UtworzStopkeJSON(nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_nazwa);

                        Sukces("Utworzono 6 nowych plików o nazwach:\n" + nowyplikJSONzawierajacyzmiany_nazwa + "\n" + nowyplikUpdateLogJSONzawierajacyinfoozmianach_nazwa + "\n" + nowyplikUpdateSchemaJSONzawierajacyschematpliku_nazwa + "\n" + nowyplikUpdateLogJSONzawierajacyinfoozmianach_nazwa + "\n" + nowyplikUpdateOldModStringsJSONzawierajacystringistarejwersji_nazwa + "\n" + nowyplikUpdateNewModStringsJSONzawierajacystringinowejwersji_nazwa + "\nZawierają one wszystkie wykryte zmiany pomiędzy starą a nową wersją oryginalnego pliku ORIGlang.\nWAŻNE: Plik .UpdateSchema.json zawiera schemat nowego pliku lokalizacji (po aktualizacji), który musi być użyty w pwrpl-converter do konwertowania plików aktualizacji pochodzących z platformy Transifex.");


                    }
                    else
                    {
                        Blad("BŁĄD: Nie podano wymaganych informacji!");
                    }


                }
                else
                {
                    Blad("BŁĄD: Nie istnieje projekt w bazie danych MySQL o podanym tokenie!");
                }

            }
            else
            {
                Blad("BŁĄD: Nie istnieje żaden projekt zaimportowany wcześniej do bazy danych MySql.");
            }

            Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
            Console.ReadKey();



        }

        public static void IntegracjaPrzetlumaczonejTresciZNowaWersjaORIGLang_v2()
        {

            string ciag_istniejacych_tokenow = PobierzCiagTokenowIstniejacychTabel();

            if (ciag_istniejacych_tokenow != "")
            {

                Console.WriteLine("Aktualnie istniejace tabele projektow (tokeny): " + ciag_istniejacych_tokenow);

                Console.Write("Wpisz numer projektu (token): ");
                string podany_token_projektu = Console.ReadLine();

                if (ciag_istniejacych_tokenow.Contains(podany_token_projektu) && podany_token_projektu != "")
                {
                    SkopiujTabeleWrazZDanymi(podany_token_projektu + "_origfoldernowejwersji", podany_token_projektu + "_po_przetlumaczonyfoldernowejwersji");
                    SkopiujTabeleWrazZDanymi(podany_token_projektu + "_origfoldernowejwersji_pliki", podany_token_projektu + "_po_przetlumaczonyfoldernowejwersji_pliki");

                    ZmienOznaczenieJezykaDlaNazwPlikowLokalizacji(podany_token_projektu + "_po_przetlumaczonyfoldernowejwersji", "enGB", "deDE");

                    List<List<dynamic>> tabelanowejwersjiORIGlang_danetabel = MySql.MySql_ComplexWithResult
                    (
                        "SELECT * FROM `" + podany_token_projektu + "_origfoldernowejwersji" + "` ORDER BY `ID` ASC;",
                        skrypt,
                        "IntegracjaPrzetlumaczonejTresciZNowaWersjaORIGLang_v2/pobierz_dane_z_tabeli(tabela_nowej_wersji_ORIGLang)"
                    );

                    string polaczenie_mysql1_id = "polaczenie_mysql1";
                    MySqlConnection polaczenie_mysql1 = MySql.MySql_Connect(skrypt, polaczenie_mysql1_id);



                    //int nowyplikJSONzawierajacyzmiany_aktualnalinia = 1;
                    for (int ilx1 = 0; ilx1 < tabelanowejwersjiORIGlang_danetabel.Count; ilx1++)
                    {

                        int tabelanowejwersjiORIGlang_danetabel_numerrekordu = ilx1 + 1;
                        Console.WriteLine("Trwa aktualizacja danych w tabeli (tabela przetłumaczonej nowej wersji lang) - rekord: " + tabelanowejwersjiORIGlang_danetabel_numerrekordu.ToString() + "/" + tabelanowejwersjiORIGlang_danetabel.Count);

                        string tabelanowejwersjiORIGlang_aktualnyKLUCZ = tabelanowejwersjiORIGlang_danetabel[ilx1][3].ToString();
                        string tabelanowejwersjiORIGlang_aktualnySTRING = tabelanowejwersjiORIGlang_danetabel[ilx1][4].ToString();


                        int tabelastarejwersjiORIGlang_sprawdzenieistnieniarekordu = MySql.MySql_CountCommonConnection
                        (
                            polaczenie_mysql1,
                            "SELECT COUNT(*) FROM `" + podany_token_projektu + "_origfolderstarejwersji" + "` WHERE `Klucz` = '" + tabelanowejwersjiORIGlang_aktualnyKLUCZ + "';",
                            skrypt,
                            "IntegracjaPrzetlumaczonejTresciZNowaWersjaORIGLang_v2/spraqdz_czy_istnieje_taki_rekord(tabela_starej_wersji_ORIGLang)"
                        );


                        if (tabelastarejwersjiORIGlang_sprawdzenieistnieniarekordu == 1)
                        {
                            List<List<dynamic>> tabelastarejwersjiORIGlang_dane1rekordu = MySql.MySql_ComplexWithResult
                            (
                                "SELECT * FROM `" + podany_token_projektu + "_origfolderstarejwersji" + "` WHERE `Klucz` = '" + tabelanowejwersjiORIGlang_aktualnyKLUCZ + "';",
                                skrypt,
                                "IntegracjaPrzetlumaczonejTresciZNowaWersjaORIGLang_v2/pobierz_dane_z_tabeli(tabela_starej_wersji_ORIGLang)"
                            );

                            //Console.WriteLine("DBG: " + tabelastarejwersjiORIGlang_dane1rekordu[0][3].ToString());
                            //Console.WriteLine("DBG: tabelastarejwersjiORIGlang_dane1rekordu.Count = " + tabelastarejwersjiORIGlang_dane1rekordu.Count.ToString());

                            if (tabelastarejwersjiORIGlang_dane1rekordu.Count == 0)
                            {

                                //nic nie rób

                            }
                            else if (tabelastarejwersjiORIGlang_dane1rekordu.Count == 1)
                            {

                                string tabelastarejwersjiORIGlang_aktualnyKLUCZ = tabelastarejwersjiORIGlang_dane1rekordu[0][3].ToString();
                                string tabelastarejwersjiORIGlang_aktualnySTRING = tabelastarejwersjiORIGlang_dane1rekordu[0][4].ToString();

                                if (tabelastarejwersjiORIGlang_aktualnySTRING != tabelanowejwersjiORIGlang_aktualnySTRING)
                                {


                                    //nic nie rób

                                }
                                else
                                {

                                    int tabelastarejwersjiORIGlang_sprawdzenieistnieniarekordu2 = MySql.MySql_CountCommonConnection
                                    (
                                        polaczenie_mysql1,
                                        "SELECT COUNT(*) FROM `" + podany_token_projektu + "_przetlumaczonyfolderstarejwersji" + "` WHERE `Klucz` = '" + tabelanowejwersjiORIGlang_aktualnyKLUCZ + "';",
                                        skrypt,
                                        "IntegracjaPrzetlumaczonejTresciZNowaWersjaORIGLang_v2/sprawdz_istnienie_rekordu(tabela_przetlumaczonej_starej_wersji_Lang)"
                                    );

                                    if (tabelastarejwersjiORIGlang_sprawdzenieistnieniarekordu2 == 1)
                                    {

                                        List<List<dynamic>> tabelaprzetlumaczonejstarejwersjilang_dane1rekordu = MySql.MySql_ComplexWithResult
                                        (
                                            "SELECT * FROM `" + podany_token_projektu + "_przetlumaczonyfolderstarejwersji" + "` WHERE `Klucz` = '" + tabelanowejwersjiORIGlang_aktualnyKLUCZ + "';",
                                            skrypt,
                                            "IntegracjaPrzetlumaczonejTresciZNowaWersjaORIGLang_v2/sprawdz_istnienie_rekordu(tabela_przetlumaczonej_starej_wersji_Lang)"
                                        );



                                        string tabelaprzetlumaczonejstarejwersjilang_aktualnyKLUCZ = tabelaprzetlumaczonejstarejwersjilang_dane1rekordu[0][3].ToString();
                                        string tabelaprzetlumaczonejstarejwersjilang_aktualnySTRING = tabelaprzetlumaczonejstarejwersjilang_dane1rekordu[0][4].ToString();


                                        if (tabelaprzetlumaczonejstarejwersjilang_dane1rekordu.Count == 0)
                                        {

                                            //nic nie rób

                                        }
                                        else if (tabelaprzetlumaczonejstarejwersjilang_dane1rekordu.Count == 1)
                                        {

                                            string zapytanie_mysql1 = "UPDATE `" + podany_token_projektu + "_po_przetlumaczonyfoldernowejwersji" + "` SET `String` = '" + tabelaprzetlumaczonejstarejwersjilang_aktualnySTRING + "' WHERE `Klucz` = '" + tabelanowejwersjiORIGlang_aktualnyKLUCZ + "';";

                                            MySql.MySql_ComplexCommonConnection
                                            (
                                                polaczenie_mysql1,
                                                zapytanie_mysql1,
                                                skrypt,
                                                "IntegracjaPrzetlumaczonejTresciZNowaWersjaORIGLang_v2/uaktualnij_dane_w_tabeli(tabela_przetlumaczonej_nowej_wersji_Lang)"
                                            );



                                            //Console.WriteLine("Integruję przetłumaczoną treść (ilx1: " + ilx1 + ").");

                                        }
                                        else
                                        {
                                            Blad("BŁĄD: Klucz o wartości " + tabelanowejwersjiORIGlang_danetabel[ilx1][1] + " jest zdublowany w tabeli! (tabelastarejwersjiORIGlang_dane1tabeli.Count: " + tabelastarejwersjiORIGlang_dane1rekordu.Count + ")");
                                        }


                                    }

                                }

                            }
                            else
                            {
                                Blad("BŁĄD: Klucz o wartości " + tabelanowejwersjiORIGlang_danetabel[ilx1][1] + " jest zdublowany w tabeli! (tabelastarejwersjiORIGlang_dane1tabeli.Count: " + tabelastarejwersjiORIGlang_dane1rekordu.Count + ")");
                            }


                        }


                    }

                    //polaczenie_mysql1.Close();
                    MySql.MySql_Disconnect(polaczenie_mysql1, skrypt, polaczenie_mysql1_id);


                    Sukces("Zintegrowano przetłumaczoną treść z nową wersją lokalizacji w bazie danych MySQL.");

                }
                else
                {
                    Blad("BLAD: Nie istnieje projekt o podanym tokenie!");
                }

            }
            else
            {
                Blad("Nie istnie żaden projekt zaimportowany do MySql.");
            }


        }

        public static void WdrazanieTresciZZewnetrznegoPlikuJSONdoMySQL_v2()
        {

            Console.Write("Podaj nazwę pliku JSON do wdrożenia: ");
            string plikJSON_nazwa = Console.ReadLine();

            if (plikJSON_nazwa != "" && File.Exists(plikJSON_nazwa))
            {
                string ciag_istniejacych_tokenow = PobierzCiagTokenowIstniejacychTabel();

                if (ciag_istniejacych_tokenow != "")
                {

                    Console.WriteLine("Aktualnie istniejace tabele projektow (tokeny): " + ciag_istniejacych_tokenow);

                    Console.Write("Wpisz numer projektu (token): ");
                    string podany_token_projektu = Console.ReadLine();

                    if (ciag_istniejacych_tokenow.Contains(podany_token_projektu) && podany_token_projektu != "")
                    {

                        UaktualnijRekordy(plikJSON_nazwa, podany_token_projektu + "_po_przetlumaczonyfoldernowejwersji");

                        Sukces("Zaktualizowano rekordy.");

                    }
                    else
                    {
                        Blad("Nie istnieje projekt o podanym tokenie.");
                    }


                }
                else
                {
                    Blad("Nie istnie żaden projekt zaimportowany do MySql.");
                }

            }
            else
            {
                Blad("BŁĄD: Nie ma takiego pliku.");
            }


        }

        public static void SprawdzanieZduplikowanychKluczy()
        {

            string ciag_istniejacych_tokenow = PobierzCiagTokenowIstniejacychTabel();

            if (ciag_istniejacych_tokenow != "")
            {

                Console.WriteLine("Aktualnie istniejace tabele projektow (tokeny): " + ciag_istniejacych_tokenow);

                Console.Write("Wpisz numer projektu (token): ");
                string podany_token_projektu = Console.ReadLine();

                if (ciag_istniejacych_tokenow.Contains(podany_token_projektu) && podany_token_projektu != "")
                {

                    string polaczenie_mysql1_id = "polaczenie_mysql1_id/SprawdzanieZduplikowanychKluczy()";
                    MySqlConnection polaczenie_mysql1 = MySql.MySql_Connect(skrypt, polaczenie_mysql1_id);



                    List<List<dynamic>> dane_tabeli = MySql.MySql_ComplexWithResult
                    (
                        "SELECT * FROM `" + podany_token_projektu + "_po_przetlumaczonyfoldernowejwersji` ORDER BY `ID` ASC;",
                        skrypt,
                        "SprawdzanieZduplikowanychKluczy/pobierz_dane_tabeli(_po_przetlumaczonyfoldernowejwersji)"
                    );

                    /*
                    int test64 = MySql.MySql_CountCommonConnection(polaczenie_mysql1, "SELECT COUNT(*) FROM `" + podany_token_projektu + "_po_przetlumaczonyfoldernowejwersji` WHERE `Klucz` = 'fed3ad3b-9726-4b4f-8cc5-68f0215bfe3f';", skrypt, "test64");

                    Console.WriteLine("test64: " + test64.ToString());
                    */

                    for (int r = 0; r < dane_tabeli.Count; r++)
                    {
                        string poszukiwany_klucz = dane_tabeli[r][3];

                        int ilosc_kluczy = MySql.MySql_CountCommonConnection
                        (
                            polaczenie_mysql1, "SELECT COUNT(*) FROM `" + podany_token_projektu + "_po_przetlumaczonyfoldernowejwersji` WHERE `Klucz` = '" + poszukiwany_klucz + "';",
                            skrypt,
                            "SprawdzanieZduplikowanychKluczy/petlaFOR(r==" + r + ")"
                        );

                        if (ilosc_kluczy == 1)
                        {
                            //nic nie rób
                        }
                        else if (ilosc_kluczy > 1)
                        {
                            //zaraportuj
                            Blad("UWAGA: Wykryto następującą liczbę tych samych kluczy \"" + poszukiwany_klucz + "\": " + ilosc_kluczy.ToString());
                        }
                        else if (ilosc_kluczy == -1)
                        {
                            Blad("Błąd: Wystąpił problem ze zliczeniem ilości rekordów w tabeli.");
                        }
                        else
                        {
                            Blad("Błąd: Wystąpił nieoczekiwany wyjątek podczas zliczania ilości rekordów w tabeli.");
                        }

                    }


                    MySql.MySql_Disconnect(polaczenie_mysql1, skrypt, polaczenie_mysql1_id);

                }
                else
                {
                    Blad("BLAD: Nie istnieje projekt o podanym tokenie!");
                }

            }
            else
            {
                Blad("Nie istnie żaden projekt zaimportowany do MySql.");
            }


        }

        public static void UaktualnijRekordy(string plikJSON_nazwa, string tabela_nazwa)
        {
            uint plik_liczbalinii = PoliczLiczbeLinii(folderglownyprogramu + plikJSON_nazwa);

            FileStream plik_fs = new FileStream(plikJSON_nazwa, FileMode.Open, FileAccess.Read);

            StreamReader plik_sr = new StreamReader(plik_fs);

            string polaczenie_mysql1_id = "polaczenie_mysql1 (metoda: WdrazanieTresciZZewnetrznegoPlikuJSONdoMySql)";
            MySqlConnection polaczenie_mysql1 = MySql.MySql_Connect(skrypt, polaczenie_mysql1_id);


            string tmp_ostatnia_odczytana_tresc_KEY = "";

            uint linia = 1;
            while (plik_sr.Peek() != -1)
            {

                /*
                string plik_tresclinii = plik_sr.ReadLine();

                
                Console.WriteLine(plikJSON_nazwa + ": Analizowanie linii nr.: " + linia + "/" + plik_liczbalinii);

                if (plik_tresclinii.Contains("\"Key\": "))
                {
                    string tresc_KEY = plik_tresclinii.TrimStart().Split(new char[] { '\"' })[3];
                    tmp_ostatnia_odczytana_tresc_KEY = tresc_KEY;


                }
                else if (plik_tresclinii.Contains("\"Value\": "))
                {
                    string tresc_STRING = plik_tresclinii.TrimStart()

                        .Replace("\\n", "<br>")
                        .Replace("\\\"", "<bs_n1>")
                        .Replace("'", "<apostrof>")
                        .Replace("\\t", "<t>")
                        .Replace("\\\\", "<bs_n2>")


                        .Split(new char[] { '\"' })[3];

                    string dodajtrescSTRING_zapytanie = "UPDATE `" + tabela_nazwa + "` SET `String` = '" + tresc_STRING + "' WHERE `Klucz` = '" + tmp_ostatnia_odczytana_tresc_KEY + "';";

                    MySql.MySql_Query
                    (
                      polaczenie_mysql1,
                      dodajtrescSTRING_zapytanie,
                      skrypt,
                      polaczenie_mysql1_id
                    );

                    Console.WriteLine(linia + "/" + plik_liczbalinii + ": Aktualizowanie w MySQL String'a o kluczu: " + tmp_ostatnia_odczytana_tresc_KEY);


                }

                */




                string tresc_linii_JSON = plik_sr.ReadLine();

                string tresclinii_ciagzmiennych = "";


                string[] linia_podzial_1 = tresc_linii_JSON.Split(new string[] { "\": \"" }, StringSplitOptions.None);

                /*
                for (int a1 = 0; a1 < linia_podzial_1.Length; a1++)
                {

                    //Console.WriteLine("linia_podzial_1[" + a1 + "]: " + linia_podzial_1[a1]);
                }
                */

                //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_1.Length: " + linia_podzial_1.Length);

                if (linia_podzial_1.Length <= 2)
                {
                    string KEYt1 = linia_podzial_1[0].Trim();
                    int KEYt1_iloscznakow = KEYt1.Length;

                    if (KEYt1_iloscznakow >= 2)
                    {

                        string[] linia_2_separatory = { KEYt1 + "\": \"" };

                        string[] linia_podzial_2 = tresc_linii_JSON.Split(linia_2_separatory, StringSplitOptions.None);

                        /*
                        for (int a2 = 0; a2 < linia_podzial_2.Length; a2++)
                        {

                            Console.WriteLine("linia_podzial_2[" + a2 + "]: " + linia_podzial_2[a2]);
                        }
                        */

                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_2.Length: " + linia_podzial_2.Length);

                        if (linia_podzial_2.Length >= 2)
                        {

                            string STRINGt1 = linia_podzial_2[1].TrimEnd();
                            int STRINGt1_iloscznakow = STRINGt1.Length;


                            //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEYt1_iloscznakow: " + KEYt1_iloscznakow);
                            //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRINGt1_iloscznakow: " + STRINGt1_iloscznakow);


                            if (KEYt1_iloscznakow >= 2 && STRINGt1_iloscznakow >= 1)
                            {
                                string KEY = KEYt1.Remove(0, 1);

                                int cofniecie_wskaznika = STRINGt1_iloscznakow - 1;
                                int usunac_znakow = 1;
                                if (linia != plik_liczbalinii - 2)
                                {
                                    cofniecie_wskaznika = STRINGt1_iloscznakow - 2;
                                    usunac_znakow = 2;
                                }

                                string STRINGt2 = STRINGt1.Remove(cofniecie_wskaznika, usunac_znakow);
                                string STRING = STRINGt2;



                                //Console.WriteLine("[linia:" + plik_linia + "] KEY:" + KEY);
                                //Console.WriteLine("[linia:" + plik_linia + "] STRING:" + STRING);


                                if (KEY != "$id")
                                {

                                    string tresc_KEY = KEY;



                                    string tresc_STRING = STRING
                                           .Replace("\\n", "<br>")
                                           .Replace("\\\"", "<bs_n1>")
                                           .Replace("'", "<apostrof>")
                                           .Replace("\\t", "<t>")
                                           .Replace("\\\\", "<bs_n2>"); ;


                                    string rodzajenawiasow = "{|}";
                                    int iloscnawiasowwlinii = 0;
                                    Regex regex = new Regex(rodzajenawiasow);
                                    MatchCollection matchCollection = regex.Matches(tresc_STRING);
                                    foreach (var match in matchCollection)
                                    {
                                        iloscnawiasowwlinii++;
                                    }
                                    if (iloscnawiasowwlinii % 2 != 0)
                                    {
                                        Blad("UWAGA: Linia nr." + linia + " ma błędną ilość nawiasów {}!: Treść linii: " + tresc_STRING);
                                    }



                                    string uaktualnijtrescSTRING_zapytanie = "UPDATE `" + tabela_nazwa + "` SET `String` = '" + tresc_STRING + "' WHERE `Klucz` = '" + tresc_KEY + "';";

                                    MySql.MySql_Query
                                    (
                                      polaczenie_mysql1,
                                      uaktualnijtrescSTRING_zapytanie,
                                      skrypt,
                                      polaczenie_mysql1_id
                                    );


                                    Console.WriteLine(linia + "/" + plik_liczbalinii + ": Aktualizowanie w MySQL String'a o kluczu: " + tresc_KEY);








                                }

                            }

                        }

                    }


                }






                linia++;
            }







            MySql.MySql_Disconnect(polaczenie_mysql1, skrypt, polaczenie_mysql1_id);



        }




        //EKSPERYMENTALNE

        public static void JSONtoTXTTransifexCOM_ZKluczami()
        {
            string nazwaplikuJSON;

            Console.Write("Podaj nazwę pliku JSON: ");
            nazwaplikuJSON = Console.ReadLine();
            if (nazwaplikuJSON == "") { nazwaplikuJSON = "test1.json"; }
            Console.WriteLine("Podano nazwę pliku: " + nazwaplikuJSON);
            if (File.Exists(folderglownyprogramu + nazwaplikuJSON))
            {
                uint plik_JSON_liczbalinii = PoliczLiczbeLinii(folderglownyprogramu + nazwaplikuJSON);

                //Console.WriteLine("Istnieje podany plik.");
                FileStream plik_JSON_fs = new FileStream(nazwaplikuJSON, FileMode.Open, FileAccess.Read);
                FileStream nowy_plik_transifexCOMkeystxt_fs = new FileStream(nazwaplikuJSON + ".keysTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);
                FileStream nowy_plik_transifexCOMstringstxt_fs = new FileStream(nazwaplikuJSON + ".stringsTransifexCOM.txt", FileMode.Create, FileAccess.ReadWrite);

                try
                {
                    int ilosc_wykrytych_STRINGS = 0;
                    int ilosc_wykrytych_VARS = 0;
                    List<List<string>> vars_tmp = new List<List<string>>(); //skladnia vars_tmp[numer_linii][0:key||1:ciag_zmiennych]
                    const char separator = ';';


                    StreamReader plik_JSON_sr = new StreamReader(plik_JSON_fs);
                    StreamWriter nowy_plik_transifexCOMkeystxt_sr = new StreamWriter(nowy_plik_transifexCOMkeystxt_fs);
                    StreamWriter nowy_plik_transifexCOMstringstxt_sr = new StreamWriter(nowy_plik_transifexCOMstringstxt_fs);

                    int plik_JSON_linia = 1;
                    while (plik_JSON_sr.Peek() != -1)
                    {
                        string tresc_linii_JSON = plik_JSON_sr.ReadLine();

                        string tresclinii_ciagzmiennych = "";

                        vars_tmp.Add(new List<string>());


                        string[] linia_podzial_1 = tresc_linii_JSON.Split(new string[] { "\": \"" }, StringSplitOptions.None);

                        /*
                        for (int a1 = 0; a1 < linia_podzial_1.Length; a1++)
                        {

                            //Console.WriteLine("linia_podzial_1[" + a1 + "]: " + linia_podzial_1[a1]);
                        }
                        */

                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_1.Length: " + linia_podzial_1.Length);

                        if (linia_podzial_1.Length <= 2)
                        {
                            string KEYt1 = linia_podzial_1[0].Trim();
                            int KEYt1_iloscznakow = KEYt1.Length;

                            if (KEYt1_iloscznakow >= 2)
                            {

                                string[] linia_2_separatory = { KEYt1 + "\": \"" };

                                string[] linia_podzial_2 = tresc_linii_JSON.Split(linia_2_separatory, StringSplitOptions.None);

                                /*
                                for (int a2 = 0; a2 < linia_podzial_2.Length; a2++)
                                {

                                    Console.WriteLine("linia_podzial_2[" + a2 + "]: " + linia_podzial_2[a2]);
                                }
                                */

                                //Console.WriteLine("[linia:" + plik_JSON_linia + "] linia_podzial_2.Length: " + linia_podzial_2.Length);

                                if (linia_podzial_2.Length >= 2)
                                {

                                    string STRINGt1 = linia_podzial_2[1].TrimEnd();
                                    int STRINGt1_iloscznakow = STRINGt1.Length;


                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEYt1_iloscznakow: " + KEYt1_iloscznakow);
                                    //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRINGt1_iloscznakow: " + STRINGt1_iloscznakow);


                                    if (KEYt1_iloscznakow >= 2 && STRINGt1_iloscznakow >= 1)
                                    {
                                        string KEY = KEYt1.Remove(0, 1);

                                        int cofniecie_wskaznika = STRINGt1_iloscznakow - 1;
                                        int usunac_znakow = 1;
                                        if (plik_JSON_linia != plik_JSON_liczbalinii - 2)
                                        {
                                            cofniecie_wskaznika = STRINGt1_iloscznakow - 2;
                                            usunac_znakow = 2;
                                        }

                                        string STRINGt2 = STRINGt1.Remove(cofniecie_wskaznika, usunac_znakow);
                                        string STRING = STRINGt2;



                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] KEY:" + KEY);
                                        //Console.WriteLine("[linia:" + plik_JSON_linia + "] STRING:" + STRING);


                                        if (KEY != "$id")
                                        {

                                            string tresc_KEY = KEY;

                                            try
                                            {
                                                //Console.WriteLine("indeks wykrytego KEY'a: " + ilosc_wykrytych_VARS);

                                                vars_tmp[ilosc_wykrytych_VARS].Add(tresc_KEY);
                                            }
                                            catch
                                            {
                                                Blad("BLAD: vars_tmp #1!");
                                            }

                                            //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja klucza o treści: " + tresc_KEY);

                                            ilosc_wykrytych_VARS++;


                                            //string tresc_STRING = STRING;


                                            string tresc_STRING = STRING

                                            .Replace("\\n", "<br>")
                                            .Replace("\\\"", "<bs_n1>")

                                            ;

                                            if (tresc_STRING == "")
                                            {
                                                tresc_STRING = " ";
                                            }

                                            /*
                                            if (tresc_STRING.Contains('{') || tresc_STRING.Contains('}'))
                                            {
                                                string rodzajenawiasow = "{|}";
                                                int iloscnawiasowwlinii = 0;
                                                Regex regex = new Regex(rodzajenawiasow);
                                                MatchCollection matchCollection = regex.Matches(tresc_STRING);
                                                foreach (var match in matchCollection)
                                                {
                                                    iloscnawiasowwlinii++;
                                                }
                                                if (iloscnawiasowwlinii % 2 == 0)
                                                {
                                                    //Console.WriteLine("Linia nr." + plik_JSON_linia + " posiada pary nawiasów {}.");

                                                    if (tresc_STRING.Contains('{') && tresc_STRING.Contains('}'))
                                                    {
                                                        string[] tresclinii_nawklamrowy_podzial1 = tresc_STRING.Split(new char[] { '{' });

                                                        for (int i1 = 0; i1 < tresclinii_nawklamrowy_podzial1.Length; i1++)
                                                        {
                                                            //Console.WriteLine("tresclinii_nawklamrowy_podzial1[" + i1.ToString() + "]: " + tresclinii_nawklamrowy_podzial1[i1]);

                                                            if (tresclinii_nawklamrowy_podzial1[i1].Contains('}'))
                                                            {
                                                                int kl_index = i1 - 1;
                                                                string tresczwnetrzanawiasuklamrowego = tresclinii_nawklamrowy_podzial1[i1].Split(new char[] { '}' })[0];
                                                                string nazwazmiennej_w_stringstxt = "<kl" + kl_index + ">"; //np. <kl0>, <kl1>, <kl2> itd.

                                                                //Console.WriteLine("tresczwnetrzanawiasuklamrowego (" + i1.ToString() + "): " + tresczwnetrzanawiasuklamrowego);

                                                                tresclinii_ciagzmiennych += "{" + tresczwnetrzanawiasuklamrowego + "}";

                                                                if (i1 + 1 != tresclinii_nawklamrowy_podzial1.Length) { tresclinii_ciagzmiennych += separator; }

                                                                tresc_STRING = tresc_STRING.Replace("{" + tresczwnetrzanawiasuklamrowego + "}", nazwazmiennej_w_stringstxt);

                                                                //Console.WriteLine("nazwazmiennej_w_stringstxt: " + nazwazmiennej_w_stringstxt);
                                                                //Console.WriteLine("tresc_STRING: " + tresc_STRING);


                                                            }


                                                        }

                                                    }



                                                }
                                                else
                                                {
                                                    Blad("BŁĄD: Linia nr." + plik_JSON_linia + " ma błędną ilość nawiasów {}!");
                                                }



                                            }
                                            else
                                            {
                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " NIE posiada pary nawiasów {}.");

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " konwersja string'a o tresci: " + tresc_STRING);

                                                //Console.WriteLine("Linia nr." + plik_JSON_linia + " zawiera VARS: " + tresclinii_ciagzmiennych);

                                            }
                                            */


                                            nowy_plik_transifexCOMstringstxt_sr.WriteLine("<" + tresc_KEY + ">" + tresc_STRING);

                                            //vars_tmp[ilosc_wykrytych_STRINGS].Add(tresclinii_ciagzmiennych);

                                            ilosc_wykrytych_STRINGS++;


                                        }

                                    }

                                }

                            }


                        }


                        Console.WriteLine("Trwa konwertowanie linii nr. " + plik_JSON_linia + "/" + plik_JSON_liczbalinii + " [" + PoliczPostepWProcentach(plik_JSON_linia, plik_JSON_liczbalinii) + "%]");

                        plik_JSON_linia++;
                    }

                    //Console.WriteLine("ilosc_wykrytych_vars:" + ilosc_wykrytych_VARS);
                    //Console.WriteLine("vars_tmp[0][0]: " + vars_tmp[0][0]);


                    for (int iv1 = 0; iv1 < vars_tmp.Count; iv1++)
                    {
                        for (int iv2 = 0; iv2 < vars_tmp[iv1].Count; iv2++)
                        {
                            //Console.WriteLine("vars_tmp[" + iv1 + "][" + iv2 + "]: " + vars_tmp[iv1][iv2]);

                            if (iv2 == 0)
                            {
                                nowy_plik_transifexCOMkeystxt_sr.Write(vars_tmp[iv1][iv2]);
                            }
                            else if (iv2 == 1)
                            {
                                if (vars_tmp[iv1][iv2] != "")
                                {
                                    nowy_plik_transifexCOMkeystxt_sr.Write(separator + vars_tmp[iv1][iv2] + "\n");
                                }
                                else
                                {
                                    nowy_plik_transifexCOMkeystxt_sr.Write("\n");
                                }
                            }

                            nowy_plik_transifexCOMkeystxt_sr.Write("\n");

                        }
                    }





                    nowy_plik_transifexCOMkeystxt_sr.Close();
                    nowy_plik_transifexCOMstringstxt_sr.Close();
                    plik_JSON_sr.Close();


                }
                catch
                {
                    Blad("BŁĄD: Wystapil nieoczekiwany błąd w dostępie do plików.");
                }

                nowy_plik_transifexCOMkeystxt_fs.Close();
                nowy_plik_transifexCOMstringstxt_fs.Close();
                plik_JSON_fs.Close();


            }
            else
            {
                Blad("BŁĄD: Brak takiego pliku.");
            }

            if (File.Exists(nazwaplikuJSON + ".keys.txt") && File.Exists(nazwaplikuJSON + ".strings.txt"))
            {
                Console.WriteLine("----------------------------------");
                Sukces("Utworzono 2 pliki TXT: \"" + nazwaplikuJSON + ".keysTransifexCOM.txt\" oraz \"" + nazwaplikuJSON + ".stringsTransifexCOM.txt\"");

            }


        }

    }
}
