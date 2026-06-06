namespace VolleyStats_Manager.Models.DTO.Export
{
    public class FullExportDto
    {
        public List<PlayerDTO> Players { get; set; } = [];
        public List<MatchDTO> Matches { get; set; } = [];
        public List<PlayerStatsRecordDTO> Statistics { get; set; } = [];
    }
}
