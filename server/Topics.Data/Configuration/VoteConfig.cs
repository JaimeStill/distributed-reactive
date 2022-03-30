namespace Topics.Data.Configuration;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class VoteConfig
{
    public static void ConfigureVote(this ModelBuilder builder)
    {
        builder
            .Entity<Vote>()
            .HasOne(x => x.Voter)
            .WithMany(x => x.Votes)
            .HasForeignKey(x => x.VoterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<Vote>()
            .HasDiscriminator(x => x.Type)
            .HasValue<Vote>("vote")
            .HasValue<CommentVote>("comment-vote")
            .HasValue<PostVote>("post-vote");
    }
}