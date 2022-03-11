using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Account;
using PagedList;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZXing;

namespace CmsShoppingCart.Controllers
{
    public class AccountController : Controller
    {
        private Db db = new Db();


        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }


        [HttpGet]
        public ActionResult LearnerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LearnerLogin(LoginUserVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //check if user is valid
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;

                }

                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }



        public ActionResult Content(int g, string Sub, int? page, ContentVM model)
        {
            int a;
            if (g != null)
            {
                a = g;
            }
            string b;
            if (Sub != null)
            {
                b = Sub;
            }

            else
            {
                a = 1;
                b = "";
            }
            //Declare List Of ProductVM
            List<ContentVM> listOfProductVM;
            //Set Page Number
            var pageNumber = page ?? 1;
            using (Db db = new Db())
            {
                //Intialize List
                listOfProductVM = db.Contents.ToArray()
                                .Where(x => x.Grade == g || x.Subject == Sub)
                                .Select(x => new ContentVM(x))
                                .ToList();

                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Content/"))
                                     .Select(fn => System.IO.Path.GetFileName(fn));
                //Populate Categories select list
                ViewBag.Categories = new SelectList(db.Subjects.ToList(), "Id", "Name");
                //set selected category
                ViewBag.SelectedCat = g.ToString();
            }

            //set pagination
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 100);
            ViewBag.OnePageOfProducts = onePageOfProducts;

            //return view with list

            return View();

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
            db.Attendances.Add(at);
            db.SaveChanges();

            return RedirectToAction("RegisterSigned");
        }



        // GET: account/Login
        [HttpGet]
        public ActionResult Login()
        {

            //confirm user is not logged in
            string username = User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
            {
                return Redirect("user-profile");

            }

            //return view
            return View();
        }


        // POST: account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //check if user is valid
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;

                }

                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }

        [Authorize]
        public ActionResult MyChildren()
        {
            var c = db.Children.Where(x => x.ParentEmail == User.Identity.Name).ToList();
            return View(c);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddChild()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult AddChild(Child c, string FirstName, string LastName,string ChildEmail, string Password, DateTime DateOfBirth, string IdNumb, string grade, HttpPostedFileBase lastreportcard, HttpPostedFileBase certificate, HttpPostedFileBase learnerpic)
        {

            var user = db.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault();

            Child cc = new Child();
            cc.Date = DateTime.UtcNow.AddHours(2);
            cc.DateOfBirth = DateOfBirth;
            cc.ParentEmail = User.Identity.Name;
            cc.FirstName = FirstName;
            cc.HomeLang = user.HomeLang;
            cc.IdNumb = IdNumb;
            cc.LastName = LastName;
            cc.Password = Password;
            cc.ChildEmail = ChildEmail;
            cc.Username = cc.ChildEmail;
            cc.Grade = c.Grade;
            cc.Status = "NEW";
            cc.LearnerPic = IdNumb + User.Identity.Name + learnerpic.FileName;

            cc.LastReport = lastreportcard.FileName.ToString();
            cc.Certificate = certificate.FileName.ToString();


            string ystring = c.IdNumb.Substring(0, 2);
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
            string moth = c.IdNumb.Substring(3, 4);

            string date = c.IdNumb.Substring(0, 6);
            string day = c.IdNumb.Substring(5, 6);

            string birth = century + ystring + moth + day;

            cc.DateOfBirth = DateTime.UtcNow.AddHours(2);

            string ggen = "";

            string g = c.IdNumb.Substring(7);

            if (int.Parse(g) >= 5)
            {
                ggen = "Male";
            }
            if (int.Parse(g) <= 4)
            {
                ggen = "Female";
            }

            cc.Gender = ggen;





            db.Children.Add(cc);
            db.SaveChanges();


            int id = cc.Id;


            var originalDirectoryy = new DirectoryInfo(string.Format("{0}Images\\Docs", Server.MapPath(@"\")));
            var pathString22 = System.IO.Path.Combine(originalDirectoryy.ToString(), "Docs\\" + id.ToString());

            var originalDirectoryx = new DirectoryInfo(string.Format("{0}Images\\DocsPic", Server.MapPath(@"\")));
            var pathString2x = System.IO.Path.Combine(originalDirectoryx.ToString(), "DocsPic\\");

            if (!Directory.Exists(pathString2x))
            {
                Directory.CreateDirectory(pathString2x);
            }

            if (!Directory.Exists(pathString22))
            {
                Directory.CreateDirectory(pathString22);
            }



            string report = lastreportcard.FileName;
            string certi = certificate.FileName;
            string learn = IdNumb + User.Identity.Name + learnerpic.FileName;


            //Check it's not null
            if (lastreportcard != null && lastreportcard.ContentLength > 0)
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Docs", Server.MapPath(@"\")));
                var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString());
                var pathString3 = System.IO.Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString() + "\\Thumbs");
                var path = string.Format("{0}\\{1}", pathString2, report);
                lastreportcard.SaveAs(path);
            }

            if (certificate != null && certificate.ContentLength > 0)
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Docs", Server.MapPath(@"\")));
                var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString());
                var pathString3 = System.IO.Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString() + "\\Thumbs");
                var path = string.Format("{0}\\{1}", pathString2, certi);
                certificate.SaveAs(path);
            }

            if (learnerpic != null && learnerpic.ContentLength > 0)
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\DocsPic", Server.MapPath(@"\")));
                var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "DocsPic\\");
                var pathString3 = System.IO.Path.Combine(originalDirectory.ToString(), "DocsPic\\" + "\\Thumbs");
                var path = string.Format("{0}\\{1}", pathString2, learn);
                learnerpic.SaveAs(path);
            }

            int idd;


            return Redirect("/Account/GetQuotation?idd=" + id);
        }


        [HttpGet]
        public ActionResult GetQuotation(int idd)
        {

            var qu = db.Fees.ToList().Take(1).FirstOrDefault();


            ViewBag.Ad = qu.AdminFee.ToString();
            ViewBag.Aft = qu.AfterHourFee.ToString();
            ViewBag.Del = qu.OneWayDiv.ToString();
            ViewBag.Sta = qu.StationaryFee.ToString();
            ViewBag.Uni = qu.UniformFee.ToString();
            ViewBag.Ev = qu.Event_sportFee.ToString();
            ViewBag.Fe = qu.Feeding_schemeFee.ToString();
            ViewBag.Id = idd.ToString();
            return View();
        }



        [HttpPost]
        public ActionResult GetQuotation(int idd, string AfterHours, string PickUp, string Delivery, string Uniform, string Stationary, string Event_sports, string Feeding_scheme, string divchoice)
        {
            var Fees = db.Fees.FirstOrDefault();
            string Administration = "YES";
            var qu = db.Fees.ToList().Take(1).FirstOrDefault();




            ViewBag.Ad = qu.AdminFee.ToString();
            ViewBag.Aft = qu.AfterHourFee.ToString();
            ViewBag.Del = qu.OneWayDiv.ToString();
            ViewBag.Sta = qu.StationaryFee.ToString();
            ViewBag.Uni = qu.UniformFee.ToString();
            ViewBag.Ev = qu.Event_sportFee.ToString();
            ViewBag.Fe = qu.Feeding_schemeFee.ToString();

            string AAdministration = "";
            string AAfterHours = "";
            string ADelivery = "";
            string AStationary = "";
            string AUniform = "";
            string AEvent_sports = "";
            string AFeeding_scheme = "";


            int Total = 0;
            if (Administration == "YES")
            {
                AAdministration = Fees.AdminFee.ToString();
                Total = Total + qu.AdminFee;
            }
            if (AfterHours == "YES")
            {
                AAfterHours = Fees.AfterHourFee.ToString();
                Total = Total + qu.AfterHourFee;
            }
            if (Delivery == "YES")
            {
                ADelivery = Fees.OneWayDiv.ToString();
                Total = Total + qu.OneWayDiv;
            }

            if (Stationary == "YES")
            {
                AStationary = Fees.StationaryFee.ToString();
                Total = Total + qu.StationaryFee;
            }

            if (Uniform == "YES")
            {
                AUniform = Fees.UniformFee.ToString();
                Total = Total + qu.UniformFee;
            }

            if (Event_sports == "YES")
            {
                AEvent_sports = Fees.Event_sportFee.ToString();
                Total = Total + qu.Event_sportFee;
            }

            if (Feeding_scheme == "YES")
            {
                AFeeding_scheme = Fees.Feeding_schemeFee.ToString();
                Total = Total + qu.Feeding_schemeFee;
            }


            Quotation quot = new Quotation();

            quot.Administration = AAdministration;
            quot.AfterHours = AAfterHours;
            quot.Delivery = ADelivery;
            quot.Uniform = AUniform;
            quot.ChildId = idd;
            quot.Stationary = AStationary;
            quot.Event_sports = AEvent_sports;
            quot.Feeding_scheme = AFeeding_scheme;
            quot.Total = Total;
            quot.Status = "NEW";

            db.Quotations.Add(quot);
            db.SaveChanges();

            int id = quot.Id;

            ViewBag.Total = Total.ToString();
            var q = db.Quotations.Where(x => x.Id == id).ToList().Take(1);
            ViewBag.Total = q.FirstOrDefault().Total.ToString();
            ViewBag.Id = idd.ToString();

            return View();
        }




        [Authorize]

        public ActionResult SubmitApplication(int id)
        {
            var user = db.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault();

            var qu = db.Quotations.Where(x => x.ChildId == id && x.Status == "NEW").ToList().OrderByDescending(b => b.Id).Take(1).FirstOrDefault();

            Quotation q = db.Quotations.Find(qu.Id);
            q.Status = "SUBMITED";

            db.SaveChanges();


            Child c = db.Children.Find(qu.ChildId);
            c.Status = "SUBMITED";
            db.SaveChanges();







            string _sender = "21916419@dut4life.ac.za";
            string _password = "Dut960401";



            string recipient = User.Identity.Name;
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
                mail.Body = "<HTML><BODY><p><div align='centre'>Please NOTE THAT YOUR APPLICATION HAS BEEN SUBMITED<br/>" +
                    "From STUDENT CORE SYSTEM</p></BODY></HTML>";

                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return View();
        }

        [Authorize]

        [HttpGet]
        public ActionResult Register(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [Authorize]

        [HttpPost]
        public ActionResult Register(HttpPostedFileBase cliniccard, ChildReg r, int id)
        {

            var chj = db.Children.Where(x => x.Id == id).FirstOrDefault();

            ChildReg reg = new ChildReg();

            reg.IdNumber = chj.IdNumb;
            reg.PlaceOfBirth = r.PlaceOfBirth;
            reg.Nationality = r.Nationality;
            reg.PreviosSchool = r.PreviosSchool;
            reg.ReasonForLeaving = r.ReasonForLeaving;
            reg.TransferNumber = r.TransferNumber;
            reg.Disability = r.Disability;
            reg.ClinicCard = cliniccard.FileName;

            db.ChildRegs.Add(reg);
            db.SaveChanges();




            var originalDirectoryy = new DirectoryInfo(string.Format("{0}Images\\Card", Server.MapPath(@"\")));
            var pathString22 = System.IO.Path.Combine(originalDirectoryy.ToString(), "Card\\" + id.ToString());


            if (!Directory.Exists(pathString22))
            {
                Directory.CreateDirectory(pathString22);
            }


            string report = cliniccard.FileName;


            //Check it's not null
            if (cliniccard != null && cliniccard.ContentLength > 0)
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Card", Server.MapPath(@"\")));
                var pathString2 = System.IO.Path.Combine(originalDirectory.ToString(), "Card\\" + id.ToString());
                var pathString3 = System.IO.Path.Combine(originalDirectory.ToString(), "Card\\" + id.ToString() + "\\Thumbs");
                var path = string.Format("{0}\\{1}", pathString2, report);
                cliniccard.SaveAs(path);
                string idd;
            }

            return Redirect("/Account/Payment?idd=" + id);
        }

        [Authorize]
        public ActionResult Payment(string idd)
        {

            int s = int.Parse(idd);
            var qo = db.Quotations.Where(x => x.ChildId == s).FirstOrDefault();
            int iddx = int.Parse(idd);

            return Redirect("https://www.payfast.co.za/eng/process?cmd=_paynow&receiver=16722238&item_name=REGISTRATION-FEE&item_description=REGISTRATION-FEE&return_url=https://2021grp16.azurewebsites.net/account/UpdateStatus&cancel_url=https://2021grp16.azurewebsites.net/account/UpdateStatus?id=" + iddx + "&amount=" + qo.Total + "&email_address=" + db.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault().EmailAddress);
        }

        [Authorize]

        public ActionResult UpdateStatus(int id, GetQuery g)
        {

            Child c = db.Children.Find(id);
            c.Status = "REGISTERED";
            c.LeearnerCode = g.Main();
            db.SaveChanges();

            var originalDirectoryx = new DirectoryInfo(string.Format("{0}Images\\Verify", Server.MapPath(@"\")));
            var pathString2x = System.IO.Path.Combine(originalDirectoryx.ToString());

            if (!Directory.Exists(pathString2x))
            {
                Directory.CreateDirectory(pathString2x);
            }

            QRCodeGenerator ObjQr = new QRCodeGenerator();

            QRCodeData qrCodeData = ObjQr.CreateQrCode(c.LeearnerCode, QRCodeGenerator.ECCLevel.Q);

            Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

            using (MemoryStream ms = new MemoryStream())

            {

                bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                byte[] byteImage = ms.ToArray();

                ViewBag.Url = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                bitMap.Save(Server.MapPath("~/images/Verify/" + User.Identity.Name + c.LeearnerCode + "qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
            }



            ViewBag.Email = c.ChildEmail;
            ViewBag.Name = c.FirstName + " " + c.LastName;
            ViewBag.IdNumber = c.IdNumb;
            ViewBag.Grade = c.Grade;
            ViewBag.Qr = User.Identity.Name + c.LeearnerCode + "qrcode.png";
            ViewBag.Pic = c.LearnerPic;





            var p = db.Users.Where(x => x.EmailAddress == c.ParentEmail).FirstOrDefault();

            UserRole userRoles = new UserRole()

            {
                RoleId = 4
            };

            db.UserRoles.Add(userRoles);
            db.SaveChanges();
            int cid = userRoles.UserId;

            //create userDTO
            User userDTO = new User()
            {
                Id = cid,
                FirstName = c.FirstName,
                LastName = c.LastName,
                EmailAddress = c.ChildEmail,
                Username = c.ChildEmail,
                Password = c.Password,
                PhoneNumber = p.PhoneNumber,
                Balance = 0,
                IdNumb = c.IdNumb,
                Date = DateTime.UtcNow.AddHours(2),
                Maritual = "Single",
                HomeLang = c.HomeLang,
                PostalAd = p.PostalAd,
                HomeAd = p.HomeAd,
                HomeNumber = p.HomeNumber,


            };
            string ystring = c.IdNumb.Substring(0, 2);
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
            string moth = c.IdNumb.Substring(3, 4);

            string date = c.IdNumb.Substring(0, 6);
            string day = c.IdNumb.Substring(5, 6);

            string birth = century + ystring + moth + day;

            userDTO.DateOfBirth = DateTime.UtcNow.AddHours(2);

            string ggen = "";

            string g2 = c.IdNumb.Substring(7);

            if (int.Parse(g2) >= 5)
            {
                ggen = "Male";
            }
            if (int.Parse(g2) <= 4)
            {
                ggen = "Female";
            }

            userDTO.Gender = ggen;

            db.Users.Add(userDTO);

            db.SaveChanges();


            string _sender = "21916419@dut4life.ac.za";
            string _password = "Dut960401";



            string recipient = User.Identity.Name;
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
                mail.Body = "<HTML><head><link rel = 'stylesheet' href = 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css'>"
       + "<style>.card {box - shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);max - width: 300px;margin: auto;text - align: center;font - family: arial;}.title {color: grey;font - size: 18px;}button {border: none;outline: 0;" +
                        "display: inline - block;padding: 8px;color: white;background - color: #000;text - align: center;cursor: pointer;width: 100 %;font - size: 18px;}a {text - decoration: none;font - size: 22px;color: black;}button: hover, a: hover {opacity: 0.7;}</style></head>" + "<BODY><p><div align='centre'>PLEASE NOTE:CHILD REGISTERED<br/>" +
                    "<br/> ID NUMBER:" + c.IdNumb + "<br/>" +
                    "From STUDENT CORE SYSTEM</p>" +
                    "<br/>" +
                    "<h2>LEARNER CARD</h2>" +
                    "<br/>" +

     "<div class='card'><img src =" + "'https://2021grp16.azurewebsites.net/images/DocsPic/DocsPic/" + c.LearnerPic + "'alt ='LERNNER PIC' style='width:100%'/><h3> " + c.FirstName + " " + c.LastName + "</h3> <img src = 'https://2021grp16.azurewebsites.net/images/Verify/" + ViewBag.Qr.ToString() + "'style='width:100%'/><p>Grade:" + c.Grade + "</p><p>Email: " + c.ChildEmail + "</p><p>" + c.IdNumb + "</p></div>"
    + "</BODY></HTML>";

                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }



            return View();
        }

        [Authorize]

        public ActionResult ChildProfile(int? id)
        {
            return View(db.Children.Where(x => x.Id == id).FirstOrDefault());
        }



        [Authorize]

        public ActionResult ChildDocs(int? id)
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




        // GET: account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/account/login");
        }


        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }



        // Post: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model, string add, string add1, string city, string city1, string prov, string prov1, int zip, int zip1)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            //check if password match

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password Does not Match");
                return View("CreateAccount", model);
            }
            using (Db db = new Db())
            {

                //make sure username is unique
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", "Username" + model.Username + "Is Taken");
                    model.Username = "";
                    return View("CreateAccount", model);
                }


                UserRole userRoles = new UserRole()

                {
                    RoleId = 2
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
                TempData["Success Message"] = "You are now registered an Account";

                //redirect  
                return Redirect("~/account/login");
            }
        }


        [Authorize]
        public ActionResult UserNavPartial()
        {

            //get the username
            string username = User.Identity.Name;
            //declare model
            UserNavPartialVM model;

            using (Db db = new Db())
            {

                //get the user
                User dto = db.Users.FirstOrDefault(x => x.Username == username);
                //build the model
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            //return patrial view with  model
            return PartialView(model);
        }



        // GET: account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {

            //get username
            string username = User.Identity.Name;

            //declare model
            UserProfileVM model;

            using (Db db = new Db())
            {
                //get user
                User dto = db.Users.FirstOrDefault(x => x.Username == username);

                //build model
                model = new UserProfileVM(dto);

            }
            //return view with model
            return View("UserProfile", model);
        }



        // Post: account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            //check if password match if need be
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password Does not Match");
                    return View("UserProfile", model);
                }

            }
            using (Db db = new Db())
            {
                //get username
                string username = User.Identity.Name;
                //make sure username is unique
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == username))
                {
                    ModelState.AddModelError("", "Username" + model.Username + "Already Exist");
                    model.Username = "";
                    return View("UserProfile", model);
                }
                //Edit DTO
                User dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                dto.Username = model.EmailAddress;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;

                }

                //Save
                db.SaveChanges();

            }
            //Set Temp Message
            TempData["Success Message"] = "You have edit your profile";


            //redirect
            return Redirect("~/account/user-profile");
        }

        public ActionResult Accademic(int id)
        {
            Child child = db.Children.Find(id);
            int gnum = int.Parse(child.Grade);
            var gsub = db.Gradesubjects.Where(x => x.GradeNumber == gnum).ToList();
            return View(gsub);
        }


        public ActionResult ParentPortal()
        {

            return View();
        }

        [HttpGet]
        public ActionResult Derigester(DelVM model)
        {
            model.MyChildren = new SelectList(db.Children.Where(x => x.ParentEmail == User.Identity.Name).ToList(), "Id", "ChildEmail");

            return View(model);
        }

        [HttpPost]
        public ActionResult Derigester(DelVM model,Deregister m)
        {

            Child ch = db.Children.Find(model.LearnerId);


            Deregister del = new Deregister()
            {
                LearnerId = model.LearnerId,
                LearnerEmail = ch.ChildEmail,
                Reason = model.Reason,
                Status = "WAITING FOR ADMIN",
                StatusNum = 1,
                Pemail = User.Identity.Name,
            };
            db.Deregisters.Add(del);
            db.SaveChanges();

            return RedirectToAction("DRequest");
        }


        public ActionResult DRequest()
        {
            var del = db.Deregisters.Where(x => x.Pemail == User.Identity.Name).ToList();
            return View(del);
        }

    }
}