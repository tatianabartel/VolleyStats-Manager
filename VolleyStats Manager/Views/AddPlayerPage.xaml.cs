using System.Text.RegularExpressions;
using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class AddPlayerPage : ContentPage
{
    public AddPlayerPage(AddPlayerViewModel vm)
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