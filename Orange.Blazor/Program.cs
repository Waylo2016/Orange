using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;


namespace Orange.Blazor;

public class Program
{
    public static async Task Main(string[] args)
    {

        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        
        var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") 
                         ?? throw new InvalidOperationException("API base URL is not configured.");
        builder.Services.AddHttpClient<OrangeQuestionApiClient>("OrangeApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        builder.Services.AddMudServices();
        
        
        var app = builder.Build();



        await app.RunAsync();
    }
}
