namespace Topics.Data.Extensions.Seed;

using Topics.Data.Entities;
using Topics.Identity.Mock;

using Microsoft.EntityFrameworkCore;

public static class UserSeed
{
    public static async Task<(User lgraham, User ehowell)> SeedUsers(this AppDbContext db)
    {
        if (await db.VerifyUsers())
        {
            Console.WriteLine("Retrieving users...");

            var lgraham = await db.Users
                .FirstOrDefaultAsync(x => x.SamAccountName == "lgraham");

            var ehowell = await db.Users
                .FirstOrDefaultAsync(x => x.SamAccountName == "ehowell");

            return (lgraham, ehowell);
        }
        else
        {
            Console.WriteLine("Seeding users...");

            var lgraham = MockProvider
                .AdUsers
                .Where(x => x.SamAccountName == "lgraham")
                .Select(x => x.ToUser())
                .FirstOrDefault();

            var ehowell = MockProvider
                .AdUsers
                .Where(x=> x.SamAccountName == "ehowell")
                .Select(x => x.ToUser())
                .FirstOrDefault();

            await db.Users.AddRangeAsync(lgraham, ehowell);
            await db.SaveChangesAsync();

            return (lgraham, ehowell);
        }
    }

    static async Task<bool> VerifyUsers(this AppDbContext db)
    {
        if (
            await db.Users.AnyAsync(x => x.SamAccountName == "lgraham")
            && await db.Users.AnyAsync(x => x.SamAccountName == "ehowell")
        )
        {
            return true;
        }
        else
            return false;
    }
}