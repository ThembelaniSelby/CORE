using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class GradesController : Controller
    {

            private Db db = new Db();

            // GET: Admin/Grades
            public async Task<ActionResult> Index()
            {
                Grade grade = new Grade();
                if (!db.Grades.Any())
                {
                    for (int i = 1; i < 13; i++)
                    {
                        grade.Number = i;
                        grade.Name = "GRADE " + grade.Number.ToString();
                        db.Grades.Add(grade);
                        await db.SaveChangesAsync();
                    }
                }


                return View(await db.Grades.ToListAsync());
            }

            // GET: Admin/Grades/Details/5
            public async Task<ActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Grade grade = await db.Grades.FindAsync(id);
                if (grade == null)
                {
                    return HttpNotFound();
                }
                return View(grade);
            }

            // GET: Admin/Grades/Create
            //public ActionResult Create()
            //{
            //    return View();
            //}

            //// POST: Admin/Grades/Create
            //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            //[HttpPost]
            //[ValidateAntiForgeryToken]
            //public async Task<ActionResult> Create([Bind(Include = "Id,Name,Number")] Grade grade)
            //{
            //    if (ModelState.IsValid)
            //    {
            //        for(int i = 0; i < 13; i++)
            //        {
            //            grade.Name = "GRADE " + grade.Number.ToString();
            //            db.Grades.Add(grade);
            //            await db.SaveChangesAsync();
            //        }

            //        return RedirectToAction("Index");
            //    }

            //    return View(grade);
            //}

            //// GET: Admin/Grades/Edit/5
            //public async Task<ActionResult> Edit(int? id)
            //{
            //    if (id == null)
            //    {
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    }
            //    Grade grade = await db.Grades.FindAsync(id);
            //    if (grade == null)
            //    {
            //        return HttpNotFound();
            //    }
            //    return View(grade);
            //}

            //// POST: Admin/Grades/Edit/5
            //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            //[HttpPost]
            //[ValidateAntiForgeryToken]
            //public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Number")] Grade grade)
            //{

            //    if (ModelState.IsValid)
            //    {
            //        grade.Name = "GRADE " + grade.Number.ToString();
            //        db.Entry(grade).State = EntityState.Modified;
            //        await db.SaveChangesAsync();
            //        return RedirectToAction("Index");
            //    }
            //    return View(grade);
            //}

            //// GET: Admin/Grades/Delete/5
            //public async Task<ActionResult> Delete(int? id)
            //{
            //    if (id == null)
            //    {
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    }
            //    Grade grade = await db.Grades.FindAsync(id);
            //    if (grade == null)
            //    {
            //        return HttpNotFound();
            //    }
            //    return View(grade);
            //}

            //// POST: Admin/Grades/Delete/5
            //[HttpPost, ActionName("Delete")]
            //[ValidateAntiForgeryToken]
            //public async Task<ActionResult> DeleteConfirmed(int id)
            //{
            //    Grade grade = await db.Grades.FindAsync(id);
            //    db.Grades.Remove(grade);
            //    await db.SaveChangesAsync();
            //    return RedirectToAction("Index");
            //}



            public async Task<ActionResult> Addsubject(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Grade grade = await db.Grades.FindAsync(id);
                if (grade == null)
                {
                    return HttpNotFound();
                }

                TempData["id"] = id;

                return View(grade);
            }




            public ActionResult Subjects()
            {
                return View(db.Subjects.ToList());
            }


            public ActionResult SubAddtograde(int? subid, int? id)
            {

                var gr = db.Grades.Find(id);
                var sub = db.Subjects.Find(subid);

                if (!db.Gradesubjects.Any(x => x.SubjectNameFix.Equals(sub.Name.Replace(" ", "-").ToLower().Replace("&", "-")) && x.Gradeid.Equals(gr.Id)))
                {
                    Gradesubject g = new Gradesubject();
                    g.Gradeid = gr.Id;
                    g.GradeName = gr.Name;
                    g.GradeNumber = gr.Number;
                    g.Subid = sub.Id;
                    g.SubjectName = sub.Name;
                    g.SubjectNameFix = sub.NameFix;

                    db.Gradesubjects.Add(g);
                    db.SaveChanges();

                    TempData["Success"] = "Subject added";
                    return Redirect("/Admin/Grades/Addsubject?id=" + id);
                }
                else
                {
                    TempData["Success"] = "This subject exit for this grade";
                    return Redirect("/Admin/Grades/Addsubject?id=" + id);
                }

            }



            public ActionResult Gradesubjects(int id)
            {
                Grade g = db.Grades.Find(id);
                return View(db.Gradesubjects.Where(x => x.GradeNumber == g.Number).ToList());
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