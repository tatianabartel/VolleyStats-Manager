using System.Text.Json.Serialization;

namespace VolleyStats_Manager.Models.DTO
{
    public class MatchDTO
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Opponent { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        [JsonIgnore]
        public int TeamId { get; set; }
        public List<MatchSetDTO> Sets { get; set; } = [];
        [JsonIgnore]
        public List<PlayerStatsDTO> PlayerStats { get; set; } = [];
        public int SetsWon => Sets.Count(s => s.OurPoints > s.OpponentPoints);
        public int SetsLost => Sets.Count(s => s.OurPoints < s.OpponentPoints);
        [JsonIgnore]
        public string DisplayName => $"{Opponent} ({MatchDate:dd.MM.yyyy})";
    }
}