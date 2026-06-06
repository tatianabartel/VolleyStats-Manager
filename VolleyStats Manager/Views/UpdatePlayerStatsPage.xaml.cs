using System.Text.RegularExpressions;
using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

[QueryProperty(nameof(StatsId), "StatsId")]
public partial class UpdatePlayerStatsPage : ContentPage
{
    public int StatsId
    {
        set => _ = ((UpdatePlayerStatsViewModel)BindingContext).LoadStatsById(value);
    }

    public UpdatePlayerStatsPage(UpdatePlayerStatsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private static void OnIntegerInput(object sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) return;
        var filtered = Regex.Replace(e.NewTextValue ?? string.Empty, @"[^\d]", "");
        if (filtered != e.NewTextValue)
            entry.Text = filtered;
    }
}