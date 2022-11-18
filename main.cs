/*
Opis:		Program treba da ima mogućnost pravljenja, čitanja i uređivanja tekstualnih datoteka. Neophodno je realizovati umetanje simbola (znakova) u datoteku, zamenu simbola u datoteci, kao i brisanje simbola. Pored čuvanja izmena, omogućiti korisniku da sačuva datoteku kao neku drugu (Save As…).
*/

using System;
using System.IO;
using System.Threading;
using System.Text;

class Program
{
    public static int X = 0, Y = 3;
    public static string naziv, poslednjeCuvanje, dosadasnji_ulaz;
    public static int trenutni_red;
    public static string pomoc = "strelice - kretanje\nn - undo\nr - redo\no - obrisi simbol\nz - zameni simbol\nu - umetni simbol\ns - save\na - save as";
    static void CitanjeDatoteke(string naziv)
    {
        if (File.Exists(naziv))
        {
            StreamReader citanje = new StreamReader(naziv);
            string[] niz = new string[1];
            int brojac = 0;
            while (!citanje.EndOfStream)
            {
                niz[brojac] = citanje.ReadLine();
                brojac++;
                Array.Resize(ref niz, brojac + 1);
            }
            citanje.Close();
            Console.WriteLine();
            PresekTeksta(ref niz);
            IspisiText(niz);
            KretanjePoDatoteci(niz, X, Y);
        }
    }
    static void IspisiText(string[] niz)
    {
        Console.SetCursorPosition(0, 3);
        for (int i = 0; i < niz.Length; i++)
        {
            Console.WriteLine(niz[i]);
        }
        Console.SetCursorPosition(X, Y);
    } 
    static string[] PresekTeksta(ref string[] niz)
    {
        //Console.SetWindowSize(50, 50);
        int i = 0;
        Array.Resize(ref niz, niz.Length - 1);
        while (i < niz.Length)
        {
            string[] red = niz[i].Split();
            if (niz[i].Length > Console.WindowWidth && red.Length != 1)
            {
                if (i == niz.Length - 1)
                {
                    Array.Resize(ref niz, niz.Length + 1);
                    niz[i + 1] = " ";
                }
                StringBuilder rec = new StringBuilder(niz[i]);
                rec.Remove(niz[i].Length - red[red.Length - 1].Length - 1, red[red.Length - 1].Length + 1);
                niz[i] = rec.ToString();
                rec = new StringBuilder(niz[i + 1]);
                if (niz[i + 1] == " ")
                {
                    rec.Insert(0, red[red.Length - 1]);
                    rec.Remove(rec.Length - 1, 1);
                }
                else rec.Insert(0, red[red.Length - 1] + " ");
                niz[i + 1] = rec.ToString();
                continue;
            }
            if (niz[i].Length > Console.WindowWidth && red.Length == 1)
            {
                if (i == niz.Length - 1)
                {
                    Array.Resize(ref niz, niz.Length + 1);
                    niz[i + 1] = "";
                }
                StringBuilder rec1 = new StringBuilder(niz[i]);
                rec1.Remove(0, Console.WindowWidth);
                niz[i + 1] += rec1.ToString();

                rec1 = new StringBuilder(niz[i]);
                rec1.Remove(Console.WindowWidth, niz[i].Length - Console.WindowWidth);
                niz[i] = rec1.ToString();
                continue;
            }
            i++;
        }
        return niz;
    }
    static void KretanjePoDatoteci(string[] niz, int X, int Y)
    {
        ConsoleKeyInfo strelica;
        bool krajCitanja = false;
        Console.SetCursorPosition(X, Y);
        while (!krajCitanja)
        {
            strelica = Console.ReadKey(true);
            if (strelica.Key == ConsoleKey.LeftArrow)
            {
                if (X > 0) X--;
                else if (X == 0)
                {
                    if (Y > 3) { Y--; X = niz[Y - 3].Length - 1; }
                    else { X = Console.CursorLeft; Y = Console.CursorTop; }
                }
            }
            else if (strelica.Key == ConsoleKey.RightArrow)
            {
                if (X < niz[Y - 3].Length - 1) X++;
                else if (X == niz[Y - 3].Length - 1)
                {
                    if (Y < niz.Length + 1) { Y++; X = 0; }
                    else { X = Console.CursorLeft; Y = Console.CursorTop; }
                }
            }
            else if (strelica.Key == ConsoleKey.DownArrow)
            {
                if (Y < niz.Length +1)
                {
                    if (X < niz[Y - 2].Length - 1) Y++;
                    else { Y++; X = niz[Y - 3].Length - 1; }
                }
                else continue;
            }
            else if (strelica.Key == ConsoleKey.UpArrow)
            {
                if (Y > 3)
                {
                    if (X < niz[Y - 4].Length - 1) Y--;
                    else { Y--; X = niz[Y - 3].Length - 1; }
                }
                else continue;
            }
            else if (strelica.Key == ConsoleKey.X)
            {
                Brisanje(niz);
                Console.SetCursorPosition(0, Y);
                Console.WriteLine(niz[Y - 3]);
                IspisiText(niz);
                Console.SetCursorPosition(X, Y);
            }
            else krajCitanja = true;
            Console.SetCursorPosition(X, Y);
        }
    }
    public static void Brisanje(string[] niz)
    {
        niz[Y - 4] = niz[Y - 4].Remove(X, 1) + " ";
    }
    public static void Main(string[] args)
    {
        trenutni_red = 0;
        poslednjeCuvanje = "Niste cuvali";
        int izborOpcije = IzborOpcija();
        if (izborOpcije == 1)//otvori
        {
            CitanjeDatoteke(naziv);
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
                    if (reci.Length < 2) //ima samo jedna rec, samo se prelazi u sledeci red
                    {
                        trenutni_red = 1;
                        dosadasnji_ulaz += "\n" + keyInfo.KeyChar;
                        ignorisi = true;
                        IzmeniInformacije();
                    }
                    else
                    {
                        //prebaci poslednju rec u sledeci red
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
    static void IzmeniInformacije()//?
    {
        Console.Clear();
        Console.Write(naziv);
        Console.Write("\n" + crteOkvir(naziv.Length));
        Console.Write("\nstrelice - kretanje\nn - undo\nr - redo\no$ - obrisi simbol\nz - zameni simbol\nu - umetni simbol\ns - save\na - save as\n" + poslednjeCuvanje + "\n" + crteOkvir(poslednjeCuvanje.Length));
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