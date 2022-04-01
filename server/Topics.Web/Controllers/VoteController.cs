namespace Topics.Web.Controllers;

using Topics.Core.ApiQuery;
using Topics.Data;
using Topics.Data.Entities;
using Topics.Data.Extensions;
using Topics.Identity;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class VoteController : Controller
{
    private readonly AppDbContext db;
    private readonly IUserProvider provider;

    public VoteController(
        AppDbContext db,
        IUserProvider provider
    )
    {
        this.db = db;
        this.provider = provider;
    }

    #region CommentVote

    [HttpGet("[action]/{voterId}")]
    [ProducesResponseType(typeof(QueryResult<Comment>), 200)]
    public async Task<IActionResult> QueryUpvotedComments(
        [FromRoute]int voterId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryUpvotedComments(
        voterId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{voterId}")]
    [ProducesResponseType(typeof(QueryResult<Comment>), 200)]
    public async Task<IActionResult> QueryDownvotedComments(
        [FromRoute]int voterId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryDownvotedComments(
        voterId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{commentId}")]
    public async Task<List<CommentVote>> GetCommentUpvotes([FromRoute]int commentId) =>
        await db.GetCommentUpvotes(commentId);

    [HttpGet("[action]/{commentId}")]
    public async Task<List<CommentVote>> GetCommentDownvotes([FromRoute]int commentId) =>
        await db.GetCommentDownvotes(commentId);

    [HttpPost("[action]")]
    public async Task<CommentVote> CastCommentVote([FromBody]CommentVote vote) =>
        await vote.CastVote(db, provider);

    [HttpPost("[action]")]
    public async Task RemoveCommentVote([FromBody]CommentVote vote) =>
        await vote.Remove(db);

    #endregion

    #region PostVote

    [HttpGet("[action]/{voterId}")]
    [ProducesResponseType(typeof(QueryResult<Post>), 200)]
    public async Task<IActionResult> QueryUpvotedPosts(
        [FromRoute]int voterId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryUpvotedPosts(
        voterId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{voterId}")]
    [ProducesResponseType(typeof(QueryResult<Post>), 200)]
    public async Task<IActionResult> QueryDownvotedPosts(
        [FromRoute]int voterId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryDownvotedPosts(
        voterId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{postId}")]
    public async Task<List<PostVote>> GetPostUpvotes([FromRoute]int postId) =>
        await db.GetPostUpvotes(postId);

    [HttpGet("[action]/{postId}")]
    public async Task<List<PostVote>> GetPostDownvotes([FromRoute]int postId) =>
        await db.GetPostDownvotes(postId);

    [HttpPost("[action]")]
    public async Task<PostVote> CastPostVote([FromBody]PostVote vote) =>
        await vote.CastVote(db, provider);

    [HttpPost("[action]")]
    public async Task RemovePostVote([FromBody]PostVote vote) =>
        await vote.Remove(db);

    #endregion
}