namespace Suppl.Library
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Collections.Generic;

    public partial class UnterrichtDb : DbContext
    {
        public UnterrichtDb()
            : base("name=UnterrichtDb")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lehrer>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Lehrer>()
                .Property(e => e.Zuname)
                .IsUnicode(false);

            modelBuilder.Entity<Lehrer>()
                .Property(e => e.Vorname)
                .IsUnicode(false);

            modelBuilder.Entity<Lehrer>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Lehrer>()
                .HasMany(e => e.Unterrichtsstundens)
                .WithRequired(e => e.Lehrer1)
                .HasForeignKey(e => e.Lehrer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lehrer>()
                .HasMany(e => e.Vertretungens)
                .WithOptional(e => e.Lehrer)
                .HasForeignKey(e => e.NeuerLehrer);

            modelBuilder.Entity<Unterrichtsstunden>()
                .Property(e => e.Klasse)
                .IsUnicode(false);

            modelBuilder.Entity<Unterrichtsstunden>()
                .Property(e => e.Lehrer)
                .IsUnicode(false);

            modelBuilder.Entity<Unterrichtsstunden>()
                .Property(e => e.Fach)
                .IsUnicode(false);

            modelBuilder.Entity<Unterrichtsstunden>()
                .Property(e => e.Raum)
                .IsUnicode(false);

            modelBuilder.Entity<Unterrichtsstunden>()
                .HasMany(e => e.Vertretungens)
                .WithRequired(e => e.Unterrichtsstunden)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vertretungen>()
                .Property(e => e.NeuerLehrer)
                .IsUnicode(false);

            modelBuilder.Entity<Vertretungen>()
                .Property(e => e.NeuerRaum)
                .IsUnicode(false);

            modelBuilder.Entity<Vertretungen>()
                .Property(e => e.NeuesFach)
                .IsUnicode(false);
        }

        public virtual DbSet<Lehrer> Lehrers { get; set; }
        public virtual DbSet<Unterrichtsstunden> Unterrichtsstundens { get; set; }
        public virtual DbSet<Vertretungen> Vertretungens { get; set; }
        /// <summary>
        /// Liefert die wirklichen Stunden an einem Tag im Schuljahr. Dies entspricht in
        /// Untis dem Stundenplan im Intranet, wo rot die Abweichungen reingerechnet werden. */
        /// </summary>
        /// <param name="tag">Der Tag, für den der Unterricht ermittelt werden soll.</param>
        /// <returns></returns>
        public IList<Realstunde> GetRealstunden(DateTime tag)
        {
            List<Realstunde> realstunden = new List<Realstunde>();
            int wochentag = (int)tag.DayOfWeek;
            /* Alle Stunden, die durch Vertretungen an diesen Tag gelangt sind, lesen.
             * Das sind die roten Stunden im Untis Stundenplan. */
            realstunden.AddRange(
                from v in Vertretungens
                where v.NeuerTag == tag
                select new Realstunde
                {
                    UntisId = v.UntisId,
                    Tag = (DateTime)v.NeuerTag,
                    Stunde = v.NeueStunde,
                    Klasse = v.Unterrichtsstunden.Klasse,
                    Lehrer = v.NeuerLehrer,
                    Fach = v.NeuesFach,
                    Raum = v.NeuerRaum,
                    Unterricht = v.Unterrichtsstunden,
                    Vertretung = v,
                }
            );
            /* Alle Stunden, die keine Vertretung für diesen Tag gespeichert haben, sind normale
             * Stunden. Sie werden im 2. Schritt hinzugefügt. */
            realstunden.AddRange(
                from u in Unterrichtsstundens
                where u.Wochentag == wochentag && !Vertretungens.Any(v => v.UntisId == u.UntisId && v.Tag == tag)
                select new Realstunde
                {
                    UntisId = u.UntisId,
                    Tag = tag,
                    Stunde = u.Stunde,
                    Klasse = u.Klasse,
                    Lehrer = u.Lehrer,
                    Fach = u.Fach,
                    Raum = u.Raum,
                    Unterricht = u,
                }
            );
            return realstunden;
        }

        /// <summary>
        /// Erstellt aus einer Realstunde (Wöchentlicher Unterricht + ein definitiges Datum) eine Vertretung. Da
        /// keine 2 Vertretungen für den selben Unterricht pro Woche existieren können, wird eine 
        /// eventuell vorhandene Vertretung vorher entfernt. Die aktuellen Daten sind aber schon
        /// in der Realstunde enthalten. */
        /// </summary>
        /// <param name="r">Die mit GetRealstunden ermittelte reale Stunde.</param>
        /// <returns>true im Erfolgsfall, false bei Datenbankfehlern.</returns>
        public bool ErstelleVertretung(Realstunde r)
        {
            DateTime tag = r.Tag;
            if (r.IstVertretung)
            {
                tag = r.Vertretung.Tag;
                Vertretungens.Remove(r.Vertretung);
            }
            Vertretungen v = new Vertretungen
            {
                UntisId = r.UntisId,
                Tag = tag,
                NeuerLehrer = r.Lehrer,
                NeuerRaum = r.Raum,
                NeuesFach = r.Fach,
                NeuerTag = r.Tag,
                NeueStunde = r.Stunde,
            };
            r.Vertretung = v;
            Vertretungens.Add(v);
            try { SaveChanges(); } catch { return false; }
            return true;
        }

        /// <summary>
        /// 1a) Lehrer meldet sich für einen Tag krank
        /// Dafür müssen auch Vertretungen berücksichtigt werden. Es kann sein, dass der Unterricht
        /// an diesen Tag verlegt wurde, und sich danach der Lehrer krank meldet. Dadurch fällt dieser
        /// Unterricht natürlich auch einmal aus.
        /// </summary>
        /// <param name="tag">Der Tag, andem der Lehrer krank ist.</param>
        /// <param name="lehrer">Das Lehrerkürzel.</param>
        /// <returns></returns>
        public int Krankmeldung(DateTime tag, string lehrer)
        {
            var realstunden = GetRealstunden(tag).Where(r => r.Lehrer == lehrer);
            /* Nachher ist realstunden leer, da wir ja den Lehrer auf NULL setzen! */
            int anzStunden = realstunden.Count();
            bool success = true;

            realstunden
                .ToList()
                .ForEach(r => { r.Lehrer = null; success &= ErstelleVertretung(r); });
            if (success) { return anzStunden; }
            return 0;
        }

        /// <summary>
        /// 1b) Klasse kommt für eine Stunde in einen neuen Raum
        /// Dafür muss der Raum natürlich einmal frei sein. Allerdings müssen wir auch eventuelle
        /// Verlegungen berücksichtigen.
        /// Diese Methode weist bei geteiltem Unterricht in 2 Räumen jedem Datensatz diesen Raum zu.
        /// Durch die Übergabe eines alten Raumes kann dies eingeschränkt werden.
        /// </summary>
        /// <param name="klasse">Die Klasse, dessen Unterricht verlegt werden soll.</param>
        /// <param name="tag">An diesem Tag wird verlegt.</param>
        /// <param name="stunde">In dieser Stunde wird verlegt.</param>
        /// <param name="raum">Der neue Raum.</param>
        /// <returns></returns>
        public bool Raumaenderung(string klasse, DateTime tag, int stunde, string raum)
        {
            var realstunden = GetRealstunden(tag).Where(r => r.Klasse == klasse && r.Stunde == stunde);
            bool success = true;

            /* Ist der Raum belegt? Dann false liefern. */
            if (GetRealstunden(tag).Any(r => r.Stunde == stunde && r.Raum == raum)) return false;

            /* Nun bekommt die Klasse ihren neuen Raum. */
            realstunden
                .ToList()
                .ForEach(r => { r.Raum = raum; success &= ErstelleVertretung(r); });

            return success;
        }

        /// <summary>
        /// 1c) Raumsuche
        /// Für einen bestimmten Tag und eine bestimmte Stunde werden alle freien Räume geliefert.
        /// Alle verfügbaren Räume sind in der Unterrichtsstunden Tabelle.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="stunde"></param>
        /// <returns></returns>
        public IEnumerable<string> Raumsuche(DateTime day, int stunde)
        {
            /* Ohne ToList kann LINQ to Entities keine Methode GetRealstunden aufrufen, da sie 
             * ja nicht in SQL existiert. */
            var raeume = Unterrichtsstundens.Select(u => u.Raum).Distinct().ToList();
            return raeume.Where(raum => !GetRealstunden(day).Any(r => r.Raum == raum && r.Stunde == stunde));
        }

        /* 1d) Unterrichtsverlegung */
        public bool Unterrichtsverlegung(Unterrichtsstunden unterricht, DateTime alterTag, DateTime neuerTag, int stunde)
        {
            Realstunde originalUnterricht = GetRealstunden(alterTag)
                .Where(r => r.UntisId == unterricht.UntisId).FirstOrDefault();

            /* Der Unterricht wurde nicht gefunden (falscher Wochentag, schon verschoben, ...) */
            if (originalUnterricht == null) { return false; }
            /* Der Lehrer hat schon Unterricht. Doppelbelegungen sind zwar in der selben Klasse möglich
             * (Mitbetreuung von Gruppen), aber dieses Detail kann ignoriert werden. */
            if (GetRealstunden(neuerTag).Any(r => r.Lehrer == unterricht.Lehrer && r.Stunde == stunde)) { return false; }
            /* Die Klasse hat schon Unterricht */
            if (GetRealstunden(neuerTag).Any(r => r.Klasse == unterricht.Klasse && r.Stunde == stunde)) { return false; }
            /* Der Raum ist nicht frei */
            if (GetRealstunden(neuerTag).Any(r => r.Raum == unterricht.Raum && r.Stunde == stunde)) { return false; }

            originalUnterricht.Tag = neuerTag;
            originalUnterricht.Stunde = stunde;
            return ErstelleVertretung(originalUnterricht);
        }
    }
}
