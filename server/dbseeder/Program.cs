[assembly:System.Runtime.Versioning.SupportedOSPlatform("windows")]
namespace dbseeder;

using Topics.Core.Extensions;
using Topics.Data;
using Topics.Data.Extensions;

using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        string connection = args.Length < 1
            ? string.Empty
            : args[0];

        while (string.IsNullOrEmpty(connection))
        {
            Console.WriteLine("Please provide a connection string:");
            connection = Console.ReadLine();
            Console.WriteLine();
        }

        try
        {
            Console.WriteLine($"Connection: {connection}");

            var builder = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connection);

            using var db = new AppDbContext(builder.Options);
            await db.Initialize();
            Console.WriteLine("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while seeding the database:");
            Console.WriteLine(ex.GetExceptionChain());
        }
    }
}