namespace Topics.Data.Configuration;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class PostConfig
{
    public static void ConfigurePost(this ModelBuilder builder)
    {
        builder
            .Entity<Post>()
            .HasOne(x => x.Author)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<PostLink>()
            .HasOne(x => x.Post)
            .WithMany(x => x.Links)
            .HasForeignKey(x => x.PostId);

        builder
            .Entity<PostUpload>()
            .HasOne(x => x.Post)
            .WithMany(x => x.Uploads)
            .HasForeignKey(x => x.PostId);

        builder
            .Entity<PostVote>()
            .HasOne(x => x.Post)
            .WithMany(x => x.Votes)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}