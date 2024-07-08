using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using System.Xml.Serialization;

using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace pwrpl_converter
{
    

    public class konfiguracja
    {
        
        const string skrypt = "konfiguracja.cs";

        public static void WygenerujDomyslnyPlikKonfiguracyjnyJesliNieIstnieje()
        {
            if (File.Exists("cfg.json") == false)
            {
                FileStream plikCFGdomyslny_fs = new FileStream("cfg.json", FileMode.Create, FileAccess.Write);
                StreamWriter plikCFGdomyslny_sw = new StreamWriter(plikCFGdomyslny_fs);

                //DOMYŚLNE WARTOŚCI KONFIGURACJI, GDY BRAK PLIKU CFG.JSON W FOLDERZE Z PROGRAMEM
                plikCFGdomyslny_sw.WriteLine("{");



                plikCFGdomyslny_sw.WriteLine("      \"autoWprowadzanieNazwPlikowWejsciowych\": \"0\",");
                plikCFGdomyslny_sw.WriteLine("      \"zdefiniowaneMakro\": \"<TUTAJ_ZDEFINIUJ_MAKRO>\",");
                plikCFGdomyslny_sw.WriteLine("      \"narzucRozmiarWProcentachDlaWielkichZnakowPL\": \"100\",");
                plikCFGdomyslny_sw.WriteLine("      \"numerPorzadkowyPierwszegoProjektuZTransifexCOM\": \"01\",");
                plikCFGdomyslny_sw.WriteLine("      \"wersjaGryPierwszegoProjektuZTransifexCOM\": \"1.0.1c\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaPlikuTXTZTransifexCOM\": \"\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaPlikuTXTAktualizacjiZTransifexCOM\": \"for_use_%NUMER_PORZADKOWY_AKTUALIZACJI%-pwr_pl-korekta-aktualizacji-%OZNACZENIE_AKTUALIZACJI%_engb-update-%OZNACZENIE_AKTUALIZACJI%jsonstringstransifexcomtxt_pl.txt\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaPlikukeysTransifexCOMTXT\": \"\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaPlikustringsTransifexCOMTXT\": \"\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaPlikuAktualizacjikeysTransifexCOMTXT\": \"plPL-update-%OZNACZENIE_AKTUALIZACJI%.json.keysTransifexCOM.txt\",");
                plikCFGdomyslny_sw.WriteLine("      \"domyslnaNazwaWygenerowanegoPlikuLokalizacjiJSON\": \"deDE.json\",");



                plikCFGdomyslny_sw.WriteLine("      \"__domyslnyPlikKonfiguracyjny\": \"1\"");

                plikCFGdomyslny_sw.WriteLine("}");
                plikCFGdomyslny_sw.Close();
                plikCFGdomyslny_fs.Close();

            }
        }

        public static JObject? WczytajDaneKonfiguracyjne()
        {

            if (File.Exists("cfg.json") == true)
            {
                return JSON.NET8.WczytajDaneZPlikuJSONdoJObject("cfg.json");
            }
            else
            {
                return null;
            }

        }

    }


}
