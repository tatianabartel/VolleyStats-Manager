using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyStats_Manager.Models.Services;
using VolleyStats_Manager.Views;

namespace VolleyStats_Manager.ViewModels;

public partial class UpdatePlayerStatsViewModel(PlayerStatsService statsService) : ObservableObject
{
    private readonly PlayerStatsService _statsService = statsService;

    private int _statsId;
    private int _playerId;

    [ObservableProperty]
    public partial string PlayerName { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string MatchName { get; set; } = string.Empty;
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

    public async Task LoadStatsById(int statsId)
    {
        var stats = await _statsService.GetStatsByIdAsync(statsId);
        if (stats == null) return;

        _statsId = stats.Id;
        _playerId = stats.PlayerId;
        PlayerName = stats.FullName;
        MatchName = $"{stats.MatchOpponent} • {stats.MatchDate:dd.MM.yyyy}";
        PointsScored = stats.PointsScored;
        Blocks = stats.Blocks;
        Aces = stats.Aces;
        Errors = stats.Errors;
        Serves = stats.Serves;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (_statsId == 0) return;

        if (PointsScored < 0 || Blocks < 0 || Aces < 0 || Errors < 0 || Serves < 0)
        {
            await Shell.Current.DisplayAlert("Error", "Statistics cannot be negative.", "OK");
            return;
        }

        try
        {
            await _statsService.UpdateStatsAsync(_statsId, PointsScored, Blocks, Aces, Errors, Serves);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private static async Task Cancel() => await Shell.Current.GoToAsync("..");
}
