namespace Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Gallery.Models;
    using System.Collections.Generic;
    using Microsoft.AspNet.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<Gallery.Models.GalleryContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Gallery.Models.GalleryContext context)
        {
            /***** Load Images *****/
            DateTime date = DateTime.Today;
            Comment c = new Comment() { CommentId = Guid.NewGuid(), Text = "Nice..", Date = date };
            Comment d = new Comment() { CommentId = Guid.NewGuid(), Text = "OMG", Date = date.AddDays(-1) };
            Comment e = new Comment() { CommentId = Guid.NewGuid(), Text = "I want to be there..", Date = date.AddDays(-2) };

            context.Comments.AddOrUpdate(x => x.CommentId, c, d, e);

            List<Comment> lc1 = new List<Comment>();
            lc1.Add(c);
            List<Comment> lc2 = new List<Comment>();
            lc2.Add(c);
            lc2.Add(d);
            List<Comment> lc3 = new List<Comment>();
            lc3.Add(c);
            lc3.Add(d);
            lc3.Add(e);

            DateTime now = DateTime.Now;
            List<Image> imageList = new List<Image> {
                                new Image() { ImageId = Guid.NewGuid(), FileName = "one.jpg"  , Title = "One"  , Comments = new List<Comment>(), Description = "2016", CreateDate = now } ,
                                new Image() { ImageId = Guid.NewGuid(), FileName = "two.jpg"  , Title = "Two"  , Comments = lc1, Description = "2016", CreateDate = now.AddSeconds(5) } ,
                                new Image() { ImageId = Guid.NewGuid(), FileName = "three.jpg", Title = "Three", Comments = lc3, Description = "2016", CreateDate = now.AddSeconds(10) } ,
                                new Image() { ImageId = Guid.NewGuid(), FileName = "four.jpg" , Title = "Four" , Comments = lc2, Description = "2016", CreateDate = now.AddSeconds(15) } ,
                                new Image() { ImageId = Guid.NewGuid(), FileName = "five.jpg" , Title = "Five" , Comments = new List<Comment>(), Description = "2016", CreateDate = now.AddSeconds(20) } ,
                                new Image() { ImageId = Guid.NewGuid(), FileName = "six.jpg"  , Title = "Six"  , Comments = new List<Comment>(), Description = "2016", CreateDate = now.AddSeconds(25) } ,
                                new Image() { ImageId = Guid.NewGuid(), FileName = "seven.jpg", Title = "Seven", Comments = new List<Comment>(), Description = "2016", CreateDate = now.AddSeconds(30) }
                };

            foreach (Image i in imageList)
            {
                context.Images.AddOrUpdate(x => x.ImageId, i);
            }

            /***** Load Albums *****/
            var albumAll = new Album() { AlbumId = Guid.NewGuid(), AlbumName = "All images", CreateDate = now };
            var albumFrance = new Album() { AlbumId = Guid.NewGuid(), AlbumName = "France", CreateDate = now.AddSeconds(5) };

            context.Albums.AddOrUpdate(x => x.AlbumId,
                                albumAll,
                                albumFrance);

            /***** Load AlbumImages *****/
            foreach (var image in imageList)
            {
                context.AlbumImages.Add(new AlbumImage() { AlbumId = albumAll.AlbumId, ImageId = image.ImageId });
            }

            context.AlbumImages.Add(new AlbumImage() { AlbumId = albumFrance.AlbumId, ImageId = imageList[3].ImageId });
            context.AlbumImages.Add(new AlbumImage() { AlbumId = albumFrance.AlbumId, ImageId = imageList[4].ImageId });
        }
    }
}
