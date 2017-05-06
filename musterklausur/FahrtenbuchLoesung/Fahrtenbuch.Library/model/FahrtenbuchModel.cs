namespace Fahrtenbuch.Library
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Collections.Generic;

    public partial class FahrtenbuchModel : DbContext
    {
        public FahrtenbuchModel()
            : base("name=FahrtenbuchConnection")
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        public IEnumerable<Vehicle> GetAvailableCars(SearchFilter f)
        {
            /* Suche alle Autos, die den Filterkriterien entsprechen */
            var availableCars = from v in Vehicles
                                where v.SeatNr == (f.SeatCount ?? v.SeatNr) &&
                                      v.Brand == (f.Brand ?? v.Brand) &&
                                      v.Model == (f.Model ?? v.Model)
                                select v;

            /* Sind diese Autos auch nicht reserviert? */
            var freeCars = from a in availableCars
                           where a.Reservations.All(r =>
                                f.EndDate <= r.DateFrom ||
                                f.StartDate >= r.DateTo
                           )
                           select a;
            if (freeCars.Count() != 0)
                return freeCars;

            /* Filter nach dem Modell ignorieren. */
            f.Model = null;
            if (freeCars.Count() != 0)
                return freeCars;

            /* Filter nach dem Hersteller ignorieren. */
            f.Brand = null;
            return freeCars;
        }


        /// <summary>
        /// Startet zu einer bestehenden Reservierung den Trip. Es wird die Reservierung gesucht, die am
        /// übergebenen Tag beginnt. Das ist auch der Starttag des Trips.
        /// Annahme: Ein Auto kann nur 1x am Tag vergeben werden.
        /// </summary>
        /// <param name="numberPlate">Kennzeichen des Autos (eindeutig)</param>
        /// <param name="svnr">SVNr des Mitarbeiters (eindeutig)</param>
        /// <param name="day">Tag, an dem der Trip beginnt. Ein Auto kann nur 1x pro Tag reserviert werden.</param>
        /// <returns>true, wenn der Trip angelegt wurde. False wenn keine Reservierung vorliegt oder ein Db Fehler auftritt.</returns>

        public bool StartTrip(string numberPlate, string svnr, DateTime day)
        {
            /* Die aktuelle Reservierung herausfinden, die der Mitarbeiter an diesem Tag für das Auto eingegeben hat. */
            Reservation currentReservation = (from r in Reservations
                                      where r.Employee.SVNR == svnr && r.Vehicle.NumberPlate == numberPlate &&
                                        DbFunctions.TruncateTime(r.DateFrom) == DbFunctions.TruncateTime(day)
                                      //r.DateFrom.Date == day.Date
                                      select r).FirstOrDefault();
            /* Das Auto mit der angegebenen Nummerntafel wurde nicht reserviert? False liefern. */
            if (currentReservation == null) { return false; }
            try
            {
                Trip t = new Trip()
                {
                    ID = Guid.NewGuid(),
                    VehicleID = currentReservation.VehicleID,
                    EmployeeID = currentReservation.EmployeeID,
                    StartDate = day,
                    EndDate = null,
                    KmBegin = currentReservation.Vehicle.Km
                };
                Trips.Add(t);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verlängert eine Reservierung, allerdings nur, wenn das Auto nicht schon reserviert wurde.
        /// Annahme: Es ist die ReservierungsId bekannt.
        /// </summary>
        /// <param name="resId">GUID der zu verlängerten Reservierung.</param>
        /// <returns>true, wenn die Reservierung erfolgreich verlängert wurde. False wenn nicht.</returns>
        public bool ExtendReservation(Guid resId, DateTime newEnddate)
        {
            Reservation res = Reservations.Find(resId);
            /* Da schon eine gültige Reservierung für das Auto da ist, müssen wir nur mehr das neue Enddatum prüfen.
             * Sprachlich formuliert: gibt es eine zukünftige Reservierung, die schon vor dem neuen Enddatum zu
             * laufen beginnt? */
            if (res.Vehicle.Reservations.Any(
                r => r.DateFrom >= res.DateTo && 
                r.DateFrom <= newEnddate))
            {
                return false;
            }
            res.DateTo = newEnddate;
            try
            {
                SaveChanges();
            }
            catch { return false;  }
            return true;
        }

        /// <summary>
        /// Schließt den Trip ab und berechnet den zu Bezahlenden Preis.
        /// </summary>
        /// <param name="numberPlate">Nummerntafel des Autos (eindeutig)</param>
        /// <param name="kmEnd">Gemessener KM Stand beim Zurückbringen.</param>
        /// <param name="endDate">Datum, an dem das Auto zurückgebracht wurde.</param>
        /// <returns>Preis in EUR</returns>
        public decimal? ReturnVehicle(string numberPlate, decimal kmEnd, DateTime endDate)
        {
            Trip currentTrip = Trips.Where(t => t.Vehicle.NumberPlate == numberPlate && t.EndDate == null).FirstOrDefault();
            if (currentTrip == null) { return null; }

            /* Den KM Stand berichtigen und das Endedatum setzen. */
            currentTrip.KmEnd = kmEnd;
            currentTrip.Vehicle.Km = kmEnd;
            currentTrip.EndDate = endDate;
            try
            {
                SaveChanges();
            }
            catch { return null; }


            return CalcPrice(currentTrip);

        }

        public decimal? CalcPrice(Trip currentTrip)
        {
            /* Annahme: Die dazupassende Reservierung ist jene Reservierung, die das gleiche Fahrzeug betrifft und die
             * am selben Tag wie der Trip startet. */
            Reservation currentReservation = Reservations.Where(
                r => r.VehicleID == currentTrip.VehicleID
                && DbFunctions.TruncateTime(r.DateFrom) == DbFunctions.TruncateTime(currentTrip.StartDate)
            ).FirstOrDefault();

            if (currentReservation == null) { return null; }

            /* Damit wir sinnvoll die NULL Werte bei decimal? Typen interpretieren, setzen wir sie in der 
             * Berechnung auf 0. */
            decimal traveledKm = ((currentTrip.KmEnd ?? currentTrip.KmBegin) - currentTrip.KmBegin);
            decimal penaltyPerDay = currentTrip.Vehicle.PenaltyPerDay ?? 0;
            decimal pricePer100km = currentTrip.Vehicle.PricePer100Km ?? 0;
            int includedKm = currentTrip.Vehicle.IncludedKm ?? 0;

            decimal price = currentTrip.Vehicle.BasicPrice
                + ((traveledKm > includedKm) ? Math.Ceiling((traveledKm - includedKm) / 100) * pricePer100km : 0)
                + penaltyPerDay * (decimal)((DateTime)currentTrip.EndDate - currentReservation.DateTo).TotalDays;
            return price;
        }
        public IEnumerable<dynamic> SearchEmployee()
        {
            return SearchEmployee(null);
        }
        public IEnumerable<dynamic> SearchEmployee(string svnr)
        {
            var erg = from e in Employees
                      select new
                      {
                          Lastname = e.Lastname,
                          Firstname = e.Firstname,
                          SVNr = e.SVNR,
                          Reservations = e.Reservations.Select(
                           r => new { r.DateFrom, r.DateTo, r.Vehicle.NumberPlate }
                          )
                      };
            if (svnr != null)
            {
                erg = erg.Where(e => e.SVNr == svnr);
            }
            return erg;
        }
    }
}
