using System.Text.Json.Serialization;

namespace VolleyStats_Manager.Models.DTO
{
    public class PlayerStatsRecordDTO
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int PlayerId { get; set; }
        [JsonIgnore]
        public int MatchId { get; set; }
        public string PlayerFirstName { get; set; } = string.Empty;
        public string PlayerLastName { get; set; } = string.Empty;
        [JsonIgnore]
        public string FullName => $"{PlayerFirstName} {PlayerLastName}".Trim();
        public string MatchOpponent { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public int PointsScored { get; set; }
        public int Blocks { get; set; }
        public int Aces { get; set; }
        public int Errors { get; set; }
        public int Serves { get; set; }
    }
}
