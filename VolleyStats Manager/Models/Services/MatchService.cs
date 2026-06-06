using Microsoft.EntityFrameworkCore;
using VolleyStats_Manager.Models.DTO;
using VolleyStatsManager.DAL;
using VolleyStatsManager.DAL.Entities;

namespace VolleyStats_Manager.Models.Services;

public class MatchService(VolleyDbContext context)
{
    private readonly VolleyDbContext _context = context;

    private static void ValidateSets(List<MatchSetDTO> sets)
    {
        for (int i = 0; i < sets.Count; i++)
        {
            var set = sets[i];
            int setNumber = set.SetNumber > 0 ? set.SetNumber : i + 1;

            if (set.OurPoints < 0 || set.OpponentPoints < 0)
                throw new ArgumentException($"Set {setNumber}: points cannot be negative.");

            int maxPoints = Math.Max(set.OurPoints, set.OpponentPoints);
            int diff = Math.Abs(set.OurPoints - set.OpponentPoints);
            int winTo = setNumber == 5 ? 15 : 25;

            if (maxPoints < winTo)
                throw new ArgumentException($"Set {setNumber}: one team must reach at least {winTo} points.");

            if (diff < 2)
                throw new ArgumentException($"Set {setNumber}: the winner must lead by at least 2 points.");

            if (maxPoints > winTo && diff != 2)
                throw new ArgumentException($"Set {setNumber}: once the score exceeds {winTo}, the winner must lead by exactly 2 points (e.g. {winTo + 1}:{winTo - 1}).");
        }

        int ourWins = sets.Count(s => s.OurPoints > s.OpponentPoints);
        int theirWins = sets.Count(s => s.OpponentPoints > s.OurPoints);

        if (ourWins != 3 && theirWins != 3)
            throw new ArgumentException("Match result is invalid — one team must win exactly 3 sets.");
    }

    public async Task<int> CreateMatchAsync(string opponent, DateTime date, int teamId, List<MatchSetDTO> sets)
    {
        if (string.IsNullOrWhiteSpace(opponent))
            throw new ArgumentException("Opponent is required.");

        if (sets == null || !sets.Any())
            throw new ArgumentException("At least one set is required.");

        if (teamId <= 0)
            throw new ArgumentException("Invalid teamId.");

        ValidateSets(sets);

        var exists = await _context.Matches.AnyAsync(m =>
            m.TeamId == teamId &&
            m.Opponent == opponent &&
            m.Date.Date == date.Date);

        if (exists)
            throw new InvalidOperationException("Match against this opponent on this date already exists.");

        var match = new Match
        {
            Opponent = opponent,
            Date = date,
            TeamId = teamId,
            Sets = sets.Select(s => new MatchSet
            {
                SetNumber = s.SetNumber,
                OurPoints = s.OurPoints,
                OpponentPoints = s.OpponentPoints
            }).ToList()
        };

        _context.Matches.Add(match);
        await _context.SaveChangesAsync();

        return match.Id;
    }

    public async Task DeleteMatchAsync(int id)
    {
        var match = await _context.Matches.FindAsync(id);

        if (match == null)
            throw new KeyNotFoundException("Match not found");

        _context.Matches.Remove(match);
        await _context.SaveChangesAsync();
    }

    public async Task<MatchDTO?> GetMatchByIdAsync(int id)
    {
        var match = await _context.Matches
            .Include(m => m.Sets)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (match == null) return null;

        var playerStats = await _context.PlayerStats
            .Include(ps => ps.Player)
            .Where(ps => ps.MatchId == id)
            .Select(ps => new PlayerStatsDTO
            {
                PlayerId = ps.PlayerId,
                PlayerName = $"{ps.Player.FirstName} {ps.Player.LastName}",
                Points = ps.PointsScored,
                Blocks = ps.Blocks,
                Aces = ps.Aces,
                Errors = ps.Errors,
                Serves = ps.Serves
            }).ToListAsync();

        return new MatchDTO
        {
            Id = match.Id,
            Opponent = match.Opponent,
            MatchDate = match.Date,
            TeamId = match.TeamId,
            Sets = match.Sets.Select(s => new MatchSetDTO
            {
                SetNumber = s.SetNumber,
                OurPoints = s.OurPoints,
                OpponentPoints = s.OpponentPoints
            }).ToList(),
            PlayerStats = playerStats
        };
    }

    public async Task UpdateMatchAsync(MatchDTO dto)
    {
        var existing = await _context.Matches
            .Include(m => m.Sets)
            .FirstOrDefaultAsync(m => m.Id == dto.Id);

        if (existing == null)
            throw new KeyNotFoundException("Match not found");

        if (string.IsNullOrWhiteSpace(dto.Opponent))
            throw new ArgumentException("Opponent is required.");

        ValidateSets(dto.Sets);

        var duplicate = await _context.Matches.AnyAsync(m =>
            m.TeamId == existing.TeamId &&
            m.Opponent == dto.Opponent &&
            m.Date.Date == dto.MatchDate.Date &&
            m.Id != dto.Id);

        if (duplicate)
            throw new InvalidOperationException("Match against this opponent on this date already exists.");

        existing.Opponent = dto.Opponent;
        existing.Date = dto.MatchDate;

        _context.MatchSets.RemoveRange(existing.Sets);

        existing.Sets = dto.Sets
            .Select(s => new MatchSet
            {
                SetNumber = s.SetNumber,
                OurPoints = s.OurPoints,
                OpponentPoints = s.OpponentPoints
            })
            .ToList();

        await _context.SaveChangesAsync();
    }

    public async Task<List<MatchDTO>> GetMatchesByTeamAsync(int teamId)
    {
        return await _context.Matches
            .AsNoTracking()
            .Include(m => m.Sets)
            .Where(m => m.TeamId == teamId)
            .OrderByDescending(m => m.Date)
            .Select(m => new MatchDTO
            {
                Id = m.Id,
                Opponent = m.Opponent,
                MatchDate = m.Date,
                TeamId = m.TeamId,
                Sets = m.Sets.Select(s => new MatchSetDTO
                {
                    SetNumber = s.SetNumber,
                    OurPoints = s.OurPoints,
                    OpponentPoints = s.OpponentPoints
                }).ToList()
            })
            .ToListAsync();
    }
}
