using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class UserBooks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserBooksId { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public Guid BookId { get; set; }
        public BookEntity Book { get; set; } = null!;
    }
}