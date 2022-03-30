namespace Topics.Data.Configuration;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class CommentConfig
{
    public static void ConfigureComment(this ModelBuilder builder)
    {
        builder
            .Entity<Comment>()
            .HasOne(x => x.Author)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<Comment>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .IsRequired(false);

        builder
            .Entity<CommentVote>()
            .HasOne(x => x.Comment)
            .WithMany(x => x.Votes)
            .HasForeignKey(x => x.CommentId);
    }
}