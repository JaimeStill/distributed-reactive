namespace Topics.Data.Extensions;

using Topics.Data.Extensions.Seed;

using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task Initialize(this AppDbContext db)
    {
        Console.WriteLine("Initializing database");
        await db.Database.MigrateAsync();
        Console.WriteLine("Database initialized");

        var users = await db.SeedUsers();
        var topic = await db.SeedTopic(users.lgraham);
        var post = await db.SeedPost(topic, users.lgraham);
        var comments = await db.SeedComments(post, users);
        await db.SeedVotes(post, comments, users);
    }
}