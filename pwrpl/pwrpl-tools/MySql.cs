using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;



namespace pwrpl_tools
{

    public class MySql
    {
        readonly static string skrypt = "MySql.cs";

        /*------------------------KONFIGURACJA POLACZENIA MYSQL----------------------*/
        readonly static private string dane_polaczenia = "server=localhost;uid=PWRlangTools;pwd=qCz1E3uNRDJHajtB;database=pwrlangtools;Connection Timeout=3000000";
        readonly static private bool info_w_konsoli = false;
        /*-------------------KONIEC KONFIGURACJI POLACZENIA MYSQL--------------------*/

        //public static MySqlConnection MysqlC;


        /* KLASY EKSPORTOWANE - POCZĄTEK */

        public static void Blad(string tresc)
        {
            //Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }

        public static void Mysql_Complex1_test(string zapytanie_mysql = "INSERT INTO `rm_bany` (IP, Nazwa_gracza_SocialClub_w_momencie_nalozenia, Powod) VALUES ('0.0.0.0','test','test_mysql');")
        {
            bool bd_rezultat = MySql_Complex(zapytanie_mysql, "cs", "Mysql_Complex1_test");

            if (bd_rezultat == true)
            {
                Console.WriteLine("Powiodło się wykonanie wysłanego do bazy danych zapytania o treści: " + zapytanie_mysql);
            }
        }


        public static void Mysql_Complex2_test(string zapytanie_mysql = "SELECT * FROM `rm3_modele_pojazdow`;")
        {
            List<List<dynamic>> bd_dane = MySql_ComplexWithResult(zapytanie_mysql, "cs", "Mysql_Complex2_test");

            if (bd_dane != null)
            {
                Console.WriteLine("Poniżej wyświetlą się dane uzyskane z bazy zapytaniem o treści: " + zapytanie_mysql);

                for (int i1 = 0; i1 < bd_dane.Count; i1++)
                {
                    for (int i2 = 0; i2 < bd_dane[i1].Count; i2++)
                    {
                        Console.WriteLine("bd_dane[" + i1 + "][" + i2 + "]: " + bd_dane[i1][i2]);
                    }
                }
            }
        }





        public static void MySql_Test()
        {

            //TEST POLACZENIA I WYKONYWANIA OPERACJI WE WSPOLPRACY Z MYSQL - POCZATEK
            //1. MySql_Query
            {
                string sql1_id = "sql1";
                MySqlConnection sql1 = MySql_Connect(skrypt, sql1_id);
                if (sql1 != null)
                {
                    MySql_Query
                    (
                        sql1,
                        "INSERT INTO `rm_bany`(`IP`, `Nazwa_gracza_SocialClub_w_momencie_nalozenia`, `Powod`) VALUES ('1.1.1.1','test','powod-test');",
                        skrypt,
                        sql1_id
                    );
                }
                MySql_Disconnect(sql1, skrypt, sql1_id);
            }

            //2. MySql_QueryWithResult
            string sql2_id = "sql2";
            MySqlConnection sql2 = MySql_Connect(skrypt, sql2_id);
            if (sql2 != null)
            {
                MySqlDataReader sql2_w = MySql_QueryWithResult
                (
                    sql2,
                    "SELECT * FROM rm_bany;",
                    skrypt,
                    sql2_id
                );

                if (sql2_w != null)
                {
                    while (sql2_w.Read())
                    {
                        string ip = sql2_w["IP"].ToString();
                        string ngSC = sql2_w["Nazwa_gracza_SocialClub_w_momencie_nalozenia"].ToString();
                        string powod = sql2_w["Powod"].ToString();

                        Console.WriteLine(ip + " | " + ngSC + " | " + powod);
                    }

                    sql2_w.Close();
                }
            }
            MySql_Disconnect(sql2, skrypt, sql2_id);

            //3. MySql_Complex
            string zapytanie_testowe3 = "INSERT INTO `rm_bany` (IP, Nazwa_gracza_SocialClub_w_momencie_nalozenia, Powod) VALUES ('0.0.0.0','test','test_mysql');";
            bool bd_rezultat = MySql_Complex(zapytanie_testowe3, "cs", "Mysql_Complex1_test");

            if (bd_rezultat == true)
            {
                Console.WriteLine("Powiodło się wykonanie wysłanego do bazy danych zapytania o treści: " + zapytanie_testowe3);
            }

            //4. MySql_ComplexWithResult
            string zapytanie_testowe4 = "SELECT * FROM `rm_bany;";
            List<List<dynamic>> bd_dane = MySql_ComplexWithResult(zapytanie_testowe4, "cs", "Mysql_Test()");

            if (bd_dane != null)
            {
                Console.WriteLine("Poniżej wyświetlą się dane uzyskane z bazy zapytaniem o treści: " + zapytanie_testowe4);

                for (int i1 = 0; i1 < bd_dane.Count; i1++)
                {
                    for (int i2 = 0; i2 < bd_dane[i1].Count; i2++)
                    {
                        Console.WriteLine("bd_dane[" + i1 + "][" + i2 + "]: " + bd_dane[i1][i2]);
                    }
                }
            }

            //TEST POLACZENIA I WYKONYWANIA OPERACJI WE WSPOLPRACY Z MYSQL - KONIEC

        }

        public static MySqlConnection MySql_Connect(string skrypt_wysylajacy, string id_zdarzenia)
        {
            MySqlConnection polaczenie = new MySqlConnection();
            polaczenie.ConnectionString = dane_polaczenia;

            try
            {
                polaczenie.Open();
                if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Otwarto polaczenie (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }
                return polaczenie;
            }
            catch (MySqlException ex)
            {

                Blad("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");

                return null;
            }
        }

        public static bool MySql_Query(MySqlConnection polaczenie, string zapytanie, string skrypt_wysylajacy, string id_zdarzenia)
        {
            if (polaczenie != null)
            {
                try
                {
                    MySqlCommand komenda = new MySqlCommand(zapytanie, polaczenie);
                    komenda.ExecuteNonQuery();

                    if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Wykonano zapytanie o tresci: " + zapytanie + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }
                    return true;
                }
                catch (MySqlException ex)
                {
                    Blad("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");

                    return false;
                }
            }
            else
            {
                Blad("[S-MySQL] Blad: Wskazane polaczenie z MySQL nie istnieje (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
                return false;
            }

        }

        public static MySqlDataReader MySql_QueryWithResult(MySqlConnection polaczenie, string zapytanie, string skrypt_wysylajacy, string id_zdarzenia)
        {
            if (polaczenie != null)
            {
                try
                {
                    MySqlCommand komenda = new MySqlCommand(zapytanie, polaczenie);
                    MySqlDataReader rezultat = komenda.ExecuteReader();

                    if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Wykonano zapytanie o tresci: " + zapytanie + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ") i zwrocono dane pobrane z bazy."); }
                    return rezultat;
                }
                catch (MySqlException ex)
                {

                    Blad("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");

                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        public static bool MySql_Disconnect(MySqlConnection polaczenie, string skrypt_wysylajacy, string id_zdarzenia)
        {
            try
            {
                polaczenie.Close();
                if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Zamknieto polaczenie (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }
                return true;
            }
            catch
            {
                Blad("[S-MySQL] Blad: Nie zamknieto wskazanego polaczenia (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
                return false;
            }
        }

        public static bool MySql_Complex(string zapytanie, string skrypt_wysylajacy, string id_zdarzenia)
        {

            /* Mysql Connect - POCZĄTEK */
            MySqlConnection polaczenie = new MySqlConnection();
            polaczenie.ConnectionString = dane_polaczenia;

            try
            {
                polaczenie.Open();
                if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Otwarto polaczenie (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }


                {
                    /* Mysql Query - POCZĄTEK */
                    if (polaczenie != null)
                    {
                        try
                        {
                            MySqlCommand komenda = new MySqlCommand(zapytanie, polaczenie);
                            komenda.ExecuteNonQuery();

                            if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Wykonano zapytanie o tresci: " + zapytanie + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }
                            return true;
                        }
                        catch (MySqlException ex)
                        {
                            Blad("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    /* Mysql Query With Result - KONIEC */


                }



            }
            catch (MySqlException ex)
            {
                Console.WriteLine("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
            }

            /* Mysql Connect - KONIEC */


            /* Mysql Disconnect - POCZĄTEK */
            try
            {
                polaczenie.Close();
                if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Zamknieto polaczenie (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }
            }
            catch
            {
                Console.WriteLine("[S-MySQL] Blad: Nie zamknieto wskazanego polaczenia (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
            }
            /* Mysql Disconnect - KONIEC */



            return true;

        }

        public static bool MySql_ComplexCommonConnection(MySqlConnection polaczenie, string zapytanie, string skrypt_wysylajacy, string id_zdarzenia)
        {


            try
            {


                {
                    /* Mysql Query - POCZĄTEK */
                    if (polaczenie != null)
                    {
                        try
                        {
                            MySqlCommand komenda = new MySqlCommand(zapytanie, polaczenie);
                            komenda.ExecuteNonQuery();

                            if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Wykonano zapytanie o tresci: " + zapytanie + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }
                            return true;
                        }
                        catch (MySqlException ex)
                        {
                            Blad("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    /* Mysql Query With Result - KONIEC */


                }



            }
            catch (MySqlException ex)
            {
                Console.WriteLine("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
            }



            return true;

        }

        public static List<List<dynamic>> MySql_ComplexWithResult(string zapytanie, string skrypt_wysylajacy, string id_zdarzenia)
        {
            List<List<dynamic>> rezultat_danych_mysql = new List<List<dynamic>>();

            /* Mysql Connect - POCZĄTEK */
            MySqlConnection polaczenie = new MySqlConnection();
            polaczenie.ConnectionString = dane_polaczenia;

            try
            {
                polaczenie.Open();
                if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Otwarto polaczenie (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }


                {
                    /* Mysql Query With Result - POCZĄTEK */
                    if (polaczenie != null)
                    {
                        try
                        {
                            MySqlCommand komenda = new MySqlCommand(zapytanie, polaczenie);
                            MySqlDataReader rezultat = komenda.ExecuteReader();

                            if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Wykonano zapytanie o tresci: " + zapytanie + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ") i zwrocono dane pobrane z bazy."); }
                            //return rezultat;

                            //Console.WriteLine("Mysql_ComplexWithResult (test1): " + rezultat.FieldCount);


                            if (rezultat.HasRows == true)
                            {
                                int indeks_wiersza = 0;
                                while (rezultat.Read())
                                {
                                    rezultat_danych_mysql.Add(new List<dynamic>());

                                    for (int indeks_kolumny = 0; indeks_kolumny < rezultat.FieldCount; indeks_kolumny++)
                                    {
                                        //Console.WriteLine("[" + indeks_wiersza + "][" + indeks_kolumny + "]: " + rezultat[indeks_kolumny]);

                                        rezultat_danych_mysql[indeks_wiersza].Add(rezultat[indeks_kolumny].ToString());
                                    }


                                    indeks_wiersza++;
                                }

                                rezultat.Close();
                            }


                        }
                        catch (MySqlException ex)
                        {

                            Blad("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
                            rezultat_danych_mysql = null;
                        }
                    }
                    else
                    {
                        rezultat_danych_mysql = null;
                    }
                    /* Mysql Query With Result - KONIEC */


                }


                /* Mysql Disconnect - POCZĄTEK */
                try
                {
                    polaczenie.Close();
                    if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Zamknieto polaczenie (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }
                }
                catch
                {
                    Blad("[S-MySQL] Blad: Nie zamknieto wskazanego polaczenia (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
                }
                /* Mysql Disconnect - KONIEC */



            }
            catch (MySqlException ex)
            {
                Blad("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
            }
            /* Mysql Connect - KONIEC */

            return rezultat_danych_mysql;

        }

        public static int MySql_CountCommonConnection(MySqlConnection polaczenie, string zapytanie, string skrypt_wysylajacy, string id_zdarzenia)
        {
            int rezultat = -1;

            try
            {


                {
                    /* Mysql Query - POCZĄTEK */
                    if (polaczenie != null)
                    {
                        try
                        {
                            MySqlCommand komenda = new MySqlCommand(zapytanie, polaczenie);
                            rezultat = int.Parse(komenda.ExecuteScalar().ToString());

                            if (info_w_konsoli) { Console.WriteLine("[S-MySQL] Wykonano zapytanie o tresci: " + zapytanie + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")"); }

                        }
                        catch (MySqlException ex)
                        {
                            Blad("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
                        }
                    }
                    /* Mysql Query With Result - KONIEC */


                }



            }
            catch (MySqlException ex)
            {
                Console.WriteLine("[S-MySQL] Blad: " + ex.Message + " (skrypt wysylajacy: " + skrypt_wysylajacy + ", ID zdarzenia: " + id_zdarzenia + ")");
            }



            return rezultat;

        }

    }
    /* KLASY EKSPORTOWANE - KONIEC */








}


