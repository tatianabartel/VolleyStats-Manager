using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

[QueryProperty(nameof(MatchId), "MatchId")]
public partial class UpdateMatchViewModel(MatchService matchService, AppSessionService session) : ObservableObject
{
    private readonly MatchService _matchService = matchService;
    private readonly AppSessionService _session = session;

    [ObservableProperty]
    public partial int MatchId { get; set; }
    [ObservableProperty]
    public partial MatchDTO? Match { get; set; }
    [ObservableProperty]
    public partial ObservableCollection<MatchSetDTO> Sets { get; set; } = [];
    public DateTime MaxMatchDate { get; } = DateTime.Today;

    partial void OnMatchIdChanged(int value) => _ = LoadMatch(value);

    private async Task LoadMatch(int id)
    {
        var matchFromDb = await _matchService.GetMatchByIdAsync(id);
        if (matchFromDb == null) return;

        Match = matchFromDb;
        Sets = new ObservableCollection<MatchSetDTO>(
            matchFromDb.Sets.Select(s => new MatchSetDTO
            {
                SetNumber = s.SetNumber,
                OurPoints = s.OurPoints,
                OpponentPoints = s.OpponentPoints
            })
        );
    }

    [RelayCommand]
    private async Task Save()
    {
        if (Match == null) return;

        Match.Opponent = Regex.Replace(Match.Opponent?.Trim() ?? string.Empty, @"\s+", " ");

        if (string.IsNullOrWhiteSpace(Match.Opponent))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter opponent name.", "OK");
            return;
        }

        if (Match.Opponent.Equals(_session.CurrentTeam?.Name, StringComparison.OrdinalIgnoreCase))
        {
            await Shell.Current.DisplayAlert("Error", "Opponent cannot be the same as your own team.", "OK");
            return;
        }

        if (Match.MatchDate.Date > DateTime.Today)
        {
            await Shell.Current.DisplayAlert("Error", "Match date cannot be in the future.", "OK");
            return;
        }

        if (Sets.Any(s => s.OurPoints < 0 || s.OpponentPoints < 0))
        {
            await Shell.Current.DisplayAlert("Error", "Points cannot be negative.", "OK");
            return;
        }

        Match.Sets = Sets.ToList();

        try
        {
            await _matchService.UpdateMatchAsync(Match);
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
