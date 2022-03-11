using CmsShoppingCart.Models.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    [Authorize]

    public class ChildController : Controller
    {
        // GET: Child
        Db db = new Db();

        public ActionResult GetCid()
        {
            var c = db.Children.Where(x => x.ChildEmail == User.Identity.Name).FirstOrDefault();
            if(c != null)
            {
                ViewBag.Cid = c.Id;

                return View();
            }
            else
            {
                ViewBag.Cid = 0;

                return View();
            }
            
        }



        public ActionResult Cheated()
        {
            return View();
        }


        public ActionResult Accademic(int id)
        {
            var sec = db.AssessmentSecurities.Where(x => x.LName == User.Identity.Name && x.Status == "CHEATED").ToList();

            if(sec.Count() >0)
            {

                return RedirectToAction("Cheated");
            }


            Child child = db.Children.Find(id);
            int gnum = int.Parse(child.Grade);
            var gsub = db.Gradesubjects.Where(x => x.GradeNumber == gnum).ToList();
            return View(gsub);
        }



        public ActionResult MyTimetable(int id)
        {
            var grade = db.Grades.Find(id);
            return View(grade);
        }
        public JsonResult GetMyEvents(int gnum)
        {
            using (Db dc = new Db())
            {
                var events = dc.Events.Where(x => x.GradeNumber == gnum).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        public ActionResult SubjectTimetable(int gnum, string subnamefix)
        {
            var sub = db.Subjects.Where(x => x.NameFix == subnamefix).ToList().FirstOrDefault();

            //var gsub = db.Gradesubjects.Where(x => x.GradeNumber == gnum && x.SubjectName == sub.Name).ToList();

            var events = db.Events.Where(x => x.GradeNumber == gnum && x.Subject == sub.Name).ToList().FirstOrDefault();


            if (events == null)
            {
                return View();
            }
            else
            {
                var eventt = db.Events.Find(events.EventID);
                return View(eventt);

            }
        }

        public JsonResult GetSubjectEvents(int eventid)
        {
            var eventt = db.Events.Find(eventid);
            using (Db dc = new Db())
            {
                var events = dc.Events.Where(x => x.GradeNumber == eventt.GradeNumber && x.Subject == eventt.Subject).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        public ActionResult AssessmentTimetable(int id)
        {
            var grade = db.Grades.Find(id);
            return View(grade);
        }


        public JsonResult GetAssessmentEvents(int gnum)
        {
            using (Db dc = new Db())
            {
                var events = dc.AssessmentEvents.Where(x => x.GradeNumber == gnum && x.End > DateTime.Now).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }








        public ActionResult SubjectAssessmentTimetable(int gnum, string subnamefix)
        {
            var sub = db.Subjects.Where(x => x.NameFix == subnamefix).ToList().FirstOrDefault();

            //var gsub = db.Gradesubjects.Where(x => x.GradeNumber == gnum && x.SubjectName == sub.Name).ToList();

            var events = db.AssessmentEvents.Where(x => x.GradeNumber == gnum && x.Subject == sub.Name).ToList().FirstOrDefault();


            if (events == null)
            {
                return View();
            }
            else
            {
                var eventt = db.AssessmentEvents.Find(events.EventID);
                return View(eventt);

            }
        }




        public JsonResult GetSubjectAssessmentEvents(int eventid)
        {
            var eventt = db.AssessmentEvents.Find(eventid);
            using (Db dc = new Db())
            {
                var events = dc.AssessmentEvents.Where(x => x.GradeNumber == eventt.GradeNumber && x.Subject == eventt.Subject && x.End > DateTime.Now).ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }




        public ActionResult StartAssessment(int? page, int id)
        {

            var asssec = db.AssessmentSecurities.Where(x => x.AssId == id && x.LName ==User.Identity.Name &&(x .Status =="CHEATED" || x.Status== "SUBMITED")).ToList();

            if(asssec.Count()> 0)
            {
                return RedirectToAction("AssessmentNotValid");
            }
            else
            {
                AssessmentSecurity secu = new AssessmentSecurity()
                {
                    AssId = id,
                    Status = "STARTED",
                    LName = User.Identity.Name,
                };
                db.AssessmentSecurities.Add(secu);
                db.SaveChanges();
            }

            var assessmentevent = db.AssessmentEvents.Where(x => x.link == id.ToString() && x.Start < DateTime.Now && x.End > DateTime.Now).ToList();

            if (assessmentevent.Count() > 0)
            {
                var r2 = new Random();
                var assq = db.AssessmentQuestions.Where(x => x.AssessmentId == id).ToList();

                List<QuestionAndAnswers> ordersForUser = new List<QuestionAndAnswers>();

                var pageNumber = page ?? 1;


                using (Db db = new Db())
                {

                    var assessmentq = db.AssessmentQuestions.Where(x => x.AssessmentId == id).ToList().OrderBy(x => x.Id);

                    //loop through List Of OrderVM
                    foreach (var assitem in assessmentq)
                    {
                        //Intialize Product Dictionary
                        Dictionary<string, string> productsAndQty = new Dictionary<string, string>();
                        //declare total

                        //Intialize List Of OrderDetailsDTO
                        var r = new Random();
                        var assessmentp = db.QuestionPosibleAnswers.Where(x => x.QuestionId == assitem.Id).ToList().OrderBy(x => r.Next());
                        int Qid = 0;
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
                            Qid = orderDetails.QuestionId;


                        }
                        //Add to ordersforuserVM List
                        ordersForUser.Add(new QuestionAndAnswers()
                        {
                            QuestionId = Qid,
                            Question = assitem.Question,
                            Mark = assitem.Marks,
                            ProductsAndQty = productsAndQty,

                        });
                    }

                }

            ViewBag.Id = id;

                return View(ordersForUser.ToPagedList(pageNumber, 1));
        }




            else
            {
                return RedirectToAction("AssessmentNotValid");
    }


}



        public ActionResult AssessmentSession(int id)
        {
            var ass = db.Assessments.Where(x => x.Id == id && x.AssessmentDate < DateTime.Now && x.AssessmentEndDate > DateTime.Now).ToList();
            if (ass.Count > 0)
            {
                return View(ass);

            }
            else
            {
                return null;

            }
        }



        public ActionResult SaveLearnerQuestionAnswer(int qid, string answer)
        {



            


            var state = db.StudentAssessmentSessions.Where(x => x.QuestionId == qid).ToList();


            if (state.Count() > 0 && state.FirstOrDefault().LearnerName == User.Identity.Name)
            {
                int Assid = int.Parse(state.FirstOrDefault().link);

                var ass1 = db.Assessments.Where(x => x.Id == Assid && x.AssessmentDate < System.DateTime.Now && x.AssessmentEndDate > System.DateTime.Now).ToList();
                if (ass1.Count() > 0)
                {
                            int stateid = state.FirstOrDefault().Id;
                            StudentAssessmentSession stae = db.StudentAssessmentSessions.Find(stateid);
                            db.StudentAssessmentSessions.Remove(stae);
                            db.SaveChanges();

                            AssessmentQuestion asq = db.AssessmentQuestions.Find(qid);
                            Assessment ass = db.Assessments.Find(asq.AssessmentId);

                            var assevent = db.AssessmentEvents.Where(x => x.link == ass.Id.ToString()).FirstOrDefault();

                            string corrwro = "";
                            if (asq.Answer == answer)
                            {
                                corrwro = "CORRECT";
                            }
                            else
                            {
                                corrwro = "WRONG";
                            }

                            StudentAssessmentSession stasse = new StudentAssessmentSession()
                            {
                                EventID = assevent.EventID,
                                Subject = asq.Subject,
                                Description = ass.Name,
                                Start = ass.AssessmentDate,
                                End = ass.AssessmentEndDate,
                                GradeNumber = int.Parse(ass.Grade),
                                TeacherId = assevent.TeacherId,
                                TeacherName = ass.Teacher,
                                subjectTeacherId = assevent.subjectTeacherId,
                                link = assevent.link,
                                Question = asq.Question,
                                QuestionId = qid,
                                StudentAnswer = answer,
                                CorrectAnswer = asq.Answer,
                                Mark = asq.Marks,
                                CorrectOrWrong = corrwro,
                                LearnerName = User.Identity.Name,
                            };
                            db.StudentAssessmentSessions.Add(stasse);
                            db.SaveChanges();

                }
                else
                {
                    return RedirectToAction("AssessmentNotValid");
                }


            }

            else
            {

                AssessmentQuestion asq = db.AssessmentQuestions.Find(qid);
                Assessment ass = db.Assessments.Find(asq.AssessmentId);


                int Assid = ass.Id;

                var ass1 = db.Assessments.Where(x => x.Id == Assid && x.AssessmentDate < System.DateTime.Now && x.AssessmentEndDate > System.DateTime.Now).ToList();
                if (ass1.Count() > 0)
                {

                    var assevent = db.AssessmentEvents.Where(x => x.link == ass.Id.ToString()).FirstOrDefault();

                            string corrwro = "";
                            if (asq.Answer == answer)
                            {
                                corrwro = "CORRECT";
                            }
                            else
                            {
                                corrwro = "WRONG";
                            }

                            StudentAssessmentSession stasse = new StudentAssessmentSession()
                            {
                                EventID = assevent.EventID,
                                Subject = asq.Subject,
                                Description = ass.Name,
                                Start = ass.AssessmentDate,
                                End = ass.AssessmentEndDate,
                                GradeNumber = int.Parse(ass.Grade),
                                TeacherId = assevent.TeacherId,
                                TeacherName = ass.Teacher,
                                subjectTeacherId = assevent.subjectTeacherId,
                                link = assevent.link,
                                Question = asq.Question,
                                QuestionId = qid,
                                StudentAnswer = answer,
                                CorrectAnswer = asq.Answer,
                                Mark = asq.Marks,
                                CorrectOrWrong = corrwro,
                                LearnerName = User.Identity.Name,
                            };
                            db.StudentAssessmentSessions.Add(stasse);
                            db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("AssessmentNotValid");
                }

            }






            return null;
        }



        public ActionResult AssessmentNotValid()
        {
            return View();
        }

        public ActionResult submitassessment(string status,int aid)
        {
            var asssec = db.AssessmentSecurities.Where(x => x.AssId == aid && x.LName == User.Identity.Name).ToList();
            var c = db.Children.Where(x => x.ChildEmail == User.Identity.Name).FirstOrDefault();

            if (asssec.Count() > 0)
            {
                int asid = asssec.FirstOrDefault().AssId;



                if(status.ToUpper() == "ASSESSMENT SUBMITED" && asssec != null)
                {

                    int secid = asssec.FirstOrDefault().Id;
                    AssessmentSecurity secs = db.AssessmentSecurities.Find(secid);
                    secs.Status = "SUBMITED";
                    db.SaveChanges();



                    string _sender = "21916419@dut4life.ac.za";
                    string _password = "Dut960401";



                    string recipient = c.ParentEmail;
                    SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    System.Net.NetworkCredential credentials =
                        new System.Net.NetworkCredential(_sender, _password);
                    client.EnableSsl = true;
                    client.Credentials = credentials;
                    try
                    {
                        var mail = new MailMessage(_sender.Trim(), recipient.Trim());
                        mail.Subject = "ASSESSMENT";
                        mail.Body = "<HTML><BODY><p><div align='centre'>PLEASE NOTE THAT YOUR CHILD ASSESSMENT HAS BEEN SUBMITED<br/>" +
                            "From STUDENT CORE SYSTEM</p></BODY></HTML>";

                        mail.IsBodyHtml = true;
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw ex;
                    }



                    ViewBag.Status = status.ToUpper();

                    return Redirect("/Child/SessionMarks?id=" + aid);
                }

                if (status.ToUpper() == "CHEATED" && asssec != null)
                {

                    int secid = asssec.FirstOrDefault().Id;
                    AssessmentSecurity secs = db.AssessmentSecurities.Find(secid);
                    secs.Status = "CHEATED";
                    db.SaveChanges();




                    string _sender = "21916419@dut4life.ac.za";
                    string _password = "Dut960401";



                    string recipient = c.ParentEmail;
                    SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    System.Net.NetworkCredential credentials =
                        new System.Net.NetworkCredential(_sender, _password);
                    client.EnableSsl = true;
                    client.Credentials = credentials;
                    try
                    {
                        var mail = new MailMessage(_sender.Trim(), recipient.Trim());
                        mail.Subject = "ASSESSMENT";
                        mail.Body = "<HTML><BODY><p><div align='centre'>PLEASE NOTE THAT YOUR CHILD HAS CHEATED ON THE ASSESSMENT<br/>" +
                            "From STUDENT CORE SYSTEM</p></BODY></HTML>";

                        mail.IsBodyHtml = true;
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw ex;
                    }


                    ViewBag.Status = status.ToUpper();
                    return View();

                }

               
            }

            if(status == null)
            {
                return RedirectToAction("AssessmentNotValid");
            }
           


            ViewBag.Status = status.ToUpper();
            return View();
        }


        public ActionResult SessionMarks(int id)
        {
            var sss = db.StudentAssessmentSessions.Where(x => x.link == id.ToString()).ToList();
            return View(sss);
        }




    }
}