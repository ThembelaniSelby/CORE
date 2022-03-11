using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class TeacherController : Controller
    {
        Db db = new Db();
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MySubjects(string grade, string subject)
        {
            if (grade != null && subject != null)
            {
                int g = int.Parse(grade);
                var sub = db.Subjectteacher.Where(x => x.TeacherName == User.Identity.Name && (x.GradeNumber == g || x.SubjectNameFix == subject)).ToList();

                return View(sub);

            }
            if (grade == null && subject != null)
            {
                var sub = db.Subjectteacher.Where(x => x.TeacherName == User.Identity.Name && x.SubjectNameFix == subject).ToList();

                return View(sub);

            }

            if (grade != null && subject == null)
            {
                int g = int.Parse(grade);
                var sub = db.Subjectteacher.Where(x => x.TeacherName == User.Identity.Name && x.GradeNumber == g).ToList();

                return View(sub);

            }
            else
            {
                var sub = db.Subjectteacher.Where(x => x.TeacherName == User.Identity.Name).ToList();

                return View(sub);
            }

        }

        public ActionResult Assessments(string subject, string grade)
        {
            var ass = db.Assessments.Where(x => x.Subject == subject && x.Grade == grade && x.Teacher == User.Identity.Name).ToList();

            return View(ass);
        }
        [HttpGet]
        public ActionResult AddAssessment(string subject, string grade)
        {
            int min = 0;
            var gmin = db.Assessments.Where(x => x.Subject == subject && x.Grade == grade).ToList();
            foreach (var item in gmin)
            {
                min = gmin.Sum(x => x.WeightInPercent);
            }

            ViewBag.subject = subject;
            ViewBag.grade = grade;
            ViewBag.max = 100 - min;


            return View();
        }
        [HttpPost]
        public ActionResult AddAssessment(Assessment model, string subject, string grade)
        {

            Assessment ass = new Assessment()
            {
                AssessmentDate = model.AssessmentDate,
                DurationInHours = model.DurationInHours,
                AssessmentEndDate = model.AssessmentDate.AddHours(model.DurationInHours),
                Grade = model.Grade,
                Subject = model.Subject,
                Teacher = User.Identity.Name,
                Name = model.Subject.ToUpper() + " " + model.Name,
                TotalMarks = 0,
                WeightInPercent = model.WeightInPercent,
                Status = "NEW",
            };
            db.Assessments.Add(ass);
            db.SaveChanges();

            return Redirect("/Teacher/AddAssessmentQuestion?id=" + ass.Id);
        }



        [HttpGet]
        public ActionResult AddAssessmentQuestion(int id)
        {
            Assessment ass = db.Assessments.Find(id);

            ViewBag.AssId = ass.Id;
            ViewBag.Subject = ass.Subject;
            return View();
        }


        [HttpPost]
        public ActionResult AddAssessmentQuestion(AssessmentQuestion model)
        {

            AssessmentQuestion asq = new AssessmentQuestion()
            {
                AssessmentId = model.AssessmentId,
                Subject = model.Subject,
                Question = model.Question,
                Answer = model.Answer,
                Marks = model.Marks,

            };
            db.AssessmentQuestions.Add(asq);
            db.SaveChanges();

            Assessment ass = db.Assessments.Find(model.AssessmentId);
            ass.TotalMarks = ass.TotalMarks + model.Marks;
            db.SaveChanges();

            TempData["Success"] = "Question added to assessment";

            return Redirect("/Teacher/QuestionPosibleAnswer?questionid=" + asq.Id);
        }

        [HttpGet]
        public ActionResult QuestionPosibleAnswer(int questionid)
        {
            var quan = db.QuestionPosibleAnswers.Where(x => x.QuestionId == questionid).ToList();

            if (quan.Count < 1)
            {
                var qua = db.AssessmentQuestions.Find(questionid);

                QuestionPosibleAnswer qps = new QuestionPosibleAnswer()
                {
                    Answer = qua.Answer,
                    AssessmentId = qua.AssessmentId,
                    QuestionId = qua.Id,
                    Subject = qua.Subject,
                    Status = "CORRECT",
                };
                db.QuestionPosibleAnswers.Add(qps);
                db.SaveChanges();


                TempData["Success"] = "Add Another Posible Answer";
                ViewBag.Assid = qua.AssessmentId;
                ViewBag.Subject = qua.Subject;
                ViewBag.Qid = questionid;
                ViewBag.Q = qua.Question;


                return View();
            }
            else
            {
                var qua = db.AssessmentQuestions.Find(questionid);


                TempData["Success"] = "Add Another Posible Answer";
                ViewBag.Assid = qua.AssessmentId;
                ViewBag.Subject = qua.Subject;
                ViewBag.Qid = questionid;
                ViewBag.Q = qua.Question;
                return View();

            }

        }

        [HttpPost]
        public ActionResult QuestionPosibleAnswer(int questionid, QuestionPosibleAnswer model)
        {

            QuestionPosibleAnswer qps = new QuestionPosibleAnswer()
            {
                Answer = model.Answer,
                Status = "INCORRECT",
                AssessmentId = model.AssessmentId,
                QuestionId = model.QuestionId,
                Subject = model.Subject,
            };
            db.QuestionPosibleAnswers.Add(qps);
            db.SaveChanges();
            TempData["Success"] = "Question and Posible Answers added to assessment";


            return Redirect("/teacher/assessment?id=" + model.AssessmentId);
        }



        public ActionResult Assessment(int id)
        {
            var ass = db.Assessments.Where(x => x.Id == id).ToList();

            return View(ass);
        }



        public ActionResult AssessmentQuestions(int id)
        {
            var assq = db.AssessmentQuestions.Where(x => x.AssessmentId == id).ToList();

            List<QuestionAndAnswers> ordersForUser = new List<QuestionAndAnswers>();




            using (Db db = new Db())
            {

                var assessmentq = db.AssessmentQuestions.Where(x => x.AssessmentId == id).ToList();

                //loop through List Of OrderVM
                foreach (var assitem in assessmentq)
                {
                    //Intialize Product Dictionary
                    Dictionary<string, string> productsAndQty = new Dictionary<string, string>();
                    //declare total

                    //Intialize List Of OrderDetailsDTO

                    var assessmentp = db.QuestionPosibleAnswers.Where(x => x.QuestionId == assitem.Id).ToList();

                    //loop through List Of OrderDetailsDTO
                    foreach (var orderDetails in assessmentp)
                    {

                        //Get The Product
                        //Get The Product Price
                        //Get The Product Name
                        string productName = orderDetails.Answer;
                        string status = orderDetails.Status;
                        //Add to Product dictionary
                        productsAndQty.Add(productName, status);


                    }
                    //Add to ordersforuserVM List
                    ordersForUser.Add(new QuestionAndAnswers()
                    {

                        Question = assitem.Question,
                        Mark = assitem.Marks,
                        ProductsAndQty = productsAndQty,

                    });
                }

            }

            //Return View With List Of OrdersForUserVM

            return View(ordersForUser);


        }

        public ActionResult ApproveAssessment(int id)
        {

            Assessment ass = db.Assessments.Find(id);

            DateTime assdate = ass.AssessmentDate;


            var asss = db.Assessments.Where(x => x.Grade == ass.Grade && x.AssessmentEndDate > assdate && assdate > x.AssessmentDate).ToList();
            if (asss.Count() == 0)
            {
                ass.Status = "APPROVED";
                db.SaveChanges();

                var st = db.Subjectteacher.Where(x => x.TeacherName == ass.Teacher).FirstOrDefault();

                AssessmentEvent ev = new AssessmentEvent()
                {
                    Subject = ass.Subject,
                    Description = ass.Name,
                    Start = ass.AssessmentDate,
                    End = ass.AssessmentEndDate,
                    ThemeColor = "Navy",
                    IsFullDay = false,
                    GradeNumber = int.Parse(ass.Grade),
                    TeacherId = int.Parse(st.TeacherId),
                    TeacherName = st.TeacherName,
                    subjectTeacherId = st.Id,
                    link = ass.Id.ToString(),

                };
                db.AssessmentEvents.Add(ev);
                db.SaveChanges();

                TempData["Success"] = "Assessment approved added & to assessment calender";
                return Redirect("/Teacher/Assessment?id=" + id);

            }
            else
            {
                TempData["Error"] = "Assessment Reject System detected assessment class for this grade, Please ajust date To Later Than: " + asss.FirstOrDefault().AssessmentEndDate;
                return Redirect("/Teacher/Assessment?id=" + id);
            }

        }

        public ActionResult AjustAssessment(int id)
        {
            Assessment ass = db.Assessments.Find(id);

            return View(ass);
        }

        [HttpPost]
        public ActionResult AjustAssessment(Assessment model)
        {
            Assessment ass = db.Assessments.Find(model.Id);
            ass.AssessmentDate = model.AssessmentDate;
            ass.AssessmentEndDate = model.AssessmentDate.AddHours(model.DurationInHours);
            ass.DurationInHours = model.DurationInHours;
            db.SaveChanges();

            TempData["Success"] = "Assessment updated try to approve again";


            return Redirect("/teacher/assessment?id=" + model.Id);
        }



        public ActionResult Calender()
        {
            var TNAME = User.Identity.Name;
            var sub = db.Subjectteacher.Where(x => x.TeacherName == User.Identity.Name).FirstOrDefault();

            var grade = db.Subjectteacher.Find(sub.Id);
            return View(grade);
        }
        public JsonResult GetMyEvents()
        {
            using (Db dc = new Db())
            {
                var events = dc.Events.Where(x => x.TeacherName == User.Identity.Name).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }



        public ActionResult Disciplined(int id)
        {
            AssessmentSecurity assec = db.AssessmentSecurities.Find(id);
            db.AssessmentSecurities.Remove(assec);
            db.SaveChanges();
            return RedirectToAction("deciplinary");
        }



        public ActionResult deciplinary()
        {
            var ct = db.AssessmentSecurities.Where(x => x.Status == "CHEATED").ToList();

            return View(ct);
        }



        public ActionResult GetAssessmentMarks(int id)
        {
            List<AssessmentMark> sm = new List<AssessmentMark>();

            var sess = db.StudentAssessmentSessions.Where(x => x.LearnerName == x.LearnerName && x.link == id.ToString() && x.CorrectOrWrong =="CORRECT").ToList();

            Assessment ass = db.Assessments.Find(id);

            ViewBag.Tot = ass.TotalMarks;

            foreach(var item in sess.GroupBy(x => x.LearnerName,(key,items) =>new { Name =key,Mark =items.Sum(x => x.Mark) }))
            {
                sm.Add(new AssessmentMark()
                {
                    LearnerName = item.Name,
                    Mark = item.Mark,
                    link = id.ToString(),
                    PercentOf100 =item.Mark/ass.WeightInPercent*100,
                    Statust ="SUBMITED"
                
                });
            }
            ViewBag.id = id;

            return View(sm);
        }

        public ActionResult ApproveMarks(int id)
        {
            List<AssessmentMark> sm = new List<AssessmentMark>();

            var sess = db.StudentAssessmentSessions.Where(x => x.LearnerName == x.LearnerName && x.link == id.ToString() && x.CorrectOrWrong == "CORRECT").ToList();

            Assessment ass = db.Assessments.Find(id);

            int gr = int.Parse(ass.Grade);

            ViewBag.Tot = ass.TotalMarks;

            foreach (var item in sess.GroupBy(x => x.LearnerName, (key, items) => new { Name = key, Mark = items.Sum(x => x.Mark) }))
            {
                sm.Add(new AssessmentMark()
                {
                    LearnerName = item.Name,
                    Mark = item.Mark,
                    link = id.ToString(),
                    PercentOf100 = item.Mark / ass.WeightInPercent * 100,
                    Statust = "SUBMITED"

                });

          

                    var mark = db.SubjectMarks.Where(x => x.LearnerName == item.Name && x.Grade ==gr && x.SubjectName ==ass.Subject).ToList();
                


                    if (mark != null)
                    {
                        SubjectMark m = new SubjectMark()
                        {
                            LearnerName = item.Name,
                            Grade =  int.Parse(ass.Grade),
                            SubjectLower = ass.Subject.ToLower(),
                            SubjectName = ass.Subject,
                            YearMark = item.Mark / ass.WeightInPercent * 100

                        };
                        db.SubjectMarks.Add(m);
                        db.SaveChanges();
                    }
                
            }



            return Redirect("Marks");
        }



        public ActionResult Marks()
        {


            var sess = db.StudentAssessmentSessions.ToList().OrderByDescending(x => x.LearnerName);
            return View(sess);
        }

    }
}