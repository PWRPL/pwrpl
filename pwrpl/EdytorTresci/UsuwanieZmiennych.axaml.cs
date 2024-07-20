using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Text.RegularExpressions;

namespace pwrpl.EdytorTresci;

public partial class UsuwanieZmiennych : Window
{
    public static UsuwanieZmiennych? okno_edytora { get; set; }
    
    public UsuwanieZmiennych()
    {
        InitializeComponent();
    }


    public static void OtworzOkno()
    {
        Dispatcher.UIThread.Post(() =>
        {
            okno_edytora = new UsuwanieZmiennych();
            okno_edytora.Title = $"{MainWindow._PWR_nazwaprogramu} {MainWindow._PWR_wersjaprogramu} by Revok ({MainWindow._PWR_rokwydaniawersji}) / Edytor treści / Usuwanie zmiennych";
            okno_edytora.edytor_input.Text = "";
            okno_edytora.Show();
        });
    }

    private static string ZTresci(string tresc)
    {
        tresc = PodmianaTekstuWTresci(tresc, @"<br>", "");
        tresc = PodmianaTekstuWTresci(tresc, @"\{g.*?\}", "");
        tresc = PodmianaTekstuWTresci(tresc, @"{/g}", "");

        string rezultat = tresc;
        return rezultat;
    }

    private static string PodmianaTekstuWTresci(string tresc, string wzorzectekstudopodmiany, string podmiana_na)
    {
        return Regex.Replace(tresc, wzorzectekstudopodmiany, podmiana_na);
    }

    private static string PobierzTresc_z_TextBox_edytor_input()
    {
        string rezultat = "";
        
        if (okno_edytora != null)
        {
            Dispatcher.UIThread.InvokeAsync(() => { rezultat = okno_edytora.edytor_input.Text; }).Wait();
        }
        
        return rezultat;
    }
    
    private static void AktualizujTresc_w_TextBox_edytor_output(string nowa_tresc)
    {
        if (okno_edytora != null)
        {
            Dispatcher.UIThread.Post(() =>
            {
                okno_edytora.edytor_output.Text = nowa_tresc;
            });
        }
    }


    private static string Wykonaj()
    {
        string input = PobierzTresc_z_TextBox_edytor_input();
        string output = UsuwanieZmiennych.ZTresci(input);;
        
        AktualizujTresc_w_TextBox_edytor_output(output);

        return output;
    }
    
    
    private void Edytor_input_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Wykonaj();
    }

    private async void WklejTrescZeSchowkaNaWejscie_Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (okno_edytora != null)
        {
            var tresc_schowka_IClipboard = this.Clipboard;
            
            string tresc_schowka = await tresc_schowka_IClipboard.GetTextAsync();
            if (tresc_schowka == null)
            {
                tresc_schowka = "";
            }

            this.edytor_input.Text = tresc_schowka;
            
        }
    }

    private async void SkopiujTrescZWyjsciaDoSchowka_Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (okno_edytora != null && okno_edytora.Clipboard != null)
        {
            string output = "";
            
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (okno_edytora.edytor_output.Text != null)
                {
                    output = okno_edytora.edytor_output.Text;
                }
            }).Wait();
            
            Dispatcher.UIThread.Post(() =>
            {
                okno_edytora.Clipboard.SetTextAsync(output);
            });
        }
    }
}