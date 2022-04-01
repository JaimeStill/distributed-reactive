namespace Topics.Data.Extensions;

using Topics.Core;
using Topics.Core.Extensions;
using Topics.Data.Entities;
using Topics.Identity;

using Microsoft.EntityFrameworkCore;

public static class VoteExtensions
{
    #region CommentVote

    public static async Task<int> GetCommentUpvotes(this AppDbContext db, int commentId) =>
        await db.CommentVotes
            .Where(x =>
                x.CommentId == commentId
                && x.Up
            )
            .CountAsync();

    public static async Task<int> GetCommentDownvotes(this AppDbContext db, int commentId) =>
        await db.CommentVotes
            .Where(x =>
                x.CommentId == commentId
                && !x.Up
            )
            .CountAsync();

    public static async Task<CommentVote> CastVote(this CommentVote vote, AppDbContext db, IUserProvider provider)
    {
        var voterId = await provider.GetUserId(db);
        var check = await vote.Verify(db, voterId);

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

    static async Task<CommentVote> Verify(this CommentVote vote, AppDbContext db, int voterId) =>
        await db.CommentVotes
            .Where(x =>
                x.VoterId == voterId
                && x.CommentId == vote.CommentId
            )
            .FirstOrDefaultAsync();

    #endregion

    #region PostVote

    static async Task<PostVote> Verify(this PostVote vote, AppDbContext db, int voterId) =>
        await db.PostVotes
            .Where(x =>
                x.VoterId == voterId
                && x.PostId == vote.PostId
            )
            .FirstOrDefaultAsync();

    #endregion

    #region Vote

    public static async Task Remove<T>(this T vote, AppDbContext db) where T : Vote
    {
        db.Remove<T>(vote);
        await db.SaveChangesAsync();
    }

    static async Task<T> Add<T>(this T vote, AppDbContext db, int voterId) where T : Vote
    {
        if (vote.Validate())
        {
            vote.VoterId = voterId;
            await db.AddAsync<T>(vote);
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