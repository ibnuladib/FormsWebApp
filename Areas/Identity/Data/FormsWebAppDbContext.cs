using FormsWebApplication.Models;
using FormsWebApplication.Models.FormsWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormsWebApplication.Data;

public class FormsWebAppDbContext : IdentityDbContext<ApplicationUser>
{
    public FormsWebAppDbContext(DbContextOptions<FormsWebAppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Template> Templates { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User - Template Relationship (Cascade Delete)
        builder.Entity<Template>()
            .HasOne(t => t.Author)
            .WithMany(u => u.Templates)
            .HasForeignKey(t => t.AuthorId)
            .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, delete their templates

        // User - Answer Relationship (Cascade Delete)
        builder.Entity<Answer>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, delete their answers

        // User - Comment Relationship (Cascade Delete)
        builder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, delete their comments

        // User - Like Relationship (Cascade Delete)
        builder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, delete their likes

        // Template - Answer Relationship (Cascade Delete)
        builder.Entity<Answer>()
            .HasOne(a => a.Template)
            .WithMany()
            .HasForeignKey(a => a.TemplateId)
            .OnDelete(DeleteBehavior.Restrict); //null

        // Template - Comment Relationship (Cascade Delete)
        builder.Entity<Comment>()
            .HasOne(c => c.Template)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TemplateId)
            .OnDelete(DeleteBehavior.Restrict); // If a template is deleted, delete its comments

        // Template - Like Relationship (Cascade Delete)
        builder.Entity<Like>()
            .HasOne(l => l.Template)
            .WithMany(t => t.Likes)
            .HasForeignKey(l => l.TemplateId)
            .OnDelete(DeleteBehavior.Restrict); // If a template is deleted, delete its likes

        // Many-to-Many: Template - Tags Relationship
        builder.Entity<TemplateTag>()
            .HasKey(tt => new { tt.TemplateId, tt.TagId });

        builder.Entity<TemplateTag>()
            .HasOne(tt => tt.Template)
            .WithMany(t => t.TemplateTags)
            .HasForeignKey(tt => tt.TemplateId)
            .OnDelete(DeleteBehavior.Cascade); // If a template is deleted, remove its tag links

        builder.Entity<TemplateTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TemplateTags)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Restrict); // Deleting a tag does NOT delete templates

        // Like Unique Constraint (Prevents duplicate likes per User-Template)
        builder.Entity<Like>()
            .HasIndex(l => new { l.UserId, l.TemplateId })
            .IsUnique();

        // Tag Unique Constraint (Ensure tags have unique names)
        builder.Entity<Tag>()
            .HasIndex(t => t.TagName)
            .IsUnique();
    }

}
