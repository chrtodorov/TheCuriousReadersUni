using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class BookEntity:AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid BookId { get; set; }

    [Required]
    [MaxLength(17)]
    public string Isbn  { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Genre { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string CoverUrl { get; set; } = string.Empty;
    
    [Required]
    public PublisherEntity? Publisher { get; set; }

    public Guid? PublisherId { get; set; }

    [Required]
    [MinLength(1)]
    public ICollection<AuthorEntity>? Authors { get; set; }

    public ICollection<BookItemEntity>? BookItems { get; set; }
    public ICollection<CommentEntity> Comments { get; set; } = new HashSet<CommentEntity>();
    public ICollection<UserBooks> UserBooks { get; set; } = new HashSet<UserBooks>();

    public Guid? BlobMetadataId { get; set; }
    public BlobMetadata? BlobMetadata { get; set; }
}