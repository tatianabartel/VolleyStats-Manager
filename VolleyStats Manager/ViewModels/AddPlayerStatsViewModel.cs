using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

public partial class AddPlayerStatsViewModel : ObservableObject
{
    private readonly PlayerStatsService _statsService;
    private readonly PlayerService _playerService;
    private readonly MatchService _matchService;
    private readonly AppSessionService _session;

    private int _pendingPlayerId;

    public ObservableCollection<PlayerDTO> Players { get; set; } = [];
    public ObservableCollection<MatchDTO> Matches { get; set; } = [];

    [ObservableProperty]
    public partial PlayerDTO? SelectedPlayer { get; set; }
    [ObservableProperty]
    public partial MatchDTO? SelectedMatch { get; set; }
    [ObservableProperty]
    public partial int PointsScored { get; set; }
    [ObservableProperty]
    public partial int Blocks { get; set; }
    [ObservableProperty]
    public partial int Aces { get; set; }
    [ObservableProperty]
    public partial int Errors { get; set; }
    [ObservableProperty]
    public partial int Serves { get; set; }

    public AddPlayerStatsViewModel(
        PlayerStatsService statsService,
        PlayerService playerService,
        MatchService matchService,
        AppSessionService session)
    {
        _statsService = statsService;
        _playerService = playerService;
        _matchService = matchService;
        _session = session;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        if (!_session.HasTeamSelected) return;

        try
        {
            var players = await _playerService.GetPlayersByTeamAsync(_session.CurrentTeamId!.Value);
            var matches = await _matchService.GetMatchesByTeamAsync(_session.CurrentTeamId!.Value);

            Players.Clear();
            foreach (var p in players) Players.Add(p);

            Matches.Clear();
            foreach (var m in matches) Matches.Add(m);

            if (_pendingPlayerId > 0)
            {
                SelectedPlayer = Players.FirstOrDefault(p => p.Id == _pendingPlayerId);
                _pendingPlayerId = 0;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    public Task PreSelectPlayer(int playerId)
    {
        SelectedPlayer = Players.FirstOrDefault(p => p.Id == playerId);
        if (SelectedPlayer == null)
            _pendingPlayerId = playerId;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SaveStats()
    {
        if (SelectedPlayer == null || SelectedMatch == null)
        {
            await Shell.Current.DisplayAlert("Error", "Select player and match", "OK");
            return;
        }

        if (PointsScored < 0 || Blocks < 0 || Aces < 0 || Errors < 0 || Serves < 0)
        {
            await Shell.Current.DisplayAlert("Error", "Statistics cannot be negative.", "OK");
            return;
        }

        try
        {
            await _statsService.CreateStatsAsync(
                SelectedPlayer.Id,
                SelectedMatch.Id,
                PointsScored,
                Blocks,
                Aces,
                Errors,
                Serves);

            await Shell.Current.GoToAsync("..");
        }
        catch (InvalidOperationException ex)
        {
            await Shell.Current.DisplayAlert("Already exists", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task Cancel() => await Shell.Current.GoToAsync("..");
}
