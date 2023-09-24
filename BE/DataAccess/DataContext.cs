using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DataAccess;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    { }

    public DbSet<AuthorEntity> Authors { get; set; } = null!;
    public DbSet<BookEntity> Books { get; set; } = null!;
    public DbSet<BookItemEntity> BookItems { get; set; } = null!;
    public DbSet<PublisherEntity> Publishers { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<AdministratorEntity> Administrators { get; set; } = null!;
    public DbSet<CustomerEntity> Customers { get; set; } = null!;
    public DbSet<LibrarianEntity> Librarians { get; set; } = null!;
    public DbSet<AddressEntity> Addresses { get; set; } = null!;
    public DbSet<RoleEntity> Roles { get; set; } = null!;
    public DbSet<BookRequestEntity> BookRequests { get; set; } = null!;
    public DbSet<BookLoanEntity> BookLoans { get; set; } = null!;
    public DbSet<BlobMetadata> BlobsMetadata { get; set; } = null!;
    public DbSet<CommentEntity> Comments { get; set; } = null!;
    public DbSet<UserBooks> UserBooks{ get; set; } = null!;
    public DbSet<GenreEntity> Genres { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookEntity>(builder =>
        {
            builder
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .UsingEntity<BookAuthor>(
                ba => ba
                    .HasOne(ba => ba.AuthorEntity)
                    .WithMany()
                    .HasForeignKey(a => a.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict),
                ba => ba
                    .HasOne(ba => ba.BookEntity)
                    .WithMany()
                    .HasForeignKey(b => b.BookId))
                .ToTable("BookAuthors")
                .HasKey(ba => new { ba.AuthorId, ba.BookId });

            builder
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasMany(b => b.BookItems)
                .WithOne(b => b.Book)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasIndex(b => b.Isbn).IsUnique();
            builder
                .HasOne(b => b.BlobMetadata)
                .WithOne(b => b.BookEntity)
                .HasForeignKey<BookEntity>(b => b.BlobMetadataId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<BookItemEntity>(builder =>
        {
            builder
                .HasOne(bi => bi.Book)
                .WithMany(b => b.BookItems)
                .HasForeignKey(bi => bi.BookId);
            builder
                .HasIndex(bi => bi.Barcode).IsUnique();
        });

        modelBuilder.Entity<UserEntity>(builder =>
        {
            builder
                .HasMany(u => u.Librarians)
                .WithOne(l => l.User);

            builder
                .HasMany(u => u.Customers)
                .WithOne(l => l.User);

            builder
                .HasMany(u => u.Administrators)
                .WithOne(l => l.User);

            builder
                .HasIndex(u => u.EmailAddress).IsUnique();
        });

        modelBuilder.Entity<CustomerEntity>(builder =>
        {
            builder
                .HasOne(c => c.Address)
                .WithOne(a => a.Customer)
                .HasForeignKey<CustomerEntity>(c => c.AddressId);
        });

        modelBuilder.Entity<RoleEntity>(builder =>
        {
            builder
                .HasMany(r => r.Users)
                .WithOne(u => u.Role);
        });

        modelBuilder.Entity<BookLoanEntity>(builder =>
        {
            builder
                .HasOne(le => le.Customer)
                .WithMany(c => c.BookLoans)
                .HasForeignKey(le => le.LoanedTo);

            builder
                .HasOne(le => le.Librarian)
                .WithMany(l => l.BookLoans)
                .HasForeignKey(le => le.LoanedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(le => le.BookItem)
                .WithOne(i => i.BookLoan)
                .HasForeignKey<BookItemEntity>(i => i.BookLoanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BookRequestEntity>(builder =>
        {
            builder
                .HasOne(re => re.Customer)
                .WithMany(c => c.BookRequests)
                .HasForeignKey(re => re.RequestedBy);

            builder
                .HasOne(re => re.Librarian)
                .WithMany(l => l.BookRequests)
                .HasForeignKey(re => re.AuditedBy);

            builder
                .HasOne(re => re.BookItem)
                .WithMany(i => i.BookRequests)
                .HasForeignKey(re => re.BookItemId);
        });

        modelBuilder.Entity<CommentEntity>(builder =>
        {
            builder
                .HasOne(c => c.Book)
                .WithMany(b => b.Comments);

            builder
                .HasOne(c => c.User)
                .WithMany(u => u.Comments);
        });

        modelBuilder.Entity<UserBooks>(builder =>
        {
            builder
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBooks)
                .HasForeignKey(ub => ub.UserId);

            builder
                .HasOne(ub => ub.Book)
                .WithMany(b => b.UserBooks)
                .HasForeignKey(ub => ub.BookId);
        });

        modelBuilder.Entity<AuthorEntity>()
            .HasIndex(a => a.Name).IsUnique();

        modelBuilder.Entity<PublisherEntity>()
            .HasIndex(p => p.Name).IsUnique();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        Save();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public void Save()
    {
       var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntity && 
            (e.State == EntityState.Added 
            || e.State == EntityState.Modified) 
            );

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
               ((AuditableEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
            }
            else
            {
                Entry((AuditableEntity)entry.Entity).Property(p => p.CreatedAt).IsModified = false;
            }
            ((AuditableEntity)entry.Entity).ModifiedAt = DateTime.UtcNow;
        }
    }
}