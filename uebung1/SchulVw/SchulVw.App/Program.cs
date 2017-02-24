using SchulVw.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchulVw.App
{
    class Program
    {
        static void Main(string[] args)
        {
            /* INITIALIZER */
            Schule s = new Schule() { Skz = 905417, Name = "HTL Spengergasse" };
            s.AddKlasse(new Klasse() { Name = "5CHIF" });
            s.AddKlasse(new Klasse() { Name = "5BHIF" });
            Schueler sch = new Schueler { Id = 1, Nachname = "Mustermann", Vorname = "Max" };
            s.Klassen["5CHIF"].AddSchueler(sch);
            // Das geht nicht, da id 1 doppelt ist.
            s.Klassen["5BHIF"].AddSchueler(new Schueler
            {
                Id = 1,
                Nachname = "Mustermann2",
                Vorname = "Max2"
            });
            
        }
    }
}
