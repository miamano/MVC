using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Gallery.Models
{
    public class GalleryContext : IdentityDbContext<GalleryUser>
    {
        public GalleryContext() : base("name=GalleryCS")
        {
        }

        public static GalleryContext Create()
        {
            return new GalleryContext();
        }

        //public DbSet<GalleryUser> GalleryUsers { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AlbumImage> AlbumImages { get; set; }
    }
}