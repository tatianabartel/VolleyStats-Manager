using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VolleyStats_Manager.Models.Services;

namespace VolleyStats_Manager.ViewModels;

public partial class AddTeamViewModel(TeamService teamService, AppSessionService session) : ObservableObject
{
    private readonly TeamService _teamService = teamService;
    private readonly AppSessionService _session = session;

    [ObservableProperty]
    public partial string TeamName { get; set; } = string.Empty;

    [RelayCommand]
    private async Task SaveTeam()
    {
        TeamName = TeamName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(TeamName))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a team name.", "OK");
            return;
        }

        if (!_session.IsLoggedIn)
        {
            await Shell.Current.DisplayAlert("Error", "Not logged in.", "OK");
            return;
        }

        try
        {
            var newTeam = await _teamService.CreateTeamAsync(
                TeamName,
                _session.CurrentUser.Id);

            _session.CurrentTeam = newTeam;

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
    [RelayCommand]
    private static async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
