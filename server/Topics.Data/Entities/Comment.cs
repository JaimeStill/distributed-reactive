namespace Topics.Data.Entities;

public class Comment
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int? ParentId { get; set; }
    public int PostId { get; set; }
    public string Text { get; set; }
    public DateTime DateCreated { get; set; }

    public User Author { get; set; }
    public Comment Parent { get; set; }
    public Post Post { get; set; }

    public IEnumerable<Comment> Children { get; set; }
    public IEnumerable<CommentVote> Votes { get; set; }
}