using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Post>(post =>
        {
            post.HasKey(p => p.Id);

            post.Property(p => p.Slug)
                .IsRequired()
                .HasMaxLength(200);

            // Garante que dois posts não terão o mesmo slug
            post.HasIndex(p => p.Slug)
                .IsUnique();

            post.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            post.Property(p => p.Status)
                .HasConversion<string>(); // salva "Draft" no banco, não "0"
        });
    }
}