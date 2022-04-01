namespace Topics.Data.Extensions.Seed;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class CommentSeed
{
    public static async Task<List<Comment>> SeedComments(this AppDbContext db, Post post, (User lgraham, User ehowell) users)
    {
        if (await db.Comments.AnyAsync(x => x.PostId == post.Id))
        {
            Console.WriteLine("Retrieving comments...");

            var comments = await db.Comments
                .Where(x => x.PostId == post.Id)
                .ToListAsync();

            return comments;
        }
        else
        {
            Console.WriteLine("Seeding comments...");

            var comments = new List<Comment>
            {
                new Comment
                {
                    AuthorId = users.ehowell.Id,
                    PostId = post.Id,
                    Text = "First comment",
                    DateCreated = new DateTime(2020, 01, 25),
                    Children = new List<Comment>
                    {
                        new Comment
                        {
                            AuthorId = users.lgraham.Id,
                            PostId = post.Id,
                            Text = "Sub-comment",
                            DateCreated = new DateTime(2020, 01, 26, 9, 45, 18),
                            Children = new List<Comment>
                            {
                                new Comment
                                {
                                    AuthorId = users.ehowell.Id,
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
                    AuthorId = users.lgraham.Id,
                    PostId = post.Id,
                    Text = "All alone",
                    DateCreated = new DateTime(2020, 01, 27)
                }
            };

            await db.Comments.AddRangeAsync(comments);
            await db.SaveChangesAsync();

            return comments;
        }
    }
}