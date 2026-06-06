using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

[QueryProperty(nameof(MatchId), "MatchId")]
public partial class MatchDetailsViewModel(MatchService matchService) : ObservableObject
{
    private readonly MatchService _matchService = matchService;

    [ObservableProperty]
    public partial int MatchId { get; set; }
    [ObservableProperty]
    public partial MatchDTO? Match { get; set; }
    public ObservableCollection<PlayerStatsDTO> Players { get; set; } = [];

    partial void OnMatchIdChanged(int value) => _ = LoadMatch(value);

    private async Task LoadMatch(int id)
    {
        var result = await _matchService.GetMatchByIdAsync(id);

        if (result == null) return;

        Match = result;
        
        Players.Clear();
        if (result.PlayerStats != null)
        {
            foreach (var stat in result.PlayerStats)
                Players.Add(stat);
        }
    }
}