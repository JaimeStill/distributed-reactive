namespace Topics.Data.Entities;

public class Vote
{
    public int Id { get; set; }
    public int VoterId { get; set; }
    public string Type { get; set; }
    public bool Up { get; set; }

    public User Voter { get; set; }
}

public class CommentVote : Vote
{
    public int CommentId { get; set; }

    public Comment Comment { get; set; }
}

public class PostVote : Vote
{
    public int PostId { get; set; }

    public Post Post { get; set; }
}