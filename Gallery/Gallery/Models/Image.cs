using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery.Models
{
    [Table("Image")]
    public class Image
    {
        public Image()
        {
            this.AlbumImages = new HashSet<AlbumImage>();
            this.Comments = new HashSet<Comment>();
        }

        [Key]
        public Guid ImageId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<AlbumImage> AlbumImages { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}