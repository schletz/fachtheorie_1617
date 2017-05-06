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
    public class TestController : ApiController
    {
        private UnterrichtDb db = new UnterrichtDb();

        // GET: api/Test
        public IHttpActionResult GetTest()
        {
            return Ok(new { Count = db.Lehrers.Count() });
        }

        /// <summary>
        /// Zählt die Unterrichtsstunden pro Klasse.
        /// Testaufruf: /api/Test/CountLessons/3ahif
        /// </summary>
        /// <param name="klasse">Der Klassenname (z. B. 3AHIF)</param>
        /// <returns></returns>
        [Route("api/Test/CountLessons/{klasse}")]
        [HttpGet]
        public IHttpActionResult CountClasses(string klasse)
        {
            return Ok(new { Count = db.Unterrichtsstundens.Where(u => u.Klasse == klasse).Count() });
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