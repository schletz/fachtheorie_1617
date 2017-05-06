using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fahrtenbuch.Library;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fahrtenbuch.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckDb()
        {
            using (var db = new FahrtenbuchModel())
            {
                List<Employee> erg = db.Employees.ToList();
                Assert.AreEqual<int>(3, erg.Count);
            }
        }
        [TestMethod]
        public void GetAvailableCarsTest()
        {
            using (FahrtenbuchModel db = new FahrtenbuchModel())
            {
                /* Nach Durchsicht der Daten in Excel
                 * ist das Auto BN-569-SV vom 20. bis 25.02.
                 * reserviert. Daher wollen wir eine Reservierung für dieses
                 * Modell und diesen Typ mit diesen Sitzplätzen, damit
                 * auch sicher kein Auto im ersten Schritt gefunden wird. */
                SearchFilter f = new SearchFilter
                {
                    StartDate = DateTime.Parse("2015-02-21T00:00:00"),
                    EndDate = DateTime.Parse("2015-02-22T23:59:59"),
                    SeatCount = 7,
                    Model = "S200",
                    Brand = "VW"
                };


                IList<Vehicle> availableCars =
                    db.GetAvailableCars(f).ToList();
                /* Testcase wurde durch den Debugger
                 * ermittelt. */
                Assert.AreEqual<string>("BN-779-UP", 
                    availableCars[0].NumberPlate);

           }
        }

        [TestMethod]
        public void StartTripTest()
        {
            const string numberPlate = "BN-779-UP";
            const string svnr = "1499593323";
            DateTime startDate = DateTime.Parse("2017-07-01");

            using (var db = new FahrtenbuchModel())
            {
                /* Alle Trips und alle Reservierungen ab dem Testtag löschen. Wichtig, damit die Unittests
                 * auch mehrfach aufgerufen werden können. */
                db.Trips.RemoveRange(db.Trips.Where(t => t.StartDate >= startDate));
                db.Reservations.RemoveRange(db.Reservations.Where(r => r.DateFrom >= startDate));
                db.SaveChanges();

                /* Eine Reservierung ausstellen, sonst kann der Trip nicht gestartet werden. */
                db.Reservations.Add(new Reservation()
                {
                    ID = Guid.NewGuid(),
                    VehicleID = db.Vehicles.Where(v => v.NumberPlate == numberPlate).Select(v => v.ID).FirstOrDefault(),
                    EmployeeID = db.Employees.Where(e => e.SVNR == svnr).Select(e => e.ID).FirstOrDefault(),
                    DateFrom = startDate,
                    DateTo = startDate.AddDays(7)
                });
                db.SaveChanges();
            }
            /* Zum Aktualisieren aller Navigation Properties den dbContext neu laden. */
            using (var db = new FahrtenbuchModel())
            {
                /* Trip starten. */
                db.StartTrip(numberPlate, svnr, startDate);
                /* In LINQ to SQL können wir nicht t.StartDate.Date für den Datumsteil nehmen, da
                 * dies nicht in SQL umgewandelt werden kann. Wir brauchen daher DbFunctions.TruncateTime. */
                Assert.AreEqual(1, db.Trips.Count(t => t.Vehicle.NumberPlate == numberPlate
                    && t.StartDate == startDate && t.Employee.SVNR == svnr));
            }
        }

        [TestMethod]
        public void ExtendReservationTest()
        {
            const string numberPlate = "BN-779-UP";
            Guid reservationToExend = Guid.NewGuid();
            /* Wir können kein Parse in LINQ to SQL aufrufen, also speichern wir das Datum vorher. */
            DateTime startDate = DateTime.Parse("2017-07-01");

            using (var db = new FahrtenbuchModel())
            {
                /* Alle Testreservierungen löschen. */
                db.Reservations.RemoveRange(db.Reservations.Where(r => r.DateFrom >= startDate));
                db.SaveChanges();

                db.Reservations.Add(new Reservation()
                {
                    ID = reservationToExend,
                    VehicleID = db.Vehicles.Where(v => v.NumberPlate == numberPlate).Select(v => v.ID).FirstOrDefault(),
                    EmployeeID = db.Employees.Select(e => e.ID).FirstOrDefault(),
                    DateFrom = startDate,
                    DateTo = DateTime.Parse("2017-07-06T23:59:59")
                });
                /* Eine zweite Reservierung ausstellen, die 1 Tag nach Ende dieser Reservierung beginnt. */
                db.Reservations.Add(new Reservation()
                {
                    ID = Guid.NewGuid(),
                    VehicleID = db.Vehicles.Where(v => v.NumberPlate == numberPlate).Select(v => v.ID).FirstOrDefault(),
                    EmployeeID = db.Employees.Select(e => e.ID).FirstOrDefault(),
                    DateFrom = DateTime.Parse("2017-07-08"),
                    DateTo = DateTime.Parse("2017-07-18 23:59:59")
                });

                db.SaveChanges();
            }
            /* Zum Aktualisieren aller Navigation Properties den dbContext neu laden. */
            using (var db = new FahrtenbuchModel())
            {
                Assert.AreEqual<bool>(false,
                    db.ExtendReservation(reservationToExend, DateTime.Parse("2017-07-10")));
                Assert.AreEqual<bool>(true,
                    db.ExtendReservation(reservationToExend, DateTime.Parse("2017-07-07 23:59:59")));
                Assert.AreEqual<DateTime>(DateTime.Parse("2017-07-07 23:59:59"),
                    db.Reservations.Find(reservationToExend).DateTo);
            }
        }

        [TestMethod]
        public void ReturnVehicleTest()
        {
            const string numberPlate = "BN-779-UP";
            const string svnr = "1499593323";
            DateTime startDate = DateTime.Parse("2017-07-01");

            using (var db = new FahrtenbuchModel())
            {
                /* Alle Testreservierungen löschen. Das startDate muss in einer Variable sein,
                 * da bei LINQ to SQL nicht direkt ein Parse aufgerufen werden kann. */
                db.Trips.RemoveRange(db.Trips.Where(t => t.StartDate >= startDate));
                db.Reservations.RemoveRange(db.Reservations.Where(r => r.DateFrom >= startDate));
                db.SaveChanges();

                db.Reservations.Add(new Reservation()
                {
                    ID = Guid.NewGuid(),
                    VehicleID = db.Vehicles.Where(v => v.NumberPlate == numberPlate).Select(v => v.ID).FirstOrDefault(),
                    EmployeeID = db.Employees.Where(e => e.SVNR == svnr).Select(e => e.ID).FirstOrDefault(),
                    DateFrom = startDate,
                    DateTo = DateTime.Parse("2017-07-06 23:59:59")
                });
                db.SaveChanges();
            }
            /* Zum Aktualisieren aller Navigation Properties den dbContext neu laden. */
            using (var db = new FahrtenbuchModel())
            {
                decimal kmEnd = db.Vehicles.Where(v => v.NumberPlate == numberPlate).Select(v => v.Km).FirstOrDefault();
                /* Trip starten. */
                if (!db.StartTrip(numberPlate, svnr, startDate))
                {
                    Assert.Fail();
                    return;
                }
                decimal? price = db.ReturnVehicle(numberPlate, kmEnd + 5000, DateTime.Parse("2017-07-08 23:59:59"));
                if (price == null)
                {
                    Assert.Fail();
                    return;
                }
                /* Das Fahrzeug 2 Tage verspätet zurückbringen */
                Assert.AreEqual<decimal>(1680, (decimal)price);

            }
        }
    }
}
