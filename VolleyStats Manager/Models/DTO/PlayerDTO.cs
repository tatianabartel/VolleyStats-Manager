using System.Text.Json.Serialization;
using VolleyStatsManager.DAL.Enums;

namespace VolleyStats_Manager.Models.DTO
{
    public class PlayerDTO
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
        public int Number { get; set; }
        public PlayerPosition Position { get; set; }
        [JsonIgnore]
        public int TeamId { get; set; }
    }
}
