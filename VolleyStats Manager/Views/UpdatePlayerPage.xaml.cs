using System.Text.RegularExpressions;
using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

[QueryProperty(nameof(PlayerId), "PlayerId")]
public partial class UpdatePlayerPage : ContentPage
{
    public int PlayerId
    {
        set => _ = ((UpdatePlayerViewModel)BindingContext).LoadPlayerById(value);
    }

    public UpdatePlayerPage(UpdatePlayerViewModel vm)
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