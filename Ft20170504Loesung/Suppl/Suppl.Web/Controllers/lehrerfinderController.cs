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
using Suppl.Library;

namespace Suppl.Web.Controllers
{
    public class Suppliervorschlag
    {
        public string Priority { get; set; }
        public IEnumerable<string> LehrerId { get; set; }
    }
    public class lehrerfinderController : ApiController
    {
        private UnterrichtDb db = new UnterrichtDb();

        private Suppliervorschlag findeSupplierung(int untisId)
        {
            Unterrichtsstunden unt = db.Unterrichtsstundens.Find(untisId);
            if (unt == null) { return null; }

            /* Infrage kommen alle Lehrer, die an diesem Wochentag überhaupt unterricht haben.
             * Außerdem dürfen sie nicht in der betreffenden Stunde unterrichten.
             * Eine Abfrage auf != unt.Stunde ist allerdings falsch, denn so werden Lehrer,
             * die auch andere Stunden am Tag unterrichten, trotzdem zurückgegeben. */
            var alleLehrer = from l in (db.Unterrichtsstundens
                                         .Where(u => u.Wochentag == unt.Wochentag)
                                         .Select(u => u.Lehrer)
                                         .Distinct())
                             where !db.Unterrichtsstundens.Any(u => u.Lehrer == l && u.Wochentag == unt.Wochentag && u.Stunde == unt.Stunde)
                             select l;

            var klassenlehrer = alleLehrer.Where(l => db.Unterrichtsstundens.Any(u => u.Klasse == unt.Klasse && u.Lehrer == l));
            if (klassenlehrer.Count() > 0) return new Suppliervorschlag() { Priority = "1", LehrerId = klassenlehrer };

            var fachlehrer = alleLehrer.Where(l => db.Unterrichtsstundens.Any(u => u.Fach == unt.Fach && u.Lehrer == l));
            if (fachlehrer.Count() > 0) return new Suppliervorschlag() { Priority = "2", LehrerId = fachlehrer };

            var hohlstunde = alleLehrer.Where(l => db.Unterrichtsstundens.Any(u => u.Wochentag == unt.Wochentag && u.Stunde < unt.Stunde) &&
                                                   db.Unterrichtsstundens.Any(u => u.Wochentag == unt.Wochentag && u.Stunde > unt.Stunde));
            if (hohlstunde.Count() > 0) return new Suppliervorschlag() { Priority = "3a", LehrerId = fachlehrer };

            return new Suppliervorschlag { Priority = "3b", LehrerId = alleLehrer };
        }

        [Route("api/lehrerfinder/{untisId}")]
        [HttpGet]

        public IHttpActionResult FindeSupplierung(string untisId)
        {
            int untisIdParsed;

            if (!int.TryParse(untisId, out untisIdParsed)) return BadRequest();
            Suppliervorschlag s = findeSupplierung(untisIdParsed);
            if (s == null) return Conflict();

            return Ok(s);
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