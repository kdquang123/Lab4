using Lab4.Data;
using Lab4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Lab4.Controllers
{
    public class LearnerController : Controller
    {
        private SchoolContext db;
        private static int pageSize = 2;
        public LearnerController(SchoolContext context) {
            db = context;
        }
        public IActionResult Index(int? mid,int? page)
        {
            if (page == null)
            {
                page = 1;
            }
            if(mid == null)
            {
                var learners = db.Learners.Include(m => m.Major).ToList();
                ViewBag.pageNumber = (learners.Count()%pageSize==0?learners.Count()/pageSize:learners.Count/pageSize+1);
                learners = db.Learners.Include(m => m.Major).ToList().GetRange(0,pageSize);
                return View(learners);
               /* }
                else
                {
                    var learners = db.Learners.Include(m => m.Major).ToList().GetRange((page - 1) * 2 + 1, (page - 1) * 2 + 2);
                    ViewBag.pageNumber = (int)(learners.Count() / 2);
                    return PartialView("LearnerTable", learners);
                } */  
            }
            else
            {
                var learners = db.Learners.Where(l=>l.MajorID==mid).Include(m => m.Major).ToList().GetRange(0,pageSize);
                return View(learners);
            }
        }

        public IActionResult Create()
        {
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstMidName,LastName,MajorID,EnrollmentDate")]Learner learner) 
        { 
            if (ModelState.IsValid)
            {
                db.Learners.Add(learner);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName");
            return View();
        }


        public IActionResult Edit(int id)
        {
            if(id==null || db.Learners == null)
            {
                return NotFound();
            }
            var learner = db.Learners.Find(id);
            if(learner == null)
            {
                return NotFound();
            }
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,[Bind("LearnerID,FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {
           if(id!=learner.LearnerID)
            {
                return NotFound();
            }
           if(ModelState.IsValid)
            {
                try
                {
                    db.Update(learner);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException) 
                {
                    if (!LearnerExists(learner.LearnerID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MajorId=new SelectList(db.Majors,"MajorID","MajorName",learner.MajorID);
            return View(learner);
        }

        private bool LearnerExists(int id)
        {
            return (db.Learners?.Any(e => e.LearnerID == id)).GetValueOrDefault();
        }

        public IActionResult Delete(int id)
        {
            if(id==null || db.Learners == null)
            {
                return NotFound();
            }

            var learner=db.Learners.Include(l=>l.Major)
                .Include(e=>e.Enrollments)
                .FirstOrDefault(m=>m.LearnerID==id);
            if (learner == null)
            {
                return NotFound();
            }
            if (learner.Enrollments.Count() > 0)
            {
                return Content("This learner has some enrollment, cann't delete!");
            }
            return View(learner);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (db.Learners == null)
            {
                return Problem("Entity set 'Learners' is null.");
            }
            var learner = db.Learners.Find(id);
            if(learner != null)
            {
                db.Learners.Remove(learner);
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult LearnerByMajorID(int mid)
        {
            var learnersTem=db.Learners.Where(l=>l.MajorID==mid).Include(m=>m.Major).ToList();
            var learners = (learnersTem.Count > pageSize ? learnersTem.GetRange(0, pageSize) : learnersTem.GetRange(0, learnersTem.Count));
            return PartialView("LearnerTable", learners);
        }


        public IActionResult LearnerByPage(int page)
        {
            var learners = db.Learners.Include(m => m.Major).ToList();
            int count = learners.Count();
            if(count%pageSize!=0 && page == count / pageSize+1)
            {
                learners = db.Learners.Include(m => m.Major).ToList().GetRange((page - 1) * pageSize, count%pageSize);
            }
            else
            {
                learners = db.Learners.Include(m => m.Major).ToList().GetRange((page - 1) * pageSize, pageSize);
            }

            return PartialView("LearnerTable", learners);
        }
    }
}
