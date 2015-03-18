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
using Newtonsoft.Json;

namespace CLIMAX.Api
{
    public class SurveysController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //// GET: api/Surveys
        //public IQueryable<Survey> GetSurveys()
        //{
        //    return db.Surveys;
        //}

        //// GET: api/Surveys/5
        //[ResponseType(typeof(Survey))]
        //public IHttpActionResult GetSurvey(int id)
        //{
        //    Survey survey = db.Surveys.Find(id);
        //    if (survey == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(survey);
        //}

        //// PUT: api/Surveys/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutSurvey(int id, Survey survey)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != survey.SurveyID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(survey).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!SurveyExists(id))
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

        // POST: api/Surveys
        [ResponseType(typeof(Survey))]
        public IHttpActionResult PostSurvey(List<string> val)
        {
            Survey survey = new Survey();
            survey.FirstName = val[0];
            survey.MiddleName = val[1];
            survey.LastName = val[2];
            int rating, treatmentId;
            if (int.TryParse(val[3], out rating))
            {
                survey.StarRating = rating;
            }
            else
            {
                return BadRequest(ModelState);
            }
            survey.Comment = val[4];
            if (int.TryParse(val[5], out treatmentId))
            {
                survey.TreatmentID = treatmentId;
            }
            else
            {
                return BadRequest(ModelState);
            }

            db.Surveys.Add(survey);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = survey.SurveyID }, survey);
        }

      
        //// DELETE: api/Surveys/5
        //[ResponseType(typeof(Survey))]
        //public IHttpActionResult DeleteSurvey(int id)
        //{
        //    Survey survey = db.Surveys.Find(id);
        //    if (survey == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Surveys.Remove(survey);
        //    db.SaveChanges();

        //    return Ok(survey);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool SurveyExists(int id)
        //{
        //    return db.Surveys.Count(e => e.SurveyID == id) > 0;
        //}
    }
}