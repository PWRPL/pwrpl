using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace pwrpl.EdytorTresci;

public partial class UsuwanieZmiennych : Window
{
    private UsuwanieZmiennych? okno_edytora { get; set; }

    public UsuwanieZmiennych()
    {
        InitializeComponent();
        
        Title = $"{MainWindow._PWR_nazwaprogramu} {MainWindow._PWR_wersjaprogramu} by Revok ({MainWindow._PWR_rokwydaniawersji}) / Edytor treści / Usuwanie zmiennych";
        edytor_input.Text = "";

        okno_edytora = this; 

    }

    private List<string> listausunietychzmiennych_tmp = new List<string>();
        
    
    

    private string ZTresci(string tresc)
    {

        tresc = PodmianaTekstuWTresci(tresc, @"<br>|<b>|</b>|\{g.*?\}|{/g}", "");
        
        string rezultat = tresc;
        return rezultat;
    }

    private string PodmianaTekstuWTresci(string tresc, string wzorzectekstudopodmiany, string podmiana_na)
    {
        Regex dopasowania_regex = new Regex(wzorzectekstudopodmiany);
        foreach (Match match in Regex.Matches(tresc, wzorzectekstudopodmiany))
        {
            listausunietychzmiennych_tmp.Add(match.Value);
        }        
        return Regex.Replace(tresc, wzorzectekstudopodmiany, podmiana_na);
    }

    private string PobierzTresc_z_TextBox_edytor_input()
    {
        string rezultat = "";
        
        if (this.okno_edytora != null)
        {
            Dispatcher.UIThread.InvokeAsync(() => { rezultat = okno_edytora.edytor_input.Text; }).Wait();
        }
        
        return rezultat;
    }
    
    private void AktualizujTresc_w_TextBox_edytor_output(string nowa_tresc)
    {
        if (okno_edytora != null)
        {
            Dispatcher.UIThread.Post(() =>
            {
                okno_edytora.edytor_output.Text = nowa_tresc;
            });
        }
    }


    private void AktualizujTresc_w_TextBox_edytor_listausunietychzmiennych()
    {
        if (okno_edytora != null)
        {
            string listausunietychzmiennych_string = "";
            for (int i = 0; i < listausunietychzmiennych_tmp.Count; i++)
            {
                string nowalinia = "\n";
                if (i == 0) { nowalinia = ""; }
                
                listausunietychzmiennych_string = listausunietychzmiennych_string + nowalinia + listausunietychzmiennych_tmp[i];
            }
            
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                okno_edytora.edytor_usunietezmienne.Text = listausunietychzmiennych_string;
            }).Wait();
        }
        
        listausunietychzmiennych_tmp.Clear();

    }


    private void Wykonaj()
    {
        string input = PobierzTresc_z_TextBox_edytor_input();
        string output = ZTresci(input);;
        
        AktualizujTresc_w_TextBox_edytor_output(output);
        AktualizujTresc_w_TextBox_edytor_listausunietychzmiennych();
    }
    
    

    private async Task WklejTrescZeSchowkaNaWejscie()
    {
        if (this.okno_edytora != null)
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
    
    

    private async Task SkopiujTrescZWyjsciaDoSchowka()
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
    
    private async void AutomatycznieZaktualizujTrescSchowkaTresciaZUsunietymiZmiennymi_Button_OnClick(object? sender, RoutedEventArgs e)
    {
        #if DEBUG
            System.Console.WriteLine("[DEBUG] Wejście w: AutomatycznieZaktualizujTrescSchowkaTresciaZUsunietymiZmiennymi_Button_OnClick");
        #endif
        
        await WklejTrescZeSchowkaNaWejscie();
        Wykonaj();
        await SkopiujTrescZWyjsciaDoSchowka();

    }


    private async void WklejTrescZeSchowkaNaWejscie_Button_OnClick(object? sender, RoutedEventArgs e)
    {
        WklejTrescZeSchowkaNaWejscie();
    }

    private async void SkopiujTrescZWyjsciaDoSchowka_Button_OnClick(object? sender, RoutedEventArgs e)
    {
        SkopiujTrescZWyjsciaDoSchowka();
    }

    
    private void Edytor_input_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Wykonaj();
    }

}