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
    public static void UsuwanieZnakowPL()
    {
        string nazwaplikuzrodlowego;

        if (cfg["autoWprowadzanieNazwPlikowWejsciowych"].ToString() == "1")
        {
            nazwaplikuzrodlowego = cfg["domyslnaNazwaWygenerowanegoPlikuLokalizacjiJSON"].ToString();
        }
        else
        {
            Console.Write("Podaj nazwę pliku: ");
            nazwaplikuzrodlowego = Console.ReadLine();
        }

        Console.WriteLine("Podano nazwę pliku: " + nazwaplikuzrodlowego);
        if (File.Exists(nazwaplikuzrodlowego))
        {
            uint plikzrodlowy_liczbalinii = PoliczLiczbeLinii(nazwaplikuzrodlowego);

            if (wl_pasekpostepu == true)
            {
                InicjalizacjaPaskaPostepu(Convert.ToInt32(plikzrodlowy_liczbalinii));
            }

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

                        if (wl_pasekpostepu == false)
                        {
                            Console.WriteLine("Trwa zapisywanie linii nr.: " + plikzrodlowy_numerlinii + "/" + plikzrodlowy_liczbalinii + " [" + PoliczPostepWProcentach(plikzrodlowy_numerlinii, plikzrodlowy_liczbalinii) + "%]");
                        }
                        else if (wl_pasekpostepu == true)
                        {
                            pasek_postepu.Refresh(plikzrodlowy_numerlinii, "Trwa zapisywanie linii...");
                        }

                        plikzrodlowy_numerlinii++;

                    }

                    nowyplik_sw.Close();
                    plikzrodlowy_sr.Close();

                    //Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine("Utworzono nowy plik nie zawierajacy polskich znakow: " + "BezZnakowPL_" + nazwaplikuzrodlowego);
                    Console.ResetColor();

                }
                catch
                {
                    //Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("BLAD: Wystapil nieoczekiwany blad w dostepie do plikow.");
                    Console.ResetColor();
                }

                nowyplik_fs.Close();
                plikzrodlowy_fs.Close();

            }
            else
            {
                //Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BLAD: Wystapil problem ze zliczeniem linii w podanym pliku!");
                Console.ResetColor();
            }

        }
        else
        {
            //Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("BLAD: Nie istnieje plik o nazwie \"" + nazwaplikuzrodlowego + "\" w folderze z programem.");
            Console.ResetColor();
        }

        Koniec();

    }

}