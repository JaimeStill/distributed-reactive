[assembly:System.Runtime.Versioning.SupportedOSPlatform("windows")]
namespace Topics.Data;

using Topics.Data.Configuration;
using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentVote> CommentVotes { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostLink> PostLinks { get; set; }
    public DbSet<PostUpload> PostUploads { get; set; }
    public DbSet<PostVote> PostVotes { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<TopicImage> TopicImages { get; set; }
    public DbSet<TopicUser> TopicUsers { get; set; }
    public DbSet<Upload> Uploads { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserImage> UserImages { get; set; }
    public DbSet<Vote> Votes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ConfigureComment();
        builder.ConfigurePost();
        builder.ConfigureTopic();
        builder.ConfigureUpload();
        builder.ConfigureUser();
        builder.ConfigureVote();

        builder
            .Model
            .GetEntityTypes()
            .Where(x => x.BaseType == null)
            .ToList()
            .ForEach(x =>
            {
                builder
                    .Entity(x.Name)
                    .ToTable(x.Name.Split('.').Last());
            });
    }
}