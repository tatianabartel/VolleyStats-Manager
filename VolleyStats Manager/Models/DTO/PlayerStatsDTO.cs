namespace VolleyStats_Manager.Models.DTO
{
    public class PlayerStatsDTO
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int Points { get; set; }
        public int Blocks { get; set; }
        public int Aces { get; set; }
        public int Serves { get; set; }
        public int Errors { get; set; }
    }
}