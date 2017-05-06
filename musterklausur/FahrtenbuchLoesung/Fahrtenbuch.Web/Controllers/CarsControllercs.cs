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
    public class CarsController : ApiController
    {
        private FahrtenbuchModel db = new FahrtenbuchModel();

        /// <summary>
        /// Liefert alle Autos.
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetCars()
        {
            /* Die Navigation Properties müssen wir ausschließen, sonst kommt es
             * zu einer zyklischen Referenz. Die Bezeichnung der Properties können wir auch
             * weglassen, wenn sie gleich wie in der Vehicles Klasse sind. */
            return Ok(db.Vehicles.Select(car => new
            {
                ID = car.ID,
                Type = car.Type,
                Brand = car.Brand,
                Model = car.Model,
                NumberPlate = car.NumberPlate,
                SeatNr = car.SeatNr,
                Km = car.Km,
                IncludedKm = car.IncludedKm,
                PricePer100Km = car.PricePer100Km,
                PenaltyPerDay = car.PenaltyPerDay
            }));
        }
        // Testaufruf: /api/Cars/BN-101-VM
        [Route("api/Cars/{numberplate}")]
        [HttpGet]
        public IHttpActionResult GetCars(string numberplate)
        {
            Vehicle car = db.Vehicles.Where(v => v.NumberPlate == numberplate).FirstOrDefault();
            /* Immer gleich bei null raus, sonst wirft das LINQ Statement einen Laufzeitfehler. 
             * Die Zurückgabe mit Ok, also HTTP 200, und einer leeren Liste, also Ok(), wäre 
             * auch in Ordnung. */
            if (car == null) { return Conflict(); }

            return Ok(new
            {
                ID = car.ID,
                Type = car.Type,
                Brand = car.Brand,
                Model = car.Model,
                NumberPlate = car.NumberPlate,
                SeatNr = car.SeatNr,
                Km = car.Km,
                IncludedKm = car.IncludedKm,
                PricePer100Km = car.PricePer100Km,
                PenaltyPerDay = car.PenaltyPerDay
            });
        }

        /// <summary>
        /// Liefert alle Verfügbaren Autos in einem Zeitraum. Dafür verwenden wir
        /// usere Methode GetAvailableCars im Model mit einem Suchfilter, der nur
        /// das Von und Bisdatum enthält.
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        [Route("api/Cars/Available/{dateFrom}/{dateTo}")]
        [HttpGet]
        public IHttpActionResult AvailableCars(string dateFrom, string dateTo)
        {
            DateTime dateFromParsed, dateToParsed;

            /* Wichtig: Parsen, da über HTTP nur Strings kommen. Ist das Datum ungültig,
             * liefern wir Bad Request. Die Parse Methode von DateTime kann alle möglichen 
             * Datumsformen interpretieren, also YYYY-MM-DD oder DD.MM.YYYY */
            if (!DateTime.TryParse(dateFrom, out dateFromParsed)) return BadRequest();
            if (!DateTime.TryParse(dateTo, out dateToParsed)) return BadRequest();

            SearchFilter f = new SearchFilter()
            {
                StartDate = dateFromParsed,
                EndDate = dateToParsed
            };
            /* Wird kein Auto gefunden, wird NICHT null geliefert, sondern eine leere Liste. */
            IEnumerable<Vehicle> available = db.GetAvailableCars(f);
            /* Der selbe anonyme Typ wie oben wird verwendet. Dieses bisschen Redundanz ist aber
             * in Ordnung, da anonyme Typen nur als Object zurückgegeben werden können. */
            return Ok(available.Select(a => new
            {
                ID = a.ID,
                Type = a.Type,
                Brand = a.Brand,
                Model = a.Model,
                NumberPlate = a.NumberPlate,
                SeatNr = a.SeatNr,
                Km = a.Km,
                IncludedKm = a.IncludedKm,
                PricePer100Km = a.PricePer100Km,
                PenaltyPerDay = a.PenaltyPerDay
            }));
        }

        /// <summary>
        /// Sollte nicht gelöscht werden, sonst wird die Datenbankverbindung nicht beendet!
        /// Das macht das db.Dispose()
        /// </summary>
        /// <param name="disposing"></param>
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