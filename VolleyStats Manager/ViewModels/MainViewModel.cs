using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;
using VolleyStats_Manager.Views;

namespace VolleyStats_Manager.ViewModels;

public partial class MainViewModel(PlayerService playerService, MatchService matchService, PlayerStatsService statsService, AppSessionService session) : ObservableObject
{
    private readonly PlayerService _playerService = playerService;
    private readonly MatchService _matchService = matchService;
    private readonly PlayerStatsService _statsService = statsService;
    private readonly AppSessionService _session = session;

    [ObservableProperty]
    public partial string UserName { get; set; } = "Coach";
    [ObservableProperty]
    public partial string TeamName { get; set; } = "No team selected";
    [ObservableProperty]
    public partial int PlayerCount { get; set; }
    [ObservableProperty]
    public partial int MatchesCount { get; set; }
    [ObservableProperty]
    public partial string WinRate { get; set; } = "0%";
    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }
    public ObservableCollection<MatchDTO> RecentMatches { get; set; } = [];
    public ObservableCollection<PlayerDTO> TeamPlayers { get; set; } = [];
    public ObservableCollection<PlayerStatsDTO> TopThreePlayers { get; set; } = [];

    public async Task InitializeAsync()
    {
        if (!_session.HasTeamSelected || !_session.IsLoggedIn) return;

        UserName = _session.CurrentUser!.UserName;
        TeamName = _session.CurrentTeam!.Name;

        var teamId = _session.CurrentTeamId!.Value;

        var players = await _playerService.GetPlayersByTeamAsync(teamId);
        PlayerCount = players.Count;

        var allMatches = await _matchService.GetMatchesByTeamAsync(teamId);
        MatchesCount = allMatches.Count;

        if (allMatches.Count != 0)
        {
            int wins = allMatches.Count(m => m.SetsWon > m.SetsLost);
            double rate = (double)wins / MatchesCount * 100;
            WinRate = $"{Math.Round(rate, 0)}%";
        }
        else
        {
            WinRate = "0%";
        }

        RecentMatches.Clear();
        foreach (var m in allMatches.Take(5))
            RecentMatches.Add(m);

        TeamPlayers.Clear();
        foreach (var p in players)
            TeamPlayers.Add(p);

        var top = await _statsService.GetStatsByTeamAsync(teamId);
        TopThreePlayers.Clear();
        foreach (var tp in top.Take(3))
            TopThreePlayers.Add(tp);
    }

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        await InitializeAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private static async Task ViewAllMatches() => await Shell.Current.GoToAsync("//MatchesPage");

    [RelayCommand]
    private async Task ViewMatchDetail(MatchDTO match)
    {
        if (match == null) return;
        await Shell.Current.GoToAsync($"{nameof(MatchDetailsPage)}?MatchId={match.Id}");
    }

    [RelayCommand]
    private async Task EditMatch(MatchDTO match)
    {
        if (match == null) return;
        await Shell.Current.GoToAsync($"{nameof(UpdateMatchPage)}?MatchId={match.Id}");
    }

    [RelayCommand]
    private async Task DeleteMatch(MatchDTO match)
    {
        if (match == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Delete", $"Delete {TeamName} vs {match.Opponent}?", "Yes", "No");
        if (!confirm) return;

        try
        {
            await _matchService.DeleteMatchAsync(match.Id);
            RecentMatches.Remove(match);
            MatchesCount--;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        await Refresh();
    }

    [RelayCommand]
    private static async Task ViewAllPlayers() => await Shell.Current.GoToAsync("//PlayersPage");

    [RelayCommand]
    private async Task ViewPlayerStats(PlayerDTO player)
    {
        if (player == null) return;
        await Shell.Current.GoToAsync($"{nameof(PlayerDetailsPage)}?PlayerId={player.Id}");
    }

    [RelayCommand]
    private async Task EditPlayer(PlayerDTO player)
    {
        if (player == null) return;
        await Shell.Current.GoToAsync($"{nameof(UpdatePlayerPage)}?PlayerId={player.Id}");
    }

    [RelayCommand]
    private async Task DeletePlayer(PlayerDTO player)
    {
        if (player == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Delete", $"Delete {player.FullName}?", "Yes", "No");
        if (!confirm) return;

        try
        {
            await _playerService.DeletePlayerAsync(player.Id);
            TeamPlayers.Remove(player);
            PlayerCount--;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        await Refresh();
    }

    [RelayCommand]
    private static async Task ViewAllStats() => await Shell.Current.GoToAsync("//TeamStatisticsPage");
}
