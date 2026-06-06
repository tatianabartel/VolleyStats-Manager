using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class MatchDetailsPage : ContentPage
{
    public MatchDetailsPage(MatchDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}