using Microsoft.EntityFrameworkCore;
using VolleyStats_Manager.Models.DTO;
using VolleyStatsManager.DAL;
using VolleyStatsManager.DAL.Entities;

namespace VolleyStats_Manager.Models.Services
{
    public class TeamService(VolleyDbContext context)
    {
        private readonly VolleyDbContext _context = context;

        public async Task<TeamDTO> CreateTeamAsync(string name, int userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Team name is required");

            var team = new Team { Name = name, UserId = userId };
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return new TeamDTO { Id = team.Id, Name = team.Name };
        }

        public async Task<List<TeamDTO>> GetTeamsByUserAsync(int userId)
        {
            return await _context.Teams
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Name)
                .Select(t => new TeamDTO { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        public async Task DeleteTeamAsync(int teamId)
        {
            var team = await _context.Teams
                .Include(t => t.Players)
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new KeyNotFoundException("Team not found");

            var playerIds = team.Players.Select(p => p.Id).ToList();
            var matchIds = team.Matches.Select(m => m.Id).ToList();

            var stats = _context.PlayerStats
                .Where(ps => playerIds.Contains(ps.PlayerId));
            _context.PlayerStats.RemoveRange(stats);

            var sets = _context.MatchSets
                .Where(ms => matchIds.Contains(ms.MatchId));
            _context.MatchSets.RemoveRange(sets);

            _context.Players.RemoveRange(team.Players);
            _context.Matches.RemoveRange(team.Matches);
            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();
        }
    }
}