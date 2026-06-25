using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers;

public class PostController : Controller
{
    private readonly AppDbContext _db;

    public PostController(AppDbContext db)
    {
        _db = db;
    }

    // GET /Post
    public async Task<IActionResult> Index()
    {
        var posts = await _db.Posts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(posts);
    }

    // GET /Post/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return NotFound();
        return View(post);
    }

    // GET /Post/Create
    public IActionResult Create() => View();

    // POST /Post/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Post post)
    {
        ModelState.Remove(nameof(Post.Slug)); // Remove Slug da  validação, pois será gerado automaticamente

        if (!ModelState.IsValid) return View(post);

        post.CreatedAt = DateTime.UtcNow;
        post.Slug = await GenerateUniqueSlugAsync(post.Title);

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET /Post/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return NotFound();
        return View(post);
    }

    // POST /Post/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Post post)
    {
        if (id != post.Id) return BadRequest();

        ModelState.Remove(nameof(Post.Slug)); // Remove Slug da  validação, pois será gerado automaticamente

        if (!ModelState.IsValid) return View(post);

        var existing = await _db.Posts.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Title = post.Title;
        existing.Slug = post.Slug;
        existing.Content = post.Content;
        existing.Summary = post.Summary;
        existing.CoverImageUrl = post.CoverImageUrl;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.Status = post.Status;

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST /Post/Publish/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return NotFound();

        if (post.Status != PostStatus.Published)
        {
            post.Status = PostStatus.Published;
            post.PublishedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // GET /Post/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return NotFound();
        return View(post);
    }

    // POST /Post/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return NotFound();

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GenerateUniqueSlugAsync(string title)
    {
        var slug = GenerateSlug(title);
        var uniqueSlug = slug;
        var counter = 1;

        while (await _db.Posts.AnyAsync(p => p.Slug == uniqueSlug))
        {
            uniqueSlug = $"{slug}-{counter++}";
        }

        return uniqueSlug;
    }

    private static string GenerateSlug(string title)
    {
        // Converte para minúsculas
        var slug = title.ToLowerInvariant();
        // Remove caracteres inválidos
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        // Substitui espaços por hífens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim('-');
        return slug;
    }
}
