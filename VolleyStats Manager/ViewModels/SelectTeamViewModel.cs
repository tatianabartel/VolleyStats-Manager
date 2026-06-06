using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VolleyStats_Manager.Models.DTO;
using VolleyStats_Manager.Models.Services;
using VolleyStats_Manager.Views;

namespace VolleyStats_Manager.ViewModels;

public partial class SelectTeamViewModel(TeamService teamService, AppSessionService session) : ObservableObject
{
    private readonly TeamService _teamService = teamService;
    private readonly AppSessionService _session = session;

    [ObservableProperty]
    public partial ObservableCollection<TeamDTO> UserTeams { get; set; } = [];

    public async Task OnAppearing() => await LoadTeams();

    [RelayCommand]
    private async Task LoadTeams()
    {
        if (!_session.IsLoggedIn) return;

        UserTeams.Clear();

        var teams = await _teamService
            .GetTeamsByUserAsync(_session.CurrentUser!.Id);

        foreach (var team in teams)
            UserTeams.Add(team);
    }

    [RelayCommand]
    private async Task SelectTeam(TeamDTO selectedTeam)
    {
        if (selectedTeam == null) return;

        _session.CurrentTeam = selectedTeam;
        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync("//MainPage");
    }

    [RelayCommand]
    private static async Task GoToAddTeam() => await Shell.Current.GoToAsync(nameof(AddTeamPage));

    [RelayCommand]
    private async Task Logout()
    {
        _session.ClearSession();
        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync("//LoginPage");
    }

    [RelayCommand]
    private async Task DeleteTeam(TeamDTO team)
    {
        if (team == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Delete Team",
            $"Delete '{team.Name}'? This will also delete all players, matches and statistics.",
            "Yes, delete", "Cancel");
        if (!confirm) return;

        try
        {
            await _teamService.DeleteTeamAsync(team.Id);
            UserTeams.Remove(team);

            if (_session.CurrentTeam?.Id == team.Id)
                _session.CurrentTeam = null;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}