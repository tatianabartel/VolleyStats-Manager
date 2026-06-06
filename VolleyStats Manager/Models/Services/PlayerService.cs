using Microsoft.EntityFrameworkCore;
using VolleyStats_Manager.Models.DTO;
using VolleyStatsManager.DAL;
using VolleyStatsManager.DAL.Entities;
using VolleyStatsManager.DAL.Enums;

namespace VolleyStats_Manager.Models.Services;

public class PlayerService(VolleyDbContext context)
{
    private readonly VolleyDbContext _context = context;

    public async Task<int> CreatePlayerAsync(string firstName, string lastName,
        int number, PlayerPosition position, int teamId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");

        if (teamId <= 0)
            throw new ArgumentException("Invalid teamId");

        var numberTaken = await _context.Players.AnyAsync(p =>
            p.TeamId == teamId && p.Number == number);

        if (numberTaken)
            throw new InvalidOperationException($"Jersey number {number} is already taken in this team.");

        var exists = await _context.Players.AnyAsync(p =>
            p.TeamId == teamId &&
            p.FirstName == firstName &&
            p.LastName == lastName);

        if (exists)
            throw new InvalidOperationException("A player with this name already exists in the team.");

        var player = new Player
        {
            FirstName = firstName,
            LastName = lastName,
            Number = number,
            Position = position,
            TeamId = teamId
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return player.Id;
    }

    public async Task DeletePlayerAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);

        if (player == null)
            throw new KeyNotFoundException("Player not found");

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();
    }

    public async Task<PlayerDTO?> GetPlayerByIdAsync(int id)
    {
        return await _context.Players
            .Where(p => p.Id == id)
            .Select(p => new PlayerDTO
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Number = p.Number,
                Position = p.Position,
                TeamId = p.TeamId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<PlayerDTO>> GetPlayersByTeamAsync(int teamId)
    {
        return await _context.Players
            .AsNoTracking()
            .Where(p => p.TeamId == teamId)
            .OrderBy(p => p.Number)
            .Select(p => new PlayerDTO
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Number = p.Number,
                Position = p.Position,
                TeamId = p.TeamId
            })
            .ToListAsync();
    }

    public async Task UpdatePlayerAsync(PlayerDTO dto)
    {
        var existing = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        if (existing == null)
            throw new KeyNotFoundException("Player not found");

        if (dto.Number != existing.Number)
        {
            var numberTaken = await _context.Players.AnyAsync(p =>
                p.TeamId == existing.TeamId && p.Number == dto.Number && p.Id != dto.Id);

            if (numberTaken)
                throw new InvalidOperationException($"Jersey number {dto.Number} is already taken in this team.");
        }

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Number = dto.Number;
        existing.Position = dto.Position;

        await _context.SaveChangesAsync();
    }
}
