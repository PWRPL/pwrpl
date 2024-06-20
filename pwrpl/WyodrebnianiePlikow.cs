using System;
using System.IO;
using System.Linq;
using Console = pwrpl.KonsolaGUI.Console;

namespace pwrpl;

public class WyodrebnianiePlikow
{
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
        .Where
        (
            x => 
            (x.ToLower().EndsWith(".txt") || x.ToLower().EndsWith(".json"))
            &&
            x.ToLower().EndsWith("runtimeconfig.json") == false
            &&
            x.ToLower().EndsWith("deps.json") == false
        ).ToArray();
        
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

            string podkatalog_nazwa = $"{aktualnadataiczas}_plikiTXTiJSON";
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
}