namespace VolleyStatsManager.DAL.Entities
{
    public class PlayerStats
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int PointsScored { get; set; }
        public int Blocks { get; set; }
        public int Aces { get; set; }
        public int Errors { get; set; }
        public int Serves { get; set; }
    }
}