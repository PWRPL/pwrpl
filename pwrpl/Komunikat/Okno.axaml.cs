using System;
using System.Runtime.Loader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace pwrpl.Komunikat;

public partial class Okno : Window
{
    public static Okno? okno_wiadomosc;
    
    public Okno()
    {
        InitializeComponent();
    }

    public static class Domyslny
    {
        public static readonly string FontFamily = "font_pwrpl";
        public static readonly double FontSize = 18;
        public static readonly double Width = 600;
        public static readonly double Height = 250;

    }
    
    
    public delegate void ButtonClick_Delegat();
    
    public static void Otworz(string tytul_okna, string tresc_wiadomosci)
    {
        Dispatcher.UIThread.Post(() => 
        {
            okno_wiadomosc = new Okno();
        
            okno_wiadomosc.Title = tytul_okna;
            okno_wiadomosc.Width = Domyslny.Width * MainWindow.skalowanie_mnoznikskali_aktualny;
            okno_wiadomosc.Height = Domyslny.Height * MainWindow.skalowanie_mnoznikskali_aktualny;
            okno_wiadomosc.tresc_wiadomosci.Text = tresc_wiadomosci;
            okno_wiadomosc.tresc_wiadomosci.FontFamily = Domyslny.FontFamily;
            okno_wiadomosc.tresc_wiadomosci.FontSize = Domyslny.FontSize * MainWindow.skalowanie_mnoznikskali_aktualny;

            okno_wiadomosc.okno_przycisk_OK.FontFamily = Domyslny.FontFamily;
            okno_wiadomosc.okno_przycisk_OK.FontSize = Domyslny.FontSize * MainWindow.skalowanie_mnoznikskali_aktualny;
            okno_wiadomosc.okno_przycisk_OK.Click += Okno_przycisk_OK_OnClick;
            okno_wiadomosc.okno_przycisk_OK.IsVisible = true;
        
            okno_wiadomosc.Show();
        });
    }
    
    public static void Otworz(string tytul_okna, string tresc_wiadomosci, ButtonClick_Delegat PrzyciskNie_OnClick_nazwawywolywanejmetody, ButtonClick_Delegat PrzyciskTak_OnClick_nazwawywolywanejmetody)
    {
        Dispatcher.UIThread.Post(() => 
        {
            okno_wiadomosc = new Okno();
        
            okno_wiadomosc.Title = tytul_okna;
            okno_wiadomosc.Width = Domyslny.Width * MainWindow.skalowanie_mnoznikskali_aktualny;
            okno_wiadomosc.Height = Domyslny.Height * MainWindow.skalowanie_mnoznikskali_aktualny;
            okno_wiadomosc.tresc_wiadomosci.Text = tresc_wiadomosci;
            okno_wiadomosc.tresc_wiadomosci.FontFamily = Domyslny.FontFamily;
            okno_wiadomosc.tresc_wiadomosci.FontSize = Domyslny.FontSize * MainWindow.skalowanie_mnoznikskali_aktualny;

            okno_wiadomosc.okno_przycisk_Nie.FontFamily = Domyslny.FontFamily;
            okno_wiadomosc.okno_przycisk_Nie.FontSize = Domyslny.FontSize * MainWindow.skalowanie_mnoznikskali_aktualny;
            okno_wiadomosc.okno_przycisk_Nie.Click += (sender, e) => PrzyciskNie_OnClick_nazwawywolywanejmetody();
            okno_wiadomosc.okno_przycisk_Nie.IsVisible = true;
            
            okno_wiadomosc.okno_przycisk_Tak.FontFamily = Domyslny.FontFamily;
            okno_wiadomosc.okno_przycisk_Tak.FontSize = Domyslny.FontSize * MainWindow.skalowanie_mnoznikskali_aktualny;
            okno_wiadomosc.okno_przycisk_Tak.Click += (sender, e) =>
            {
                Zamknij();
                PrzyciskTak_OnClick_nazwawywolywanejmetody();
            };
            okno_wiadomosc.okno_przycisk_Tak.IsVisible = true;
        
            okno_wiadomosc.Show();
        });
    }

    public static void Zamknij()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (okno_wiadomosc != null)
            {
                okno_wiadomosc.Close();
            }
        });
    }
    
    public static void Okno_przycisk_OK_OnClick(object? sender, RoutedEventArgs e)
    {
        Zamknij();
    }

}