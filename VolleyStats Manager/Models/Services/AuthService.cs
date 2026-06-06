using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VolleyStats_Manager.Models.DTO;
using VolleyStatsManager.DAL;
using VolleyStatsManager.DAL.Entities;

namespace VolleyStats_Manager.Models.Services;

public class AuthService(VolleyDbContext context)
{
    private readonly VolleyDbContext _context = context;
    private readonly PasswordHasher<User> _hasher = new();

    private static string? ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return "Username cannot be empty.";
        if (userName.Any(char.IsWhiteSpace))
            return "Username cannot contain spaces.";
        if (userName.Length < 4)
            return "Username must be at least 4 characters long.";
        return null;
    }

    private static string? ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "Password cannot be empty.";
        if (password.Any(char.IsWhiteSpace))
            return "Password cannot contain spaces.";
        if (password.Length < 8)
            return "Password must be at least 8 characters long.";
        return null;
    }

    public async Task<UserDTO> RegisterAsync(string userName, string password)
    {
        var userNameError = ValidateUserName(userName);
        if (userNameError != null)
            throw new ArgumentException(userNameError);

        var passwordError = ValidatePassword(password);
        if (passwordError != null)
            throw new ArgumentException(passwordError);

        if (await _context.Users.AnyAsync(u => u.UserName == userName))
            throw new InvalidOperationException("User already exists");

        var user = new User
        {
            UserName = userName,
            PasswordHash = _hasher.HashPassword(null, password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO { Id = user.Id, UserName = user.UserName };
    }

    public async Task<UserDTO?> LoginAsync(string userName, string password)
    {
        var userNameError = ValidateUserName(userName);
        if (userNameError != null)
            throw new ArgumentException(userNameError);

        var passwordError = ValidatePassword(password);
        if (passwordError != null)
            throw new ArgumentException(passwordError);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user == null)
            return null;

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result != PasswordVerificationResult.Success)
            return null;

        return new UserDTO { Id = user.Id, UserName = user.UserName };
    }
}