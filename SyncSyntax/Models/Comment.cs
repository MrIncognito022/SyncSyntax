using System.ComponentModel.DataAnnotations;

namespace SyncSyntax.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User name is required.")]
        [MaxLength(100, ErrorMessage = "User name cannot exceed 100 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "User email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
        public string UserEmail { get; set; }
        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; } = DateTime.Now;

        [Required]
        public string Content { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
