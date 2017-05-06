using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Fahrtenbuch.Library;

namespace Fahrtenbuch.Web.Controllers
{
    public class EmployeeController : ApiController
    {
        private FahrtenbuchModel db = new FahrtenbuchModel();

        // GET: api/Employee
        public IQueryable<Employee> GetEmployees()
        {
            return db.Employees;
        }

        // Testrequest: /api/Employee/Reservations
        /// <summary>
        /// Liefert alle Reservierungen, die ein Mitarbeiter eingegeben hat.
        /// </summary>
        /// <returns></returns>
        [Route("api/Employee/Reservations")]
        [HttpGet]
        public IHttpActionResult GetReservations()
        {
            var result = from e in db.Employees
                         select new
                         {
                             ID = e.ID,
                             Firstname = e.Firstname,
                             Lastname = e.Lastname,
                             Reservations = e.Reservations.Select(r =>
                             new
                             {
                                 ReservationId = r.ID,
                                 StartDate = r.DateFrom,
                                 EndDate = r.DateTo,
                                 NumberPlate = r.Vehicle.NumberPlate
                             })
                         };
            return Ok(result);
        }


        // Testrequest: /api/Employee/52499e6f-cd1a-464f-b3be-cf2e9178290d/Earnings
        /// <summary>
        /// Liefert die Summe aller Einnahmen, die ein Mitarbeiter geniert hat. 
        /// Die Einkünfte sind die Preise, die die Kunden beim Beenden des Trips bezahlen
        /// müssen. Daher summieren wir mit einer Methode CalcPrice im Model einfach auf.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [Route("api/Employee/{guid}/Earnings")]
        [HttpGet]
        public IHttpActionResult CalcEarnings(string guid)
        {
            Guid guidParsed;

            if (!Guid.TryParse(guid, out guidParsed)) return BadRequest();

            /* Find kann nach dem Primärschlüssel suchen. */
            Employee e = db.Employees.Find(guidParsed);
            if (e == null) { return Conflict(); }

            /* CalcPrice kann auch null liefern. Das ist bei LINQ immer schlecht, daher
             * sichern wir uns mit ?? dagegen ab. */
            decimal earnings = e.Trips
                .Sum(t => db.CalcPrice(t) ?? 0);

            return Ok(new { ID = e.ID, Firstname = e.Firstname, Lastname = e.Lastname, Earnigs = earnings });

        }

        // Testrequest: /api/Employee/52499e6f-cd1a-464f-b3be-cf2e9178290d/Earnings/2014-01-01/2015-01-01
        /// <summary>
        /// Summiert alle Einkünfte auf, die der Mitarbeiter in einem gewissen Zeitraum
        /// generiert hat. Dafür verwenden wir das EndDate Property des Trips, da der Kunde erst
        /// bezahlt, wenn der Trip beendet wird.
        /// </summary>
        /// <param name="guid">GUID des Mitarbeiters</param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [Route("api/Employee/{guid}/Earnings/{dateFrom}/{dateTo}")]
        [HttpGet]
        public IHttpActionResult CalcEarnings(string guid, string dateFrom, string dateTo)
        {
            Guid guidParsed;
            DateTime dateFromParsed, dateToParsed;

            if (!Guid.TryParse(guid, out guidParsed)) return BadRequest();
            if (!DateTime.TryParse(dateFrom, out dateFromParsed)) return BadRequest();
            if (!DateTime.TryParse(dateTo, out dateToParsed)) return BadRequest();

            Employee e = db.Employees.Find(guidParsed);
            if (e == null) { return Conflict(); }

            /* EndDate kann auch NULL sein (offene Trips). Damit wir mit >= und einem Date
             * Wert vergleichen können, müssen wir entweder casten oder mit ?? einen 
             * Standardwert liefern. Cast liefert aber einen Laufzeitfehler, wenn EndDate
             * NULL ist. */
            decimal earnings = e.Trips
                .Where(t => (t.EndDate ?? DateTime.MinValue).Date >= dateFromParsed.Date && 
                            (t.EndDate ?? DateTime.MaxValue).Date <= dateToParsed.Date)
                .Sum(t => db.CalcPrice(t) ?? 0);

            return Ok(new { ID = e.ID, Firstname = e.Firstname, Lastname = e.Lastname, Earnigs = earnings });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}