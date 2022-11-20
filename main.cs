/*
Opis:		Program treba da ima mogućnost pravljenja, čitanja i uređivanja tekstualnih datoteka. Neophodno je realizovati umetanje simbola (znakova) u datoteku, zamenu simbola u datoteci, kao i brisanje simbola. Pored čuvanja izmena, omogućiti korisniku da sačuva datoteku kao neku drugu (Save As…).
*/

using System;
using System.IO;
using System.Threading;
using System.Text;

class Program
{
    public static int X = 0, Y = 2;
    public static string naziv, poslednjeCuvanje, dosadasnji_ulaz;
    public static int trenutni_red;
    public static string pomoc = "\nstrelice - kretanje\nx - obrisi simbol\nz - zameni simbol\ni - umetni simbol\ns - save\na - save as\nEsc - izađi iz funkcije\nEsc - izlazak iz programa\n";
    public static string[] nizNaEkranu = new string[1];
    public static string[] nizDatoteke = new string[1];
    public static int[,] redistart = new int[100, 2];

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
        dosadasnji_ulaz = "";
        if (File.Exists(naziv))
        {
            StreamReader citanje = new StreamReader(naziv);

            int brojac = 0;
            while (!citanje.EndOfStream)
            {
                if (brojac > 0)
                {
                    Array.Resize(ref nizDatoteke, brojac + 1);
                }
                nizDatoteke[brojac] = citanje.ReadLine();
                brojac++;
            }
            citanje.Close();
            Console.WriteLine();
            IspisiText();
            KretanjePoDatoteci();
        }
    }
    static void IspisiBojom(ConsoleColor c, string tekst, bool line)
    {
        Console.ForegroundColor = c;
        if (line) Console.WriteLine(tekst);
        else Console.Write(tekst);
        Console.ForegroundColor = ConsoleColor.White;
    }
    static void IspisiText()
    {
        // string[] nizNaEkranu = new string[ niz.Length ];
        int lokalX = 0;
        int lokalY = 2;
        Console.Clear();
        IspisiBojom(ConsoleColor.Yellow, naziv + "\n" + CrteOkvir(naziv.Length + 1), false);
        int duzinaNajvecegBr = nizDatoteke.Length.ToString().Length;
        for (int i = 0; i < nizDatoteke.Length; i++)
        {
            if (nizDatoteke[i] != null)
            {
                string ispisRed = nizDatoteke[i];
                Console.SetCursorPosition(lokalX, lokalY);
                IspisiBojom(ConsoleColor.DarkBlue, (i + 1) + " ", false);
                int duzinaBr = (i + 1).ToString().Length;
                //    lokalX += 2;
                int startPozicija = 0;
                while (ispisRed != null)
                {
                    if (lokalY > nizNaEkranu.Length)
                    {
                        Array.Resize(ref nizNaEkranu, lokalY - 1);

                    }
                    redistart[lokalY - 2, 0] = i;
                    redistart[lokalY - 2, 1] = startPozicija;
                    if (ispisRed.Length > 78)
                    {
                        nizNaEkranu[lokalY - 2] = ispisRed.Substring(0, 78);
                        ispisRed = ispisRed.Substring(78);
                        startPozicija += 78;
                    }
                    else
                    {
                        nizNaEkranu[lokalY - 2] = ispisRed;
                        ispisRed = null;
                    }
                    if (nizDatoteke[i].Length > 78 && nizNaEkranu[lokalY - 2] != nizDatoteke[i].Substring(0, 78))
                    {
                        Console.Write(" ");
                    }
                    Console.WriteLine(nizNaEkranu[lokalY - 2]);
                    lokalY++;
                }
            }
        }
    }
    static void KretanjePoDatoteci()
    {
        ConsoleKeyInfo strelica;
        bool krajCitanja = false;
        bool insert = false;
        bool menjanje = false;
        X = 2;
        Y = 2;
        Console.SetCursorPosition(X, Y);

        while (!krajCitanja)
        {
            strelica = Console.ReadKey(true);
            if (strelica.Key == ConsoleKey.LeftArrow)
            {
                if (X > 2) X--;
                else if (X == 2)
                {
                    if (Y > 2) { Y--; X = nizNaEkranu[Y - 2].Length - 1; }
                    else { X = Console.CursorLeft; Y = Console.CursorTop; }
                }
            }
            else if (strelica.Key == ConsoleKey.RightArrow)
            {
                if (X < nizNaEkranu[Y - 2].Length + 1) X++;
                else if (X == nizNaEkranu[Y - 2].Length + 1)
                {
                    if (Y < nizNaEkranu.Length + 2) { Y++; X = 2; }
                    else { X = Console.CursorLeft; Y = Console.CursorTop; }
                }
            }
            else if (strelica.Key == ConsoleKey.DownArrow)
            {
                if (Y < nizNaEkranu.Length + 2)
                {
                    if (X < nizNaEkranu[Y - 2].Length - 1) Y++;
                    else { Y++; X = nizNaEkranu[Y - 2].Length - 1; }
                }
                else continue;
            }
            else if (strelica.Key == ConsoleKey.UpArrow)
            {
                if (Y > 2)
                {
                    if (X < nizNaEkranu[Y - 3].Length + 1) Y--;
                    else { Y--; X = nizNaEkranu[Y - 2].Length + 1; }
                }
                else continue;
            }
            else if (strelica.Key == ConsoleKey.O && !insert)
            {
                Brisanje(nizDatoteke);
                Console.SetCursorPosition(0, Y);
                Console.WriteLine(nizDatoteke[Y - 1]);
                IspisiText();
                Console.SetCursorPosition(X, Y);
            }
            else if (strelica.Key == ConsoleKey.I && !insert)
            {
                insert = true;
            }
            else if (insert && strelica.Key == ConsoleKey.Enter)
            {
                Umetanje(nizDatoteke, "\n");
                dosadasnji_ulaz += "\n";
                IzmeniInformacije();
            }
            else if (insert && strelica.Key == ConsoleKey.Spacebar)
            {
                Umetanje(nizDatoteke, " ");
                dosadasnji_ulaz += " ";
                IzmeniInformacije();
            }
            else if (strelica.Key == ConsoleKey.Backspace && insert)
            {
                ObrisiJedan();
                Brisanje(nizDatoteke);
                IzmeniInformacije();
            }
            else if (strelica.Key != ConsoleKey.Escape && insert)
            {
                Umetanje(nizDatoteke, strelica.Key.ToString());
                IspisiText();
                Console.SetCursorPosition(X, Y);
            }
            else if (strelica.Key == ConsoleKey.Escape && insert)
            {
                insert = false;
                Console.SetCursorPosition(0, Y);

                Console.SetCursorPosition(X, Y);
            }
            else if (strelica.Key == ConsoleKey.Z)
            {
                menjanje = true;
            }
            else if (menjanje)
            {
                menjanje = false;
                ZameniSimbol(nizDatoteke, strelica.Key.ToString());
                Console.SetCursorPosition(0, Y);
                Console.WriteLine(nizNaEkranu[Y - 2]);
                IspisiText();
                Console.SetCursorPosition(X, Y);
            }
            else if (strelica.Key == ConsoleKey.S)
            {
                Sacuvaj(naziv);
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
            //else krajCitanja = true;
            Console.SetCursorPosition(X, Y);
        }
    }
    static void Brisanje(string[] niz)
    {
        int reduDatoteci = redistart[Y - 2, 0];
        int pozicijaURedu = X - 2 + redistart[Y - 2, 1];
        niz[reduDatoteci] = niz[reduDatoteci].Remove(pozicijaURedu, 1) + " ";
    }
    static void Umetanje(string[] niz, string tekst)
    {
        int reduDatoteci = redistart[Y - 2, 0];
        int pozicijaURedu = X - 2 + redistart[Y - 2, 1];
        niz[reduDatoteci] = niz[reduDatoteci].Insert(pozicijaURedu, tekst);
        X++;
    }
    static void ZameniSimbol(string[] niz, string simbol)
    {
        int reduDatoteci = redistart[Y - 2, 0];
        int pozicijaURedu = X - 2 + redistart[Y - 2, 1];
        niz[reduDatoteci] = niz[reduDatoteci].Replace(niz[Y - 2].Substring(pozicijaURedu, 1), simbol);
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
        pisanje.Close();
        trenutni_red = 0;
        IzmeniInformacije();
        CitanjeDatoteke(naziv);
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
        IspisiBojom(ConsoleColor.Yellow, naziv, false);//dodati boju
        Console.Write("\n" + CrteOkvir(naziv.Length));
        Console.Write("\n" + dosadasnji_ulaz);
        int _y = dosadasnji_ulaz.Split('\n').Length;
        Console.SetCursorPosition(X, _y);
    }
    static void Sacuvaj(string naziv)
    {
        poslednjeCuvanje = DateTime.Now.ToString();
        IzmeniInformacije();
        StreamWriter pisanje = new StreamWriter(naziv);
        string[] odvojenoNaRedove = dosadasnji_ulaz.Split('\n');
        for (int i = 0; i < nizDatoteke.Length; i++)
        {
          pisanje.Write(nizDatoteke[i]);
        }
        pisanje.Close();
    }
    static void SaveAs(string noviNaziv)
    {
        //string kopija = naziv;
        //File.Replace( naziv, noviNaziv, kopija );
        //bila je greska zbog ovoga, ako bude trebalo vratite
        naziv = noviNaziv;
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