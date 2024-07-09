using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using JSON;
using Newtonsoft.Json.Linq;
using pwrpl.Identyfikacja;
using pwrpl.Komunikat;

namespace pwrpl;


public partial class MainWindow : Window
{
    public const string _PWR_nazwaprogramu = "pwrpl";
    public const string _PWR_wersjaprogramu = "v.0.4b";
    public const string _PWR_rokwydaniawersji = "2024";

    public static readonly string pwrpl_katalogglownyprogramu = Directory.GetCurrentDirectory();

    public readonly static char sc = System.IO.Path.DirectorySeparatorChar;
    
    public readonly static string cfg_skalowanie_plikpath = APPDATA($"pwrpl{sc}{_PWR_wersjaprogramu}{sc}domyslne{sc}skalowanie.cfg");
    public static double skalowanie_mnoznikskali_aktualny = 1.0d;
    
    public static List<Kontrolka> kontrolki_lista = new List<Kontrolka>();
    
    public static class GUI
    {
        public static Canvas powloka_konsoli { get; set; }
    }
    

    public static Thread? konsolaGUI_osobnywatek;
    
    public MainWindow()
    {
        
        InitializeComponent();
        
        
        //this.Opened += (_, _) => SkalowanieInterfejsu(1.5d);

        
        #if DEBUG
            this.AttachDevTools(); //F12 po uruchomieniu programu wywołuje Avalonia DevTools
            menu_debug.IsVisible = true;
        #endif


        if (File.Exists(cfg_skalowanie_plikpath) == false)
        {
            this.Opened += (_, _) => ZapiszWszystkieKontrolkiDoPlikuJSON();
        }
        
        
        Title = $"{_PWR_nazwaprogramu} {_PWR_wersjaprogramu} by Revok ({_PWR_rokwydaniawersji}) - Zawiera: {pwrpl_tools.pwrpltools._PWR_nazwaprogramu} {pwrpl_tools.pwrpltools._PWR_wersjaprogramu}, {pwrpl_converter.pwrpl_converter._PWR_nazwaprogramu} {pwrpl_converter.pwrpl_converter._PWR_wersjaprogramu}";
        
        GUI.powloka_konsoli = new Canvas();
        //GUI.powloka_konsoli.IsVisible = false;
        
        siatka_glowna.Children.Add(GUI.powloka_konsoli);
        
        
        konsolaGUI_osobnywatek = new Thread(InicjalizacjaKonsoli);
        konsolaGUI_osobnywatek.Start();
        
    }
    

