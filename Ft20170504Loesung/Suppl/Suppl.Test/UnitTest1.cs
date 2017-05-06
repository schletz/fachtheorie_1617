using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Suppl.Library;
using System.Linq;
using System.Collections.Generic;

namespace Suppl.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDb()
        {
            using (UnterrichtDb db = new UnterrichtDb())
            {
                Assert.AreEqual<int>(228, db.Lehrers.Count());
            }
        }

        /// <summary>
        /// Löscht alle Vertretungsdaten nach dem 1.7. Diese entstehen nur durch Tests.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            using (UnterrichtDb db = new UnterrichtDb())
            {
                db.Vertretungens.RemoveRange(db.Vertretungens.Where(v => v.Tag > new DateTime(2017, 7, 1)));
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Löscht alle Vertretungsdaten nach dem 1.7. Diese entstehen nur durch Tests.
        /// </summary>
        [TestCleanup()]
        public void Clean()
        {
            using (UnterrichtDb db = new UnterrichtDb())
            {
                db.Vertretungens.RemoveRange(db.Vertretungens.Where(v => v.Tag > new DateTime(2017, 7, 1)));
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void GetRealstundenTest()
        {

            using (UnterrichtDb db = new UnterrichtDb())
            {
                IList<Realstunde> realstunden = db.GetRealstunden(new DateTime(2017, 3, 13)).ToList();
                Assert.AreEqual<int>(0, realstunden.Where(r => r.Klasse == "4BHIF").Count());
                Assert.AreEqual<int>(10, realstunden.Where(r => r.Klasse == "4AHIF").Count());
                Assert.AreEqual<int>(0, realstunden.Where(r => r.Lehrer == "UK").Count());

                realstunden = db.GetRealstunden(new DateTime(2017, 7, 4)).ToList();
                Assert.AreEqual<int>(6, realstunden.Where(r => r.Lehrer == "SZ").Count());
                Assert.AreEqual<int>(6, db.Krankmeldung(new DateTime(2017, 7, 4), "SZ"));
                Assert.AreEqual<int>(0, db.Krankmeldung(new DateTime(2017, 7, 4), "SZ"));

                Assert.AreEqual(true, db.Raumaenderung("5CHIF", new DateTime(2017, 7, 7), 3, "B1.15"));
                /* Da die Klasse wegverschoben wurde, kann sie nun erfolgreich wieder in C5.06 geschoben werden. */
                Assert.AreEqual(true, db.Raumaenderung("5CHIF", new DateTime(2017, 7, 7), 3, "C5.06"));
                Assert.AreEqual(false, db.Raumaenderung("5CHIF", new DateTime(2017, 7, 7), 2, "C5.07"));

                Unterrichtsstunden testUnterricht = db.Unterrichtsstundens
                    .Where(u => u.Lehrer == "SZ" && u.Wochentag == 5 && u.Stunde == 3)
                    .FirstOrDefault();

                Assert.AreEqual(true, db.Unterrichtsverlegung(testUnterricht, new DateTime(2017, 7, 7), new DateTime(2017, 7, 9), 1));
                /* false, da der Unterricht am 7. ja schon verschoben wurde. */
                Assert.AreEqual(false, db.Unterrichtsverlegung(testUnterricht, new DateTime(2017, 7, 7), new DateTime(2017, 7, 16), 1));
                /* Der verlegte Unterricht vom 7., der jetzt am 9. liegt, wird auf den 16.7. verschoben. */
                Assert.AreEqual(true, db.Unterrichtsverlegung(testUnterricht, new DateTime(2017, 7, 9), new DateTime(2017, 7, 16), 1));
                /* false, da am 16. schon der Lehrer belegt ist. */
                Assert.AreEqual(false, db.Unterrichtsverlegung(testUnterricht, new DateTime(2017, 7, 14), new DateTime(2017, 7, 16), 1));

                int anzRaeume = db.Unterrichtsstundens.Select(u => u.Raum).Distinct().Count();
                Assert.AreEqual(anzRaeume - 1, db.Raumsuche(new DateTime(2017, 7, 16), 1).Count());
            }
        }
    }
}
