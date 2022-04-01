namespace Topics.Data.Extensions;

using Topics.Core;
using Topics.Core.ApiQuery;
using Topics.Core.Extensions;
using Topics.Data.Entities;
using Topics.Identity;

using Microsoft.EntityFrameworkCore;

public static class VoteExtensions
{
    #region CommentVote

    static IQueryable<CommentVote> SetIncludes(this DbSet<CommentVote> votes) =>
        votes.Include(x => x.Comment)
             .ThenInclude(x => x.Author);

    static IQueryable<Comment> Search(this IQueryable<Comment> comments, string search) =>
        comments.Where(x =>
            x.Text.ToLower().Contains(search.ToLower())
            || x.Author.DisplayName.ToLower().Contains(search.ToLower())
            || x.Author.EmailAddress.ToLower().Contains(search.ToLower())
        );

    public static async Task<QueryResult<Comment>> QueryUpvotedComments(
        this AppDbContext db,
        int voterId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Comment>.GenerateQuery(
        page, pageSize, search, sort,
        db.CommentVotes
            .SetIncludes()
            .Where(x =>
                x.VoterId == voterId
                && x.Up
            )
            .Select(x => x.Comment),
        Search
    );

    public static async Task<QueryResult<Comment>> QueryDownvotedComments(
        this AppDbContext db,
        int voterId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Comment>.GenerateQuery(
        page, pageSize, search, sort,
        db.CommentVotes
            .SetIncludes()
            .Where(x =>
                x.VoterId == voterId
                && !x.Up
            )
            .Select(x => x.Comment),
        Search
    );

    public static async Task<List<CommentVote>> GetCommentUpvotes(this AppDbContext db, int commentId) =>
        await db.CommentVotes
            .Where(x =>
                x.CommentId == commentId
                && x.Up
            )
            .ToListAsync();

    public static async Task<List<CommentVote>> GetCommentDownvotes(this AppDbContext db, int commentId) =>
        await db.CommentVotes
            .Where(x =>
                x.CommentId == commentId
                && !x.Up
            )
            .ToListAsync();

    static async Task<CommentVote> Verify(this CommentVote vote, AppDbContext db, int voterId) =>
        await db.CommentVotes
            .Where(x =>
                x.VoterId == voterId
                && x.CommentId == vote.CommentId
            )
            .FirstOrDefaultAsync();
    #endregion

    #region PostVote

    static IQueryable<PostVote> SetIncludes(this DbSet<PostVote> votes) =>
        votes.Include(x => x.Post)
                .ThenInclude( x => x.Author)
             .Include(x => x.Post)
                .ThenInclude(x => x.Topic)
             .AsSplitQuery();

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

    public static async Task<QueryResult<Post>> QueryUpvotedPosts(
        this AppDbContext db,
        int voterId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Post>.GenerateQuery(
        page, pageSize, search, sort,
        db.PostVotes
            .SetIncludes()
            .Where(x =>
                x.VoterId == voterId
                && x.Up
            )
            .Select(x => x.Post),
        Search
    );

    public static async Task<QueryResult<Post>> QueryDownvotedPosts(
        this AppDbContext db,
        int voterId,
        string page,
        string pageSize,
        string search,
        string sort
    ) => await QueryContainer<Post>.GenerateQuery(
        page, pageSize, search, sort,
        db.PostVotes
            .SetIncludes()
            .Where(x =>
                x.VoterId == voterId
                && !x.Up
            )
            .Select(x => x.Post),
        Search
    );

    public static async Task<List<PostVote>> GetPostUpvotes(this AppDbContext db, int postId) =>
        await db.PostVotes
            .Where(x =>
                x.PostId == postId
                && x.Up
            )
            .ToListAsync();

    public static async Task<List<PostVote>> GetPostDownvotes(this AppDbContext db, int postId) =>
        await db.PostVotes
            .Where(x =>
                x.PostId == postId
                && !x.Up
            )
            .ToListAsync();

    static async Task<PostVote> Verify(this PostVote vote, AppDbContext db, int voterId) =>
        await db.PostVotes
            .Where(x =>
                x.VoterId == voterId
                && x.PostId == vote.PostId
            )
            .FirstOrDefaultAsync();

    #endregion

    #region Vote

    public static async Task<T> CastVote<T>(this T vote, AppDbContext db, IUserProvider provider) where T : Vote
    {
        var voterId = await provider.GetUserId(db);
        var check = await vote.Check(db, voterId);

        if (check is not null)
        {
            await check.Remove(db);

            if (check.Up != vote.Up)
                return await vote.Add(db, voterId);
            else
                return check;
        }
        else
            return await vote.Add(db, voterId);
    }

    public static async Task Remove<T>(this T vote, AppDbContext db) where T : Vote
    {
        db.Remove(vote);
        await db.SaveChangesAsync();
    }

    static async Task<T> Check<T>(this T vote, AppDbContext db, int voterId) where T : Vote
    {
        return vote switch
        {
            CommentVote cv => await cv.Verify(db, voterId) as T,
            PostVote pv => await pv.Verify(db, voterId) as T,
            _ => null,
        };
    }

    static async Task<T> Add<T>(this T vote, AppDbContext db, int voterId) where T : Vote
    {
        if (vote.Validate())
        {
            vote.VoterId = voterId;
            await db.AddAsync(vote);
            await db.SaveChangesAsync();

            return vote;
        }

        return null;
    }

    static bool Validate<T>(this T vote)
    {
        return vote switch
        {
            CommentVote cv => cv.Validate(),
            PostVote pv => pv.Validate(),
            _ => false,
        };
    }

    #endregion
}