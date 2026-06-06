using VolleyStats_Manager.ViewModels;

namespace VolleyStats_Manager.Views;

public partial class ImportExportPage : ContentPage
{
    public ImportExportPage(ImportExportViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}