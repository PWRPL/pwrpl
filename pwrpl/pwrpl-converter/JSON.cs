using Console = pwrpl.KonsolaGUI.Console;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using System.Xml.Serialization;

using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSON
{
    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }
    }


    public static class Formatowanie
    {
        /// <summary>
        /// Formatuje zserializowany ciąg JSON wstawiając wcięcia, akapity, spacje i nowe linie dla czytelności.
        /// </summary>
        /// <returns>string</returns>
        public static string FormatujJSON(string zseriializowanedaneJSON, string akapit = "  ")
        {
            List<char> wszystkie_znaki = new List<char>();
            for (var i1 = 0; i1 < zseriializowanedaneJSON.Length; i1++)
            {
                wszystkie_znaki.Add(zseriializowanedaneJSON[i1]);
            }


            var wciecie = 0;
            var cytat = false;
            var sb = new StringBuilder();
            for (var i2 = 0; i2 < wszystkie_znaki.Count; i2++)
            {
                var znak = wszystkie_znaki[i2];

                if (znak == '{')
                {
                    sb.Append(znak);
                    if (!cytat)
                    {
                        int i3 = i2 + 1;
                        char kolejny_znak;

                        if (i3 < wszystkie_znaki.Count)
                        {
                            kolejny_znak = wszystkie_znaki[i3];
                        }
                        else
                        {
                            kolejny_znak = '0';
                        }

                        if (kolejny_znak != '}')
                        {
                            sb.AppendLine();

                            Enumerable.Range(0, ++wciecie).ForEach(item => sb.Append(akapit));
                        }
                        else
                        {
                            Enumerable.Range(0, ++wciecie).ForEach(item => sb.Append(""));

                        }
                    }
                }
                else if (znak == '[')
                {
                    sb.Append(znak);
                    if (!cytat)
                    {
                        int i3 = i2 + 1;
                        char kolejny_znak;

                        if (i3 < wszystkie_znaki.Count)
                        {
                            kolejny_znak = wszystkie_znaki[i3];
                        }
                        else
                        {
                            kolejny_znak = '0';
                        }

                        if (kolejny_znak != ']')
                        {
                            sb.AppendLine();

                            Enumerable.Range(0, ++wciecie).ForEach(item => sb.Append(akapit));
                        }
                        else
                        {
                            Enumerable.Range(0, ++wciecie).ForEach(item => sb.Append(""));

                        }
                    }
                }
                else if (znak == '}')
                {
                    if (!cytat)
                    {
                        int i4 = i2 - 1;
                        char poprzedni_znak;

                        if (i4 < wszystkie_znaki.Count)
                        {
                            poprzedni_znak = wszystkie_znaki[i4];
                        }
                        else
                        {
                            poprzedni_znak = '0';
                        }

                        if (poprzedni_znak != '{')
                        {
                            sb.AppendLine();

                            Enumerable.Range(0, --wciecie).ForEach(item => sb.Append(akapit));
                        }
                        else
                        {
                            Enumerable.Range(0, --wciecie).ForEach(item => sb.Append(""));
                        }
                    }
                    sb.Append(znak);
                }
                else if (znak == ']')
                {
                    if (!cytat)
                    {
                        int i4 = i2 - 1;
                        char poprzedni_znak;

                        if (i4 < wszystkie_znaki.Count)
                        {
                            poprzedni_znak = wszystkie_znaki[i4];
                        }
                        else
                        {
                            poprzedni_znak = '0';
                        }

                        if (poprzedni_znak != '[')
                        {
                            sb.AppendLine();

                            Enumerable.Range(0, --wciecie).ForEach(item => sb.Append(akapit));
                        }
                        else
                        {
                            Enumerable.Range(0, --wciecie).ForEach(item => sb.Append(""));
                        }
                    }
                    sb.Append(znak);
                }
                else if (znak == '"')
                {
                    sb.Append(znak);
                    bool escaped = false;
                    var index = i2;
                    while (index > 0 && zseriializowanedaneJSON[--index] == '\\')
                        escaped = !escaped;
                    if (!escaped)
                        cytat = !cytat;
                }
                else if (znak == ',')
                {
                    sb.Append(znak);
                    if (!cytat)
                    {
                        sb.AppendLine();
                        Enumerable.Range(0, wciecie).ForEach(item => sb.Append(akapit));
                    }
                }
                else if (znak == ':')
                {
                    sb.Append(znak);
                    if (!cytat)
                        sb.Append(" ");
                }
                else
                {
                    sb.Append(znak);
                }
            }

            wszystkie_znaki.Clear();

            return sb.ToString();
        }

    }

    public static class NET6
    {
        /// <summary>
        ///     Wczytuje dane z pliku JSON metodą bezpośrednią.
        ///     <para>Nie wymaga deklarowania klas ze schematem danych.</para>
        ///     <para>Używa biblioteki <see href="https://learn.microsoft.com/pl-pl/dotnet/api/system.text.json?view=net-6.0">System.Text.Json</see>.</para>
        /// </summary>
        /// <returns>
        ///     dynamic[]?
        ///     <para>dynamic[0]=lista stalych (gdzie lista stałych[indeks_stalej]=nazwa stałej) i dynamic[1]=lista wartości (gdzie lista wartości[indeks_stalej][indeks_wartosci]=wartość stałej)</para>
        /// </returns>
        public static dynamic[]? WczytajStaleIIchWartosciZPlikuJSON(string sciezka_do_pliku_JSON)
        {
            List<dynamic> lista_stalych = new List<dynamic>();
            List<List<dynamic>> lista_wartosci = new List<List<dynamic>>();


            if (File.Exists(sciezka_do_pliku_JSON) == true)
            {
                ReadOnlySpan<byte> konfiguracja_wczytanyplik = File.ReadAllBytes(sciezka_do_pliku_JSON);

                var options = new JsonReaderOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                };
                var konfiguracja_dane = new Utf8JsonReader(konfiguracja_wczytanyplik, options);

                string? tmp_ostatniodczytanyTokenType = null;
                string? tmp_ostatniaodczytanastala = null;
                int tmp_aktualnyindeksstalej = 0;

                while (konfiguracja_dane.Read())
                {

                    if (konfiguracja_dane.TokenType.ToString() != "StartObject" && konfiguracja_dane.TokenType.ToString() != "EndObject")
                    {
                        lista_wartosci.Add(new List<dynamic>());

                        string aktualny_typ_wartosci = konfiguracja_dane.TokenType.ToString();
                        string aktualna_wartosc = konfiguracja_dane.GetString().ToString();

                        // "PropertyName" to stała
                        // "String" to wartość stałej

                        if (tmp_ostatniodczytanyTokenType == null)
                        {
                            if (aktualny_typ_wartosci == "String")
                            {
                                //Console.WriteLine("[DEBUG] A");

                                //nic nie rób
                            }
                            else if (aktualny_typ_wartosci == "PropertyName")
                            {
                                //Console.WriteLine("[DEBUG] B");

                                tmp_ostatniaodczytanastala = aktualna_wartosc;
                                lista_stalych.Add(aktualna_wartosc);
                            }
                        }
                        else if (tmp_ostatniodczytanyTokenType == "PropertyName")
                        {

                            if (aktualny_typ_wartosci == "String")
                            {
                                //Console.WriteLine("[DEBUG] C");

                                lista_wartosci[tmp_aktualnyindeksstalej].Add(aktualna_wartosc);
                            }
                            else if (aktualny_typ_wartosci == "PropertyName")
                            {
                                //Console.WriteLine("[DEBUG] D");

                                tmp_ostatniaodczytanastala = aktualna_wartosc;
                                tmp_aktualnyindeksstalej++;
                                lista_stalych.Add(aktualna_wartosc);
                            }

                        }
                        else if (tmp_ostatniodczytanyTokenType == "String")
                        {

                            if (aktualny_typ_wartosci == "String")
                            {
                                //Console.WriteLine("[DEBUG] E");

                                lista_wartosci[tmp_aktualnyindeksstalej].Add(aktualna_wartosc);
                            }
                            else if (aktualny_typ_wartosci == "PropertyName")
                            {
                                //Console.WriteLine("[DEBUG] F");

                                tmp_ostatniaodczytanastala = aktualna_wartosc;
                                tmp_aktualnyindeksstalej++;
                                lista_stalych.Add(aktualna_wartosc);
                            }

                        }

                        tmp_ostatniodczytanyTokenType = aktualny_typ_wartosci;


                    }


                }

                dynamic[] tablica_list_danych = { lista_stalych, lista_wartosci };

                return tablica_list_danych;


            }
            else
            {
                return null;
            }





            /*
            for (int i0 = 0; i0 < lista_stalych.Count; i0++)
            {
                Console.WriteLine("[DEBUG] lista_stalych[" + i0 + "]: " + lista_stalych[i0]);
            }

            for (int i1 = 0; i1 < lista_wartosci.Count; i1++)
            {
                //Console.WriteLine("lista_wartosci[" + i1 + "]: " + lista_wartosci[i1]);

                for (int i2 = 0; i2 < lista_wartosci[i1].Count; i2++)
                {
                    Console.WriteLine("[DEBUG] lista_wartosci[" + i1 + "][" + i2 + "]: " + lista_wartosci[i1][i2]);
                }


            }
            */







        }


        /// <summary>
        ///     [DEBUG]
        ///     <para>Wyświetla stałe i wartości wczytane wcześniej z pliku JSON metodą bezpośrednią (tj. WczytajStaleIIchWartosciZPlikuJSON).</para>
        /// </summary>
        public static void WyswietlWszystkieStaleIIchWartosci(List<dynamic> lista_stalych, List<List<dynamic>> lista_wartosci)
        {

            for (int i0 = 0; i0 < lista_stalych.Count; i0++)
            {
                Console.WriteLine("[DEBUG] lista_stalych[" + i0 + "]: " + lista_stalych[i0]);
            }

            for (int i1 = 0; i1 < lista_wartosci.Count; i1++)
            {
                //Console.WriteLine("lista_wartosci[" + i1 + "]: " + lista_wartosci[i1]);

                for (int i2 = 0; i2 < lista_wartosci[i1].Count; i2++)
                {
                    Console.WriteLine("[DEBUG] lista_wartosci[" + i1 + "][" + i2 + "]: " + lista_wartosci[i1][i2]);
                }


            }

        }


        /// <summary>
        ///     Wczytuje stałe i wartości z pliku JSON, wcześniej go deserializując.
        ///     <para>WYMAGA zadeklarowania klas ze schematem danych.</para>
        ///     <para>(Automatyczny import klas możliwy jako wklejenie treści pliku JSON w VS2022: "Edycja/Wklej specjalne/Wklej dane JSON jako klasy")</para>
        ///     <para>Używa biblioteki <see href="https://learn.microsoft.com/pl-pl/dotnet/api/system.text.json?view=net-6.0">System.Text.Json</see>.</para>
        /// </summary>
        /// <returns>dynamic?</returns>
        public static dynamic? WczytajStaleIIchWartosciZPlikuJSON<Nazwa_klasy>(string sciezka_do_pliku_JSON)
        {

            if (File.Exists(sciezka_do_pliku_JSON) == true)
            {
                string wczytanyplik_JSON = File.ReadAllText(sciezka_do_pliku_JSON);
                dynamic? dane = System.Text.Json.JsonSerializer.Deserialize<Nazwa_klasy>(wczytanyplik_JSON);

                return dane;

            }
            else
            {
                return null;
            }

        }


        /// <summary>
        ///     <para>Wczytuje dane z pliku JSON do JObcject bez konieczności deserializowania.</para>
        ///     <para>Nie wymaga deklarowania klas ze schematem danych oraz umożliwia modyfikowanie wartości kluczy bezpośrednio w JObject.</para>
        ///     <para>Używa biblioteki <see href="https://www.newtonsoft.com/json/help/html/Introduction.htm">Newtonsoft</see>.</para>
        /// </summary>
        /// <returns>JObject?
        ///          <para>Aby odczytać daną wartość należy korzystać z rzutowania typów zmiennych - kilka przykładów:</para>
        ///          <para>a) (string?)dane["klucz1"]["podklucz1"]</para>
        ///          <para>b) (bool?)dane["klucz2"]["podklucz3"][indeks_podpodklucza2]["podpodpodklucz6"]</para>
        ///          <para>c) (int?)dane["klucz9"]</para>
        ///          <para>d) (decimal?)dane[indeks_klucza3][indeks_podklucza8]</para>
        /// </returns>
        public static JObject? WczytajDaneZPlikuJSON(string sciezka_do_pliku_JSON)
        {
            JObject? dane;

            if (File.Exists(sciezka_do_pliku_JSON) == true)
            {
                var plikJSON_tresc = File.ReadAllText(sciezka_do_pliku_JSON);

                dane = JObject.Parse(plikJSON_tresc);
            }
            else
            {
                dane = null;
            }

            return dane;
        }


        /// <summary>
        ///     <para>Serializuje i formatuje dane, a następnie zapisuje je do pliku JSON.</para>
        ///     <para>Używa biblioteki <see href="https://www.newtonsoft.com/json/help/html/Introduction.htm">Newtonsoft</see>.</para>
        /// </summary>
        /// <returns>bool</returns>
        public static bool ZapiszDaneDoPlikuJSON(JObject dane, string sciezka_do_zapisywanego_pliku_JSON)
        {
            bool rezultat = false;

            var plikJSON_tresc = JsonConvert.SerializeObject(dane);

            if (File.Exists(sciezka_do_zapisywanego_pliku_JSON) == true) { File.Delete(sciezka_do_zapisywanego_pliku_JSON); }
            File.WriteAllText(sciezka_do_zapisywanego_pliku_JSON, Formatowanie.FormatujJSON(plikJSON_tresc));

            if (File.Exists(sciezka_do_zapisywanego_pliku_JSON) == true)
            {
                rezultat = true;
            }

            return rezultat;
        }
    }

    public static class NET8
    {
        /// <summary>
        ///     Wczytuje dane z pliku JSON, wcześniej go deserializując.
        ///     <para>WYMAGA zadeklarowania klas ze schematem danych.</para>
        ///     <para>(Automatyczny import klas możliwy jako wklejenie treści pliku JSON w VS2022: "Edycja/Wklej specjalne/Wklej dane JSON jako klasy".)</para>
        ///     <para>Używa biblioteki <see href="https://learn.microsoft.com/pl-pl/dotnet/api/system.text.json?view=net-8.0">System.Text.Json</see>.</para>
        /// </summary>
        /// <returns>dynamic?</returns>
        public static dynamic? WczytajDaneZPlikuJSON_SystemText<Nazwa_klasy>(string sciezka_do_pliku_JSON)
        {
            dynamic? dane;

            if (File.Exists(sciezka_do_pliku_JSON) == true)
            {
                string wczytanyplik_json = File.ReadAllText(sciezka_do_pliku_JSON);

                if (wczytanyplik_json != null)
                {
                    dane = System.Text.Json.JsonSerializer.Deserialize<Nazwa_klasy>(wczytanyplik_json);
                }
                else
                {
                    dane = "NULL";
                    //Console.WriteLine("[DEBUG] BŁĄD: Wczytany plik JSON który wskazano jest pusty.");
                }

            }
            else
            {
                dane = "NULL";
                //Console.WriteLine("[DEBUG] BŁĄD: Plik JSON który wskazano nie istnieje.");
            }

            return dane;

        }
        
        /// <summary>
        ///     Wczytuje dane z pliku JSON, wcześniej go deserializując.
        ///     <para>WYMAGA zadeklarowania klas ze schematem danych.</para>
        ///     <para>(Automatyczny import klas możliwy jako wklejenie treści pliku JSON w VS2022: "Edycja/Wklej specjalne/Wklej dane JSON jako klasy".)</para>
        ///     <para>Używa biblioteki <see href="https://www.newtonsoft.com/json/help/html/Introduction.htm">Newtonsoft</see>.</para>
        /// </summary>
        /// <returns>dynamic?</returns>
        public static dynamic? WczytajDaneZPlikuJSON_Newtonsoft<Nazwa_klasy>(string sciezka_do_pliku_JSON)
        {
            dynamic? dane;

            if (File.Exists(sciezka_do_pliku_JSON) == true)
            {
                string wczytanyplik_json = File.ReadAllText(sciezka_do_pliku_JSON);

                if (wczytanyplik_json != null)
                {
                    dane = Newtonsoft.Json.JsonConvert.DeserializeObject<Nazwa_klasy>(wczytanyplik_json);
                }
                else
                {
                    dane = "NULL";
                    //Console.WriteLine("[DEBUG] BŁĄD: Wczytany plik JSON który wskazano jest pusty.");
                }

            }
            else
            {
                dane = "NULL";
                //Console.WriteLine("[DEBUG] BŁĄD: Plik JSON który wskazano nie istnieje.");
            }

            return dane;

        }


        /// <summary>
        ///     <para>Wczytuje dane z pliku JSON do JObcject bez konieczności deserializowania.</para>
        ///     <para>Nie wymaga deklarowania klas ze schematem danych oraz umożliwia modyfikowanie wartości kluczy bezpośrednio w JObject.</para>
        ///     <para>Używa biblioteki <see href="https://www.newtonsoft.com/json/help/html/Introduction.htm">Newtonsoft</see>.</para>
        /// </summary>
        /// <returns>JObject?
        ///          <para>Aby odczytać daną wartość należy korzystać z rzutowania typów zmiennych - kilka przykładów:</para>
        ///          <para>a) (string?)dane["klucz1"]["podklucz1"]</para>
        ///          <para>b) (bool?)dane["klucz2"]["podklucz3"][indeks_podpodklucza2]["podpodpodklucz6"]</para>
        ///          <para>c) (int?)dane["klucz9"]</para>
        ///          <para>d) (decimal?)dane[indeks_klucza3][indeks_podklucza8]</para>
        /// </returns>
        public static JObject? WczytajDaneZPlikuJSONdoJObject(string sciezka_do_pliku_JSON)
        {
            JObject? dane;

            if (File.Exists(sciezka_do_pliku_JSON) == true)
            {
                var plikJSON_tresc = File.ReadAllText(sciezka_do_pliku_JSON);

                dane = JObject.Parse(plikJSON_tresc);
            }
            else
            {
                dane = null;
            }

            return dane;
        }


        /// <summary>
        ///     <para>Serializuje i formatuje dane, a następnie zapisuje je do pliku JSON.</para>
        ///     <para>Używa biblioteki <see href="https://www.newtonsoft.com/json/help/html/Introduction.htm">Newtonsoft</see>.</para>
        /// </summary>
        /// <returns>bool</returns>
        public static bool ZapiszDaneZJObjectDoPlikuJSON(JObject dane, string sciezka_do_zapisywanego_pliku_JSON)
        {
            bool rezultat = false;

            var plikJSON_tresc = JsonConvert.SerializeObject(dane);

            if (File.Exists(sciezka_do_zapisywanego_pliku_JSON) == true) { File.Delete(sciezka_do_zapisywanego_pliku_JSON); }
            File.WriteAllText(sciezka_do_zapisywanego_pliku_JSON, Formatowanie.FormatujJSON(plikJSON_tresc));

            if (File.Exists(sciezka_do_zapisywanego_pliku_JSON) == true)
            {
                rezultat = true;
            }

            return rezultat;
        }
    }
    
}
