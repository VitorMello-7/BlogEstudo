using System.ComponentModel.DataAnnotations;


namespace BlogApp.Models;

public class Post
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Summary { get; set; }

    public string? CoverImageUrl { get; set; }

    public PostStatus Status { get; set; } = PostStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}

public enum PostStatus
{
    Draft,
    Published,
    Archived
}