namespace Topics.Web.Controllers;

using Topics.Core.ApiQuery;
using Topics.Data;
using Topics.Data.Entities;
using Topics.Data.Extensions;
using Topics.Identity;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class PostController : Controller
{
    private readonly AppDbContext db;
    private readonly IUserProvider provider;

    public PostController(
        AppDbContext db,
        IUserProvider provider
    )
    {
        this.db = db;
        this.provider = provider;
    }

    #region Post

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(QueryResult<Post>), 200)]
    public async Task<IActionResult> QueryPosts(
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryPosts(
        page, pageSize, search, sort
    ));

    [HttpGet("[action]/{topicId}")]
    [ProducesResponseType(typeof(QueryResult<Post>), 200)]
    public async Task<IActionResult> QueryTopicPosts(
        [FromRoute]int topicId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryTopicPosts(
        topicId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{authorId}")]
    [ProducesResponseType(typeof(QueryResult<Post>), 200)]
    public async Task<IActionResult> QueryPublishedAuthorPosts(
        [FromRoute]int authorId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryPublishedAuthorPosts(
        authorId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{authorId}")]
    [ProducesResponseType(typeof(QueryResult<Post>), 200)]
    public async Task<IActionResult> QueryUnpublishedAuthorPosts(
        [FromRoute]int authorId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryUnpublishedAuthorPosts(
        authorId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{url}")]
    public async Task<Post> GetPost([FromRoute]string url) =>
        await db.GetPost(url);

    [HttpPost("[action]")]
    public async Task<Post> SavePost([FromBody]Post post) =>
        await post.Save(db, provider);

    [HttpPost("[action]")]
    public async Task TogglePostPublished([FromBody]Post post) =>
        await post.TogglePublished(db);

    [HttpPost("[action]")]
    public async Task RemovePost([FromBody]Post post) =>
        await post.Remove(db);

    #endregion

    #region PostLink

    [HttpGet("[action]/{postId}")]
    public async Task<List<PostLink>> GetPostLinks([FromRoute]int postId) =>
        await db.GetPostLinks(postId);

    [HttpPost("[action]")]
    public async Task<PostLink> SavePostLink([FromBody]PostLink link) =>
        await link.Save(db);

    [HttpPost("[action]")]
    public async Task RemovePostLink([FromBody]PostLink link) =>
        await link.Remove(db);

    #endregion
}