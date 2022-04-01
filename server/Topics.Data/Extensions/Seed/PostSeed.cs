namespace Topics.Data.Extensions.Seed;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class PostSeed
{
    public static async Task<Post> SeedPost(this AppDbContext db, Topic topic, User user)
    {
        if (await db.Posts.AnyAsync(x => x.TopicId == topic.Id && x.Title == "Detailed Post"))
        {
            Console.WriteLine("Retrieving post...");

            return await db.Posts
                .FirstOrDefaultAsync(x =>
                    x.TopicId == topic.Id
                    && x.Title == "Detailed Post"
                );
        }
        else
        {
            Console.WriteLine("Seeding post...");

            var post = new Post
            {
                AuthorId = user.Id,
                TopicId = topic.Id,
                Title = "Detailed Post",
                Description = "A description",
                DateCreated = new DateTime(2020, 01, 24),
                DatePublished = new DateTime(2020, 01, 25),
                IsPublished = true,
                Text = "This is a test post seeded by the database.",
                Url = "details-detailedpost"
            };

            await db.Posts.AddAsync(post);
            await db.SaveChangesAsync();

            return post;
        }
    }
}