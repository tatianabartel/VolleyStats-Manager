using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VolleyStatsManager.DAL;

namespace VolleyStats_Manager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var dbContext = MauiProgram.CreateMauiApp().Services.GetRequiredService<VolleyDbContext>();
            dbContext.Database.Migrate();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}