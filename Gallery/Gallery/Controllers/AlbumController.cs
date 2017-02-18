using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Gallery.Repositories;
using Gallery.Models;
using Gallery.ModelViews;
using static Gallery.IdentityConfig;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Gallery.Controllers
{
    public class AlbumController : Controller
    {
        public static GalleryRepositories Repo;

        public AlbumController()
        {
            Repo = GalleryRepositories.getRepo();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            AlbumView av = new AlbumView();
            av.Albums = Repo.SelectAllAlbums();
            return View(av);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetCommentsForAlbum(string albumId)
        {
            var comments = Repo.SelectCommentsFromAlbumByID(new Guid(albumId));

            if (comments != null || comments.Count != 0)
            {
                List<Object> list = new List<Object>();

                for (int i = 0; i < comments.Count; i++)
                {
                    var result = new
                    {
                        Date = comments[i].Date,
                        Text = comments[i].Text,
                    };
                    list.Add(result);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return new EmptyResult();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddComment(string albumId, string comment)
        {
            if (!string.IsNullOrWhiteSpace(comment))
            {
                Comment newComment = new Comment() { CommentId = Guid.NewGuid(), Text = comment, Date = DateTime.Now };
                Repo.InsertAlbumComment(new Guid(albumId), newComment);
            }
            return new EmptyResult();
        }
 
        [Authorize]
        [HttpGet]
        public ActionResult CreateAlbum()
        {
            return PartialView("CreateAlbum");
        }

        [Authorize]
        [HttpPost]
        public ActionResult AjaxCreateAlbum(string name)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    ModelState.AddModelError("error", "Error: Albumname is empty!");
                    return View();
                }
                if (name.ToLower().CompareTo("All images".ToLower()) == 0)
                {
                    ModelState.AddModelError("error", "Special album. Dont't use this name.");
                    return View();
                }

                Guid albumId = Guid.NewGuid();
                Album newAlbum = new Album() { AlbumId = albumId, AlbumName = name, CreateDate = DateTime.Now };
                Repo.InsertAlbum(newAlbum);
            }
            return new EmptyResult();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult OpenAlbum(Guid albumId)
        {
            return View(albumId);
        }

        [HttpGet]
        public ActionResult AjaxOpenAlbum(Guid id)
        {
            List<Guid> tmpImageIDs = (Repo.GetImageIDsForAlbum(id));
            return Json(tmpImageIDs, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Guid albumId)
        {
            AlbumView av = new AlbumView();
            if (Repo.IsAlbumSpecial(albumId))
            {
                string userId = User.Identity.GetUserId();
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                if (userManager.IsInRole(userId, "Admin"))
                {
                    av.Album = Repo.SelectAlbumByID(albumId);
                    return PartialView(av);
                }
                else
                {
                    ModelState.AddModelError("error", "Error: Only admin can edit this album.");
                    return new EmptyResult();
                }
            }
            else
            {
                av.Album = Repo.SelectAlbumByID(albumId);
                return PartialView(av);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AjaxEdit(string id, string name)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    ModelState.AddModelError("error", "Error: Albumname is empty!");
                    return View();
                }
                if (Repo.IsAlbumSpecial(new Guid(id)) && (name.ToLower().CompareTo("All images".ToLower()) != 0))
                {
                    ModelState.AddModelError("error", "Error: Special catalog. It has to be called: All images");
                    return View();
                }
                else
                {
                    Album updateAlbum = new Album();
                    updateAlbum.AlbumId = new Guid(id);
                    updateAlbum.AlbumName = name;
                    Repo.UpdateAlbum(updateAlbum);
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete(Guid albumId)
        {
            if (Repo.IsAlbumSpecial(albumId))
            {
                ModelState.AddModelError("error", "Error: Noone can delete this album.");
            }
            else
            {
                Repo.DeleteAlbum(albumId);
            }
            return RedirectToAction("Index");
        }
    }
}