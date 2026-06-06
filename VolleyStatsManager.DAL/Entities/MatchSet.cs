namespace VolleyStatsManager.DAL.Entities
{
    public class MatchSet
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;
        public int SetNumber { get; set; }
        public int OurPoints { get; set; }
        public int OpponentPoints { get; set; }
    }
}
