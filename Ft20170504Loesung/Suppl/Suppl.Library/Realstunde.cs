using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suppl.Library
{
    /// <summary>
    /// Fügt die Unterrichtsstunde und die Vertretung zusammen.
    /// </summary>
    public class Realstunde
    {
        public int UntisId { get; set; }
        public DateTime Tag { get; set; }
        public int Stunde { get; set; }
        public string Klasse { get; set; }
        public string Lehrer { get; set; }
        public string Fach { get; set; }
        public string Raum { get; set; }
        public Unterrichtsstunden Unterricht { get; set; }
        public Vertretungen Vertretung { get; set; }
        public bool IstVertretung { get { return Vertretung != null; } }
    }
}
