using System;
using System.IO;
using System.Linq;
using pwrpl.Komunikat;

namespace pwrpl;

public class WyodrebnianiePlikow
{
    private static readonly char sc = pwrpl.MainWindow.sc;

    private static bool FiltrowanieNazwy_PlikiTXTiJSON(string nazwa_pliku)
    {
        bool czytowlasciwyplik = false;
        string x = Path.GetFileName(nazwa_pliku);

        if (x.ToLower().EndsWith(".txt") == true || x.ToLower().EndsWith(".json") == true)
        {
            if (x.ToLower().EndsWith("runtimeconfig.json") == false && x.ToLower().EndsWith("deps.json") == false)
            {
                if
                (
                    (x.Contains("plPL") == false && x.Contains("plPL-update") == false && x.Contains("NOWY_") == false)
                    ||
                    ((x.Contains("plPL") == true || x.Contains("plPL-update") == true) && x.Contains("NOWY_") == true)
                )
                {
                    if (x != "cfg.json")
                    {
                        if (x.Contains("#") == false)
                        {
                            czytowlasciwyplik = true;
                        }
                    }
                }
            }
        }

        return czytowlasciwyplik;
    }
    
    private static bool FiltrowanieNazwy_Metadane(string nazwa_pliku)
    {
        bool czytowlasciwyplik = false;
        string x = Path.GetFileName(nazwa_pliku);


        if (x.ToLower().EndsWith(".txt") == true || x.ToLower().EndsWith(".json") == true)
        {
            if (x.ToLower().EndsWith("runtimeconfig.json") == false && x.ToLower().EndsWith("deps.json") == false)
            {
                if (x == "cfg.json")
                {
                    czytowlasciwyplik = true;
                }
                else if (x.Contains("NOWY_") == false)
                {
                    if (x.Contains("plPL") == true || x.Contains("plPL-update") == true)
                    {
                        if (x.Contains("keysTransifexCOM.txt") == true)
                        {
                            czytowlasciwyplik = true;
                        }
                    }
                    else if (x.Contains("#") == true && x.Contains("Update") == true)
                    {
                        czytowlasciwyplik = true;
                    }
                    
                }
                
            }
        }
        
        return czytowlasciwyplik;
    }

    
    public static void WyodrebnijPlikiTXTiJSON()
    {
        #if DEBUG
            Console.WriteLine("[DEBUG] Zaakceptowano wyodrębnianie plików .txt i .json.");
        #endif

        string[] wszystkieplikiTXTiJSON = Directory.GetFiles
        (
            pwrpl.MainWindow.pwrpl_katalogglownyprogramu,
            "*.*"
        )
        .Where(x => FiltrowanieNazwy_PlikiTXTiJSON(x) == true).ToArray();
        
        #if DEBUG
            Console.WriteLine("[DEBUG] Wyszukiwanie plików .txt i .json w katalogu: " + MainWindow.pwrpl_katalogglownyprogramu);
            if (wszystkieplikiTXTiJSON.Length != 0)
            {
                foreach (var plik in wszystkieplikiTXTiJSON)
                {
                    Console.WriteLine("[DEBUG] " + plik);
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] Nie znaleziono żadnych plików .txt i .json w katalogu z programem.");
            }
        #endif

        if (wszystkieplikiTXTiJSON.Length != 0)
        {
            char sc = pwrpl.MainWindow.sc;

            string aktualnadataiczas = DateTime.Now.ToString("yyyyMMddHHmmss");

            string podkatalog_nazwa = $"__{aktualnadataiczas}_plikiTXTiJSON";
            string podkatalog_sciezka = $"{pwrpl.MainWindow.pwrpl_katalogglownyprogramu}{sc}{podkatalog_nazwa}";

            if (Directory.Exists(podkatalog_sciezka) == false)
            {
                Directory.CreateDirectory(podkatalog_sciezka);
            }

            bool powodzenie_przenoszenia = true;
            foreach (string plik_sciezka in wszystkieplikiTXTiJSON)
            {
                string? plik_nazwa = plik_sciezka.Split(sc).LastOrDefault();
                if (plik_nazwa != null)
                {
                    if (File.Exists(plik_sciezka) == true)
                    {
                        File.Move(plik_sciezka, $"{podkatalog_sciezka}{sc}{plik_nazwa}");
                    }
                    else
                    {
                        powodzenie_przenoszenia = false;
                    }
                }
                else
                {
                    powodzenie_przenoszenia = false;
                }

            }

            if (powodzenie_przenoszenia == true)
            {
                Komunikat.Okno.Otworz($"Powodzenie operacji", $"Wyodbrębniono pliki .txt i .json z katalogu programu do podkatalogu o nazwie \"{podkatalog_nazwa}\".\nIlość plików: {wszystkieplikiTXTiJSON.Length}");
            }
            else
            {
                Komunikat.Okno.Otworz($"Niepowodzenie operacji", $"Wystąpiły niespodziewane problemy z wyodrębnianiem plików .txt i .json do podkatalogu o nazwie \"{podkatalog_nazwa}\".");
            }

        }
        else
        {
            pwrpl.Komunikat.Okno.Otworz("Wyszukiwanie plików *.txt i *.json", "Nie znaleziono żadnych plików .txt i .json w katalogu z programem.");
        }

    }
    
