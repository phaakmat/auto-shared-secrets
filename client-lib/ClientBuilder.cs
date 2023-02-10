using System.Text.Json;
using Configuration;
namespace client_lib;
public static class ClientBuilder
{
    public static WebApplication CreateClient(this WebApplicationBuilder builder)
    {
        builder.Host.ConfigureAppConfiguration((context, config) =>
        {
            context.HostingEnvironment.EnvironmentName = "Development";
            config.AddConfiguration(context.HostingEnvironment);
        });
        var app = builder.Build();

        var name = app.Configuration["ClientName"];
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseRouting();

        var config = app.Configuration[$"Clients:{name}:Config"];

        app.MapGet("/", () => new
        {
            Config = JsonSerializer.Deserialize<ClientProperties>(config),
            Secret = app.Configuration[$"Clients:{name}:Secret"]
        });

        return app;
    }

    public class ClientProperties
    {
        public string[]? AllowedGrantTypes { get; set; }
        public string[]? AllowedScopes { get; set; }
        public string? ClientId { get; set; }
        public string[]? RedirectUris { get; set; }
    }

}
