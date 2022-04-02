namespace Topics.Data.Extensions;

using Topics.Core;
using Topics.Core.ApiQuery;
using Topics.Core.Extensions;
using Topics.Data.Entities;
using Topics.Identity;

using Microsoft.EntityFrameworkCore;

public static class TopicExtensions
{
    #region Topic

    static IQueryable<Topic> SetIncludes(this DbSet<Topic> topics) =>
        topics.Include(x => x.Owner);

    static IQueryable<Topic> Search(this IQueryable<Topic> topics, string search) =>
        topics.Where(x =>
            x.Name.ToLower().Contains(search.ToLower())
            || x.Url.ToLower().Contains(search.ToLower())
            || x.Owner.DisplayName.ToLower().Contains(search.ToLower())
            || x.Owner.EmailAddress.ToLower().Contains(search.ToLower())
        );

    public static async Task<string> GetTopicImage(this AppDbContext db, int topicId, string url)
    {
        try
        {
            var image = await db.GetTopicImage(topicId);

            if (image is not null)
                return image.Url;

            return url.GetDefaultTopicImage();
        }
        catch
        {
            return url.GetDefaultTopicImage();
        }
    }

    public static async Task<QueryResult<Topic>> QueryTopics(
        this AppDbContext db,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Topic>.GenerateQuery(
        page, pageSize, search, sort,
        db.Topics
            .SetIncludes(),
        Search
    );

    public static async Task<QueryResult<Topic>> QueryOwnerTopics(
        this AppDbContext db,
        int ownerId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Topic>.GenerateQuery(
        page, pageSize, search, sort,
        db.Topics
            .SetIncludes()
            .Where(x => x.OwnerId == ownerId),
        Search
    );

    public static async Task<Topic> GetTopic(this AppDbContext db, string url) =>
        await db.Topics
            .FirstOrDefaultAsync(x => x.Url.ToLower() == url.ToLower());

    public static async Task<bool> Verify(this Topic topic, AppDbContext db) =>
        !await db.Topics
            .AnyAsync(x =>
                x.Id != topic.Id
                && x.Name.ToLower() == topic.Name.ToLower()
            );

    public static async Task<int> Save(this Topic topic, AppDbContext db, IUserProvider provider)
    {
        if (await topic.Validate(db))
        {
            topic.Url = topic.Name.UrlEncode();

            if (topic.Id > 0)
                await topic.Update(db);
            else
            {
                var userId = await provider.GetUserId(db);
                await topic.Add(db, userId);
            }

            return topic.Id;
        }

        return 0;
    }

    public static async Task Remove(this Topic topic, AppDbContext db)
    {
        db.Topics.Remove(topic);
        await db.SaveChangesAsync();
    }

    public static async Task<bool> IsTopicOwner(this AppDbContext db, int topicId, int userId) =>
        await db.Topics
            .AnyAsync(x =>
                x.Id == topicId
                && x.OwnerId == userId
            );

    static async Task Add(this Topic topic, AppDbContext db, int userId)
    {
        topic.DateCreated = DateTime.UtcNow;
        topic.OwnerId = userId;
        await db.Topics.AddAsync(topic);
        await db.SaveChangesAsync();
    }

    static async Task Update(this Topic topic, AppDbContext db)
    {
        db.Topics.Update(topic);
        await db.SaveChangesAsync();
    }

    static async Task<bool> Validate(this Topic topic, AppDbContext db)
    {
        if (string.IsNullOrEmpty(topic.Name))
            throw new AppException("Topic must have a name");

        var check = await topic.Verify(db);

        if (check is false)
            throw new AppException($"{topic.Name} is already a Topic");

        return true;
    }

    #endregion

    #region TopicUser

    static IQueryable<TopicUser> SetIncludes(this IQueryable<TopicUser> users) =>
        users.Include(x => x.Topic);

    static IQueryable<TopicUser> Search(this IQueryable<TopicUser> users, string search) =>
        users.Where(x =>
            x.Topic.Name.ToLower().Contains(search.ToLower())
        );

    static IQueryable<User> Search(this IQueryable<User> users, string search) =>
        users.Where(x =>
            x.DisplayName.ToLower().Contains(search.ToLower())
            || x.EmailAddress.ToLower().Contains(search.ToLower())
        );

    public static async Task<QueryResult<User>> QueryTopicUsers(
        this AppDbContext db,
        int topicId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<User>.GenerateQuery(
        page, pageSize, search, sort,
        db.TopicUsers
            .Where(x => x.TopicId == topicId)
            .Select(x => x.User),
        Search
    );

    public static async Task<QueryResult<Topic>> QueryAvailableTopics(
        this AppDbContext db,
        int userId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Topic>.GenerateQuery(
        page, pageSize, search, sort,
        db.Topics
            .Where(x =>
                x.OwnerId != userId
                && !x.Members.Any(y => y.UserId == userId)
            ),
        Search
    );

    public static async Task<QueryResult<TopicUser>> QueryUserTopics(
        this AppDbContext db,
        int userId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<TopicUser>.GenerateQuery(
        page, pageSize, search, sort,
        db.TopicUsers
            .SetIncludes()
            .Where(x => x.UserId == userId),
        Search
    );

    public static async Task<TopicUser> Save(this TopicUser user, AppDbContext db)
    {
        if (await user.Validate(db))
        {
            await db.TopicUsers.AddAsync(user);
            await db.SaveChangesAsync();

            return user;
        }

        return null;
    }

    public static async Task ToggleAdmin(this TopicUser user, AppDbContext db)
    {
        user.IsAdmin = !user.IsAdmin;
        db.TopicUsers.Update(user);
        await db.SaveChangesAsync();
    }

    public static async Task ToggleBanned(this TopicUser user, AppDbContext db)
    {
        user.IsBanned = !user.IsBanned;
        db.TopicUsers.Update(user);
        await db.SaveChangesAsync();
    }

    public static async Task Remove(this TopicUser user, AppDbContext db)
    {
        db.TopicUsers.Remove(user);
        await db.SaveChangesAsync();
    }

    public static async Task<bool> IsTopicAuthority(this AppDbContext db, int topicId, int userId)
    {
        if (await db.IsTopicOwner(topicId, userId))
            return true;

        return await db.TopicUsers
            .AnyAsync(x =>
                x.TopicId == topicId
                && x.IsAdmin
                && !x.IsBanned
                && x.UserId == userId
            );
    }

    static async Task<bool> Validate(this TopicUser user, AppDbContext db)
    {
        if (user.TopicId < 1)
            throw new AppException("A Topic membership must specify a Topic");

        if (user.UserId < 1)
            throw new AppException("A Topic membership must specify a User");

        var check = await db.TopicUsers
            .AnyAsync(x =>
                x.UserId == user.UserId
                && x.TopicId == user.TopicId
            );

        if (check)
            throw new AppException("The specified User is already a member of the provided Topic");

        return true;
    }

    #endregion
}