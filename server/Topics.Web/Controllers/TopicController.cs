namespace Topics.Web.Controllers;

using Topics.Core.ApiQuery;
using Topics.Core.Upload;
using Topics.Data;
using Topics.Data.Entities;
using Topics.Data.Extensions;
using Topics.Identity;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class TopicController : Controller
{
    private readonly AppDbContext db;
    private readonly IUserProvider provider;
    private readonly UploadConfig config;

    public TopicController(
        AppDbContext db,
        IUserProvider provider,
        UploadConfig config
    )
    {
        this.db = db;
        this.provider = provider;
        this.config = config;
    }

    #region Topic

    [HttpGet("[action]/{topicId}")]
    public async Task<string> GetTopicImage([FromRoute]int topicId) =>
        await db.GetTopicImage(topicId, config.UrlBasePath);

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(QueryResult<Topic>), 200)]
    public async Task<IActionResult> QueryTopics(
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryTopics(
        page, pageSize, search, sort
    ));

    [HttpGet("[action]/{ownerId}")]
    [ProducesResponseType(typeof(QueryResult<Topic>), 200)]
    public async Task<IActionResult> QueryOwnerTopics(
        [FromRoute]int ownerId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryOwnerTopics(
        ownerId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{url}")]
    public async Task<Topic> GetTopic([FromRoute]string url) =>
        await db.GetTopic(url);

    [HttpPost("[action]")]
    public async Task<bool> VerifyTopic([FromBody]Topic topic) =>
        await topic.Verify(db);

    [HttpPost("[action]")]
    public async Task<int> SaveTopic([FromBody]Topic topic) =>
        await topic.Save(db, provider);

    [HttpPost("[action]")]
    public async Task RemoveTopic([FromBody]Topic topic) =>
        await topic.Remove(db);

    #endregion

    #region TopicUser

    [HttpGet("[action]/{topicId}")]
    [ProducesResponseType(typeof(QueryResult<User>), 200)]
    public async Task<IActionResult> QueryTopicUsers(
        [FromRoute]int topicId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryTopicUsers(
        topicId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{userId}")]
    [ProducesResponseType(typeof(QueryResult<Topic>), 200)]
    public async Task<IActionResult> QueryAvailableTopics(
        [FromRoute]int userId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryAvailableTopics(
        userId, page, pageSize, search, sort
    ));

    [HttpGet("[action]/{userId}")]
    [ProducesResponseType(typeof(QueryResult<TopicUser>), 200)]
    public async Task<IActionResult> QueryUserTopics(
        [FromRoute]int userId,
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryUserTopics(
        userId, page, pageSize, search, sort
    ));

    [HttpPost("[action]")]
    public async Task<TopicUser> SaveTopicUser([FromBody]TopicUser user) =>
        await user.Save(db);

    [HttpPost("[action]")]
    public async Task ToggleAdmin([FromBody]TopicUser user) =>
        await user.ToggleAdmin(db);

    [HttpPost("[action]")]
    public async Task ToggleBanned([FromBody]TopicUser user) =>
        await user.ToggleBanned(db);

    [HttpPost("[action]")]
    public async Task RemoveTopicUser([FromBody]TopicUser user) =>
        await user.Remove(db);

    #endregion
}