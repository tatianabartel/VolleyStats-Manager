using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

public partial class TeamStatisticsViewModel(
    PlayerStatsService statsService,
    PlayerService playerService,
    AppSessionService session) : ObservableObject
{
    private readonly PlayerStatsService _statsService = statsService;
    private readonly PlayerService _playerService = playerService;
    private readonly AppSessionService _session = session;

    private List<PlayerStatsRecordDTO> _rawStats = [];
    private List<PlayerDTO> _allPlayers = [];

    [ObservableProperty]
    public partial ObservableCollection<PlayerStatsDTO> LeaderBoard { get; set; } = [];
    [ObservableProperty]
    public partial string TeamName { get; set; } = string.Empty;
    [ObservableProperty]
    public partial PlayerDTO? SelectedPlayer { get; set; }
    [ObservableProperty]
    public partial int? SelectedMonth { get; set; }
    [ObservableProperty]
    public partial int? SelectedYear { get; set; }
    public ObservableCollection<PlayerDTO> Players { get; set; } = [];
    public List<int> Months { get; } = Enumerable.Range(1, 12).ToList();
    public List<int> Years { get; set; } = [];

    partial void OnSelectedPlayerChanged(PlayerDTO? value) => ApplyFilters();
    partial void OnSelectedMonthChanged(int? value) => ApplyFilters();
    partial void OnSelectedYearChanged(int? value) => ApplyFilters();

    public async Task LoadStatisticsAsync()
    {
        if (!_session.HasTeamSelected) return;

        TeamName = _session.CurrentTeam!.Name;
        var teamId = _session.CurrentTeamId!.Value;

        _allPlayers = await _playerService.GetPlayersByTeamAsync(teamId);

        var playerIds = _allPlayers.Select(p => p.Id).ToList();

        _rawStats = await _statsService.GetRawStatsByTeamAsync(playerIds);

        Years = _rawStats
            .Select(s => s.MatchDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();
        OnPropertyChanged(nameof(Years));

        Players.Clear();
        foreach (var p in _allPlayers) Players.Add(p);

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = _rawStats.AsEnumerable();

        if (SelectedPlayer != null)
            filtered = filtered.Where(s => s.PlayerId == SelectedPlayer.Id);

        if (SelectedMonth != null)
            filtered = filtered.Where(s => s.MatchDate.Month == SelectedMonth);

        if (SelectedYear != null)
            filtered = filtered.Where(s => s.MatchDate.Year == SelectedYear);

        var playersToShow = SelectedPlayer != null
            ? _allPlayers.Where(p => p.Id == SelectedPlayer.Id).ToList()
            : _allPlayers;

        var result = playersToShow.Select(p =>
        {
            var pStats = filtered.Where(s => s.PlayerId == p.Id).ToList();
            return new PlayerStatsDTO
            {
                PlayerId = p.Id,
                PlayerName = p.FullName,
                Points = pStats.Sum(s => s.PointsScored),
                Aces = pStats.Sum(s => s.Aces),
                Blocks = pStats.Sum(s => s.Blocks),
                Errors = pStats.Sum(s => s.Errors),
                Serves = pStats.Sum(s => s.Serves)
            };
        })
        .OrderByDescending(ps => ps.Points)
        .ToList();

        LeaderBoard.Clear();
        foreach (var s in result)
            LeaderBoard.Add(s);
    }

    [RelayCommand]
    private void ResetFilters()
    {
        SelectedPlayer = null;
        SelectedMonth = null;
        SelectedYear = null;
    }
}
