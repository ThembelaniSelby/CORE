using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Account;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ZXing;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        Db db = new Db();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignReg()
        {
            return View();
        }


        public ActionResult DRequests()
        {
            var dl = db.Deregisters.Where(x => x.StatusNum == 1).ToList();

            return View(dl);
        }


        public ActionResult ApproveD(int id)
        {

            Deregister del = db.Deregisters.Find(id);



            

            int us = db.Users.Where(x => x.EmailAddress == del.LearnerEmail).FirstOrDefault().Id;
            User uss = db.Users.Find(us);
            db.Users.Remove(uss);
            db.SaveChanges();


            string _sender = "21916419@dut4life.ac.za";
            string _password = "Dut960401";



            string recipient = del.Pemail;
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
                mail.Subject = "CHILD DEREGISTERED";
                mail.Body = "<HTML><BODY><p><div align='centre'>PLEASE NOTE: THAT YOUR CHILD HAS BEEN DEREGISTERED<br/>" +
                    "<br/> CHILD:" + del.LearnerId+ "<br/>" +
                    "From STUDENT CORE SYSTEM</p></BODY></HTML>";

                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }



            return RedirectToAction("DRequests");
            
        }





        public ActionResult GetCapture(string imageData)
        {
            byte[] data = Convert.FromBase64String(imageData);

            var barcodeReader = new BarcodeReader();

            byte[] imaa = data;

            Stream stream = new MemoryStream(imaa);

            var barcodeBitmap = (Bitmap)Bitmap.FromStream(stream);

            var barcodeResult = barcodeReader.Decode(barcodeBitmap);

            string link = barcodeResult.ToString();

            var child = db.Children.Where(x => x.LeearnerCode == link).FirstOrDefault();

            Attendance at = new Attendance();
            at.Date = DateTime.UtcNow.AddHours(2).Date.ToString();
            at.Attendancee = "PRESENT";
            at.ChildId = child.Id;
            at.ChildName = child.FirstName;
            at.ChildIdNumber = child.IdNumb;

            db.Attendances.Add(at);
            db.SaveChanges();


            return RedirectToAction("CreateRegister");
        }

        [HttpPost]
        public ActionResult Remove(int? id)
        {
            Attendance a = db.Attendances.Find(id);
            db.Attendances.Remove(a);
            db.SaveChanges();
            return RedirectToAction("CreateRegister");
        }


        public ActionResult TodayReg()
        {
            string date = DateTime.UtcNow.AddHours(2).Date.ToString();

            var red = db.Attendances.Where(x => x.Date.ToString() == date).ToList();
            return View(red);
        }

        public ActionResult CreateRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRegister(string g, string date)
        {
            var st = db.Children.Where(x => x.Grade == g).ToList();

            foreach (var item in st)
            {
                Attendance a = new Attendance();
                a.ChildId = st.FirstOrDefault().Id;
                a.Date = DateTime.UtcNow.AddHours(2).Date.ToString();
                a.Attendancee = "ABSENT";
                db.Attendances.Add(a);
                db.SaveChanges();
            }


            return RedirectToAction("CreateRegister");
        }



        [HttpGet]
        public ActionResult AddFees()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFees(Fee fee)
        {
            if (db.Fees.Any())
            {
                TempData["Danger Messege"] = "Please edit existing fees";
                return RedirectToAction("Fees");
            }

            Fee fe = new Fee();
            fe.AdminFee = fee.AdminFee;
            fe.AfterHourFee = fee.AfterHourFee;
            fe.Event_sportFee = fee.Event_sportFee;
            fe.Feeding_schemeFee = fee.Feeding_schemeFee;
            fe.StationaryFee = fee.StationaryFee;
            fe.UniformFee = fee.UniformFee;
            db.Fees.Add(fe);
            db.SaveChanges();


            return RedirectToAction("Fees");
        }

        public ActionResult Fees()
        {
            return View(db.Fees.ToList());
        }

        [HttpGet]
        public ActionResult UpdateFee(int? id)
        {
            var Fees = db.Fees.Where(x => x.Id == id).FirstOrDefault();
            return View(Fees);
        }

        [HttpPost]
        public ActionResult UpdateFee(Fee ff, int? id)
        {

            Fee fe = db.Fees.Find(id);

            fe.AdminFee = ff.AdminFee;
            fe.AfterHourFee = ff.AfterHourFee;
            fe.Event_sportFee = ff.Event_sportFee;
            fe.Feeding_schemeFee = ff.Feeding_schemeFee;
            fe.StationaryFee = ff.StationaryFee;
            fe.UniformFee = ff.UniformFee;
            db.SaveChanges();

            return RedirectToAction("Fees");
        }


        public ActionResult Applications()
        {
            var aps = db.Children.Where(x => x.Status != "APPROVED").ToList();

            return View(aps);
        }


        public ActionResult ApproveApplication(int id)
        {
            db.Children.Where(x => x.Id == id).FirstOrDefault();

            Child c = db.Children.Find(id);
            c.Status = "APPROVED";
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
                mail.Subject = "APPLICATION STATUS";
                mail.Body = "<HTML><BODY><p><div align='centre'>PLEASE NOTE:  APPLICATION APPROVED PROCEED TO REGISTRATION<br/>" +
                    "<br/> ID NUMBER:" + c.IdNumb + "<br/>" +
                    "From STUDENT CORE SYSTEM</p></BODY></HTML>";

                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }




            return RedirectToAction("Applications");
        }


        public ActionResult RejectApplication(int id)
        {
            db.Children.Where(x => x.Id == id).FirstOrDefault();

            Child c = db.Children.Find(id);
            c.Status = "REJECTED";
            db.SaveChanges();

            return RedirectToAction("Applications");
        }


        public ActionResult Application(int? id)
        {
            return View(db.Children.Where(x => x.Id == id).FirstOrDefault());
        }

        public ActionResult Details(int id)
        {
            ChildVM model;
            using (Db db = new Db())
            {

                Child dto = db.Children.Find(id);
                if (dto == null)
                {
                    return Content("That Application Does not Exist");
                }
                model = new ChildVM(dto);

                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Docs/Docs/" + id))
                                       .Select(fn => System.IO.Path.GetFileName(fn));

                if (model.GalleryImages == null)
                {
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Docs", Server.MapPath(@"\")));
                    var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString());
                    Directory.CreateDirectory(pathString2);
                }
                return View(model);

            }


        }


        [HttpGet]
        public ActionResult UploadContent()
        {
            ViewBag.Subjects = db.Subjects.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult UploadContent(Content c, HttpPostedFileBase VideoLesson, string type)
        {

            Content cc = new Content();

            cc.Grade = c.Grade;
            cc.Subject = c.Subject;
            cc.VideoLesson = VideoLesson.FileName;
            cc.Type = type;
            int sub = int.Parse(c.Subject);

            var s = db.Subjects.Where(x => x.Id == sub).FirstOrDefault();


            string Vid = VideoLesson.FileName;



            //Check it's not null
            if (VideoLesson != null && VideoLesson.ContentLength > 0)
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Content\\", Server.MapPath(@"\")));
                var pathString2 = System.IO.Path.Combine(originalDirectory.ToString());

                var path = string.Format("{0}\\{1}", pathString2, Vid);

                if (!Directory.Exists(pathString2))
                {
                    Directory.CreateDirectory(pathString2);
                }

                VideoLesson.SaveAs(path);
            }


            //if (DocumentLesson != null && DocumentLesson.ContentLength > 0)
            //{
            //    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Content", Server.MapPath(@"\")));
            //    var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "\\");

            //    var path = string.Format("{0}\\{1}", pathString2, Vid);

            //    if (!Directory.Exists(pathString2))
            //    {
            //        Directory.CreateDirectory(pathString2);
            //    }

            //    DocumentLesson.SaveAs(path);
            //}


            //if (PhotoLesson != null && PhotoLesson.ContentLength > 0)
            //{
            //    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Content", Server.MapPath(@"\")));
            //    var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "\\");

            //    var path = string.Format("{0}\\{1}", pathString2, Vid);

            //    if (!Directory.Exists(pathString2))
            //    {
            //        Directory.CreateDirectory(pathString2);
            //    }

            //    PhotoLesson.SaveAs(path);
            //}



            //if (Quiz != null && Quiz.ContentLength > 0)
            //{
            //    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Content", Server.MapPath(@"\")));
            //    var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "\\");

            //    var path = string.Format("{0}\\{1}", pathString2, Vid);

            //    if (!Directory.Exists(pathString2))
            //    {
            //        Directory.CreateDirectory(pathString2);
            //    }

            //    Quiz.SaveAs(path);
            //}

            //if (Homework != null && Homework.ContentLength > 0)
            //{
            //    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Content", Server.MapPath(@"\")));
            //    var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "\\");

            //    var path = string.Format("{0}\\{1}", pathString2, Vid);

            //    if (!Directory.Exists(pathString2))
            //    {
            //        Directory.CreateDirectory(pathString2);
            //    }

            //    Homework.SaveAs(path);
            //}

            db.Contents.Add(cc);
            db.SaveChanges();

            return RedirectToAction("Content");
        }


        public ActionResult Content(int? g, string Sub)
        {

            ContentVM model;
            using (Db db = new Db())
            {
                if (g != null)
                {

                    Content dto = db.Contents.Where(x => x.Grade == g && x.Subject == Sub).FirstOrDefault();
                    if (dto == null)
                    {
                        return Content("That Application Does not Exist");
                    }
                    model = new ContentVM(dto);

                    model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Content/"))
                                           .Select(fn => System.IO.Path.GetFileName(fn));


                    return View(model);
                }
                else
                {
                    Content dto = db.Contents.FirstOrDefault();
                    if (dto == null)
                    {
                        return Content("That Application Does not Exist");
                    }
                    model = new ContentVM(dto);

                    model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Content/"))
                                           .Select(fn => System.IO.Path.GetFileName(fn));


                    return View(model);
                }

            }


        }


        /// <summary>
        /// //subjects section
        /// </summary>
        /// <returns></returns>

        public async Task<ActionResult> Grades()
        {
            int g = 1;
            for (int i = 1; i < 12; i++)
            {

            }
            return View(await db.Grades.ToListAsync());
        }



        public ActionResult Subjects()
        {
            var subs = db.Subjects.ToList();
            return View(subs);
        }


        [HttpGet]
        public ActionResult AddSubJect()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddSubJect(Subject s)
        {

            if (!db.Subjects.Any(x => x.NameFix.Equals(s.Name.Replace(" ", "-").ToLower().Replace("&", "-"))))
            {
                Subject sub = new Subject();

                sub.Name = s.Name;
                sub.NameFix = sub.Name.Replace(" ", "-").ToLower().Replace("&", "-");
                db.Subjects.Add(sub);
                db.SaveChanges();
                TempData["Success"] = "Subject added";


                return RedirectToAction("Subjects");
            }
            else
            {
                TempData["Error"] = "Subject exist for this grade";

                return RedirectToAction("Subjects");
            }

        }

        public ActionResult AssignTeacher(int id)
        {
            SubTVM model = new SubTVM();
            model.SubjectName = db.Gradesubjects.Find(id).SubjectName;

            List<UserVM> teachers = new List<UserVM>();
            var userrole = db.UserRoles.Where(x => x.RoleId == 3).ToList();
            foreach (var item in userrole)
            {
                var teach = db.Users.Where(x => x.Id == item.UserId).ToList();
                foreach (var row in teach)
                {
                    teachers.Add(new UserVM()
                    {

                        Id = row.Id,
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        EmailAddress = row.EmailAddress,
                        Username = row.Username,
                        Password = row.Password,
                        PhoneNumber = row.PhoneNumber,
                        Gender = row.Gender,
                        DateOfBirth = row.DateOfBirth,
                        IdNumb = row.IdNumb,
                        Balance = row.Balance,
                        Maritual = row.Maritual,
                        HomeLang = row.HomeLang,
                        PostalAd = row.PostalAd,
                        HomeAd = row.HomeAd,
                        HomeNumber = row.HomeNumber,
                    });
                }

            }

            model.Teachers = new SelectList(teachers.ToList(), "Id", "EmailAddress");

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignTeacher(int id, SubTVM model)
        {

            Gradesubject subg = db.Gradesubjects.Find(id);
            int tid = int.Parse(model.TeacherId);
            User user = db.Users.Find(tid);

            if (!db.Subjectteacher.Any(x => x.SubjectNameFix.Equals(subg.SubjectNameFix.Replace(" ", "-").ToLower().Replace("&", "-")) && x.GradeNumber.Equals(subg.GradeNumber)))
            {
                Subjectteacher st = new Subjectteacher();
                st.GradeName = subg.GradeName;
                st.GradeNumber = subg.GradeNumber;
                st.SubjectName = subg.SubjectName;
                st.SubjectNameFix = subg.SubjectNameFix;
                st.TeacherId = model.TeacherId;
                st.TeacherName = user.Username;

                db.Subjectteacher.Add(st);
                db.SaveChanges();
                TempData["Success"] = "Teacher assign to subject grade";

                return Redirect("/Admin/Grades/Addsubject?id=" + subg.GradeNumber);
            }
            else
            {
                TempData["Error"] = "Subject already has a Teacher Allocated For This Grade";

                return Redirect("/Admin/Grades/Addsubject?id=" + subg.GradeNumber);

            }
        }

        public ActionResult AddMeeting()
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var apiSecret = "Zaez0RWJ0msxmSKg9za1A30eOjjthm5m3YZ3";
            byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "JSQV6bS8RNO0JzFl6aYFRg",
                Expires = now.AddSeconds(300),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var client = new RestClient("https://api.zoom.us/v2/users/ndawondemuzi99@gmail.com/meetings");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { topic = "Meeting with Thembelani", duration = "10", start_time = "2021-04-20T05:00:00", type = "2" });

            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
            IRestResponse restResponse = client.Execute(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;
            var jObject = JObject.Parse(restResponse.Content);

            ViewBag.Host = (string)jObject["start_url"];
            ViewBag.Join = (string)jObject["join_url"];
            ViewBag.Code = Convert.ToString(numericStatusCode);

            return View();
            //Host.Text = 
            //Join.Text = (string)jObject["join_url"];
            //Code.Text = Convert.ToString(numericStatusCode);
        }
        public ActionResult Timetable()
        {
            return View();
        }
        public JsonResult GetEvents()
        {
            using (Db dc = new Db())
            {
                var events = dc.Events.ToList();
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }


        public ActionResult Slot1(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 1 && x.GradeNumber == gradeid).ToList().Take(3);
            return View(slot);
        }
        public ActionResult Slot1f(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 1 && x.GradeNumber == gradeid).ToList().Skip(3).Take(2);
            return View(slot);
        }
        public ActionResult Slot2(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 2 && x.GradeNumber == gradeid).ToList().Take(3);
            return View(slot);
        }
        public ActionResult Slot2f(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 2 && x.GradeNumber == gradeid).ToList().Skip(3).Take(2);
            return View(slot);
        }
        public ActionResult Slot3(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 3 && x.GradeNumber == gradeid).ToList().Take(3);
            return View(slot);
        }
        public ActionResult Slot3f(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 3 && x.GradeNumber == gradeid).ToList().Skip(3).Take(2);
            return View(slot);
        }
        public ActionResult Slot4(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 4 && x.GradeNumber == gradeid).ToList().Take(3);
            return View(slot);
        }
        public ActionResult Slot4f(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 4 && x.GradeNumber == gradeid).ToList().Skip(3).Take(2);
            return View(slot);
        }
        public ActionResult Slot5(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 5 && x.GradeNumber == gradeid).ToList().Take(3);
            return View(slot);
        }
        public ActionResult Slot5f(int gradeid)
        {
            var slot = db.Slots.Where(x => x.DayNumber == 5 && x.GradeNumber == gradeid).ToList().Skip(3).Take(2);
            return View(slot);
        }



        [HttpGet]
        public ActionResult CreateTimetable(int gradeid)
        {

            if (db.Days.ToList().Count() < 1)
            {
                Day d1 = new Day()
                {
                    Name = "MONDAY",
                    Number = 1,
                };
                db.Days.Add(d1);
                db.SaveChanges();



                Day d2 = new Day()
                {
                    Name = "TUESDAY",
                    Number = 2,
                };
                db.Days.Add(d2);
                db.SaveChanges();


                Day d3 = new Day()
                {
                    Name = "WEDNESDAY",
                    Number = 3,
                };
                db.Days.Add(d3);
                db.SaveChanges();


                Day d4 = new Day()
                {
                    Name = "THURSDAY",
                    Number = 4,
                };
                db.Days.Add(d4);
                db.SaveChanges();


                Day d5 = new Day()
                {
                    Name = "FRIDAY",
                    Number = 5,
                };
                db.Days.Add(d5);
                db.SaveChanges();
            }


            if (db.Time.ToList().Count() < 1)
            {
                Time t1 = new Time()
                {
                    Start = "07:00",
                    End = "08:00"
                };
                db.Time.Add(t1);
                db.SaveChanges();



                Time t2 = new Time()
                {
                    Start = "08:00",
                    End = "09:00"
                };
                db.Time.Add(t2);
                db.SaveChanges();


                Time t3 = new Time()
                {
                    Start = "09:00",
                    End = "10:00"
                };
                db.Time.Add(t3);
                db.SaveChanges();


                Time t4 = new Time()
                {
                    Start = "11:00",
                    End = "12:00"
                };
                db.Time.Add(t4);
                db.SaveChanges();

                Time t5 = new Time()
                {
                    Start = "12:00",
                    End = "13:00"
                };
                db.Time.Add(t5);
                db.SaveChanges();

            }

            if (db.Slots.Where(x => x.GradeNumber == gradeid).ToList().Count() < 1)
            {
                var day = db.Days.ToList();


                foreach (var item in day)
                {
                    var time = db.Time.ToList();
                    foreach (var iitem in time)
                    {

                        Slot SL = new Slot()
                        {

                            DayName = item.Name,
                            DayNumber = item.Number,
                            Start = iitem.Start,
                            End = iitem.End,
                            GradeNumber = gradeid,
                            ThemeColor = "Red",
                            TeacherId = 0,
                            TeacherName = "",
                            subjectTeacherId = 0,
                            Subject = "",
                            Description = "",
                            EventId = 0,
                        };
                        db.Slots.Add(SL);
                        db.SaveChanges();
                    }
                }


            }
            else
            {


            }

            ViewBag.Grade = gradeid.ToString();

            return View();
        }

        [HttpGet]
        public ActionResult EditSlot(int id)
        {

            SlotVM model;
            using (Db db = new Db())
            {
                Slot dto = db.Slots.Find(id);
                model = new SlotVM(dto);
                model.SUBJECTS = new SelectList(db.Subjectteacher.Where(x => x.GradeNumber == dto.GradeNumber).ToList(), "Id", "SubjectName");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSlot(SlotVM model)
        {
            Slot sl = db.Slots.Find(model.Id);

            Subjectteacher st = db.Subjectteacher.Find(model.subjectTeacherId);

            sl.ThemeColor = model.ThemeColor;
            sl.subjectTeacherId = model.subjectTeacherId;
            sl.TeacherName = st.TeacherName;
            sl.TeacherId = int.Parse(st.TeacherId);
            sl.Subject = st.SubjectName;
            sl.Description = sl.Subject + " " + model.Description;
            db.SaveChanges();




            DateTime startDate = DateTime.Now.Date.AddDays(-31);
            DateTime endDate = DateTime.Now.AddDays(180).Date;

            int days = (endDate - startDate).Days;


            string myDay = sl.DayNumber.ToString();
            var myDayDayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), myDay); //convert string to DayOfWeek type

            var allDates = new List<DateTime>();
            for (int i = 0; i < days; i++)
            {
                var currDate = startDate.AddDays(i);
                if (currDate.DayOfWeek == myDayDayOfWeek)
                {
                    string ctime = sl.Start + ":00 AM";
                    DateTime start = DateTime.Parse(currDate.ToString().Replace("12:00:00 AM", ctime));



                    Event ev = new Event()
                    {
                        Subject = sl.Subject,
                        Description = sl.Subject + " " + model.Description,
                        Start = start,
                        End = start.AddHours(1),
                        ThemeColor = model.ThemeColor,
                        IsFullDay = false,
                        GradeNumber = st.GradeNumber,
                        TeacherId = int.Parse(st.TeacherId),
                        TeacherName = st.TeacherName,
                        subjectTeacherId = st.Id,

                    };
                    db.Events.Add(ev);
                    db.SaveChanges();



                    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var now = DateTime.UtcNow;
                    var apiSecret = "Conuj0e68dQ51wGQpFQhemDfxKBaeiDOCMGr";
                    byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Issuer = "YZLMG47LRhi4Hd7SrvA-rQ",
                        Expires = now.AddDays(180),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    var client = new RestClient("https://api.zoom.us/v2/users/ndawondemuzi99@gmail.com/meetings");
                    var request = new RestRequest(Method.POST);
                    request.RequestFormat = DataFormat.Json;



                    request.AddJsonBody(new { topic = sl.Subject + " " + model.Description, duration = "60", start_time = start, type = "2" });

                    request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
                    IRestResponse restResponse = client.Execute(request);
                    HttpStatusCode statusCode = restResponse.StatusCode;
                    int numericStatusCode = (int)statusCode;
                    var jObject = JObject.Parse(restResponse.Content);


                    Event evl = db.Events.Find(ev.EventID);
                    evl.link = (string)jObject["join_url"];
                    evl.startlink = (string)jObject["start_url"];
                    db.SaveChanges();


                }

            }

            TempData["Success"] = "Timetable slot updated";

            return Redirect("/Admin/Admin/CreateTimetable?gradeid=" + st.GradeNumber);
        }


        [HttpGet]
        public ActionResult AssignClassTeacher(int id)
        {
            ClassTVM model = new ClassTVM();

            model.GradeTeachers = new SelectList(db.Subjectteacher.Where(x => x.GradeNumber == id).ToList(), "Id", "TeacherName");

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignClassTeacher(ClassTVM row, int GradeNumber)
        {
            var ctt = db.Classteachers.Where(x => x.GradeNumber == GradeNumber).ToList();
            if (ctt.Count() == 1)
            {
                int currentct = ctt.FirstOrDefault().Id;
                Classteacher cttt = db.Classteachers.Find(currentct);
                db.Classteachers.Remove(cttt);
                db.SaveChanges();

                var teac = db.Subjectteacher.Where(x => x.Id == row.Id).FirstOrDefault();

                Classteacher ct = new Classteacher()
                {
                    SubjectName = teac.SubjectName,
                    SubjectNameFix = teac.SubjectNameFix,
                    GradeName = teac.GradeName,
                    GradeNumber = teac.GradeNumber,
                    TeacherName = teac.TeacherName,
                    TeacherId = teac.TeacherId,
                };
                db.Classteachers.Add(ct);
                db.SaveChanges();

                var gra = db.Grades.Where(x => x.Number == ct.GradeNumber).FirstOrDefault();


                TempData["Success"] = "Class teacher Updated";

                return Redirect("/Admin/Grades/Addsubject?id=" + gra.Id);

            }

            else
            {
                var teac = db.Subjectteacher.Where(x => x.Id == row.Id).FirstOrDefault();

                Classteacher ct = new Classteacher()
                {
                    SubjectName = teac.SubjectName,
                    SubjectNameFix = teac.SubjectNameFix,
                    GradeName = teac.GradeName,
                    GradeNumber = teac.GradeNumber,
                    TeacherName = teac.TeacherName,
                    TeacherId = teac.TeacherId,
                };
                db.Classteachers.Add(ct);
                db.SaveChanges();
                var gra = db.Grades.Where(x => x.Number == ct.GradeNumber).FirstOrDefault();


                TempData["Success"] = "New class teacher Allocated";

                return Redirect("/Admin/Grades/Addsubject?id=" + gra.Id);
            }


        }


        public ActionResult Teachers()
        {

            List<UserVM> teachers = new List<UserVM>();
            var userrole = db.UserRoles.Where(x => x.RoleId == 3).ToList();
            foreach (var item in userrole)
            {
                var teach = db.Users.Where(x => x.Id == item.UserId).ToList();
                foreach (var row in teach)
                {
                    teachers.Add(new UserVM()
                    {

                        Id = row.Id,
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        EmailAddress = row.EmailAddress,
                        Username = row.Username,
                        Password = row.Password,
                        PhoneNumber = row.PhoneNumber,
                        Gender = row.Gender,
                        DateOfBirth = row.DateOfBirth,
                        IdNumb = row.IdNumb,
                        Balance = row.Balance,
                        Maritual = row.Maritual,
                        HomeLang = row.HomeLang,
                        PostalAd = row.PostalAd,
                        HomeAd = row.HomeAd,
                        HomeNumber = row.HomeNumber,
                    });
                }

            }

            return View(teachers.ToList());
        }

        [HttpGet]
        public ActionResult AddTeacher()
        {
            return View("AddTeacher");
        }

        [HttpPost]
        public ActionResult AddTeacher(UserVM model, string add, string add1, string city, string city1, string prov, string prov1, int zip, int zip1)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View("AddTeacher", model);
            }
            //check if password match

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password Does not Match");
                return View("AddTeacher", model);
            }
            using (Db db = new Db())
            {

                //make sure username is unique
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", "Username" + model.Username + "Is Taken");
                    model.Username = "";
                    return View("AddTeacher", model);
                }


                UserRole userRoles = new UserRole()

                {
                    RoleId = 3
                };

                db.UserRoles.Add(userRoles);
                db.SaveChanges();
                int id = userRoles.UserId;

                //create userDTO
                User userDTO = new User()
                {
                    Id = id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Username = model.EmailAddress,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Balance = 0,
                    IdNumb = model.IdNumb,
                    Date = DateTime.UtcNow.AddHours(2),
                    Maritual = model.Maritual,
                    HomeLang = model.HomeLang,
                    PostalAd = add + " " + city + " " + prov + " " + zip,
                    HomeAd = add1 + " " + city1 + " " + prov1 + " " + zip1,
                    HomeNumber = model.HomeNumber,


                };
                string ystring = model.IdNumb.Substring(0, 2);
                int year = int.Parse(ystring);
                string century = "";


                if (year < 10)
                {
                    century = "20";
                }
                else
                {
                    century = "19";

                }
                string moth = model.IdNumb.Substring(3, 4);

                string date = model.IdNumb.Substring(0, 6);
                string day = model.IdNumb.Substring(5, 6);

                string birth = century + ystring + moth + day;

                userDTO.DateOfBirth = DateTime.UtcNow.AddHours(2);

                string ggen = "";

                string g = model.IdNumb.Substring(7);

                if (int.Parse(g) >= 5)
                {
                    ggen = "Male";
                }
                if (int.Parse(g) <= 4)
                {
                    ggen = "Female";
                }

                userDTO.Gender = ggen;

                db.Users.Add(userDTO);

                db.SaveChanges();




                //Create Temp Message
                TempData["Success Message"] = "New Teacher added";

                //redirect  
                return RedirectToAction("Teachers");
            }
        }

        public ActionResult EmailTimeTable(Grade model, int gradeid)
        {


            var slot1 = db.Slots.Where(x => x.DayNumber == 1 && x.GradeNumber == gradeid).ToList().Take(5);


            var slot2 = db.Slots.Where(x => x.DayNumber == 2 && x.GradeNumber == gradeid).ToList().Take(5);


            var slot3 = db.Slots.Where(x => x.DayNumber == 3 && x.GradeNumber == gradeid).ToList().Take(5);


            var slot4 = db.Slots.Where(x => x.DayNumber == 4 && x.GradeNumber == gradeid).ToList().Take(5);


            var slot5 = db.Slots.Where(x => x.DayNumber == 5 && x.GradeNumber == gradeid).ToList().Take(5);


            string invon = "Grade " + gradeid + "timetable";
            System.IO.FileStream fs = new FileStream(Server.MapPath("~/Images/") + invon + ".pdf", FileMode.Create);
            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fs);

            pdfDoc.Open();
            try
            {

                //Top Heading
                Chunk chunk = new Chunk(DateTime.UtcNow.Date.ToString(), FontFactory.GetFont("Arial", 5, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK));
                pdfDoc.Add(chunk);



                //Horizontal Line
                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);


                //Table
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                //0=Left, 1=Centre, 2=Right
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;
                ////////
                chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk); chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk);
                chunk = new Chunk(invon.ToString().ToUpper() + "\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk);
                chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk);
                chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk); chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk); chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk);



                string coment = "PLEASE NOTE THAT THE BREAK TIME IS FROM 10:00 TO 11:00";

                Chunk chunk2 = new Chunk(coment.ToString(), FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLDITALIC, BaseColor.RED));
                pdfDoc.Add(chunk2);
                chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk); chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk); chunk = new Chunk("\n", FontFactory.GetFont("Daytona Condensed Light", 15, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk);

                //Cell no 1
                PdfPCell cell = new PdfPCell();


                chunk = new Chunk("", FontFactory.GetFont("Helvetica Neue", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell();
                cell.Border = 0;

                var para4 = new Paragraph(chunk);
                para4.Alignment = Element.ALIGN_LEFT;
                para4.Alignment = -100;

                cell.AddElement(para4);
                table.AddCell(cell);



                chunk = new Chunk("", FontFactory.GetFont("Helvetica Neue", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell();
                cell.Border = 0;

                var para5 = new Paragraph(chunk);
                para5.Alignment = Element.ALIGN_LEFT;
                para5.Alignment = -100;

                cell.AddElement(para5);
                table.AddCell(cell);

                //Add table to document
                pdfDoc.Add(table);



                //Table
                table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = -0f;



                if (slot1 != null)
                {
                    cell = new PdfPCell();
                    chunk = new Chunk("SUBJECT PERIODS", FontFactory.GetFont("Helvetica Neue", 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell.Colspan = 5;
                    var para13 = new Paragraph(chunk);
                    para13.Alignment = Element.ALIGN_CENTER;


                    cell.AddElement(para13);
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    table.AddCell(" 07:00 - 08:00 ");
                    table.AddCell(" 08:00 - 09:00 " + Environment.NewLine);
                    table.AddCell(" 09:00 - 10:00 " + Environment.NewLine);
                    table.AddCell(" 11:00 - 12:00 " + Environment.NewLine);
                    table.AddCell(" 12:00 - 13:00 " + Environment.NewLine);

                    pdfDoc.Add(table);

                    table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 30f;
                }




                foreach (var itemm in slot1)
                {

                    line = new Paragraph(new Chunk(itemm.Subject.ToUpper() + "\n \n" + itemm.TeacherName.ToString().ToLower(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);
                }

                foreach (var itemm in slot2)
                {

                    line = new Paragraph(new Chunk(itemm.Subject.ToUpper() + "\n \n" + itemm.TeacherName.ToString().ToLower(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);
                }
                foreach (var itemm in slot3)
                {

                    line = new Paragraph(new Chunk(itemm.Subject.ToUpper() + "\n \n" + itemm.TeacherName.ToString().ToLower(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);
                }

                foreach (var itemm in slot4)
                {

                    line = new Paragraph(new Chunk(itemm.Subject.ToUpper() + "\n \n" + itemm.TeacherName.ToString().ToLower(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);
                }
                foreach (var itemm in slot5)
                {

                    line = new Paragraph(new Chunk(itemm.Subject.ToUpper() + "\n \n" + itemm.TeacherName.ToString().ToLower(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);
                }

                pdfDoc.Add(table);



                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                pdfDoc.CloseDocument();
                fs.Close();

                var students = db.Children.Where(x => x.Grade == gradeid.ToString()).ToList();

                foreach (var item in students)
                {
                    string sender = "21916419@dut4life.ac.za";
                    string password = "Dut960401";


                    string recipient = item.ParentEmail;
                    SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sender, password);
                    client.EnableSsl = true;
                    client.Credentials = credentials;
                    Attachment data = new Attachment(Server.MapPath("~/Images/" + invon + ".pdf"));


                    var mail = new MailMessage(sender.Trim(), recipient.Trim());

                    mail.Subject = "INVOICE";
                    mail.Body = "Plese  find Invoice Attachement";
                    mail.Attachments.Add(data);
                    client.Send(mail);
                }



                TempData["Success"] = "Timetable Sent!!";



                return Redirect("/Admin/Admin/createtimetable?gradeid=" + gradeid);
            }

            catch (Exception ex)
            {

                pdfWriter.CloseStream = false;
                pdfDoc.Close();

                TempData["Error"] = "Failed To Send Timetable!!";
                throw ex;

                return Redirect("/Admin/Admin/createtimetable?gradeid=" + gradeid);

            }

        }

    }
}