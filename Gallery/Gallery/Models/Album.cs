using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gallery.Models
{
    [Table("Album")]
    public class Album
    {
        public Album()
        {
            this.AlbumImages = new HashSet<AlbumImage>();
            this.Comments = new HashSet<Comment>();
        }

        [Key]
        public Guid AlbumId { get; set; }
        public string AlbumName { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<AlbumImage> AlbumImages { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}