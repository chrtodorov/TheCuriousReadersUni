using BusinessLayer.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class CommentEntity : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CommentId { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; } = null!;

        [Required]
        public CommentStatus Status { get; set; }

        public BookEntity Book { get; set; } = null!;
        public Guid BookId { get; set; }

        public UserEntity User { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}