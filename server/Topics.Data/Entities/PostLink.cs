namespace Topics.Data.Entities;

public class PostLink
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Text { get; set; }
    public string Url { get; set; }

    public Post Post { get; set; }
}