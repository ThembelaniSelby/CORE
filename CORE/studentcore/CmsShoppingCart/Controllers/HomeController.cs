using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        Db db = new Db();
        // GET: Home
        public ActionResult Home()
        {
            return View();
        }

       
    }
}