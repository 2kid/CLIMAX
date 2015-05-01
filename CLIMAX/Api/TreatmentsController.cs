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
using CLIMAX.Models;

namespace CLIMAX.Api
{
    public class TreatmentsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Treatments
        public IQueryable<Treatments> GetTreatments()
        {
            return db.Treatments;
        }

        //// GET: api/Treatments/5
        //[ResponseType(typeof(Treatments))]
        //public IHttpActionResult GetTreatments(int id)
        //{
        //    Treatments treatments = db.Treatments.Find(id);
        //    if (treatments == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(treatments);
        //}

        //// PUT: api/Treatments/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutTreatments(int id, Treatments treatments)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != treatments.TreatmentsID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(treatments).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TreatmentsExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/Treatments
        //[ResponseType(typeof(Treatments))]
        //public IHttpActionResult PostTreatments(Treatments treatments)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Treatments.Add(treatments);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = treatments.TreatmentsID }, treatments);
        //}

        //// Disable: api/Treatments/5
        //[ResponseType(typeof(Treatments))]
        //public IHttpActionResult DisableTreatments(int id)
        //{
        //    Treatments treatments = db.Treatments.Find(id);
        //    if (treatments == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Treatments.Remove(treatments);
        //    db.SaveChanges();

        //    return Ok(treatments);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool TreatmentsExists(int id)
        //{
        //    return db.Treatments.Count(e => e.TreatmentsID == id) > 0;
        //}
    }
}