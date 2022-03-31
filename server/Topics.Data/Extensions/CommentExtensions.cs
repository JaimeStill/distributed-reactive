namespace Topics.Data.Extensions;

using Topics.Core;
using Topics.Core.ApiQuery;
using Topics.Core.Extensions;
using Topics.Data.Entities;
using Topics.Identity;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public static class CommentExtensions
{
    static IQueryable<Comment> SetIncludes(this DbSet<Comment> comments) =>
        comments.Include(x => x.Author);

    static Expression<Func<Comment, Comment>> SearchComments(string search)
    {
        Expression<Func<Comment, Comment>> result = comment => new Comment
        {
            Id = comment.Id,
            AuthorId = comment.AuthorId,
            Author = comment.Author,
            DateCreated = comment.DateCreated,
            ParentId = comment.ParentId,
            PostId = comment.PostId,
            Text = comment.Text,
            Children = comment.Children.Any() ?
                comment.Children.Where(x =>
                    x.Text.ToLower().Contains(search.ToLower())
                    || x.Author.DisplayName.ToLower().Contains(search.ToLower())
                )
                : new List<Comment>()
        };

        return result;
    }

    static IQueryable<Comment> Search(this IQueryable<Comment> comments, string search) =>
        comments.Where(x =>
            x.Text.ToLower().Contains(search.ToLower())
            || x.Author.DisplayName.ToLower().Contains(search.ToLower())
            || x.Children.AsQueryable().Select(SearchComments(search)).Any()
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
        db.Comments
          .SetIncludes()
          .Where(x =>
            x.PostId == postId
            && !x.ParentId.HasValue
          ),
          Search
    );
}