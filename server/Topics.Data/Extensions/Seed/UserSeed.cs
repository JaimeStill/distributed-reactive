namespace Topics.Data.Extensions.Seed;

using Topics.Data.Entities;
using Topics.Identity.Mock;

using Microsoft.EntityFrameworkCore;

public static class UserSeed
{
    public static async Task<List<User>> SeedUsers(this AppDbContext db)
    {
        if (await db.Users.AnyAsync())
        {
            Console.WriteLine("Retrieving users...");
            return await db.Users.ToListAsync();
        }
        else
        {
            Console.WriteLine("Seeding users...");

            var users = MockProvider.AdUsers
                .Select(x => x.ToUser())
                .ToList();

            await db.Users.AddRangeAsync(users);
            await db.SaveChangesAsync();

            return users;
        }
    }
}