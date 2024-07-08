using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace pwrpl_converter;

public class API
{
    
    private static char sc = pwrpl_converter.sc;
    private static string folderglownyprogramu = pwrpl_converter.folderglownyprogramu;
    private static readonly string folderAPI_path = $"{folderglownyprogramu}{sc}API";
    
    public static string WygenerujTokenZdarzenia(int length)
    {
        Random losuj = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        return new string(Enumerable.Repeat(chars, length).Select(s => s[losuj.Next(s.Length)]).ToArray());
    }
    public static void UtworzZdarzenie(string skrypt_wysylajacy, string token_zdarzenia, string tresc_zdarzenia)
    {
        if (Directory.Exists(folderAPI_path) == false) { Directory.CreateDirectory(folderAPI_path); }
        
        string plikzdarzenia_path = $"{folderAPI_path}{sc}{skrypt_wysylajacy}-{token_zdarzenia}.event.tmp";
            
        if (File.Exists(plikzdarzenia_path) == true) { File.Delete(plikzdarzenia_path); }
        
        FileStream plikzdarzenia_fs = new FileStream(plikzdarzenia_path, FileMode.CreateNew, FileAccess.Write);

        try
        {
            StreamWriter plikzdarzenia_sw = new StreamWriter(plikzdarzenia_fs);
            plikzdarzenia_sw.WriteLine(tresc_zdarzenia);
            plikzdarzenia_sw.Close();
        }
        catch
        {
            pwrpl_converter.Blad($"Błąd (API): Wystąpił nieoczekiwany problem z dostępem do pliku: {plikzdarzenia_path}");
            throw;
        }
            
        plikzdarzenia_fs.Close();

        if (File.Exists(plikzdarzenia_path.Replace(".tmp", "")) == true) { File.Delete(plikzdarzenia_path.Replace(".tmp", "")); }
        
        if (File.Exists(plikzdarzenia_path) == true) { File.Move(plikzdarzenia_path, plikzdarzenia_path.Replace(".tmp", "")); }
        
    }
}