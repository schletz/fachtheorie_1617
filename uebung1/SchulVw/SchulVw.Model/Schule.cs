using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchulVw.Model
{
    public class Schule
    {
        public int Skz { get; set; }
        public string Name { get; set; }
        /* C# 5 Initialisierung eines Default Properties */
        public Dictionary<string, Klasse> Klassen { get; } 
            = new Dictionary<string, Klasse>();
        public Schueler FindSchuelerById(int id)
        {
            /* SO GIBTS GARANTIERT NUR DIE NOTE >= 4 */
            foreach (Klasse k in Klassen.Values)
            {
                Schueler found = k.FindSchuelerById(id);
                if (found != null)
                {
                    return found;
                }
            }
            /*
            var res = (from k in Klassen.Values
                       from s in k.Schueler
                       where s.Id == id
                       select s).FirstOrDefault();
            */
            return null;
        }
        public bool AddKlasse(Klasse k)
        {
            if (k == null) { return false; }
            try
            {
                Klassen.Add(k.Name, k);
                k.Schule = this;
                return true;
            }
            catch { }
            return false;
        }
    }
}
