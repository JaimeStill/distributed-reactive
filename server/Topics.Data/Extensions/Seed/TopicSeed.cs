namespace Topics.Data.Extensions.Seed;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class TopicSeed
{
    public static async Task SeedTopics(this AppDbContext db, List<User> users)
    {
        if (!await db.Topics.AnyAsync())
        {
            Console.WriteLine("Seeding topics...");

            var lgraham = users.FirstOrDefault(x => x.SamAccountName == "lgraham");
            var ehowell = users.FirstOrDefault(x => x.SamAccountName == "ehowell");

            var post = new Post
            {
                AuthorId = lgraham.Id,
                Title = "Detailed Post",
                Description = "A description",
                DateCreated = new DateTime(2020, 01, 24),
                DatePublished = new DateTime(2020, 01, 25),
                IsPublished = true,
                Text = "This is a test post seeded by the database.",
                Url = "details-detailedpost"
            };

            var topic = new Topic
            {
                OwnerId = lgraham.Id,
                DateCreated = new DateTime(2020, 01, 20),
                Name = "Details",
                Url = "details",
                Posts = new List<Post>
                {
                    post
                }
            };

            await db.Topics.AddAsync(topic);
            await db.SaveChangesAsync();

            var comments = new List<Comment>
            {
                new Comment
                {
                    AuthorId = ehowell.Id,
                    PostId = post.Id,
                    Text = "First comment",
                    DateCreated = new DateTime(2020, 01, 25),
                    Children = new List<Comment>
                    {
                        new Comment
                        {
                            AuthorId = lgraham.Id,
                            PostId = post.Id,
                            Text = "Sub-comment",
                            DateCreated = new DateTime(2020, 01, 26, 9, 45, 18),
                            Children = new List<Comment>
                            {
                                new Comment
                                {
                                    AuthorId = ehowell.Id,
                                    PostId = post.Id,
                                    Text = "This is getting deep.",
                                    DateCreated = new DateTime(2020, 01, 26, 10, 24, 40)
                                }
                            }
                        }
                    }
                },
                new Comment
                {
                    AuthorId = lgraham.Id,
                    PostId = post.Id,
                    Text = "All alone",
                    DateCreated = new DateTime(2020, 01, 27)
                }
            };

            await db.Comments.AddRangeAsync(comments);
            await db.SaveChangesAsync();
        }
    }
}