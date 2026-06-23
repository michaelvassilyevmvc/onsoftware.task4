namespace onsoftware.task4.Areas.Identity.Data;

public class Log
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}