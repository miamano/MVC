using System.Collections.Generic;
using Gallery.Models;

namespace Gallery.ModelViews
{
    public class AlbumView
    {
        public Album Album { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}