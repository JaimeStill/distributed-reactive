namespace Topics.Data.Entities;

public class TopicUser
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public int UserId { get; set; }
    public int IsAdmin { get; set; }
    public int IsBanned { get; set; }

    public Topic Topic { get; set; }
    public User User { get; set; }
}