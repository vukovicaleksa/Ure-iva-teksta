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
    public static string pomoc = "\nstrelice - kretanje\nx - obrisi simbol\nz - zameni simbol\ni - umetni simbol\ns - save\na - save as\nEsc - izađi iz funkcije\nEsc-izlazak iz programa\n";
    static int IzborOpcija()
    {
        bool provera = false;
        int greska = 0;
        string izbor = "";
        while (!provera)
        {
            if (greska == 1)
            {
                izbor = OgranicenUlaz("0 - pomoc\n2 - napravite ovaj fajl\n3 - ponovo unesi adresu", new string[] { "0", "2", "3" }, true);
            }
            else if (greska == 2)
            {
                izbor = OgranicenUlaz("0 - pomoc\n1 - otvorite fajl\n3 - ponovo unesi adresu", new string[] { "0", "1", "3" }, true);
            }
            else
            {
                izbor = OgranicenUlaz("0 - pomoc\n1 - otvorite fajl\n2 - napravite fajl", new string[] { "0", "1", "2" }, true);
            }
            if (izbor == "0")
            {
                IspisiBojom(ConsoleColor.Yellow, "POMOC\n-----", true);
                Console.WriteLine(pomoc);
                string[] odvojeno = pomoc.Split('\n');
                Console.WriteLine(CrteOkvir(odvojeno[odvojeno.Length - 1].Length));
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
            IspisiText(niz);
            KretanjePoDatoteci(niz, X, Y);
        }
    }
    static void IspisiBojom(ConsoleColor c, string tekst, bool line)
    {
        Console.ForegroundColor = c;
        if (line) Console.WriteLine(tekst);
        else Console.Write(tekst);
        Console.ForegroundColor = ConsoleColor.White;
    }
    static void IspisiText(string[] niz)
    {
        X = 0;
        Y = 2;
        Console.Clear();
        IspisiBojom(ConsoleColor.Yellow, naziv + "\n" + CrteOkvir(naziv.Length + 1), true);
        int duzinaNajvecegBr = niz.Length.ToString().Length;
        for (int i = 0; i < niz.Length; i++)
        {
            if (niz[i] != null)
            {
                Console.SetCursorPosition(X, Y);
                IspisiBojom(ConsoleColor.DarkBlue, (i + 1) + " ", false);
                int duzinaBr = (i+1).ToString().Length;
                X += 2;
                for (int a = 0; a < duzinaNajvecegBr-duzinaBr; a++)
                {
                  Console.Write(' ');
                }
                for (int j = 0; j < niz[i].Length; j++)
                {
                    if (X <= 80) { Console.Write(niz[i][j]); X++; }
                    else { Y++; X = 0; Console.SetCursorPosition(X + 2, Y); Console.Write(niz[i][j]); X++; }
                }
                Console.WriteLine();
                Y++;
                X = 0;
            }
        }
    }
    static void KretanjePoDatoteci(string[] niz, int X, int Y)
    {
        ConsoleKeyInfo strelica;
        bool krajCitanja = false;
        bool insert = false;
        bool menjanje = false;
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
                if (Y < niz.Length + 1)
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
            else if (strelica.Key == ConsoleKey.O && !insert)
            {
                Brisanje(niz);
                Console.SetCursorPosition(0, Y);
                Console.WriteLine(niz[Y - 1]);
                IspisiText(niz);
                Console.SetCursorPosition(X, Y);
            }
            else if (strelica.Key == ConsoleKey.I && !insert)
            {
                insert = true;
            }
            else if (strelica.Key != ConsoleKey.Escape && insert)
            {
                Umetanje(niz, strelica.Key.ToString());
            }
            else if (strelica.Key == ConsoleKey.Escape && insert)
            {
                insert = false;
                Console.SetCursorPosition(0, Y);
                Console.WriteLine(niz[Y - 2]);
                IspisiText(niz);
                Console.SetCursorPosition(X, Y);
            }
            else if (strelica.Key == ConsoleKey.Z)
            {
                menjanje = true;
            }
            else if (menjanje)
            {
                menjanje = false;
                ZameniSimbol(niz, strelica.Key.ToString());
                Console.SetCursorPosition(0, Y);
                Console.WriteLine(niz[Y - 2]);
                IspisiText(niz);
                Console.SetCursorPosition(X, Y);
            }
            else if (strelica.Key == ConsoleKey.A)
            {
                Console.Clear();
                string naziv = "";
                while (naziv.Length == 0)
                {
                    Console.WriteLine("Kako želite da nazovete novu datoteku?");
                    naziv = Console.ReadLine();
                }
                SaveAs(naziv);
            }
            else if (strelica.Key == ConsoleKey.Escape)
            {
                krajCitanja = true;
            }
            else krajCitanja = true;
            Console.SetCursorPosition(X, Y);
        }
    }
    static void Brisanje(string[] niz)
    {
        niz[Y - 2] = niz[Y - 2].Remove(X, 1) + " ";
    }
    static void Umetanje(string[] niz, string tekst)
    {
        niz[Y - 2] = niz[Y - 2].Insert(X, tekst);
        X++;
    }
    static void ZameniSimbol(string[] niz, string simbol)
    {
        niz[Y - 2] = niz[Y - 2].Replace(niz[Y - 2], simbol);
    }
    static string OgranicenUlaz(string pitanje, string[] dozvoljeno, bool obrisi)
    {
        string odgovor = "";
        while (!NizSadrzi(dozvoljeno, odgovor))
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
    static bool NizSadrzi(string[] niz, string vrednost)
    {
        foreach (string s in niz) { if (s == vrednost) return true; }
        return false;
    }
    static string CrteOkvir(int duzina)
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
                if (!ignorisi)
                {
                    dosadasnji_ulaz += keyInfo.KeyChar;
                    trenutni_red++;
                }
            }
        }
        while (true);
    }
    static void IzmeniInformacije()
    {
        Console.Clear();
        Console.Write(naziv);
        Console.Write("\n" + CrteOkvir(naziv.Length));
        Console.Write(pomoc + "Poslednji put sacuvano: " + poslednjeCuvanje + "\n" + CrteOkvir(poslednjeCuvanje.Length + 24));
        Console.Write("\n" + dosadasnji_ulaz);
    }
    static void Sacuvaj(string naziv)
    {
        poslednjeCuvanje = DateTime.Now.ToString();
        IzmeniInformacije();
        StreamWriter pisanje = new StreamWriter(naziv);
        string[] odvojenoNaRedove = dosadasnji_ulaz.Split('\n');
        int br1 = 0;
        foreach (string s in odvojenoNaRedove)
        {
            if (br1 == odvojenoNaRedove.Length - 1) pisanje.Write(s);
            else pisanje.WriteLine(s);
            br1++;
        }
        pisanje.Close();
    }
    static void SaveAs(string noviNaziv)
    {
        string kopija = naziv;
        File.Replace(naziv, noviNaziv, kopija);
        Sacuvaj(noviNaziv);
    }
    public static void Main(string[] args)
    {
        trenutni_red = 0;
        poslednjeCuvanje = "Niste sacuvali";
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
}