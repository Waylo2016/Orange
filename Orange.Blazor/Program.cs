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
        
        var apiBaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}_api/orangeapi/");

        builder.Services.AddHttpClient<OrangeApiClient>(client =>
        {
            client.BaseAddress = apiBaseAddress;
        });
        builder.Services.AddMudServices();
        
        
        var app = builder.Build();



        await app.RunAsync();
    }
}
