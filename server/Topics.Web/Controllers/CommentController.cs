namespace Topics.Web.Controllers;

using Topics.Core.ApiQuery;
using Topics.Data;
using Topics.Data.Entities;
using Topics.Data.Extensions;
using Topics.Identity;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class CommentController : Controller
{
    private readonly AppDbContext db;
    private readonly IUserProvider provider;

    public CommentController(
        AppDbContext db,
        IUserProvider provider
    )
    {
        this.db = db;
        this.provider = provider;
    }

    [HttpGet("[action]/{postId}")]
    [ProducesResponseType(typeof(QueryResult<Comment>), 200)]
    public async Task<IActionResult> QueryComments(
        [FromRoute]int postId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryComments(
        postId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{authorId}")]
    [ProducesResponseType(typeof(QueryResult<Comment>), 200)]
    public async Task<IActionResult> QueryAuthoredComments(
        [FromRoute]int authorId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryAuthoredComments(
        authorId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{commentId}")]
    public async Task<List<Comment>> GetSubComments([FromRoute]int commentId) =>
        await db.GetSubComments(commentId);

    [HttpGet("[action]/{commentId}")]
    public async Task<Comment> GetComment([FromRoute]int commentId) =>
        await db.GetComment(commentId);

    [HttpPost("[action]")]
    public async Task<Comment> SaveComment([FromBody]Comment comment) =>
        await comment.Save(db, provider);

    [HttpPost("[action]")]
    public async Task RemoveComment([FromBody]Comment comment) =>
        await comment.Remove(db);
}