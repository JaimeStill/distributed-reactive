namespace Topics.Data.Extensions;

using Topics.Core;
using Topics.Core.ApiQuery;
using Topics.Data.Entities;
using Topics.Identity;

using Microsoft.EntityFrameworkCore;

public static class CommentExtensions
{
    static IQueryable<Comment> SetIncludes(this DbSet<Comment> comments) =>
        comments.Include(x => x.Author);

    static IQueryable<Comment> Search(this IQueryable<Comment> comments, string search) =>
        comments.Where(x =>
            x.Text.ToLower().Contains(search.ToLower())
            || x.Author.DisplayName.ToLower().Contains(search.ToLower())
            || x.Author.EmailAddress.ToLower().Contains(search.ToLower())
        );

    public static async Task<QueryResult<Comment>> QueryComments(
        this AppDbContext db,
        int postId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Comment>.GenerateQuery(
        page, pageSize, search, sort,
        string.IsNullOrEmpty(search)
            ? db.Comments
                .SetIncludes()
                .Where(x =>
                    x.PostId == postId
                    && !x.ParentId.HasValue
                )
            : db.Comments
                .SetIncludes()
                .Where(x => x.PostId == postId),
          Search
    );

    public static async Task<QueryResult<Comment>> QueryAuthoredComments(
        this AppDbContext db,
        int authorId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Comment>.GenerateQuery(
        page, pageSize, search, sort,
        db.Comments
            .SetIncludes()
            .Where(x => x.AuthorId == authorId),
        Search
    );

    public static async Task<List<Comment>> GetSubComments(this AppDbContext db, int commentId) =>
        await db.Comments
            .SetIncludes()
            .Where(x => x.ParentId == commentId)
            .OrderBy(x => x.DateCreated)
            .ToListAsync();

    public static async Task<Comment> GetComment(this AppDbContext db, int commentId) =>
        await db.Comments
            .SetIncludes()
            .FirstOrDefaultAsync(x => x.Id == commentId);

    public static async Task<Comment> Save(this Comment comment, AppDbContext db, IUserProvider provider)
    {
        if (comment.Validate())
        {
            if (comment.Id > 0)
                await comment.Update(db);
            else
            {
                var userId = await provider.GetUserId(db);
                await comment.Add(db, userId);
            }

            return comment;
        }

        return null;
    }

    public static async Task Remove(this Comment comment, AppDbContext db)
    {
        db.Comments.Remove(comment);
        await db.SaveChangesAsync();
    }

    public static async Task<bool> IsCommentAuthority(this AppDbContext db, Comment comment, int userId)
    {
        var post = await db.Posts.FindAsync(comment.PostId);

        if (await db.IsPostAuthority(post, userId))
            return true;

        return comment.AuthorId == userId;
    }

    static async Task Add(this Comment comment, AppDbContext db, int userId)
    {
        comment.DateCreated = DateTime.UtcNow;
        comment.AuthorId = userId;
        await db.Comments.AddAsync(comment);
        await db.SaveChangesAsync();
    }

    static async Task Update(this Comment comment, AppDbContext db)
    {
        db.Comments.Update(comment);
        await db.SaveChangesAsync();
    }

    static bool Validate(this Comment comment)
    {
        if (comment.PostId < 1)
            throw new AppException("A Comment must be associated with a Post");

        if (string.IsNullOrEmpty(comment.Text))
            throw new AppException("A Comment must have a value");

        return true;
    }
}