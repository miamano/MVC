using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gallery.Models;
using Gallery.ModelViews;
using Gallery.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using static Gallery.IdentityConfig;

namespace Gallery.Controllers
{
    public class ImageController : Controller
    {
        public static GalleryRepositories Repo;

        [AllowAnonymous]
        [HttpGet]
        public ActionResult AjaxGetImage(Guid id)
        {
            Repo = GalleryRepositories.getRepo();
            Image img = Repo.SelectImageByID(id);

            var result = new {
                ImageId = img.ImageId,
                FileName = img.FileName,
                Title = img.Title,
                Description = img.Description,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Ajax
        [Authorize]
        [HttpPost]
        public ActionResult Edit(string imageId, string albumId)
        {
            ImageView iv = new ImageView();
            iv.AlbumId = new Guid(albumId);
            if (Repo.IsAlbumSpecial(iv.AlbumId))
            {
                string userId = User.Identity.GetUserId();
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                
                if (userManager.IsInRole(userId, "Admin"))
                {
                    iv.Image = Repo.SelectImageByID(new Guid(imageId));
                    return PartialView(iv);
                }
                else
                {
                    ModelState.AddModelError("error", "Error: Only admin can edit this image.");
                    return RedirectToAction("AjaxGetImage", new { albumId = albumId });
                }
            }
            else
            {
                iv.Image = Repo.SelectImageByID(new Guid(imageId));
                return PartialView(iv);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AjaxEdit(string albumId, string imgId, string title, string description)
        {
            if (ModelState.IsValid)
            {
                ImageView iv = new ImageView();
                iv.Image = new Models.Image(){ ImageId = new Guid(imgId), Title = title, Description = description };
                Repo.UpdateImage(iv.Image);
            }
            return new EmptyResult();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Details(string imageId, string albumId)
        {
            ImageView iv = new ImageView();
            iv.AlbumId = new Guid(albumId);
            iv.Image = Repo.SelectImageByID(new Guid(imageId));
            return PartialView(iv);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AjaxDetails(ImageView imgView)      
        {
            if (ModelState.IsValid)
            {
                Repo.UpdateImage(imgView.Image);
            }
            return new EmptyResult();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddComment(string imageId, string albumId)
        {
            ImageView iv = new ImageView();
            iv.AlbumId = new Guid(albumId);
            iv.Image = Repo.SelectImageByID(new Guid(imageId));
            return PartialView(iv);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AjaxAddComment(string albumId, string imageId, string comment)
        {
            Comment newComment = new Comment() { CommentId = Guid.NewGuid(), Text = comment, Date = DateTime.Now };
            Repo.InsertImageComment(new Guid(imageId), newComment);
            return new EmptyResult();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetCommentsForImage(string imageId)
        {
            var comments = Repo.SelectCommentsFromImageByID(new Guid(imageId));
            comments.OrderByDescending(s => s.Date);

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

        [Authorize]
        [HttpPost]
        public ActionResult Delete(Guid imageId, Guid albumId)
        {
            if (Repo.IsAlbumSpecial(albumId))
            {
                string userId = User.Identity.GetUserId();
                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                if (userManager.IsInRole(userId, "Admin"))
                {
                    Repo.DeleteImage(imageId, albumId);
                }
                else
                {
                    ModelState.AddModelError("error", "Error: Only admin can edit this image.");
                }
            }
            else
            {
                Repo.DeleteImage(imageId, albumId);
            }
            return new EmptyResult();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Upload(Guid albumId)
        {
            ImageView iv = new ImageView();
            iv.AlbumId = albumId;
            return View(iv);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Upload(ImageView imgView, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (files == null || files.ElementAt(0) == null || files.ElementAt(0).ContentLength == 0)
                {
                    ModelState.AddModelError("error", "Please, upload a file.");
                    return View(imgView);
                }
                else
                {
                    var tmpExtraSeconds = 0;
                    foreach (var file in files)
                    {
                        string newImg = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(
                                               Server.MapPath("~/Images"), newImg);
                        if (newImg.ToLower().EndsWith(".jpg") || newImg.ToLower().EndsWith(".png"))
                        {
                            // file is uploaded
                            file.SaveAs(path);

                            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                            {
                                file.InputStream.CopyTo(ms);
                                byte[] array = ms.GetBuffer();
                            }

                            if (imgView.Image.Title == null) imgView.Image.Title = "Title";
                            if (imgView.Image.Description == null) imgView.Image.Description = "Description";

                            Image newImage = new Image()
                            {
                                ImageId = Guid.NewGuid(),
                                FileName = newImg,
                                Title = imgView.Image.Title,
                                Description = imgView.Image.Description,
                                Comments = new List<Comment>(),
                                CreateDate = DateTime.Now.AddSeconds(tmpExtraSeconds)
                            };
                            Repo = GalleryRepositories.getRepo();
                            Repo.InsertImage(newImage, imgView.AlbumId);
                            tmpExtraSeconds += 5;
                        }
                    }
                }
            }
            return RedirectToAction("OpenAlbum", "Album", new { albumId = imgView.AlbumId });
        }
    }
}