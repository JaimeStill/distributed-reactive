namespace Topics.Data.Configuration;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class TopicConfig
{
    public static void ConfigureTopic(this ModelBuilder builder)
    {
        builder
            .Entity<Topic>()
            .HasOne(x => x.Owner)
            .WithMany(x => x.Topics)
            .HasForeignKey(x => x.OwnerId);

        builder
            .Entity<TopicUser>()
            .HasOne(x => x.Topic)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.TopicId);

        builder
            .Entity<TopicUser>()
            .HasOne(x => x.User)
            .WithMany(x => x.Memberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}