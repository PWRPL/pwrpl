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

}