namespace Topics.Data.Entities;

public class Post
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int TopicId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public string Text { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DatePublished { get; set; }
    public bool IsPublished { get; set; }

    public User Author { get; set; }
    public Topic Topic { get; set; }

    public IEnumerable<Comment> Comments { get; set; }
    public IEnumerable<PostLink> Links { get; set; }
    public IEnumerable<PostUpload> Uploads { get; set; }
    public IEnumerable<PostVote> Votes { get; set; }
}