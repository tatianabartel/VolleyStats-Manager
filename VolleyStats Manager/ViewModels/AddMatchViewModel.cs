using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

public partial class AddMatchViewModel(MatchService matchService, AppSessionService session) : ObservableObject
{
    private readonly MatchService _matchService = matchService;
    private readonly AppSessionService _session = session;

    [ObservableProperty]
    public partial DateTime MatchDate { get; set; } = DateTime.Today;
    [ObservableProperty]
    public partial string Opponent { get; set; } = string.Empty;
    public DateTime MaxMatchDate { get; } = DateTime.Today;

    [ObservableProperty]
    public partial ObservableCollection<MatchSetDTO> Sets { get; set; } = [
        new MatchSetDTO { SetNumber = 1 },
        new MatchSetDTO { SetNumber = 2 },
        new MatchSetDTO { SetNumber = 3 }
        ];

    [RelayCommand]
    public async Task AddSet()
    {
        if (Sets.Count >= 5) return;

        int ourWins = Sets.Count(s => s.OurPoints > s.OpponentPoints);
        int theirWins = Sets.Count(s => s.OpponentPoints > s.OurPoints);

        if (ourWins >= 3 || theirWins >= 3)
        {
            await Shell.Current.DisplayAlert("Cannot add set",
                "The match is already decided — one team has already won 3 sets.", "OK");
            return;
        }

        Sets.Add(new MatchSetDTO { SetNumber = Sets.Count + 1 });
    }

    [RelayCommand]
    public async Task SaveMatch()
    {
        Opponent = Regex.Replace(Opponent?.Trim() ?? string.Empty, @"\s+", " ");

        if (string.IsNullOrWhiteSpace(Opponent))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter the opponent team name.", "OK");
            return;
        }

        if (MatchDate.Date > DateTime.Today)
        {
            await Shell.Current.DisplayAlert("Error", "Match date cannot be in the future.", "OK");
            return;
        }

        if (!_session.HasTeamSelected) return;

        if (Opponent.Trim().Equals(_session.CurrentTeam!.Name.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            await Shell.Current.DisplayAlert("Error", "Opponent cannot be the same as your own team.", "OK");
            return;
        }

        if (Sets.Any(s => s.OurPoints < 0 || s.OpponentPoints < 0))
        {
            await Shell.Current.DisplayAlert("Error", "Points cannot be negative.", "OK");
            return;
        }

        try
        {
            await _matchService.CreateMatchAsync(
                Opponent,
                MatchDate,
                _session.CurrentTeamId!.Value,
                Sets.ToList());

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    public static async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}