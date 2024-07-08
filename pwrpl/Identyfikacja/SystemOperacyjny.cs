using System;
using System.IO;
using Microsoft.Win32;

namespace pwrpl.Identyfikacja;

public class SystemOperacyjny
{
    public string Nazwa { get; set; }
    public string WersjaGlowna { get; set; }
    public string Build { get; set; }
    public string Podbuild { get; set; }
    public string ServicePack { get; set; }

    public override string ToString()
    {
        string wyswietlany_string;
        if (Podbuild != "" && Podbuild != " ")
        {
            wyswietlany_string = $"{Nazwa}\n{WersjaGlowna}\n{Build}.{Podbuild}\n{ServicePack}";
        }
        else
        {
            wyswietlany_string = $"{Nazwa}\n{WersjaGlowna}\n{Build}\n{ServicePack}";
        }

        return wyswietlany_string;
    }
    
    public static SystemOperacyjny Wersja()
    {
            /*
            +-----------------------------------------------------------+
            | OS                    | Platform     | Major | Minor | Build    |
            +-----------------------------------------------------------+
            | Windows 95          | Win32Windows |   4   |   0   |          |
            | Windows 98         | Win32Windows |   4   |  10   |          |
            | Windows Me        | Win32Windows |   4   |  90   |          |
            | Windows NT 4.0  | Win32NT      |   4   |   0   |          |
            | Windows 2000    | Win32NT      |   5   |   0   |          |
            | Windows XP      | Win32NT      |   5   |   1   |          |
            | Windows 2003    | Win32NT      |   5   |   2   |          |
            | Windows Vista   | Win32NT      |   6   |   0   |          |
            | Windows 2008    | Win32NT      |   6   |   0   |          |
            | Windows 7       | Win32NT      |   6   |   1   |          |
            | Windows 2008 R2 | Win32NT      |   6   |   1   |          |
            | Windows 8       | Win32NT      |   6   |   2   |          |
            | Windows 8.1     | Win32NT      |   6   |   3   |          |
            | Windows 10      | Win32NT      |  10   |   0   | <22000   |
            | Windows 11      | Win32NT      |  10   |   0   |  22000<= |
            +-----------------------------------------------------------+
            */

            /*
            Console.WriteLine("[DEBUG] Environment.OSVersion==" + Environment.OSVersion);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Platform==" + Environment.OSVersion.Platform);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version==" + Environment.OSVersion.Version);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.Major==" + Environment.OSVersion.Version.Major);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.MajorRevision==" + Environment.OSVersion.Version.MajorRevision);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.Minor==" + Environment.OSVersion.Version.Minor);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.MinorRevision==" + Environment.OSVersion.Version.MinorRevision);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.Revision==" + Environment.OSVersion.Version.Revision);
            Console.WriteLine("[DEBUG] Environment.OSVersion.Version.Build==" + Environment.OSVersion.Version.Build);
            Console.WriteLine("[DEBUG] Environment.OSVersion.ServicePack==" + Environment.OSVersion.ServicePack);
            Console.WriteLine("[DEBUG] Environment.OSVersion.VersionString==" + Environment.OSVersion.VersionString);
            */

            string? glownanazwa_OS;
            string? glowneoznaczeniewersji_OS = "NULL";
            string? numerbuildu_OS = Environment.OSVersion.Version.Build.ToString();
            string? podnumerbuildu_OS = "-1";
            string? servicepack_OS = "";

            string[] pelnynumerwersji_OS = Environment.OSVersion.Version.ToString().Split(new char[] { '.' });


            if (Environment.OSVersion.Platform.ToString() == "Win32NT")
            {
                if (Environment.OSVersion.Version.Major == 4 && Environment.OSVersion.Version.Minor == 0)
                {
                    glowneoznaczeniewersji_OS = "NT 4.0";
                }
                else if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 0)
                {
                    glowneoznaczeniewersji_OS = "2000";
                }
                else if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 1)
                {
                    glowneoznaczeniewersji_OS = "XP";
                }
                else if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 2)
                {
                    glowneoznaczeniewersji_OS = "2003";
                }
                else if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0)
                {
                    glowneoznaczeniewersji_OS = "Vista/2008";
                }
                else if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
                {
                    glowneoznaczeniewersji_OS = "7/2008 R2";
                }
                else if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2)
                {
                    glowneoznaczeniewersji_OS = "8";
                }
                else if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 3)
                {
                    glowneoznaczeniewersji_OS = "8.1";
                }
                else if (Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Minor == 0)
                {
                    if (Environment.OSVersion.Version.Build < 22000)
                    {
                        glowneoznaczeniewersji_OS = "10";
                    }
                    else if (Environment.OSVersion.Version.Build >= 22000)
                    {
                        glowneoznaczeniewersji_OS = "11";
                    }
                }

                servicepack_OS = Environment.OSVersion.ServicePack;
            }
            else if (Environment.OSVersion.Platform.ToString() == "Win32Windows")
            {
                if (Environment.OSVersion.Version.Major == 4 && Environment.OSVersion.Version.Minor == 0)
                {
                    glowneoznaczeniewersji_OS = "95";
                }
                else if (Environment.OSVersion.Version.Major == 4 && Environment.OSVersion.Version.Minor == 10)
                {
                    glowneoznaczeniewersji_OS = "98";
                }
                else if (Environment.OSVersion.Version.Major == 4 && Environment.OSVersion.Version.Minor == 90)
                {
                    glowneoznaczeniewersji_OS = "Me";
                }

                servicepack_OS = Environment.OSVersion.ServicePack;
            }
            else if (Environment.OSVersion.Platform.ToString() == "Unix" || Environment.OSVersion.Platform.ToString() == "Linux")
            {
                if (File.Exists("/etc/os-release") == true)
                {
                    
                    FileStream etcosrelease_fs = new FileStream("/etc/os-release", FileMode.Open, FileAccess.Read);

                    try
                    {
                        StreamReader plik_etcosrelease_sr = new StreamReader(etcosrelease_fs);

                        while (plik_etcosrelease_sr.Peek() != -1)
                        {
                            string tresc_linii = plik_etcosrelease_sr.ReadLine();

                            if (tresc_linii.Contains("NAME") == true && tresc_linii.Contains("PRETTY_NAME") == false && tresc_linii.Contains("CODENAME") == false)
                            {
                                glowneoznaczeniewersji_OS = tresc_linii.Replace("NAME=", "").Replace("\"", "");
                                //Console.WriteLine("[DEBUG] glowneoznaczeniewersji_OS == " + glowneoznaczeniewersji_OS);
                            }
                            else if (tresc_linii.Contains("VERSION_ID") == true)
                            {
                                numerbuildu_OS = tresc_linii.Replace("VERSION_ID=", "").Replace("\"", "");;
                                //Console.WriteLine("[DEBUG] numerbuildu_OS == " + numerbuildu_OS);
                            }
                            else if (tresc_linii.Contains("VERSION_CODENAME") == true)
                            {
                                podnumerbuildu_OS = tresc_linii.Replace("VERSION_CODENAME=", "").Replace("\"", "");;
                                //Console.WriteLine("[DEBUG] podnumerbuildu_OS == " + podnumerbuildu_OS);
                            }
                            else if (tresc_linii.Contains("ID") && (tresc_linii.Contains("VERSION_ID") == false))
                            {
                                servicepack_OS = tresc_linii.Replace("ID=", "").Replace("\"", "");;
                                //Console.WriteLine("[DEBUG] servicepack_OS == " + servicepack_OS);
                            }
                        }
                    }
                    catch
                    {
                        //pusta
                    }
                }
                else
                {
                    glowneoznaczeniewersji_OS = Environment.OSVersion.Platform.ToString();
                    
                    string[] osversion_split = Environment.OSVersion.ToString().Split(" ");
                    if (osversion_split.Length > 1)
                    {
                        numerbuildu_OS = osversion_split[1];
                    }
                    else
                    {
                        numerbuildu_OS = osversion_split[0];
                    }

                    podnumerbuildu_OS = "";
                    servicepack_OS = "";


                }
            }

            if (Environment.OSVersion.ToString().Contains("Microsoft Windows") == true)
            {
                glownanazwa_OS = "Microsoft Windows";

                podnumerbuildu_OS = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("UBR").ToString();

            }
            else if (Environment.OSVersion.ToString().Contains("Unix") == true || Environment.OSVersion.ToString().Contains("Linux"))
            {
                glownanazwa_OS = "Unix";
            }
            else
            {
                glownanazwa_OS = "NULL";
            }

            SystemOperacyjny wersja_OS = new SystemOperacyjny { Nazwa = glownanazwa_OS, WersjaGlowna = glowneoznaczeniewersji_OS, Build = "build " + numerbuildu_OS.ToString(), Podbuild = podnumerbuildu_OS.ToString(), ServicePack = servicepack_OS };

            return wersja_OS;

        }

}
