namespace Topics.Web.Controllers;

using Topics.Core.Upload;
using Topics.Data;
using Topics.Data.Entities;
using Topics.Data.Extensions;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class UploadController : Controller
{
    private readonly AppDbContext db;
    private readonly UploadConfig config;

    public UploadController(AppDbContext db, UploadConfig config)
    {
        this.db = db;
        this.config = config;
    }

    #region Image

    [HttpGet("[action]")]
    public string GetDefaultTopicImage() =>
        config.UrlBasePath.GetDefaultTopicImage();

    [HttpGet("[action]")]
    public string GetDefaultUserImage() =>
        config.UrlBasePath.GetDefaultUserImage();

    #endregion

    #region PostUpload

    private readonly string postPath = "post-uploads/";

    [HttpGet("[action]/{postId}")]
    public async Task<List<PostUpload>> GetPostUploads([FromRoute]int postId) =>
        await db.GetPostUploads(postId);

    [HttpPost("[action]/{postId}")]
    public async Task CreatePostUploads([FromRoute]int postId) =>
        await db.CreatePostUploads(
            Request.Form.Files,
            $@"{config.DirectoryBasePath}{postPath}{postId}/".Replace('\\', '/'),
            $@"{config.UrlBasePath}{postPath}{postId}/".Replace('\\', '/'),
            postId
        );

    [HttpPost("[action]")]
    public async Task RemovePostUpload([FromBody]PostUpload upload) =>
        await upload.Remove(db);

    #endregion

    #region TopicImage

    private readonly string topicPath = "topic-images/";

    [HttpGet("[action]/{topicId}")]
    public async Task<TopicImage> GetTopicImage([FromRoute]int topicId) =>
        await db.GetTopicImage(topicId);

    [HttpPost("[action]/{topicId}")]
    public async Task UploadTopicImage([FromRoute]int topicId) =>
        await db.UploadTopicImage(
            Request.Form.Files,
            $@"{config.DirectoryBasePath}{topicPath}{topicId}/".Replace('\\', '/'),
            $@"{config.UrlBasePath}{topicPath}{topicId}/".Replace('\\', '/'),
            topicId
        );

    [HttpGet("[action]/{topicId}")]
    public async Task RemoveTopicImage([FromRoute]int topicId) =>
        await db.RemoveTopicImage(topicId);

    #endregion

    #region UserImaage

    private readonly string userPath = "user-images/";

    [HttpGet("[action]/{userId}")]
    public async Task<UserImage> GetUserImage([FromRoute]int userId) =>
        await db.GetUserImage(userId);

    [HttpPost("[action]/{userId}")]
    public async Task UploadUserImage([FromRoute]int userId) =>
        await db.UploadUserImage(
            Request.Form.Files,
            $@"{config.DirectoryBasePath}{userPath}{userId}/".Replace('\\', '/'),
            $@"{config.UrlBasePath}{userPath}{userId}/".Replace('\\', '/'),
            userId
        );

    [HttpGet("[action]/{userId}")]
    public async Task RemoveUserImage([FromRoute]int userId) =>
        await db.RemoveUserImage(userId);

    #endregion
}