namespace Bubnilka.Shared.Models;

public class Chat
{
    public int Id { get; set; }
    public int UserId1 { get; set; }
    public int UserId2 { get; set; }
    public User User1 { get; set; } = null!;
    public User User2 { get; set; } = null!;
    public List<Message> Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }
}
