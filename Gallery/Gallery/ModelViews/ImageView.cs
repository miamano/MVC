using System;
using Gallery.Models;

namespace Gallery.ModelViews
{
    public class ImageView
    {
        public Guid AlbumId { get; set; }
        public Image Image { get; set; }
    }
}