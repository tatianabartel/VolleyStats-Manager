using Microsoft.EntityFrameworkCore;
using VolleyStats_Manager.Models.DTO;
using VolleyStatsManager.DAL;
using VolleyStatsManager.DAL.Entities;

namespace VolleyStats_Manager.Models.Services;

public class PlayerStatsService
{
    private readonly VolleyDbContext _context;

    public PlayerStatsService(VolleyDbContext context)
    {
        _context = context;
    }

    private static void ValidateStatValues(int points, int blocks, int aces, int serves)
    {
        if (blocks > points)
            throw new ArgumentException("Blocks cannot exceed points scored.");

        if (aces > points)
            throw new ArgumentException("Aces cannot exceed points scored.");

        if (aces > serves)
            throw new ArgumentException("Aces cannot exceed serves.");
    }

    private async Task<(int teamTotal, int opponentTotal)> GetMatchTotals(int matchId)
    {
        var match = await _context.Matches
            .Include(m => m.Sets)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null)
            throw new KeyNotFoundException("Match not found.");

        return (
            match.Sets.Sum(s => s.OurPoints),
            match.Sets.Sum(s => s.OpponentPoints)
        );
    }

    private async Task<int> GetRemainingTeamPoints(int matchId, int teamTotal, int excludeStatsId = 0)
    {
        int alreadyAssigned = await _context.PlayerStats
            .Where(ps => ps.MatchId == matchId && ps.Id != excludeStatsId)
            .SumAsync(ps => ps.PointsScored);

        return teamTotal - alreadyAssigned;
    }

    public async Task<int> CreateStatsAsync(int playerId, int matchId,
        int points, int blocks, int aces, int errors, int serves)
    {
        if (playerId <= 0)
            throw new ArgumentException("Invalid playerId");

        if (matchId <= 0)
            throw new ArgumentException("Invalid matchId");

        ValidateStatValues(points, blocks, aces, serves);

        var (teamTotal, opponentTotal) = await GetMatchTotals(matchId);

        int remaining = await GetRemainingTeamPoints(matchId, teamTotal);
        if (points > remaining)
            throw new ArgumentException(
                $"Points exceed team's available points for this match. Maximum for this player: {remaining}.");

        if (errors > opponentTotal)
            throw new ArgumentException(
                $"Errors cannot exceed opponent's total points in this match ({opponentTotal}).");

        var exists = await _context.PlayerStats.AnyAsync(ps =>
            ps.PlayerId == playerId && ps.MatchId == matchId);

        if (exists)
            throw new InvalidOperationException(
                "Statistics for this player in this match already exist");

        var stats = new PlayerStats
        {
            PlayerId = playerId,
            MatchId = matchId,
            PointsScored = points,
            Blocks = blocks,
            Aces = aces,
            Errors = errors,
            Serves = serves
        };

        _context.PlayerStats.Add(stats);
        await _context.SaveChangesAsync();

        return stats.Id;
    }

    public async Task DeleteStatsAsync(int id)
    {
        var stats = await _context.PlayerStats.FindAsync(id);

        if (stats == null)
            throw new KeyNotFoundException("Stats not found");

        _context.PlayerStats.Remove(stats);
        await _context.SaveChangesAsync();
    }

    public async Task<PlayerStatsRecordDTO?> GetStatsByIdAsync(int id)
    {
        var stats = await _context.PlayerStats
            .Include(ps => ps.Player)
            .Include(ps => ps.Match)
            .FirstOrDefaultAsync(ps => ps.Id == id);

        if (stats == null) return null;

        return new PlayerStatsRecordDTO
        {
            Id = stats.Id,
            PlayerId = stats.PlayerId,
            MatchId = stats.MatchId,
            PlayerFirstName = stats.Player.FirstName,
            PlayerLastName = stats.Player.LastName,
            MatchOpponent = stats.Match.Opponent,
            MatchDate = stats.Match.Date,
            PointsScored = stats.PointsScored,
            Blocks = stats.Blocks,
            Aces = stats.Aces,
            Errors = stats.Errors,
            Serves = stats.Serves
        };
    }

    public async Task<List<PlayerStatsRecordDTO>> GetStatsByPlayerAsync(int playerId)
    {
        return await _context.PlayerStats
            .Include(ps => ps.Player)
            .Include(ps => ps.Match)
            .Where(ps => ps.PlayerId == playerId)
            .OrderByDescending(ps => ps.Id)
            .Select(ps => new PlayerStatsRecordDTO
            {
                Id = ps.Id,
                PlayerId = ps.PlayerId,
                MatchId = ps.MatchId,
                PlayerFirstName = ps.Player.FirstName,
                PlayerLastName = ps.Player.LastName,
                MatchOpponent = ps.Match.Opponent,
                MatchDate = ps.Match.Date,
                PointsScored = ps.PointsScored,
                Blocks = ps.Blocks,
                Aces = ps.Aces,
                Errors = ps.Errors,
                Serves = ps.Serves
            })
            .ToListAsync();
    }

    public async Task<List<PlayerStatsDTO>> GetStatsByMatchAsync(int matchId)
    {
        return await _context.PlayerStats
            .Include(ps => ps.Player)
            .Where(ps => ps.MatchId == matchId)
            .Select(ps => new PlayerStatsDTO
            {
                PlayerId = ps.PlayerId,
                PlayerName = $"{ps.Player.FirstName} {ps.Player.LastName}",
                Points = ps.PointsScored,
                Blocks = ps.Blocks,
                Aces = ps.Aces,
                Errors = ps.Errors,
                Serves = ps.Serves
            })
            .ToListAsync();
    }

    public async Task<List<PlayerStatsDTO>> GetStatsByTeamAsync(int teamId,
        int? month = null, int? year = null)
    {
        var playerIds = await _context.Players
            .Where(p => p.TeamId == teamId)
            .Select(p => p.Id)
            .ToListAsync();

        var query = _context.PlayerStats
            .Include(ps => ps.Player)
            .Include(ps => ps.Match)
            .Where(ps => playerIds.Contains(ps.PlayerId));

        if (month.HasValue)
            query = query.Where(ps => ps.Match.Date.Month == month.Value);

        if (year.HasValue)
            query = query.Where(ps => ps.Match.Date.Year == year.Value);

        var rawStats = await query.ToListAsync();

        return rawStats
            .GroupBy(ps => ps.PlayerId)
            .Select(g => new PlayerStatsDTO
            {
                PlayerId = g.Key,
                PlayerName = $"{g.First().Player.FirstName} {g.First().Player.LastName}",
                Points = g.Sum(s => s.PointsScored),
                Blocks = g.Sum(s => s.Blocks),
                Aces = g.Sum(s => s.Aces),
                Errors = g.Sum(s => s.Errors),
                Serves = g.Sum(s => s.Serves)
            })
            .ToList();
    }

    public async Task UpdateStatsAsync(int statsId, int points, int blocks, int aces, int errors, int serves)
    {
        var existing = await _context.PlayerStats
            .FirstOrDefaultAsync(ps => ps.Id == statsId);

        if (existing == null)
            throw new KeyNotFoundException("Stats not found");

        ValidateStatValues(points, blocks, aces, serves);

        var (teamTotal, opponentTotal) = await GetMatchTotals(existing.MatchId);

        int remaining = await GetRemainingTeamPoints(existing.MatchId, teamTotal, excludeStatsId: statsId);
        if (points > remaining)
            throw new ArgumentException(
                $"Points exceed team's available points for this match. Maximum for this player: {remaining}.");

        if (errors > opponentTotal)
            throw new ArgumentException(
                $"Errors cannot exceed opponent's total points in this match ({opponentTotal}).");

        existing.PointsScored = points;
        existing.Blocks = blocks;
        existing.Aces = aces;
        existing.Errors = errors;
        existing.Serves = serves;

        await _context.SaveChangesAsync();
    }

    public async Task<List<PlayerStatsRecordDTO>> GetRawStatsByTeamAsync(List<int> playerIds)
    {
        return await _context.PlayerStats
            .Include(ps => ps.Player)
            .Include(ps => ps.Match)
            .Where(ps => playerIds.Contains(ps.PlayerId))
            .Select(ps => new PlayerStatsRecordDTO
            {
                Id = ps.Id,
                PlayerId = ps.PlayerId,
                MatchId = ps.MatchId,
                PlayerFirstName = ps.Player.FirstName,
                PlayerLastName = ps.Player.LastName,
                MatchOpponent = ps.Match.Opponent,
                MatchDate = ps.Match.Date,
                PointsScored = ps.PointsScored,
                Blocks = ps.Blocks,
                Aces = ps.Aces,
                Errors = ps.Errors,
                Serves = ps.Serves
            })
            .ToListAsync();
    }
}
