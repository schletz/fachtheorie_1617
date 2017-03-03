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
        public ICollection Pruefungen { set; get; } = new List();
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

    class Program
    {
        static void Main(string[] args)
        {
            List sl = new List();

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

            result1.ToList().ForEach(p => Console.WriteLine(p));
            // *************************************************************************************
            // Welchen Notendurchschnitt hat der Schüler mit der ID 2? Hinweis: Verwende Average
            // *************************************************************************************


            result2.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Welche Schüler hatten in D eine Prüfung? Hinweis: Verwende Any
            // *************************************************************************************
            result3.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Welche Schüler hatten schlechtere Noten als Befriedigend? Hinweis: Verwende Any
            // *************************************************************************************
            result4.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Gibt es Schüler mit mehr als 3 Prüfungen? Verwende die Längeneigenschaft der Liste.
            // *************************************************************************************
            result5.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Liste alle Prüfungen auf, die mehr als 12 Punkte ergaben. Verwende dafür 2 mal
            // die from Klausel, indem du die Schülerliste mit den Prüfungen jedes
            // Schülers verknüpft. Unter http://stackoverflow.com/a/6429081 findest du ein Beispiel.
            // Sortiere sie nach der Anzahl der Punkte in absteigender Reihenfolge.
            // *************************************************************************************
            result6.ToList().ForEach(p => Console.WriteLine(p));

            // *************************************************************************************
            // Welches Punktemittel hatten alle POS Prüfungen? Ermittle diesen Wert mit Average,
            // wobei jedoch diese Funktion eine Lambda Expression mitgegeben werden muss. 
            // *************************************************************************************
            result7.ToList().ForEach(p => Console.WriteLine(p));

            Console.ReadKey();
        }
    }
}
