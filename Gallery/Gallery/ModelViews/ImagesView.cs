using System.Collections.Generic;
using Gallery.Models;

namespace Gallery.ModelViews
{
    public class ImagesView
    {
        public Album Album { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}