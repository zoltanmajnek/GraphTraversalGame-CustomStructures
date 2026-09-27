using System;

namespace CABTEX_félévesSztf2
{
    internal class JatekVezerlo
    {
        JatekMezo jatekMezo;
        int nyertes;

        void LepesEsemeny(ND lepes) { /* itt is lehetne pop-push-kiír, de mindent rajzol a jatekmező. Console.WriteLine(lepes); */ }

        public void Indul()
        {
            do
            {
                jatekMezo = new JatekMezo(); Console.Clear(); //jatekMezo.lepesTortent+=LepesEsemeny; 
                do
                {
                    Console.Write("Gráf neve (graf.txt az alapértelmezett) ,vagy írjon 'gen'-t a generáláshoz: "); string nev = Console.ReadLine();
                    if (nev == "") nev = "graf.txt";
                    if (nev == "gen") jatekMezo.General();
                    else try { jatekMezo.Betolt(nev); } catch (Exception e) { Console.WriteLine(e.Message); }
                } while (!jatekMezo.Ready);

                jatekMezo.Kirajzol();
                nyertes=jatekMezo.FokozatValaszt();
                while (nyertes == 0)
                {
                    jatekMezo.Kirajzol();
                    nyertes = jatekMezo.JatekosLep(); 
                    if (nyertes!=4) jatekMezo.GepLep(); else nyertes=0;
                }
                Console.SetCursorPosition(50, 1);
                Console.Write(nyertes == 1 ? "Vesztettél!" : "Nyertél.");

            } while (nyertes == 3 || jatekMezo.UjraKezdes());
        }
    }
}