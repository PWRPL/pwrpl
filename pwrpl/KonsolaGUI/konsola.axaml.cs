using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using pwrpl;

namespace pwrpl.KonsolaGUI;


public partial class Console : UserControl
{
    public Console()
    {
        InitializeComponent();
        
        instancja = this;
        
    }

    public static Console? instancja { get; set; }
    
    public static string? komenda_tmp = null;


    
    private static readonly int konsola_buforlinii = 500; //ustawienie bardzo dużego bufora spowolni działanie narzędzi

    public static bool konsola_zablokowaneczyszczeniebufora;
    
    
    public static int konsola_aktualnailoscliniiwbuforze
    {
        get { return _konsola_aktualnailoscliniiwbuforze; }
        set
        {
            _konsola_aktualnailoscliniiwbuforze = value;
            if (konsola_zablokowaneczyszczeniebufora == false)
            {
                if (_konsola_aktualnailoscliniiwbuforze > konsola_buforlinii)
                {
                    BlokadaCzyszczeniaBufora_WLlubWYL(true);
                    CzyszczenieBufora();
                }
            }
            else
            {
                if (_konsola_aktualnailoscliniiwbuforze > konsola_buforlinii * 2)
                {
                    BlokadaCzyszczeniaBufora_WLlubWYL(false);
                }
            }
        }
    }
    private static int _konsola_aktualnailoscliniiwbuforze = 0;

    public static Thread? pwrpltools_osobnywatek;
    public static Thread? pwrplconverter_osobnywatek;
    
    //public static List<Narzedzie> uruchomione_narzedzia = new List<Narzedzie>();
    
    private static void BlokadaCzyszczeniaBufora_WLlubWYL(bool truebywlaczyc_lub_falsebywylaczyc)
    {
        konsola_zablokowaneczyszczeniebufora = truebywlaczyc_lub_falsebywylaczyc;
    }
    
    public static void Inicjalizacja()
    {
        
        Dispatcher.UIThread.Post(() =>
        {
            Console.WriteLine("Inicjalizacja wbudowanej konsoli powiodła się...");
        });
        
    }

    private void konsola_button_uruchompwrpltools_Click(object? sender, RoutedEventArgs e)
    {
        if (pwrplconverter_osobnywatek == null || pwrplconverter_osobnywatek.IsAlive == false)
        {
            ZainicjalizujWatekDla_pwrpltools();
        }
        else
        {
            Console.WriteLine("pwrpl-tools jest już uruchomiony...");
        }
    }

    
    private void konsola_button_uruchompwrplconverter_Click(object? sender, RoutedEventArgs e)
    {
        if (pwrplconverter_osobnywatek == null || pwrplconverter_osobnywatek.IsAlive == false)
        {
            ZainicjalizujWatekDla_pwrplconverter();
        }
        else
        {
            Console.WriteLine("pwrpl-converter jest już uruchomiony...");
        }
    }

    private static void ZainicjalizujWatekDla_pwrpltools()
    {
        pwrpltools_osobnywatek = new Thread(Uruchom_pwrpltools);
        pwrpltools_osobnywatek.Start();
        
        PanelWprowadzania_Wyswietlanie(true);
    }

    private static void ZainicjalizujWatekDla_pwrplconverter()
    {
        pwrplconverter_osobnywatek = new Thread(Uruchom_pwrplconverter);
        pwrplconverter_osobnywatek.Start();
        
        PanelWprowadzania_Wyswietlanie(true);
    }

    private static void Uruchom_pwrpltools()
    {
        pwrpl_tools.pwrpltools.Main3(new string[0]);
        
    }
    private static void Uruchom_pwrplconverter()
    {
        pwrpl_converter.pwrpl_converter.Main2(new string[0]);
    }

    public static void ZamkniecieNarzedzi()
    {
        if (pwrpltools_osobnywatek != null) { if (pwrpltools_osobnywatek.IsAlive == true) { pwrpltools_osobnywatek.Interrupt(); } }
        if (pwrplconverter_osobnywatek != null) { if (pwrplconverter_osobnywatek.IsAlive == true) { pwrplconverter_osobnywatek.Interrupt(); } }
    }

