namespace SchulVw.Model
{
    public class Schueler
    {
        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Langname
        {
            get
            {
                return Nachname + " " + Vorname;
            }
        }
        public Klasse Klasse { get; set; }
    }
}