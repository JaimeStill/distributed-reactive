namespace Topics.Data.Extensions;

using Topics.Core;
using Topics.Core.ApiQuery;
using Topics.Core.Extensions;
using Topics.Data.Entities;
using Topics.Identity;

using Microsoft.EntityFrameworkCore;

public static class IdentityExtensions
{
    static IQueryable<User> Search(this IQueryable<User> users, string search) =>
        users.Where(x =>
            x.EmailAddress.ToLower().Contains(search.ToLower())
            || x.DisplayName.ToLower().Contains(search.ToLower())
            || x.DistinguishedName.ToLower().Contains(search.ToLower())
            || x.FirstName.ToLower().Contains(search.ToLower())
            || x.LastName.ToLower().Contains(search.ToLower())
            || x.MiddleName.ToLower().Contains(search.ToLower())
            || x.SamAccountName.ToLower().Contains(search.ToLower())
            || x.UserPrincipalName.ToLower().Contains(search.ToLower())
            || x.VoiceTelephoneNumber.ToLower().Contains(search.ToLower())
        );

    public static async Task<string> GetUserImage(this AppDbContext db, int userId, string url)
    {
        try
        {
            var image = await db.GetUserImage(userId);

            if (image is not null)
                return image.Url;

            return url.GetDefaultUserImage();
        }
        catch
        {
            return url.GetDefaultUserImage();
        }
    }

    public static async Task<QueryResult<User>> QueryUsers(
        this AppDbContext db,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<User>.GenerateQuery(
        page, pageSize, search, sort,
        db.Users,
        Search
    );

    public static async Task<User> GetUser(this AppDbContext db, int id) =>
        await db.Users
            .FindAsync(id);

    public static async Task<User> GetUserByGuid(this AppDbContext db, Guid guid) =>
        await db.Users
            .FirstOrDefaultAsync(x => x.Guid == guid);

    public static async Task<int> GetUserIdByGuid(this AppDbContext db, Guid guid) =>
        await db.Users
            .Where(x => x.Guid == guid)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

    public static async Task<User> SyncUser(this AdUser adUser, AppDbContext db)
    {
        var user = await db.GetUserByGuid(adUser.Guid.Value);

        user = user == null
            ? await db.AddUser(adUser)
            : await db.SyncUser(adUser, user);

        return await db.GetUser(user.Id);
    }

    public static async Task<User> AddUser(this AppDbContext db, AdUser adUser)
    {
        User user = null;

        if (await adUser.Validate(db))
        {
            user = adUser.ToUser();
            user.DateJoined = DateTime.UtcNow;
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }

        return user;
    }

    public static async Task UpdateUser(this AppDbContext db, User user)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync();
    }

    public static async Task RemoveUser(this AppDbContext db, User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }

    public static async Task<bool> Validate(this AdUser user, AppDbContext db)
    {
        if (user is null)
            throw new AppException("The AD User provided was null");

        if (user.Guid is null)
            throw new AppException("The provided AD User does not have a GUID");

        var check = await db.GetUserByGuid(user.Guid.Value);

        if (check is not null)
            throw new AppException("The provided user already has an account");

        return true;
    }

    public static User ToUser(this AdUser adUser)
    {
        var user = new User();
        adUser.MergeUser(user);
        return user;
    }

    public static async Task<int> GetUserId(this IUserProvider provider, AppDbContext db) =>
        await db.GetUserIdByGuid(provider.CurrentUser.Guid.Value);

    static async Task<User> SyncUser(this AppDbContext db, AdUser adUser, User user)
    {
        adUser.MergeUser(user);
        await db.UpdateUser(user);

        return user;
    }

    static void MergeUser(this AdUser adUser, User user)
    {
        if (string.IsNullOrEmpty(user.DisplayName))
            user.DisplayName = adUser.DisplayName;

        user.DistinguishedName = adUser.DistinguishedName;
        user.EmailAddress = adUser.EmailAddress;
        user.FirstName = adUser.GivenName;
        user.Guid = adUser.Guid.Value;
        user.LastName = adUser.Surname;
        user.MiddleName = adUser.MiddleName;
        user.SamAccountName = adUser.SamAccountName;
        user.UserPrincipalName = adUser.UserPrincipalName;
        user.VoiceTelephoneNumber = adUser.VoiceTelephoneNumber;
        user.SocketName = $@"{adUser.GetDomainPrefix()}\{adUser.SamAccountName}";
        user.DateJoined = DateTime.UtcNow;
    }
}