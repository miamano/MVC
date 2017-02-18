using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Gallery.Models
{
    [Table("Comment")]
    public class Comment
    {
        //public enum Category { Album , Image }
        [Key]
        public Guid CommentId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        //public Category Origin { get; set; }

        public virtual Album Album { get; set; }
        public virtual Image Image { get; set; }
        public virtual GalleryUser GalleryUser { get; set; }
    }
}