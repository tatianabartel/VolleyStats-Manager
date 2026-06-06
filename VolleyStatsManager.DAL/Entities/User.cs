using System.ComponentModel.DataAnnotations;

namespace VolleyStatsManager.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public List<Team> Teams { get; set; } = [];
    }
}