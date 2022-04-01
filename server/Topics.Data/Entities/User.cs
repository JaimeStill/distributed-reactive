namespace Topics.Data.Entities;

public class User
{
    public int Id { get; set; }

    public int DefaultPageSize { get; set; }
    public bool UseDarkTheme { get; set; }
    public DateTime DateJoined { get; set; }

    /*
        Properties mapped from Topics.Identity.AdUser.cs
    */
    public Guid Guid { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }
    public string DistinguishedName { get; set; }
    public string SamAccountName { get; set; }
    public string UserPrincipalName { get; set; }
    public string VoiceTelephoneNumber { get; set; }
    public string SocketName { get; set; }

    /* One to One */
    public UserImage Image { get; set; }

    public IEnumerable<Comment> Comments { get; set; }
    public IEnumerable<Post> Posts { get; set; }
    public IEnumerable<Topic> Topics { get; set; }
    public IEnumerable<TopicUser> Memberships { get; set; }
    public IEnumerable<Vote> Votes { get; set; }
}