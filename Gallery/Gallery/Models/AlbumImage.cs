using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery.Models
{
    [Table("AlbumImage")]
    public class AlbumImage
    {
        [Key, ForeignKey("Album"), Column(Order = 0), ScaffoldColumn(false)]
        public Guid AlbumId { get; set; }

        [Key, ForeignKey("Image"), Column(Order = 1), ScaffoldColumn(false)]
        public Guid ImageId { get; set; }

        public virtual Album Album { get; set; }
        public virtual Image Image { get; set; }
    }
}