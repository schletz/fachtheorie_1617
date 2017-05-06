using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using Suppl.Library;

namespace Suppl.Web.Controllers
{
    public class lehrerstundenplanController : ApiController
    {
        private UnterrichtDb db = new UnterrichtDb();

        [Route("api/lehrerstundenplan/{lehrer}")]
        [HttpGet]
        public IHttpActionResult GetNormalstundenplan(string lehrer)
        {
            return Ok(from u in db.Unterrichtsstundens
                      where u.Lehrer == lehrer
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
        [Route("api/lehrerstundenplan/{lehrer}/{datum}")]
        [HttpGet]
        public IHttpActionResult GetTagesstundenplan(string lehrer, string datum)
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
                      where r.Lehrer == lehrer
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