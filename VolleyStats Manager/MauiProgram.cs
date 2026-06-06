using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VolleyStats_Manager.Models.Services;
using VolleyStats_Manager.ViewModels;
using VolleyStats_Manager.Views;
using VolleyStatsManager.DAL;

namespace VolleyStats_Manager;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Montserrat-Regular.ttf", "Montserrat");
                fonts.AddFont("Montserrat-SemiBold.ttf", "MontserratSemiBold");
                fonts.AddFont("Montserrat-Bold.ttf", "MontserratBold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // database registration
        builder.Services.AddDbContext<VolleyDbContext>(options =>
            options.UseSqlite("Data Source=volleystats.db"));

        builder.Services.AddTransient<MatchService>();
        builder.Services.AddTransient<AuthService>();
        builder.Services.AddTransient<PlayerService>();
        builder.Services.AddTransient<PlayerStatsService>();
        builder.Services.AddSingleton<AppSessionService>();
        builder.Services.AddTransient<TeamService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<AddMatchViewModel>();
        builder.Services.AddTransient<MatchesViewModel>();
        builder.Services.AddTransient<MatchDetailsViewModel>();
        builder.Services.AddTransient<UpdateMatchViewModel>();
        builder.Services.AddTransient<PlayersViewModel>();
        builder.Services.AddTransient<AddPlayerViewModel>();
        builder.Services.AddTransient<PlayerDetailsViewModel>();
        builder.Services.AddTransient<AddPlayerStatsViewModel>();
        builder.Services.AddTransient<SelectTeamViewModel>();
        builder.Services.AddTransient<AddTeamViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<TeamStatisticsViewModel>();
        builder.Services.AddTransient<UpdatePlayerViewModel>();
        builder.Services.AddTransient<ImportExportViewModel>();
        builder.Services.AddTransient<UpdatePlayerStatsViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<AddMatchPage>();
        builder.Services.AddTransient<MatchesPage>();
        builder.Services.AddTransient<MatchDetailsPage>();
        builder.Services.AddTransient<UpdateMatchPage>();
        builder.Services.AddTransient<PlayersPage>();
        builder.Services.AddTransient<AddPlayerPage>();
        builder.Services.AddTransient<PlayerDetailsPage>();
        builder.Services.AddTransient<AddPlayerStatsPage>();
        builder.Services.AddTransient<SelectTeamPage>();
        builder.Services.AddTransient<AddTeamPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<TeamStatisticsPage>();
        builder.Services.AddTransient<UpdatePlayerPage>();
        builder.Services.AddTransient<ImportExportPage>(); 
        builder.Services.AddTransient<UpdatePlayerStatsPage>();

        return builder.Build();
    }
}