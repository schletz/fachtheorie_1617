using System.Collections.Generic;

namespace SchulVw.Model
{
    public class Klasse
    {
        public string Name { get; set; }
        public Schule Schule { get; set; }
        public List<Schueler> Schueler { get; } = new List<Schueler>();
        public Schueler FindSchuelerById (int id)
        {
            foreach (Schueler s in Schueler)
            {
                if (s.Id == id)
                {
                    return s;
                }
            }
            return null;
        }
        public bool AddSchueler (Schueler s)
        {
            if (s == null) { return false; }
            if (s.Klasse == null &&
                Schule.FindSchuelerById(s.Id) != null)
            {
                return false;
            }
            if (Schueler.Count < 5)
            {
                Schueler.Add(s);
                /* NICHT VERGESSEN, sonst bleibt die Rückreferenz NULL!! */
                s.Klasse?.Schueler.Remove(s);
                s.Klasse = this;
                return true;
            }
            return false;
        }

    }
}