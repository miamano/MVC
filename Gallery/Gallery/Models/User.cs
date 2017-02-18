using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Gallery.Models
{
    [Table("User")]
    public class GalleryUser : IdentityUser    {
        public GalleryUser()
        {
            this.Comments = new HashSet<Comment>();
        }

        [Required]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}