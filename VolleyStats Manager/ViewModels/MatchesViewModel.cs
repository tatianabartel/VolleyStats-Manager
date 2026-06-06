using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;
using VolleyStats_Manager.Views;

namespace VolleyStats_Manager.ViewModels;

public partial class MatchesViewModel(MatchService matchService, AppSessionService session) : ObservableObject
{
    private readonly MatchService _matchService = matchService;
    private readonly AppSessionService _session = session;

    public ObservableCollection<MatchDTO> Matches { get; set; } = [];

    [RelayCommand]
    public async Task LoadMatches()
    {
        if (!_session.HasTeamSelected) return;

        var matchList = await _matchService
            .GetMatchesByTeamAsync(_session.CurrentTeamId!.Value);

        Matches.Clear();
        foreach (var m in matchList)
            Matches.Add(m);
    }

    [RelayCommand]
    public static async Task GoToAddMatchPage() => await Shell.Current.GoToAsync(nameof(AddMatchPage));

    [RelayCommand]
    public static async Task ViewMatch(MatchDTO match) => await Shell.Current.GoToAsync($"{nameof(MatchDetailsPage)}?MatchId={match.Id}");

    [RelayCommand]
    public static async Task EditMatch(MatchDTO match) => await Shell.Current.GoToAsync($"{nameof(UpdateMatchPage)}?MatchId={match.Id}");

    [RelayCommand]
    public async Task DeleteMatch(MatchDTO match)
    {
        if (match == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Confirm", $"Delete {_session.CurrentTeam!.Name} vs {match.Opponent}?", "Yes", "No");
        if (!confirm) return;

        try
        {
            await _matchService.DeleteMatchAsync(match.Id);
            Matches.Remove(match);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}