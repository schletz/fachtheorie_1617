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
    public class klassenstundenplanController : ApiController
    {
        private UnterrichtDb db = new UnterrichtDb();
        [Route("api/klassenstundenplan/{klasse}")]
        [HttpGet]
        public IHttpActionResult GetNormalstundenplan(string klasse)
        {
            return Ok(from u in db.Unterrichtsstundens
                      where u.Klasse == klasse
                      select new
                      {
                          u.UntisId,
                          u.Wochentag,
                          u.Stunde,
                          u.Klasse,
                          u.Lehrer,
                          u.Fach,
                          u.Raum
                      });
        }
        [Route("api/klassenstundenplan/{klasse}/{datum}")]
        [HttpGet]
        public IHttpActionResult GetTagesstundenplan(string klasse, string datum)
        {
            DateTime datumParsed;
            try
            {
                datumParsed = new DateTime(
                    int.Parse(datum.Substring(0, 4)),
                    int.Parse(datum.Substring(4, 2)),
                    int.Parse(datum.Substring(6, 2)));
            }
            catch { return BadRequest(); }
            return Ok(from r in db.GetRealstunden(datumParsed)
                      where r.Klasse == klasse
                      select new
                      {
                          r.UntisId,
                          r.Unterricht.Wochentag,
                          r.Stunde,
                          r.Klasse,
                          r.Lehrer,
                          r.Fach,
                          r.Raum
                      });
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