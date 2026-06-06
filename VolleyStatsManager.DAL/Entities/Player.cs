using System.ComponentModel.DataAnnotations;
using VolleyStatsManager.DAL.Enums;

namespace VolleyStatsManager.DAL.Entities
{
    public class Player
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public int Number { get; set; }
        [Required]
        public PlayerPosition Position { get; set; }
        public int TeamId { get; set; }
        [Required]
        public Team Team { get; set; } = null!;
        public virtual ICollection<PlayerStats> Stats { get; set; } = new List<PlayerStats>();
    }
}
