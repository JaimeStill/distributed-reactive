namespace Topics.Data.Configuration;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class UploadConfig
{
    public static void ConfigureUpload(this ModelBuilder builder)
    {
        builder
            .Entity<TopicImage>()
            .HasOne(x => x.Topic)
            .WithOne(x => x.Image)
            .HasForeignKey<TopicImage>(x => x.TopicId)
            .IsRequired(false);

        builder
            .Entity<UserImage>()
            .HasOne(x => x.User)
            .WithOne(x => x.Image)
            .HasForeignKey<UserImage>(x => x.UserId)
            .IsRequired(false);

        builder
            .Entity<Upload>()
            .HasDiscriminator(x => x.Type)
            .HasValue<Upload>("upload")
            .HasValue<PostUpload>("post-upload")
            .HasValue<TopicImage>("topic-image")
            .HasValue<UserImage>("user-image");
    }
}