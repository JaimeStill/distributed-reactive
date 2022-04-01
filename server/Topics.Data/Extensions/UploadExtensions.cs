namespace Topics.Data.Extensions;

using Topics.Core;
using Topics.Core.Extensions;
using Topics.Data.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public static class UploadExtensions
{
    #region Uploads

    public static void DeleteAll<T>(this List<T> uploads) where T : Upload =>
        uploads.ForEach(upload => upload.Delete());

    public static void Delete<T>(this T upload) where T : Upload
    {
        if (File.Exists(upload.Path))
            File.Delete(upload.Path);
    }

    #endregion

    #region Image

    public static string GetDefaultTopicImage(this string url) =>
        $"{url}topic.svg";

    public static string GetDefaultUserImage(this string url) =>
        $"{url}user.svg";

    #endregion

    #region PostUpload

    public static async Task<List<PostUpload>> GetPostUploads(this AppDbContext db, int postId) =>
        await db.PostUploads
            .Where(x => x.PostId == postId)
            .OrderBy(x => x.Name)
            .ToListAsync();

    public static async Task CreatePostUploads(this AppDbContext db, IFormFileCollection files, string path, string url, int postId)
    {
        if (files.Validate())
            foreach (var file in files)
                await db.CreatePostUpload(file, path, url, postId);
    }

    public static async Task Remove(this PostUpload upload, AppDbContext db)
    {
        db.PostUploads.Remove(upload);
        await db.SaveChangesAsync();
        upload.Delete();
    }

    static async Task CreatePostUpload(this AppDbContext db, IFormFile file, string path, string url, int postId)
    {
        var upload = await file.Write(new PostUpload(), path, url);
        upload.PostId = postId;
        await db.PostUploads.AddAsync(upload);
        await db.SaveChangesAsync();
    }

    #endregion

    #region TopicImage

    public static async Task<TopicImage> GetTopicImage(this AppDbContext db, int topicId) =>
        await db.TopicImages
            .FirstOrDefaultAsync(x => x.TopicId == topicId);

    public static async Task UploadTopicImage(this AppDbContext db, IFormFileCollection files, string path, string url, int topicId)
    {
        if (files.Validate("image"))
        {
            await db.RemoveTopicImage(topicId);

            var file = files[0];
            await db.AddTopicImage(file, path, url, topicId);
        }
    }

    public static async Task RemoveTopicImage(this AppDbContext db, int topicId)
    {
        var image = await db.TopicImages
            .FirstOrDefaultAsync(x => x.TopicId == topicId);

        if (image is not null)
        {
            db.TopicImages.Remove(image);
            await db.SaveChangesAsync();
            image.Delete();
        }
    }

    static async Task AddTopicImage(this AppDbContext db, IFormFile file, string path, string url, int topicId)
    {
        var image = await file.Write(new TopicImage(), path, url);
        image.TopicId = topicId;
        await db.TopicImages.AddAsync(image);
        await db.SaveChangesAsync();
    }

    #endregion

    #region UserImage

    public static async Task<UserImage> GetUserImage(this AppDbContext db, int userId) =>
        await db.UserImages
            .FirstOrDefaultAsync(x => x.UserId == userId);

    public static async Task UploadUserImage(this AppDbContext db, IFormFileCollection files, string path, string url, int userId)
    {
        if (files.Validate("image"))
        {
            await db.RemoveUserImage(userId);

            var file = files[0];
            await db.AddUserImage(file, path, url, userId);
        }
    }

    public static async Task RemoveUserImage(this AppDbContext db, int userId)
    {
        var image = await db.UserImages
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (image is not null)
        {
            db.UserImages.Remove(image);
            await db.SaveChangesAsync();
            image.Delete();
        }
    }

    static async Task AddUserImage(this AppDbContext db, IFormFile file, string path, string url, int userId)
    {
        var image = await file.Write(new UserImage(), path, url);
        image.UserId = userId;
        await db.UserImages.AddAsync(image);
        await db.SaveChangesAsync();
    }

    #endregion

    #region Internal

    static async Task<T> Write<T>(this IFormFile file, T upload, string path, string url) where T : Upload
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        file.Create(upload, path, url);
        using var stream = new FileStream(upload.Path, FileMode.Create);
        await file.CopyToAsync(stream);

        return upload;
    }

    static void Create<T>(this IFormFile file, T upload, string path, string url) where T : Upload
    {
        var f = file.CreateSafeName(path);

        upload.File = f;
        upload.Name = file.Name;
        upload.Path = $"{path}{f}";
        upload.Url = $"{url}{f}";
        upload.FileType = file.ContentType;
        upload.Size = file.Length;
    }

    static string CreateSafeName(this IFormFile file, string path)
    {
        var increment = 0;
        var fileName = file.FileName.UrlEncode();
        var newName = fileName;

        while (File.Exists(path + newName))
        {
            var extension = fileName.Split('.').Last();
            newName = $"{fileName.Replace($".{extension}", "")}_{++increment}.{extension}";
        }

        return newName;
    }

    static bool Validate(this IFormFileCollection files, string filetype = null)
    {
        if (!files.Any())
            throw new AppException("No files were provided for upload");

        if (!string.IsNullOrEmpty(filetype))
        {
            var invalid = files
                .Any(f => f.ContentType
                    .Split('/')[0]
                    .ToLower() != filetype.ToLower()
                );

            if (invalid)
                throw new AppException($"All uploads should be of type {filetype}");
        }

        return true;
    }

    #endregion
}