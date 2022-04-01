namespace Topics.Data.Extensions;

using Topics.Core;
using Topics.Core.ApiQuery;
using Topics.Core.Extensions;
using Topics.Data.Entities;
using Topics.Identity;

using Microsoft.EntityFrameworkCore;

public static class PostExtensions
{
    #region Post

    static IQueryable<Post> SetIncludes(this DbSet<Post> posts) =>
        posts.Include(x => x.Author)
             .Include(x => x.Topic);

    static IQueryable<Post> Search(this IQueryable<Post> posts, string search) =>
        posts.Where(x =>
            x.Title.ToLower().Contains(search.ToLower())
            || x.Description.ToLower().Contains(search.ToLower())
            || x.Text.ToLower().Contains(search.ToLower())
            || x.Url.ToLower().Contains(search.ToLower())
            || x.Author.DisplayName.ToLower().Contains(search.ToLower())
            || x.Author.EmailAddress.ToLower().Contains(search.ToLower())
            || x.Topic.Name.ToLower().Contains(search.ToLower())
        );

    public static async Task<QueryResult<Post>> QueryPosts(
        this AppDbContext db,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Post>.GenerateQuery(
        page, pageSize, search, sort,
        db.Posts
            .SetIncludes()
            .Where(x => x.IsPublished),
        Search
    );

    public static async Task<QueryResult<Post>> QueryTopicPosts(
        this AppDbContext db,
        int topicId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Post>.GenerateQuery(
        page, pageSize, search, sort,
        db.Posts
            .SetIncludes()
            .Where(x =>
                x.TopicId == topicId
                && x.IsPublished
            ),
        Search
    );

    public static async Task<QueryResult<Post>> QueryPublishedAuthorPosts(
        this AppDbContext db,
        int authorId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Post>.GenerateQuery(
        page, pageSize, search, sort,
        db.Posts
            .SetIncludes()
            .Where(x =>
                x.AuthorId == authorId
                && x.IsPublished
            ),
        Search
    );

    public static async Task<QueryResult<Post>> QueryUnpublishedAuthorPosts(
        this AppDbContext db,
        int authorId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Post>.GenerateQuery(
        page, pageSize, search, sort,
        db.Posts
            .SetIncludes()
            .Where(x =>
                x.AuthorId == authorId
                && !x.IsPublished
            ),
        Search
    );

    public static async Task<Post> GetPost(this AppDbContext db, string url) =>
        await db.Posts
            .FirstOrDefaultAsync(x => x.Url.ToLower() == url.ToLower());

    public static async Task<Post> Save(this Post post, AppDbContext db, IUserProvider provider)
    {
        if (post.Validate())
        {
            post.Url = await post.GenerateUrl(db);

            if (post.Id > 0)
                await post.Update(db);
            else
            {
                var userId = await provider.GetUserId(db);
                await post.Add(db, userId);
            }

            return post;
        }

        return null;
    }

    public static async Task TogglePublished(this Post post, AppDbContext db)
    {
        post.IsPublished = !post.IsPublished;

        if (post.IsPublished)
            post.DatePublished = DateTime.UtcNow;

        db.Posts.Update(post);
        await db.SaveChangesAsync();
    }

    public static async Task Remove(this Post post, AppDbContext db)
    {
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
    }

    public static async Task<bool> IsPostAuthority(this AppDbContext db, Post post, int userId)
    {
        if (await db.IsTopicAuthority(post.TopicId, userId))
            return true;

        return post.AuthorId == userId;
    }

    static async Task Add(this Post post, AppDbContext db, int userId)
    {
        post.DateCreated = DateTime.UtcNow;
        post.AuthorId = userId;
        await db.Posts.AddAsync(post);
        await db.SaveChangesAsync();
    }

    static async Task Update(this Post post, AppDbContext db)
    {
        db.Posts.Update(post);
        await db.SaveChangesAsync();
    }

    static async Task<string> GenerateUrl(this Post post, AppDbContext db)
    {
        var increment = 0;
        var topic = await db.Topics.FindAsync(post.TopicId);
        var url = $"{topic.Url}-{post.Title.UrlEncode()}";

        while (await db.PostUrlExists(url))
            url = $"{topic.Url}-{post.Title.UrlEncode()}-{++increment}";

        return url;
    }

    static async Task<bool> PostUrlExists(this AppDbContext db, string url) =>
        await db.Posts
            .AnyAsync(x => x.Url == url);

    static bool Validate(this Post post)
    {
        if (post.TopicId < 1)
            throw new AppException("A Post must be assigned a Topic");

        if (string.IsNullOrEmpty(post.Title))
            throw new AppException("A Post must have a Title");

        return true;
    }

    #endregion

    #region PostLink

    public static async Task<List<PostLink>> GetPostLinks(this AppDbContext db, int postId) =>
        await db.PostLinks
            .Where(x => x.PostId == postId)
            .OrderBy(x => x.Text)
            .ToListAsync();

    public static async Task<PostLink> Save(this PostLink link, AppDbContext db)
    {
        if (link.Validate())
        {
            if (string.IsNullOrEmpty(link.Text))
                link.Text = link.Url;

            if (link.Id > 0)
                await link.Add(db);
            else
                await link.Update(db);

            return link;
        }

        return null;
    }

    public static async Task Remove(this PostLink link, AppDbContext db)
    {
        db.PostLinks.Remove(link);
        await db.SaveChangesAsync();
    }

    static async Task Add(this PostLink link, AppDbContext db)
    {
        await db.PostLinks.AddAsync(link);
        await db.SaveChangesAsync();
    }

    static async Task Update(this PostLink link, AppDbContext db)
    {
        db.PostLinks.Update(link);
        await db.SaveChangesAsync();
    }

    static bool Validate(this PostLink link)
    {
        if (link.PostId < 1)
            throw new AppException("A Post Link must be associated with a Post");

        if (string.IsNullOrEmpty(link.Url))
            throw new AppException("A Post Link must have a URL");

        return true;
    }

    #endregion
}