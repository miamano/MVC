using System;
using System.Collections.Generic;
using System.Linq;
using Gallery.Models;

namespace Gallery.Repositories
{
    //TODO - improvement: do generic and heritage of Repo
    public class GalleryRepositories
    {
        public static GalleryRepositories repo = null;

        public static GalleryRepositories getRepo()
        {
            if (repo == null)
            {
                repo = new GalleryRepositories();
            }
            return repo;
        }

        /****    IMAGE    ****/

        public List<Image> SelectAllImages()
        {
            using (var context = new GalleryContext())
            {
                return context.Images.OrderByDescending(s => s.CreateDate).ToList();
            }
        }

        public List<Image> SelectAllImagesByID(List<Guid> imgIdList)
        {
            using (var context = new GalleryContext())
            {
                List<Image> tmp = new List<Image>();
                foreach (var id in imgIdList)
                {
                    var image = context.Images.Where(p => p.ImageId == id).FirstOrDefault();
                    tmp.Add(image);
                }
                tmp.OrderByDescending(s => s.CreateDate);
                return tmp;
            }
        }

        public Image SelectImageByID(Guid id)
        {
            using (var context = new GalleryContext())
            {
                var image = context.Images.Where(p => p.ImageId == id).FirstOrDefault();
                return image;
            }
        }

        public List<Comment> SelectCommentsFromImageByID(Guid id)
        {

            List<Comment> list = new List<Comment>();
            using (var context = new GalleryContext())
            {
                list = context.Images.Where(p => p.ImageId == id).FirstOrDefault().Comments.ToList();
            }
            list.OrderByDescending(s => s.Date);
            return list;
        }

        public void UpdateImage(Image img)
        {
            using (var context = new GalleryContext())
            {
                var image = context.Images.Where(p => p.ImageId == img.ImageId).FirstOrDefault();
                image.Title = img.Title;
                image.Description = img.Description;
                //image.CreateDate = img.CreateDate;
                context.SaveChanges();
            }
        }

        public void DeleteImage(Guid imgId, Guid albumId)
        {
            DeleteImageFromAlbumImage(imgId, albumId);
        }

        public void InsertImage(Image img, Guid albumId)
        {
            using (var context = new GalleryContext())
            {
                context.Images.Add(img);
                context.AlbumImages.Add(new AlbumImage() { AlbumId = albumId, ImageId = img.ImageId });
                if (!IsAlbumSpecial(albumId))
                {
                    var albumAllImg = context.Albums.Where(a => a.AlbumName == "All images").FirstOrDefault();
                    if (albumAllImg != null)
                    {
                        context.AlbumImages.Add(new AlbumImage() { AlbumId = albumAllImg.AlbumId, ImageId = img.ImageId });
                    }
                }
                context.SaveChanges();
            }
        }

        public void InsertImageComment(Guid imgID, Comment newComment)
        {
            using (var context = new GalleryContext())
            {
                var image = context.Images.Where(p => p.ImageId == imgID).FirstOrDefault();
                image.Comments.Add(newComment);
                context.SaveChanges();
            }
        }

        /****     ALBUM    ****/

        public void InsertAlbumComment(Guid albumId, Comment newComment)
        {
            using (var context = new GalleryContext())
            {
                var album = context.Albums.Where(p => p.AlbumId == albumId).FirstOrDefault();
                album.Comments.Add(newComment);
                context.SaveChanges();
            }
        }

        public List<Comment> SelectCommentsFromAlbumByID(Guid id)
        {

            List<Comment> list = new List<Comment>();
            using (var context = new GalleryContext())
            {
                list = context.Albums.Where(p => p.AlbumId == id).FirstOrDefault().Comments.ToList();
            }
            list.OrderByDescending(s => s.Date);
            return list;
        }

        public List<Album> SelectAllAlbums()
        {
            using (var context = new GalleryContext())
            {
                //return context.Albums.Select(s => s).ToList();
                return context.Albums.OrderByDescending(s => s.CreateDate).ToList();
            }
        }

        public Album SelectAlbumByID(Guid id)
        {
            using (var context = new GalleryContext())
            {
                return context.Albums.Where(p => p.AlbumId == id).FirstOrDefault();
            }
        }

        public void UpdateAlbum(Album alb)
        {
            using (var context = new GalleryContext())
            {
                var album = context.Albums.Where(p => p.AlbumId == alb.AlbumId).FirstOrDefault();
                album.AlbumName = alb.AlbumName;
                //album.AlbumComment = alb.AlbumComment;
                //album.CreateDate = alb.CreateDate;
                context.SaveChanges();
            }
        }

        public void DeleteAlbum(Guid albumId)
        {
            using (var context = new GalleryContext())
            {
                var deleteItems = context.AlbumImages.Where(ai => ai.AlbumId == albumId).ToList();
                var albumComments = context.Comments.Where(a => a.Album.AlbumId == albumId).ToList();

                foreach (var di in deleteItems)
                {
                    var imgComments = context.Comments.Where(p => p.Image.ImageId == di.ImageId).ToList();
                    foreach (var ic in imgComments)
                    {
                        context.Comments.Remove(ic);
                    }
                    context.AlbumImages.Remove(di);
                }

                foreach (var ac in albumComments)
                {
                    context.Comments.Remove(ac);
                }


                var item = context.Albums.Where(p => p.AlbumId == albumId).FirstOrDefault();
                context.Albums.Remove(item);

                context.SaveChanges();
            }
        }

        public void InsertAlbum(Album newAlbum)
        {
            using (var context = new GalleryContext())
            {
                context.Albums.Add(newAlbum);
                context.SaveChanges();
            }
        }

        public bool IsAlbumSpecial(Guid albumId)
        {
            using (var context = new GalleryContext())
            {
                var item = context.Albums.Where(p => p.AlbumId == albumId).FirstOrDefault();
                return (item.AlbumName == "All images");
            }
        }

        /****    ALBUMIMAGE    ****/

        public void DeleteImageFromAlbumImage(Guid imageId, Guid albumId)
        {
            using (var context = new GalleryContext())
            {
                context.AlbumImages.Remove(context.AlbumImages.Where<AlbumImage>(ai => ai.AlbumId == albumId && ai.ImageId == imageId).FirstOrDefault());
                context.SaveChanges();
            }
        }

        public List<Guid> GetImageIDsForAlbum(Guid albumId)
        {
            using (var context = new GalleryContext())
            {
                var filter = context.AlbumImages.Where<AlbumImage>(ai => ai.AlbumId == albumId);
                return filter.Select(p => p.ImageId).ToList();
            }
        }


        /****** USERS ******/
        public List<GalleryUser> SelectAllUsers()
        {
            using (var context = new GalleryContext())
            {
                return context.Users.Select(s => s).ToList();
            }
        }

        public GalleryUser SelectUserByID(Guid userId)
        {
            using (var context = new GalleryContext())
            {
                var user = context.Users.Where(p => p.Id == userId.ToString()).FirstOrDefault();
                return user;
            }
        }

        public void UpdateUser(GalleryUser person)
        {
            using (var context = new GalleryContext())
            {
                var user = context.Users.Where(p => p.Id == person.Id).FirstOrDefault();
                user.FullName = person.FullName;
                user.Email = person.Email;
                user.Password = person.Password;
                user.UserName = person.UserName;
                context.SaveChanges();
            }
        }

        public void DeleteUser(Guid userId)
        {
            using (var context = new GalleryContext())
            {
                context.Users.Remove(context.Users.Where<GalleryUser>(p => p.Id == userId.ToString()).FirstOrDefault());
                context.SaveChanges();
            }
        }
    }
}