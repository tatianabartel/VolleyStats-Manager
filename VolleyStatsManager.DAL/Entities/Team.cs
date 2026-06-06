using System.ComponentModel.DataAnnotations;

namespace VolleyStatsManager.DAL.Entities
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!; 
        public List<Player> Players { get; set; } = []; 
        public List<Match> Matches { get; set; } = []; 
    }
}