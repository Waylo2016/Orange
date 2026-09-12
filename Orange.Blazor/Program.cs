using MudBlazor.Services;


namespace Orange.Blazor;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        
        
        builder.AddServiceDefaults();
        
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        builder.Services.AddMudServices();
        

        builder.Services.AddHttpClient<OrangeQuestionApiClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://orange-api");
        });
        
        var app = builder.Build();
        
        app.UseStaticFiles();
        app.UseAntiforgery();
        
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        
        app.Run();
    }
}
