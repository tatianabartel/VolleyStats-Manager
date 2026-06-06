using System.Windows.Input;
using VolleyStats_Manager.Models.Services;
using VolleyStats_Manager.Views;

namespace VolleyStats_Manager
{
    public partial class AppShell : Shell
    {
        public ICommand LogoutCommand { get; }

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;

            Routing.RegisterRoute(nameof(AddMatchPage), typeof(AddMatchPage));
            Routing.RegisterRoute(nameof(MatchDetailsPage), typeof(MatchDetailsPage));
            Routing.RegisterRoute(nameof(UpdateMatchPage), typeof(UpdateMatchPage));
            Routing.RegisterRoute(nameof(AddPlayerPage), typeof(AddPlayerPage));
            Routing.RegisterRoute(nameof(PlayerDetailsPage), typeof(PlayerDetailsPage));
            Routing.RegisterRoute(nameof(AddPlayerStatsPage), typeof(AddPlayerStatsPage));
            Routing.RegisterRoute(nameof(AddTeamPage), typeof(AddTeamPage));
            Routing.RegisterRoute(nameof(UpdatePlayerPage), typeof(UpdatePlayerPage));
            Routing.RegisterRoute(nameof(UpdatePlayerStatsPage), typeof(UpdatePlayerStatsPage));
            Routing.RegisterRoute(nameof(SelectTeamPage), typeof(SelectTeamPage));
        }
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout", "Are you sure?", "Yes", "No");
            if (!confirm) return;

            var session = Handler.MauiContext.Services.GetService<AppSessionService>();
            session?.ClearSession();

            Shell.Current.FlyoutIsPresented = false;

            await GoToAsync("//LoginPage");
        }

        private async void OnChangeTeamClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;
            await GoToAsync("//SelectTeamPage");
        }
    }
}
