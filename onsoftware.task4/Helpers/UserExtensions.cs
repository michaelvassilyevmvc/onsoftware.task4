
using onsoftware.task4.Areas.Identity.Data;

namespace onsoftware.task4.Helpers;

public static class UserExtensions
{
    public static IQueryable<ApplicationUser> Filter(
        this IQueryable<ApplicationUser> query,
        string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return query;

        filter = filter.Trim();

        return query.Where(x =>
            x.FirstName!.Contains(filter) ||
            x.LastName!.Contains(filter) ||
            x.Email!.Contains(filter));
    }
    
    public static IQueryable<ApplicationUser> Sort(
        this IQueryable<ApplicationUser> query,
        string? sort)
    {
        return sort switch
        {
            "email" => query.OrderBy(x => x.Email),

            "email_desc" => query.OrderByDescending(x => x.Email),

            "created" => query.OrderBy(x => x.CreatedAt),

            "created_desc" => query.OrderByDescending(x => x.CreatedAt),

            _ => query.OrderByDescending(x => x.UpdatedAt)
        };
    }
}