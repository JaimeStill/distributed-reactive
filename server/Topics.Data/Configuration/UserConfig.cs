namespace Topics.Data.Configuration;

using Topics.Data.Entities;

using Microsoft.EntityFrameworkCore;

public static class UserConfig
{
    public static void ConfigureUser(this ModelBuilder builder)
    {
        builder
            .Entity<User>()
            .Property(x => x.DefaultPageSize)
            .HasDefaultValue(20);
    }
}