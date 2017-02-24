using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchulVw.Model;

namespace SchulVw.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddSchueler()
        {
            Schule s = new Schule() { Skz = 905417, Name = "HTL Spengergasse" };
            s.AddKlasse(new Klasse() { Name = "5CHIF" });
            s.AddKlasse(new Klasse() { Name = "5BHIF" });
            Schueler sch = new Schueler { Id = 1, Nachname = "Mustermann", Vorname = "Max" };
            s.Klassen["5BHIF"].AddSchueler(sch);
            s.Klassen["5CHIF"].AddSchueler(sch);
            s.Klassen["5CHIF"].AddSchueler(new Schueler() { Id = 2, Vorname = "", Nachname = "" });
            s.Klassen["5CHIF"].AddSchueler(new Schueler() { Id = 3, Vorname = "", Nachname = "" });
            s.Klassen["5CHIF"].AddSchueler(new Schueler() { Id = 4, Vorname = "", Nachname = "" });
            s.Klassen["5CHIF"].AddSchueler(new Schueler() { Id = 5, Vorname = "", Nachname = "" });
            // Prüfen, wenn der Schüler NULL ist
            Assert.AreEqual<bool>(false, s.Klassen["5CHIF"].AddSchueler(null));
            // Zuordnung OK?
            Assert.AreEqual<Schueler>(sch, s.Klassen["5CHIF"].FindSchuelerById(1));
            // Mehr als 5 Schüler gehen nicht?
            Assert.AreEqual<bool>(false, s.Klassen["5CHIF"].AddSchueler(
                new Schueler() { Id = 6, Vorname = "", Nachname = "" }
            ));

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
