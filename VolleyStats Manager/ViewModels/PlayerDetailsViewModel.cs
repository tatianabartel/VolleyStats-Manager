using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;
using VolleyStats_Manager.Views;

namespace VolleyStats_Manager.ViewModels;

[QueryProperty(nameof(PlayerId), "PlayerId")]
public partial class PlayerDetailsViewModel(PlayerService playerService, PlayerStatsService statsService) : ObservableObject
{
    private readonly PlayerService _playerService = playerService;
    private readonly PlayerStatsService _statsService = statsService;

    [ObservableProperty]
    public partial int PlayerId { get; set; }
    [ObservableProperty]
    public partial PlayerDTO? Player { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<PlayerStatsRecordDTO> Stats { get; set; } = [];

    partial void OnPlayerIdChanged(int value) => _ = LoadData(value);

    public async Task LoadData(int id)
    {
        Player = await _playerService.GetPlayerByIdAsync(id);
        if (Player == null) return;

        await RefreshStats();
    }

    private async Task RefreshStats()
    {
        var loaded = await _statsService.GetStatsByPlayerAsync(PlayerId);
        Stats.Clear();
        foreach (var s in loaded)
            Stats.Add(s);
    }

    [RelayCommand]
    private async Task AddStats() => await Shell.Current.GoToAsync($"{nameof(AddPlayerStatsPage)}?PlayerId={PlayerId}");

    [RelayCommand]
    private async Task EditStats(int statsId) => await Shell.Current.GoToAsync($"{nameof(UpdatePlayerStatsPage)}?StatsId={statsId}");

    [RelayCommand]
    private async Task DeleteStats(int statsId)
    {
        bool confirm = await Shell.Current.DisplayAlert("Delete", "Delete these statistics?", "Yes", "No");
        if (!confirm) return;

        try
        {
            await _statsService.DeleteStatsAsync(statsId);
            await RefreshStats();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}