using Gallery.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using static Gallery.IdentityConfig;

namespace Gallery.Controllers
{
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Information()
        {
            ViewBag.Message = "Your info page.";

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

    }
}