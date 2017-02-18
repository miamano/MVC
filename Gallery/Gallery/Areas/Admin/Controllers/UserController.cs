using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gallery.Repositories;
using Gallery.Models;

namespace Gallery.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        public static GalleryRepositories Repo;

        // GET: Admin/User
        public ActionResult Index()
        {
            Repo = GalleryRepositories.getRepo();
            return View(Repo.SelectAllUsers());
        }

        [HttpGet]
        public ActionResult Edit(Guid userId)
        {
            return View(Repo.SelectUserByID(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GalleryUser user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.FullName))
                {
                    ModelState.AddModelError("error", "Error: username, password or fullname is empty!");
                    return View(user);
                }
                Repo.UpdateUser(user);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(Guid userId)
        {
            Repo.DeleteUser(userId);
            return RedirectToAction("Index");
        }
    }
}