    public static void WyodrebnijMetadane()
    {
        #if DEBUG
            Console.WriteLine("[DEBUG] Zaakceptowano wyodrębnianie metadanych.");
        #endif

        string[] tylkoglownykatalog_plikiTXTiJSON = Directory.GetFiles
        (
            pwrpl.MainWindow.pwrpl_katalogglownyprogramu,
            "*.*"
        )
        .Where(x => FiltrowanieNazwy_Metadane(x) == true).ToArray();
        
        #if DEBUG
            Console.WriteLine("[DEBUG] Wyszukiwanie plików .txt i .json w katalogu: " + MainWindow.pwrpl_katalogglownyprogramu);
            if (tylkoglownykatalog_plikiTXTiJSON.Length != 0)
            {
                foreach (var plik in tylkoglownykatalog_plikiTXTiJSON)
                {
                    Console.WriteLine("[DEBUG] " + plik);
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] Nie znaleziono żadnych plików metadanych keysTransifexCOM.txt.");
            }
        #endif

        
        
        char sc = pwrpl.MainWindow.sc;

        string aktualnadataiczas = DateTime.Now.ToString("yyyyMMddHHmmss");

        string podkatalog_nazwa = $"__{aktualnadataiczas}_metadane";
        string podkatalog_sciezka = $"{pwrpl.MainWindow.pwrpl_katalogglownyprogramu}{sc}{podkatalog_nazwa}{sc}metadane";


        if (tylkoglownykatalog_plikiTXTiJSON.Length != 0 || Directory.Exists($"{MainWindow.pwrpl_katalogglownyprogramu}{sc}update") == true)
        {
            bool powodzenie_przenoszenia = true;
        
            if (Directory.Exists($"{MainWindow.pwrpl_katalogglownyprogramu}{sc}update") == true)
            {
                powodzenie_przenoszenia =  PrzeniesKatalogZCalaZawartoscia
                (
                    $"{MainWindow.pwrpl_katalogglownyprogramu}{sc}update",
                    $"{podkatalog_sciezka}{sc}update"
                );
                        
            }
        
            if (Directory.Exists(podkatalog_sciezka) == false)
            {
                Directory.CreateDirectory(podkatalog_sciezka);
            }
        
            foreach (string plik_sciezka in tylkoglownykatalog_plikiTXTiJSON)
            {
                string? plik_nazwa = plik_sciezka.Split(sc).LastOrDefault();
                if (plik_nazwa != null)
                {
                    if (File.Exists(plik_sciezka) == true)
                    {
                        if (Path.GetFileName(plik_sciezka).Contains("#") == true && Path.GetFileName(plik_sciezka).Contains("Update") == true)
                        {
                            File.Move(plik_sciezka, $"{podkatalog_sciezka}{sc}update{sc}{plik_nazwa}");
                        }
                        else
                        {
                            File.Move(plik_sciezka, $"{podkatalog_sciezka}{sc}{plik_nazwa}");
                        }
                    
                    }
                    else
                    {
                        powodzenie_przenoszenia = false;
                    }
                }
                else
                {
                    powodzenie_przenoszenia = false;
                }

            }
        

            if (powodzenie_przenoszenia == true)
            {
                Okno.Otworz($"Powodzenie operacji", $"Wyodbrębniono metadane z katalogu programu do podkatalogu o nazwie \"{podkatalog_nazwa}\".");
            }
            else
            {
                Okno.Otworz($"Niepowodzenie operacji", $"Wystąpiły niespodziewane problemy z wyodrębnianiem metadanych do podkatalogu o nazwie \"{podkatalog_nazwa}\".\nNie wszystkie pliki zostały przeniesione.");
            }
            
        }
        else
        {
            Okno.Otworz("Sprawdzanie metadanych", $"Nie znaleziono żadnych metadanych w katalogu z programem, które można byłoby wyodrębnić.");
        }
        

    }

    public static bool PrzeniesKatalogZCalaZawartoscia(string katalogzrodlowy_path, string katalogdocelowy_path)
    {
        #if DEBUG
            Console.WriteLine($"[DEBUG] katalogzrodlowy_path=={katalogzrodlowy_path}");
            Console.WriteLine($"[DEBUG] katalogdocelowy_path=={katalogdocelowy_path}");
        #endif
        
        bool sukces_operacji = false;
        
        
        if (Directory.Exists(katalogzrodlowy_path) == true)
        {
            Directory.CreateDirectory(katalogdocelowy_path);
            
            foreach (var plik in Directory.GetFiles(katalogzrodlowy_path))
            {
                string plikdocelowy = Path.Combine(katalogdocelowy_path, Path.GetFileName(plik));
                File.Move(plik, plikdocelowy);
                
                //Console.WriteLine($"[DEBUG] Próba przeniesienia pliku z: {plik} do {plikdocelowy}");
            }
            
            foreach (var katalog in Directory.GetDirectories(katalogzrodlowy_path))
            {
                string docelowykatalog = Path.Combine(katalogdocelowy_path, Path.GetFileName(katalog));
                Directory.Move(katalog, docelowykatalog);
                
                //Console.WriteLine($"[DEBUG] Próba przeniesienia katalogu z: {katalog} do {docelowykatalog}");

            }

            sukces_operacji = true;
            
        }
        else
        {
            //nie istnieje podany folder źródłowy
            
            #if DEBUG
                  KonsolaGUI.Console.WriteLine($"[DEBUG] Nie istnieje podany folder źródłowy.");
            #endif

        }

        if (sukces_operacji == true)
        {
            if (Directory.Exists(katalogzrodlowy_path) == true) { Directory.Delete(katalogzrodlowy_path); }
        }
        
        return sukces_operacji;
    }

    
}