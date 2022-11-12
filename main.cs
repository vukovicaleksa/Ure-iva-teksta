/*
Opis:		Program treba da ima mogućnost pravljenja, čitanja i uređivanja tekstualnih datoteka. Neophodno je realizovati umetanje simbola (znakova) u datoteku, zamenu simbola u datoteci, kao i brisanje simbola. Pored čuvanja izmena, omogućiti korisniku da sačuva datoteku kao neku drugu (Save As…).
*/

using System;
using System.IO;
using System.Threading;

class Program
{
    public struct Objekat
    {
        public char[] a;
    }
    public static string naziv, poslednjeCuvanje, dosadasnji_ulaz;
    public static int trenutni_red;
    public static string pomoc = "strelice - kretanje\nn$ - undo\nr$ - redo\no$ - obrisi simbol\nz$ - zameni simbol\nu$ - umetni simbol\ns$ - save\na$ - save as";
    public static void Main(string[] args)
    {
        trenutni_red = 0;
        poslednjeCuvanje = "Niste cuvali";
        int izborOpcije = IzborOpcija();
        if (izborOpcije == 1)//otvori
        {

        }
        else if (izborOpcije == 2)//napravi
        {
            NoviFajl();
        }
    }
    static string ogranicenUlaz(string pitanje, string[] dozvoljeno, bool obrisi)
    {
        string odgovor = "";
        while (!nizSadrzi(dozvoljeno, odgovor))
        {
            Console.WriteLine(pitanje);
            odgovor = Console.ReadLine().ToUpper();
            if (obrisi)
            {
                Console.Clear();
            }
        }
        return odgovor;
    }
    static bool nizSadrzi(string[] niz, string vrednost)
    {
        foreach (string s in niz) { if (s == vrednost) return true; }
        return false;
    }
    static string crteOkvir(int duzina)
    {
        string i1 = "";
        for (int i = 0; i <= duzina; i++)
        {
            i1 += "-";
        }
        return i1;
    }
    static void ObrisiJedan()
    {
        if (dosadasnji_ulaz.Length > 0)
        {
            dosadasnji_ulaz = dosadasnji_ulaz.Remove(dosadasnji_ulaz.Length - 1).TrimEnd('\n');
            trenutni_red--;
            string[] redovi = dosadasnji_ulaz.Split('\n');
            if (trenutni_red <= 0) trenutni_red = redovi[redovi.Length - 1].Length;
            IzmeniInformacije();
        }
    }
    static void NoviFajl()
    { 
        StreamWriter pisanje = new StreamWriter(naziv);
        ConsoleKeyInfo keyInfo;
        dosadasnji_ulaz = "";
        trenutni_red = 0;
        IzmeniInformacije();
        do
        {
            keyInfo = Console.ReadKey(false);
            if (keyInfo.Key == ConsoleKey.Backspace) //obrisi
            {
                ObrisiJedan();
            }
            else if (keyInfo.Key == ConsoleKey.Enter) //novired
            {
                trenutni_red = 0;
                Console.Write("\n");
                dosadasnji_ulaz += "\n";
            }
            else if (char.IsLetterOrDigit(keyInfo.KeyChar) || char.IsWhiteSpace(keyInfo.KeyChar) || char.IsSymbol(keyInfo.KeyChar) || char.IsPunctuation(keyInfo.KeyChar))
            {
                bool ignorisi = false;
                if (trenutni_red >= 40)
                {
                    int l = dosadasnji_ulaz.Length;
                    char p = dosadasnji_ulaz[l - 1];
                    string[] reci = dosadasnji_ulaz.Split(' ');
                    Console.WriteLine(reci.Length);
                    if (reci.Length < 2)
                    {
                        trenutni_red = 1;
                        dosadasnji_ulaz += "\n" + keyInfo.KeyChar;
                        ignorisi = true;
                        IzmeniInformacije();
                    }
                    else
                    {
                        int id_poslednje = 0;
                        int brojac = 0;
                        foreach (string rec in reci)
                        {
                            if (brojac != reci.Length - 1)
                            {
                                brojac++;
                                id_poslednje += (rec.Length);
                            }
                        }
                        id_poslednje += reci.Length - 1;
                        if (p != ' ')
                        {
                            trenutni_red = 0;
                            dosadasnji_ulaz = dosadasnji_ulaz.Substring(0, id_poslednje);
                            dosadasnji_ulaz += "\n" + reci[reci.Length - 1] + keyInfo.KeyChar;
                            IzmeniInformacije();
                        }
                        else
                        {
                            trenutni_red = 0;
                            Console.Write("\n");
                            dosadasnji_ulaz += "\n";
                        }
                    }
                }
                if (!ignorisi)
                {
                    dosadasnji_ulaz += keyInfo.KeyChar;
                    trenutni_red++;
                }
            }
        }
        while (true);
    }
    static void IzmeniInformacije()//
    {
        Console.Clear();
        Console.Write(naziv);
        Console.Write("\n" + crteOkvir(naziv.Length));
        Console.Write("\nCTRL+S = CUVANJE\nCTRL+Z = UNDO\nCTRL+Y = REDO\nCTRL+R = ZAMENI\n" + poslednjeCuvanje + "\n" + crteOkvir(poslednjeCuvanje.Length));
        Console.Write("\n" + dosadasnji_ulaz);
    }
    static int IzborOpcija()
    {
        bool provera = false;
        int greska = 0;
        string izbor = "";
        while (!provera)
        {
            if (greska == 1)
            {
                izbor = ogranicenUlaz("0 - pomoc\n2 - napravite ovaj fajl\n3 - ponovo unesi adresu", new string[] { "0", "2", "3" }, true);
            }
            else if (greska == 2)
            {
                izbor = ogranicenUlaz("0 - pomoc\n1 - otvorite fajl\n3 - ponovo unesi adresu", new string[] { "0", "1", "3" }, true);
            }
            else
            {
                izbor = ogranicenUlaz("0 - pomoc\n1 - otvorite fajl\n2 - napravite fajl", new string[] { "0", "1", "2" }, true);
            }
            if (izbor == "0")
            {
                Console.WriteLine("POMOC\n-----");
                Console.WriteLine(pomoc);
                string[] odvojeno = pomoc.Split('\n');
                Console.WriteLine(crteOkvir(odvojeno[odvojeno.Length - 1].Length));
            }
            else if (izbor == "1")
            {
                if (greska != 2)
                {
                    Console.WriteLine("Unesite naziv fajla :");
                    naziv = Console.ReadLine();
                }
                if (!File.Exists(naziv))
                {
                    greska = 1;
                    Console.Clear();
                    Console.WriteLine("Fajl ne postoji.");
                }
                else provera = true;
            }
            else if (izbor == "2")
            {
                if (greska != 1)
                {
                    Console.WriteLine("Unesite naziv fajla :");
                    naziv = Console.ReadLine();
                }
                if (!File.Exists(naziv)) provera = true;
                else
                {
                    greska = 2;
                    Console.Clear();
                    Console.WriteLine("Fajl postoji");
                }
            }
            else if (izbor == "3")
            {
                Console.WriteLine("Unesite naziv fajla :");
                naziv = Console.ReadLine();
                if (greska == 1)
                {
                    if (File.Exists(naziv))
                    {
                        izbor = "1";
                        provera = true;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Fajl ne postoji");
                    }
                }
                else if (File.Exists(naziv))
                {
                    Console.Clear();
                    Console.WriteLine("Fajl postoji");
                }
                else
                {
                    izbor = "2";
                    provera = true;
                }
            }
        }
        return int.Parse(izbor);
    }
}