namespace Topics.Data.Extensions.Seed;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class VoteSeed
{
    public static async Task SeedVotes(this AppDbContext db, Post post, List<Comment> comments, (User lgraham, User ehowell) users)
    {
        var rand = new Random();

        if (!await db.PostVotes.AnyAsync(x => x.PostId == post.Id))
        {
            Console.WriteLine("Seeding post votes...");

            var pv = new PostVote
            {
                PostId = post.Id,
                Up = rand.NextBool(),
                VoterId = users.GetAltId(post.AuthorId)
            };

            await db.PostVotes.AddAsync(pv);
            await db.SaveChangesAsync();
        }

        foreach (var comment in comments)
        {
            if (!await db.CommentVotes.AnyAsync(x => x.CommentId == comment.Id))
            {
                Console.WriteLine($"Seeding comment vote {comment.Id}...");

                var cv = new CommentVote
                {
                    CommentId = comment.Id,
                    Up = rand.NextBool(),
                    VoterId = users.GetAltId(comment.AuthorId)
                };

                await db.CommentVotes.AddAsync(cv);
            }
        }

        await db.SaveChangesAsync();
    }

    static bool NextBool(this Random r) => r.NextDouble() >= 0.5;

    static int GetAltId(this (User lgraham, User ehowell) users, int userId) =>
        userId == users.lgraham.Id
            ? users.ehowell.Id
            : users.lgraham.Id;
}