    private static void PanelWprowadzania_Wyswietlanie(bool czy_wyswietlac)
    {
        if (czy_wyswietlac == true)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (instancja != null)
                {
                    instancja.konsola_button_uruchompwrpltools.IsVisible = false;
                    instancja.konsola_button_uruchompwrplconverter.IsVisible = false;
                    instancja.konsola_in.IsVisible = true;
                }
            });
        }
        else
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (instancja != null)
                {
                    instancja.konsola_in.IsVisible = false;
                    instancja.konsola_button_uruchompwrpltools.IsVisible = true;
                    instancja.konsola_button_uruchompwrplconverter.IsVisible = true;
                }
            });
        }
    }

    public static void Konsola_ZaktualizujTresc(string tresc)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (instancja != null)
            {
                if (instancja.konsola_out != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(instancja.konsola_out.Text);
                    sb.Append(tresc);
                    instancja.konsola_out.Text = sb.ToString();
                
                    Konsola_PrzewinNaSamDol();
                }
            }
        });
    }

    public static void Konsola_PrzewinNaSamaGore()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (instancja != null)
            {
                if (instancja.konsola_out != null)
                {
                    instancja.konsola_out.CaretIndex = int.MinValue;
                }
                else
                {
                    #if DEBUG
                        System.Console.WriteLine("instancja.konsola_out==null");
                    #endif
                }
            }
            else
            {
                #if DEBUG
                    System.Console.WriteLine("instancja==null");
                #endif
            }
        });
    }
    public static void Konsola_PrzewinNaSamDol()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (instancja != null)
            {
                if (instancja.konsola_out != null)
                {
                    instancja.konsola_out.CaretIndex = int.MaxValue;
                }
                else
                {
#if DEBUG
                    System.Console.WriteLine("instancja.konsola_out==null");
#endif
                }
            }
            else
            {
#if DEBUG
                System.Console.WriteLine("instancja==null");
#endif
            }
        });
    }

    
    private void konsola_in_wykrycieklawisza(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
                var _sender = sender as TextBox;
            
                //Console.WriteLine($"[DEBUG] Kliknięto ENTER w: konsola_in. sender=={sender}, e=={e}, _sender.Text=={_sender.Text}");

                Dispatcher.UIThread.Post(() =>
                {
                    komenda_tmp = _sender.Text;

                    _sender.Text = "";
                });
        }
    }

    
    public static void Write(string tresc)
    {
        Konsola_ZaktualizujTresc(tresc);
        konsola_aktualnailoscliniiwbuforze++;
    }
    
    public static void WriteLine(string tresc)
    {
        if (tresc.Contains("Kliknij ENTER aby zakończyć") == false)
        {
            Konsola_ZaktualizujTresc(tresc + "\n");
            konsola_aktualnailoscliniiwbuforze++;
        }
    }
    
    
    public static ConsoleColor BackgroundColor()
    {
        return ConsoleColor.Black;
    }
    
    
    public static void ResetColor()
    {
        
    }

    public static void ReadKey()
    {
        WriteLine("Narzędzie zakończyło pracę...");
        
        PanelWprowadzania_Wyswietlanie(false);
    }
    
    public static string ReadLine()
    {
        
        string tresc_wprowadzonej_komendy = "";
        
        Sprawdz_komenda_tmp:
        if (komenda_tmp == null)
        {
            Thread.Sleep(200);
            goto Sprawdz_komenda_tmp;
        }
        else
        {
            tresc_wprowadzonej_komendy = komenda_tmp;
            komenda_tmp = null;
        }
        
        Write(tresc_wprowadzonej_komendy + "\n");
        return tresc_wprowadzonej_komendy;
        
        
    }

    public static void CzyszczenieBufora()
    {

        //Dispatcher.UIThread.Post(() => { pwrpl.MainWindow.Konsola_ZaktualizujTresc($"[DEBUG] ? {konsola_aktualnailoscliniiwbuforze} > {konsola_buforlinii}\n"); });

       Dispatcher.UIThread.Post(() =>
       {
           if (instancja != null)
           {
               instancja.konsola_out.Text = "";
               konsola_aktualnailoscliniiwbuforze = 0;
           }
       });
       WriteLine($"[Wcześniejsze wpisy w konsoli zostały wyczyszczone, ponieważ ich ilość przekroczyła bufor {konsola_buforlinii} linii.]");

    }
    
    
    
    public enum ConsoleColor
    {
        /// <summary>The color black.</summary>
        Black,
        /// <summary>The color dark blue.</summary>
        DarkBlue,
        /// <summary>The color dark green.</summary>
        DarkGreen,
        /// <summary>The color dark cyan (dark blue-green).</summary>
        DarkCyan,
        /// <summary>The color dark red.</summary>
        DarkRed,
        /// <summary>The color dark magenta (dark purplish-red).</summary>
        DarkMagenta,
        /// <summary>The color dark yellow (ochre).</summary>
        DarkYellow,
        /// <summary>The color gray.</summary>
        Gray,
        /// <summary>The color dark gray.</summary>
        DarkGray,
        /// <summary>The color blue.</summary>
        Blue,
        /// <summary>The color green.</summary>
        Green,
        /// <summary>The color cyan (blue-green).</summary>
        Cyan,
        /// <summary>The color red.</summary>
        Red,
        /// <summary>The color magenta (purplish-red).</summary>
        Magenta,
        /// <summary>The color yellow.</summary>
        Yellow,
        /// <summary>The color white.</summary>
        White,
    }
    
}