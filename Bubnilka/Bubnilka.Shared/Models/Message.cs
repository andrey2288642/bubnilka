namespace Bubnilka.Shared.Models;

public class Message
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
    public Chat Chat { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
