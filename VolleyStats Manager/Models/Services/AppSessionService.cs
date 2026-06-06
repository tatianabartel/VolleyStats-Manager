using VolleyStats_Manager.Models.DTO;

namespace VolleyStats_Manager.Models.Services
{
    public class AppSessionService
    {
        public UserDTO? CurrentUser { get; set; }
        public TeamDTO? CurrentTeam { get; set; }
        public int? CurrentTeamId => CurrentTeam?.Id;

        public bool IsLoggedIn => CurrentUser != null;
        public bool HasTeamSelected => CurrentTeam != null;

        public void ClearSession()
        {
            CurrentUser = null;
            CurrentTeam = null;
        }
    }
}