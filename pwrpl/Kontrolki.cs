using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace pwrpl;

public class Kontrolka
{
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Width { get; set; }
    public string? Height { get; set; }
    
    public string? FontSize { get; set; }
}

public static class Kontrolki
{
    public static IEnumerable<Control> PobierzWszystkieIstniejaceKontrolki(this Control kontrolka)
    {
        if (kontrolka == null)
            throw new ArgumentNullException(nameof(kontrolka));

        var kontrolki = new List<Control> { kontrolka };

        foreach (var child in kontrolka.GetVisualChildren())
        {
            if (child is Control childControl)
            {
                kontrolki.AddRange(childControl.PobierzWszystkieIstniejaceKontrolki());
            }
        }

        return kontrolki;
    }
}