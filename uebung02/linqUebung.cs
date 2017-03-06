using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqTest
{
    class Schueler
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public ICollection<Pruefung> Pruefungen { set; get; } = new List<Pruefung>();
        public override string ToString()
        {
            return String.Format("ID: {0}, Name: {1}", Id, Name);
        }
    }

    class Pruefung
    {
        public string Gegenstand { set; get; }
        public int Punkte { set; get; }
        public int Note { set; get; }
        public override string ToString()
        {
            return String.Format("{0}: {1} ({2} Punkte)", Gegenstand, Note, Punkte);
        }
    }

    class Schuelerliste
    {
        private ICollection<Schueler> schuelerColl = new List<Schueler>();

        public void Add(Schueler s)
        {
            schuelerColl.Add(s);
        }

        public Schuelerliste Where(Func<Schueler, bool> filterFunction)
        {
            Schuelerliste result = new Schuelerliste();
            foreach (Schueler s in schuelerColl)
            {
                if (filterFunction(s) == true)
                {
                    result.Add(s);
                }
            }
            return result;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Schueler> sl = new List<Schueler>();
            Schuelerliste mySl = new Schuelerliste();

            mySl.Add(new Schueler { Id = 1, Name= "XXX" });
            mySl.Add(new Schueler { Id = 2, Name= "XXX" });
            mySl.Add(new Schueler { Id = 3, Name = "XXX" });

            Schuelerliste result = mySl.Where(xx => xx.Id == 1);

            Schueler sch = new Schueler { Id = 1, Name = "Mustermann" };
            sch.Pruefungen.Add(new Pruefung { Gegenstand = "POS", Punkte = 12, Note = 5 });
            sch.Pruefungen.Add(new Pruefung { Gegenstand = "DBI", Punkte = 14, Note = 4 });
            sch.Pruefungen.Add(new Pruefung { Gegenstand = "D", Punkte = 16, Note = 3 });
            sl.Add(sch);
            sch = new Schueler { Id = 2, Name = "Musterfrau" };
            sch.Pruefungen.Add(new Pruefung { Gegenstand = "POS", Punkte = 14, Note = 4 });
            sch.Pruefungen.Add(new Pruefung { Gegenstand = "DBI", Punkte = 24, Note = 1 });
            sch.Pruefungen.Add(new Pruefung { Gegenstand = "E", Punkte = 20, Note = 2 });
            sl.Add(sch);

            // *************************************************************************************
            // Welche Prüfungen hat der Schüler mit der Id 1? Überlege dir dafür, ob die Where 
            // Klausel einen Schüler oder eine Liste von Schülern liefert.
            // *************************************************************************************
            var result1 = (from s in sl
                           where s.Id == 1
                           select s.Pruefungen).FirstOrDefault();
            result1 = sl.Where(s => s.Id == 1).Select(s => s.Pruefungen).FirstOrDefault();

            result1.ToList().ForEach(p => Console.WriteLine(p));
            // *************************************************************************************
            // Welchen Notendurchschnitt hat der Schüler mit der ID 2? Hinweis: Verwende Average
            // *************************************************************************************
            double result2 = (from s in sl
                           where s.Id == 2
                           select s.Pruefungen.Average(p => p.Note)).FirstOrDefault();

            Console.WriteLine(result2);

            // *************************************************************************************
            // Welche Schüler hatten in D eine Prüfung? Hinweis: Verwende Any
            // *************************************************************************************
            var result3 = (from s in sl
                           where s.Pruefungen.Any(p=>p.Gegenstand == "D") == true
                           select s);

            result3 = sl.Where(s => s.Pruefungen.Any(p => p.Gegenstand == "D"));
            result3.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Welche Schüler hatten schlechtere Noten als Befriedigend? Hinweis: Verwende Any
            // *************************************************************************************
            var result4 = (from s in sl
                           where s.Pruefungen.Any(p => p.Note > 3)
                           select s);

            result4 = sl.Where(s => s.Pruefungen.Any(p => p.Note > 3));
            result4.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Gibt es Schüler mit mehr als 3 Prüfungen? Verwende die Längeneigenschaft der Liste.
            // *************************************************************************************
            IEnumerable<Schueler> result5 = (from s in sl
                           where s.Pruefungen.Count > 3
                           select s);

            result5 = sl.Where(s => s.Pruefungen.Count > 3);
            result5.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Liste alle Prüfungen auf, die mehr als 12 Punkte ergaben. Verwende dafür 2 mal
            // die from Klausel, indem du die Schülerliste mit den Prüfungen jedes
            // Schülers verknüpft. Unter http://stackoverflow.com/a/6429081 findest du ein Beispiel.
            // Sortiere sie nach der Anzahl der Punkte in absteigender Reihenfolge.
            // *************************************************************************************
            /* Ich muss von sl starten, da ich keine Liste aller Prüfungen habe */
            /* Mit foreach müsste ich
             * foreach (Schueler s in sl) {
             *   foreach (Pruefung p in s.Pruefungen) {
             *      ...
             *    }
             *  }
             *  schreiben.
             */
            IEnumerable<Pruefung> result6 = (from s in sl
                           from p in s.Pruefungen
                           where p.Punkte > 12
                           select p);

            result6.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Welches Punktemittel hatten alle POS Prüfungen? Ermittle diesen Wert mit Average,
            // wobei jedoch diese Funktion eine Lambda Expression mitgegeben werden muss. 
            // *************************************************************************************
            /* Hier generiere ich eine Liste von int Werten. Average funktioniert auch so */
            double result7 = (from s in sl
                              from p in s.Pruefungen
                              where p.Gegenstand == "POS"
                              select p.Punkte).Average();
            /* Wenn ich die ganzen Prüfungen nehme, muss ich Average sagen, welches Feld es 
             * mitteln soll */
            result7 = (from s in sl
                              from p in s.Pruefungen
                              where p.Gegenstand == "POS"
                              select p).Average(p=>p.Punkte);

            Console.WriteLine(result7);
            Console.ReadKey();
        }
    }
}
