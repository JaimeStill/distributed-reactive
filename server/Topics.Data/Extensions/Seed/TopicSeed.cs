namespace Topics.Data.Extensions.Seed;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class TopicSeed
{
    public static async Task<Topic> SeedTopic(this AppDbContext db, User user)
    {
        if (await db.Topics.AnyAsync(x => x.Name == "Details"))
        {
            Console.WriteLine("Retreiving topic...");

            return await db.Topics
                .FirstOrDefaultAsync(x => x.Name == "Details");
        }
        else
        {
            Console.WriteLine("Seeding topic...");

            var topic = new Topic
            {
                OwnerId = user.Id,
                DateCreated = new DateTime(2020, 01, 20),
                Name = "Details",
                Url = "details",
            };

            await db.Topics.AddAsync(topic);
            await db.SaveChangesAsync();

            return topic;
        }
    }
}