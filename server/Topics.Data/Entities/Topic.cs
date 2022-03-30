namespace Topics.Data.Entities;

public class Topic
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public DateTime DateCreated { get; set; }

    public User Owner { get; set; }

    /* One to One */
    public TopicImage Image { get; set; }

    public IEnumerable<Post> Posts { get; set; }
    public IEnumerable<TopicUser> Members { get; set; }
}