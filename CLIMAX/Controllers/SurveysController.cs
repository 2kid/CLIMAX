
using CLIMAX.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace CLIMAX.Controllers
{
    public class SurveysController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Surveys
        [Authorize]
        public ActionResult Index()
        {
            var surveys = db.Surveys.Include(s => s.Treatments);
            return View(surveys.ToList());
        }

        // GET: Surveys/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }

        // GET: Surveys/Create
        [AllowAnonymous]
        public ActionResult Create(string SurveyCodeId, string Code)
        {
            ViewBag.TreatmentID = new SelectList(db.Treatments.Where(r=>r.isEnabled).ToList(), "TreatmentsID", "TreatmentName");
            ViewBag.SurveyCodeID = SurveyCodeId;
            ViewBag.Code = Code;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "FirstName,MiddleName,LastName,StarRating,Comment,TreatmentID")] Survey survey, FormCollection form)
        {
            int id;
            if (int.TryParse(form["SurveyCodeID"], out id))
            {
                SurveyCode surveyCode = db.SurveyCode.Where(r => r.SurveyCodeID == id).Single();
                if (surveyCode != null)
                {
                    if (surveyCode.Code == form["SurveyCode"] && surveyCode.isUsed == false)
                    {
                        surveyCode.isUsed = true;
                        db.Entry(surveyCode).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        
                        db.Surveys.Add(survey);
                        await db.SaveChangesAsync();
         
                       
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return RedirectToAction("ConfirmCode", "Surveys");
        }

        public ActionResult ConfirmCode()
        {
          return  View();
        }

        [HttpPost]
        public ActionResult ConfirmCode([Bind(Include = "SurveyCodeID,Code")] SurveyCode surveyCode)
        {
            if (ModelState.IsValid)
            {
                SurveyCode survey = db.SurveyCode.Find(surveyCode.SurveyCodeID);
                if (survey != null)
                {
                    if (survey.Code == surveyCode.Code && survey.isUsed == false)
                    {                    
                        return RedirectToAction("Create", "Surveys", new { SurveyCodeId = surveyCode.SurveyCodeID, Code = surveyCode.Code });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Confirmation code is invalid or has already been used");              
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Confirmation code is invalid or has already been used");            
                }
            }
           return View();
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
