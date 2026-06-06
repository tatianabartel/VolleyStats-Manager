using System.ComponentModel.DataAnnotations;

namespace VolleyStatsManager.DAL.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Opponent { get; set; } = string.Empty;
        public string DisplayName => $"{Opponent}  •  {Date:dd.MM.yyyy}";
        public int TeamId { get; set; }
        [Required]
        public Team Team { get; set; } = null!;
        public List<MatchSet> Sets { get; set; } = [];
        public int SetsWon => Sets?.Count(s => s.OurPoints > s.OpponentPoints) ?? 0;
        public int SetsLost => Sets?.Count(s => s.OurPoints < s.OpponentPoints) ?? 0;
    }
}