    private static string APPDATA(string sciezka_wewnatrz_APPDATA)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), sciezka_wewnatrz_APPDATA);
    }

    
    private void SkalowanieInterfejsu(double mnoznik_skali)
    {
        skalowanie_mnoznikskali_aktualny = mnoznik_skali;
        
        //pobieranie domyślnych ustawień skalowania (100%) z pliku konfiguracyjnego
        if (File.Exists(cfg_skalowanie_plikpath) == true)
        {
            JObject? domyslne = JSON.NET8.WczytajDaneZPlikuJSONdoJObject(cfg_skalowanie_plikpath);

            if (domyslne != null)
            {
                if (domyslne["Kontrolki"] != null)
                {
                    
                    for (int i1 = 1; i1 < domyslne["Kontrolki"].Count(); i1++)
                    {
                        if (domyslne["Kontrolki"][i1]["FontSize"] == null)
                        {
                            if 
                            (
                                domyslne["Kontrolki"][i1]["Type"] != null
                                &&
                                domyslne["Kontrolki"][i1]["Name"] != null
                                &&
                                domyslne["Kontrolki"][i1]["Width"] != null
                                &&
                                domyslne["Kontrolki"][i1]["Height"] != null
                            )
                            {
                                kontrolki_lista.Add(new Kontrolka
                                {
                                    Type = domyslne["Kontrolki"][i1]["Type"].ToString(),
                                    Name = domyslne["Kontrolki"][i1]["Name"].ToString(),
                                    Width = domyslne["Kontrolki"][i1]["Width"].ToString(),
                                    Height = domyslne["Kontrolki"][i1]["Height"].ToString()
                                });
                            
                                //Console.WriteLine(domyslne["Kontrolki"][i1]["Type"].ToString());
                            }
                        }
                        else
                        {
                            if 
                            (
                                domyslne["Kontrolki"][i1]["Type"] != null
                                &&
                                domyslne["Kontrolki"][i1]["Name"] != null
                                &&
                                domyslne["Kontrolki"][i1]["Width"] != null
                                &&
                                domyslne["Kontrolki"][i1]["Height"] != null
                                &&
                                domyslne["Kontrolki"][i1]["FontSize"] != null && double.Parse(domyslne["Kontrolki"][i1]["FontSize"].ToString()) != 0
                            )
                            {
                                kontrolki_lista.Add(new Kontrolka
                                {
                                    Type = domyslne["Kontrolki"][i1]["Type"].ToString(),
                                    Name = domyslne["Kontrolki"][i1]["Name"].ToString(),
                                    Width = domyslne["Kontrolki"][i1]["Width"].ToString(),
                                    Height = domyslne["Kontrolki"][i1]["Height"].ToString(),
                                    FontSize = domyslne["Kontrolki"][i1]["FontSize"].ToString()
                                });
                            
                                //Console.WriteLine(domyslne["Kontrolki"][i1]["Type"].ToString());
                            }
                            
                        }
                    }

                    /*
                    foreach (Kontrolka test in kontrolki_lista)
                    {
                        Console.WriteLine(test.Type);
                    }
                    */
                }
            }

            //Console.WriteLine($"kontrolki_lista.Count={kontrolki_lista.Count}");
            
            if (kontrolki_lista.Count != 0)
            {
                var wszystkie_kontrolki = this.PobierzWszystkieIstniejaceKontrolki();
                //int t2 = 0;
                foreach (var kontrolka in wszystkie_kontrolki)
                {

                    //Console.WriteLine(kontrolka.GetType().ToString());

                    string[] typkontrolki_tmp_split = kontrolka.GetType().ToString().Split('.');
                    string typ_kontrolki = typkontrolki_tmp_split[typkontrolki_tmp_split.Length - 1];
                    
                    Kontrolka wyszukiwana_kontrolka = kontrolki_lista.Find(x => x.Type == typ_kontrolki && x.Name != null && x.Name == kontrolka.Name);
                    
                    
                    if
                    (
                        wyszukiwana_kontrolka != null
                        &&
                        wyszukiwana_kontrolka.Width != "NaN"
                        &&
                        wyszukiwana_kontrolka.Height != "NaN"
                    )
                    {
                        //Console.WriteLine($"Znaleziono domyślne ustawienia skalowania kontrolki \"{kontrolka.Name}\", które można użyć.");
                        
                        
                        kontrolka.Width = double.Parse(wyszukiwana_kontrolka.Width) * mnoznik_skali;
                        kontrolka.Height = double.Parse(wyszukiwana_kontrolka.Height) * mnoznik_skali;

                        
                        if (wyszukiwana_kontrolka.FontSize != null && double.Parse(wyszukiwana_kontrolka.FontSize) != 0)
                        {
                            var fontSizeProperty = kontrolka.GetType().GetProperty("FontSize");
                            if (fontSizeProperty != null && fontSizeProperty.PropertyType == typeof(double))
                            {
                                if (fontSizeProperty.GetValue(kontrolka) != null && double.Parse(fontSizeProperty.GetValue(kontrolka).ToString()) != 0)
                                {
                                    double rozmiarczcionki_domyslny = double.Parse(wyszukiwana_kontrolka.FontSize);
                                    double rozmiarczcionki_poprzeskalowaniu = (rozmiarczcionki_domyslny * mnoznik_skali);

                                    //Console.WriteLine($"[DEBUG] Próba ustawienia rozmiaru czcionki w kontrolce \"{kontrolka.Name}\" z {rozmiarczcionki_poprzedni} na {rozmiarczcionki_poprzeskalowaniu}");
                                    
                                    fontSizeProperty.SetValue(kontrolka, rozmiarczcionki_poprzeskalowaniu);
                                }
                            }
                        }
                        
                    }


                    //Console.WriteLine($"[DEBUG] Ustawiono skalę kontrolki \"{kontrolka.GetType().Name}\" na: {mnoznik_skali}");

                    //t2++;

                    //Console.WriteLine($"[DEBUG] Znaleziono domyślną wartość dla kontrolki: {kontrolka.Name}");
                }
            }

        }

        //Console.WriteLine("t2==" + t2);
    }

    private void ZapiszWszystkieKontrolkiDoPlikuJSON()
    {
        var wszystkie_kontrolki = this.PobierzWszystkieIstniejaceKontrolki();

        JArray jArray = new JArray();

        //int t = 0;
        foreach (var kontrolka in wszystkie_kontrolki)
        {
            JObject kontrolkaJson = new JObject();
            kontrolkaJson.Add("Type", kontrolka.GetType().Name);
            kontrolkaJson.Add("Name", kontrolka.Name);
            kontrolkaJson.Add("Width", kontrolka.Width.ToString());
            kontrolkaJson.Add("Height", kontrolka.Height.ToString());
            
            var fontSizeProperty = kontrolka.GetType().GetProperty("FontSize");
            if (fontSizeProperty != null && fontSizeProperty.PropertyType == typeof(double))
            {
                if (fontSizeProperty.GetValue(kontrolka) != null && double.Parse(fontSizeProperty.GetValue(kontrolka).ToString()) != 0)
                {
                    kontrolkaJson.Add("FontSize", fontSizeProperty.GetValue(kontrolka).ToString());
                }
            }


            jArray.Add(kontrolkaJson);
            //t++;
        }
        
        //Console.WriteLine("t==" + t);

        JObject finalObject = new JObject();
        finalObject.Add("Kontrolki", jArray);


        string[] cfg_skalowanie_plikpath_split = cfg_skalowanie_plikpath.Split(sc);
        string cfg_skalowanie_folderpath = cfg_skalowanie_plikpath.Replace(cfg_skalowanie_plikpath_split[cfg_skalowanie_plikpath_split.Length - 1], "");

        if (Directory.Exists(cfg_skalowanie_folderpath) == false) { Directory.CreateDirectory(cfg_skalowanie_folderpath); }
        JSON.NET8.ZapiszDaneZJObjectDoPlikuJSON(finalObject, cfg_skalowanie_plikpath);
        
    }

    private void OtworzLinkWPrzegladarceInternetowej(string link)
    {
        if (link != "")
        {
            Process.Start(new ProcessStartInfo { FileName = @link, UseShellExecute = true });
        }
    }

    private void InicjalizacjaKonsoli()
    {
        KonsolaGUI.Console.Inicjalizacja();
    }
    
    
    private void MainWindow_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        #if DEBUG
            if (File.Exists(cfg_skalowanie_plikpath) == true) { File.Delete(cfg_skalowanie_plikpath); }
        #endif

        if (konsolaGUI_osobnywatek != null)
        {
            if (konsolaGUI_osobnywatek.IsAlive == true) { konsolaGUI_osobnywatek.Interrupt(); }
        }
        
        KonsolaGUI.Console.ZamkniecieNarzedzi();
    }


    private void Menu_interfejs_skalowanie_50_OnClick(object? sender, RoutedEventArgs e)
    {
        SkalowanieInterfejsu(0.5d);
        this.UpdateLayout();
        
        Dispatcher.UIThread.Post(() =>
        {
            Console.Konsola_PrzewinNaSamaGore(); //wymagane, aby przewinięcie na sam dół zadziało
            Console.Konsola_PrzewinNaSamDol();
        });
    }
    private void Menu_interfejs_skalowanie_75_OnClick(object? sender, RoutedEventArgs e)
    {
        SkalowanieInterfejsu(0.75d);
        this.UpdateLayout();
        
        Dispatcher.UIThread.Post(() =>
        {
            Console.Konsola_PrzewinNaSamaGore(); //wymagane, aby przewinięcie na sam dół zadziało
            Console.Konsola_PrzewinNaSamDol();
        });
    }

    private void Menu_interfejs_skalowanie_100_OnClick(object? sender, RoutedEventArgs e)
    {
        SkalowanieInterfejsu(1.0d);
        this.UpdateLayout();
        
        Dispatcher.UIThread.Post(() =>
        {
            Console.Konsola_PrzewinNaSamaGore(); //wymagane, aby przewinięcie na sam dół zadziało
            Console.Konsola_PrzewinNaSamDol();
        });
    }

    
    private void Menu_interfejs_skalowanie_125_OnClick(object? sender, RoutedEventArgs e)
    {
        SkalowanieInterfejsu(1.25d);
        this.UpdateLayout();
        
        Dispatcher.UIThread.Post(() =>
        {
            Console.Konsola_PrzewinNaSamaGore(); //wymagane, aby przewinięcie na sam dół zadziało
            Console.Konsola_PrzewinNaSamDol();
        });
    }
    private void Menu_interfejs_skalowanie_150_OnClick(object? sender, RoutedEventArgs e)
    {
        SkalowanieInterfejsu(1.5d);
        this.UpdateLayout();
        
        Dispatcher.UIThread.Post(() =>
        {
            Console.Konsola_PrzewinNaSamaGore(); //wymagane, aby przewinięcie na sam dół zadziało
            Console.Konsola_PrzewinNaSamDol();
        });
    }
    private void Menu_interfejs_skalowanie_175_OnClick(object? sender, RoutedEventArgs e)
    {
        SkalowanieInterfejsu(1.75d);
        this.UpdateLayout();
        
        Dispatcher.UIThread.Post(() =>
        {
            Console.Konsola_PrzewinNaSamaGore(); //wymagane, aby przewinięcie na sam dół zadziało
            Console.Konsola_PrzewinNaSamDol();
        });
    }
    private void Menu_interfejs_skalowanie_200_OnClick(object? sender, RoutedEventArgs e)
    {
        SkalowanieInterfejsu(2.0d);
        this.UpdateLayout();
        
        Dispatcher.UIThread.Post(() =>
        {
            Console.Konsola_PrzewinNaSamaGore(); //wymagane, aby przewinięcie na sam dół zadziało
            Console.Konsola_PrzewinNaSamDol();
        });
    }


    private void Menu_interfejs_motyw_jasny_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        }
    }

    private void Menu_interfejs_motyw_ciemny_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
    }
    
    private void Menu_chmura_narzedziadeweloperskie_OnClick(object? sender, RoutedEventArgs e)
    {
        OtworzLinkWPrzegladarceInternetowej(Chmura.Linki.narzedziadeweloperskie);
    }
    
    private void Menu_chmura_metadane_OnClick(object? sender, RoutedEventArgs e)
    {
        OtworzLinkWPrzegladarceInternetowej(Chmura.Linki.metadane);
    }

    private void Menu_chmura_builderinstalatoraipaczki_OnClick(object? sender, RoutedEventArgs e)
    {
        OtworzLinkWPrzegladarceInternetowej(Chmura.Linki.builderinstalatoraipaczki);
    }

    private void Menu_chmura_polskieczcionki_OnClick(object? sender, RoutedEventArgs e)
    {
        OtworzLinkWPrzegladarceInternetowej(Chmura.Linki.polskieczcionki);
    }

    private void Menu_chmura_oryginalnepliki_OnClick(object? sender, RoutedEventArgs e)
    {
        OtworzLinkWPrzegladarceInternetowej(Chmura.Linki.oryginalnepliki);
    }

    private void Menu_chmura_transifexclientikonfiguracja_OnClick(object? sender, RoutedEventArgs e)
    {
        OtworzLinkWPrzegladarceInternetowej(Chmura.Linki.transifexclientikonfiguracja);
    }

    private void Menu_chmura_winrar_OnClick(object? sender, RoutedEventArgs e)
    {
        OtworzLinkWPrzegladarceInternetowej(Chmura.Linki.winrar);
    }

    private void Menu_chmura_udostepnionepubliczniepolonizacje_OnClick(object? sender, RoutedEventArgs e)
    {
        OtworzLinkWPrzegladarceInternetowej(Chmura.Linki.udostepnionepubliczniepolonizacje);
    }

    private void Menu_wyodrebnijpliki_plikiTXTiJSON_OnClick(object? sender, RoutedEventArgs e)
    {
        #if DEBUG
            Console.WriteLine("[DEBUG] Wejście w: Menu_wyodrebnijpliki_plikiTXTiJSON_OnClick");
        #endif
        
        OknoKomunikatu_wyodrebnijplikiTXTiJSON();
    }
    
    
    private void Menu_wyodrebnijpliki_metadane_OnClick(object? sender, RoutedEventArgs e)
    {
        #if DEBUG
            Console.WriteLine("[DEBUG] Wejście w: Menu_wyodrebnijpliki_metadane_OnClick");
        #endif
        
        OknoKomunikatu_wyodrebnijmetadane();
    }

    
    
    private async void OknoKomunikatu_wyodrebnijplikiTXTiJSON()
    {
        Komunikat.Okno.Otworz("Wyodrębnianie plików", "Pliki .txt i .json (oprócz metadanych) zostaną wyodrębnione z katalogu programu do nowego podkatalogu. Tym samym przestaną być widoczne przez narzędzia.\nCzy chcesz wykonać tę operację?", pwrpl.Komunikat.Okno.Zamknij, pwrpl.WyodrebnianiePlikow.WyodrebnijPlikiTXTiJSON);
    }
    
    private async void OknoKomunikatu_wyodrebnijmetadane()
    {
        Komunikat.Okno.Otworz("Wyodrębnianie metadanych", "Metadane zostaną wyodrębnione z katalogu programu do nowego podkatalogu. Tym samym przestaną być widoczne przez pwrpl-converter.\nCzy chcesz wykonać tę operację?", pwrpl.Komunikat.Okno.Zamknij, pwrpl.WyodrebnianiePlikow.WyodrebnijMetadane);
    }

}

