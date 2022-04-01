namespace Topics.Web.Controllers;

using Topics.Core.ApiQuery;
using Topics.Data;
using Topics.Data.Entities;
using Topics.Data.Extensions;
using Topics.Identity;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class IdentityController : Controller
{
    private readonly AppDbContext db;
    private readonly IUserProvider provider;

    public IdentityController(AppDbContext db, IUserProvider provider)
    {
        this.db = db;
        this.provider = provider;
    }

    #region AdUser

    [HttpGet("[action]")]
    public async Task<List<AdUser>> GetDomainUsers() =>
        await provider.GetDomainUsers();

    [HttpGet("[action]/{search}")]
    public async Task<List<AdUser>> SearchDomainUsers([FromRoute]string search) =>
        await provider.SearchDomainUsers(search);

    [HttpGet("[action]")]
    public AdUser GetCurrentUser() => provider.CurrentUser;

    #endregion

    #region User

    [HttpGet("[action]")]
    [ProducesResponseType(typeof(QueryResult<User>), 200)]
    public async Task<IActionResult> QueryUsers(
        [FromQuery]string page,
        [FromQuery]string pageSize,
        [FromQuery]string search,
        [FromQuery]string sort
    ) => Ok(await db.QueryUsers(page, pageSize, search, sort));

    [HttpGet("[action]/{id}")]
    public async Task<User> GetUser([FromRoute]int id) => await db.GetUser(id);

    [HttpGet("[action]")]
    public async Task<int> GetUserIdByGuid() => await provider.GetUserId(db);

    [HttpGet("[action]")]
    public async Task<User> SyncUser() => await provider.CurrentUser.SyncUser(db);

    [HttpPost("[action]")]
    public async Task AddUser([FromBody]AdUser adUser) => await db.AddUser(adUser);

    [HttpPost("[action]")]
    public async Task UpdateUser([FromBody]User user) => await db.UpdateUser(user);

    [HttpPost("[action]")]
    public async Task RemoveUser([FromBody]User user) => await db.RemoveUser(user);

    #endregion

}