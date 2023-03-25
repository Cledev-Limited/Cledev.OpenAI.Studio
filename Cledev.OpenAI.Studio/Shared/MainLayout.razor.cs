using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Cledev.OpenAI.Studio.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    private PageInfo? PageInfo { get; set; }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += LocationChanged!;
    }

    protected override void OnParametersSet()
    {
        RefreshPageInfo();
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= LocationChanged!;
    }

    private void LocationChanged(object sender, LocationChangedEventArgs args)
    {
        RefreshPageInfo();
    }

    private void RefreshPageInfo()
    {
        var pagePath = new Uri(NavigationManager.Uri).AbsolutePath;

        if (pagePath == "/fine-tuning")
        {
            PageInfo = new PageInfo("Fine-Tuning", "fa-solid fa-screwdriver-wrench");
        }
        else if (pagePath == "/")
        {
            PageInfo = null;
        }

        StateHasChanged();
    }
}

public class PageInfo
{
    public string HeaderText { get; set; }
    public string HeaderIcon { get; set; }

    public PageInfo(string headerText, string headerIcon)
    {
        HeaderText = headerText;
        HeaderIcon = headerIcon;
    }
}
