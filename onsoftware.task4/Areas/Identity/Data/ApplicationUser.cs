using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace onsoftware.task4.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column("fname", TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }

    [PersonalData]
    [Column("lname", TypeName = "nvarchar(100)")]
    public string LastName { get; set; }

    [PersonalData]
    [Column("position", TypeName = "nvarchar(100)")]
    public string? Position { get; set; }

    [Column("company", TypeName = "nvarchar(100)")]
    public string? Company { get; set; }

    [Column("status")] public Status Status { get; set; } = Status.Unverified;

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at", TypeName = "datetime2")]
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Log> Logs { get; set; } = [];

    public DateTime LastSeen => Logs.OrderByDescending(x => x.CreatedAt)
        .FirstOrDefault()
        ?.CreatedAt ?? DateTime.UtcNow;

    public int[] Last7DaysLogs => Logs.Where(x => x.CreatedAt >= DateTime.UtcNow.AddDays(-7))
        .GroupBy(x => x.CreatedAt.Date)
        .OrderBy(x => x.Key)
        .Select(x => x.Count())
        .ToArray();

    public void SetBlock()
    {
        Status = Status.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUnblock()
    {
        Status = EmailConfirmed ? Status.Active : Status.Unverified;
        UpdatedAt = DateTime.UtcNow;
    }
}