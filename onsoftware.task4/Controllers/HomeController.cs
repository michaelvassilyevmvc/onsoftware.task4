using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onsoftware.task4.Areas.Identity.Data;
using onsoftware.task4.Helpers;
using onsoftware.task4.Models;

namespace onsoftware.task4.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AuthDbContext _context;

    public HomeController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AuthDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public IActionResult Index(string? sort, string? filter)
    {
        ViewBag.CurrentSort = sort;
        ViewBag.EmailSort = sort == "email" ? "email_desc" : "email";
        ViewBag.Filter = filter;

        var users = _context.Users
            .Include(x => x.Logs)
            .OrderBy(x => x.UpdatedAt)
            .Filter(filter)
            .Sort(sort)
            .ToList();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> PanelAction(string action, List<string> selectedUsers)
    {
        if (!selectedUsers.Any() && action != "clear")
        {
            TempData.Info("Nothing selected - nothing done");
            return RedirectToAction(nameof(Index));
        }

        var users = _context.Users
            .Where(x => selectedUsers.Contains(x.Id))
            .ToList();

        return action switch
        {
            "block" => await BlockUsers(users),
            "unblock" => await UnblockUsers(users),
            "delete" => await DeleteUsers(users),
            "clear" => await ClearBlockedUsers(),
            _ => RedirectToAction(nameof(Index))
        };
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<IActionResult> BlockUsers(List<ApplicationUser> users)
    {
        foreach (var user in users)
        {
            user.SetBlock();
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }

        await _context.SaveChangesAsync();
        TempData.Success("Users blocked successfully");

        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> UnblockUsers(List<ApplicationUser> users)
    {
        foreach (var user in users)
        {
            user.SetUnblock();
            await _userManager.SetLockoutEndDateAsync(user, null);
        }

        await _context.SaveChangesAsync();
        TempData.Success("Users unblocked successfully");

        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> DeleteUsers(List<ApplicationUser> users)
    {
        var currentUserId = _userManager.GetUserId(User);

        foreach (var user in users)
        {
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData.Error(string.Join("; ", result.Errors.Select(x => x.Description)));
                return RedirectToAction(nameof(Index));
            }

            if (user.Id == currentUserId)
            {
                await _signInManager.SignOutAsync();
                TempData.Success("Your account has been deleted");

                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }

        TempData.Success("Users deleted successfully");
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> ClearBlockedUsers()
    {
        var blockedUsers = _context.Users
            .Where(x => x.Status == Status.Blocked)
            .ToList();

        if (!blockedUsers.Any())
        {
            TempData.Info("No blocked users found");
            return RedirectToAction(nameof(Index));
        }

        return await DeleteUsers(blockedUsers);
    }

    
}