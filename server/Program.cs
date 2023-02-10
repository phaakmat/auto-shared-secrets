using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Linq;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var client = new SecretClient(vaultUri: new Uri(app.Configuration["KeyVault:Uri"]), credential: new DefaultAzureCredential());

var regex = new Regex("Clients--(.*?)--.*");
var clientSecrets = client.GetPropertiesOfSecrets()
    .Select(o => regex.Match(o.Name))
    .Where(o => o.Success)
    .Select(o => o.Groups[1].Value)
    .Distinct()
    .ToArray();

app.MapGet("/", () =>
{
    return clientSecrets.Select(clientName => new
    {
        Name = clientName,
        Config = client.GetSecret($"Clients--{clientName}--Config").Value,
        Secret = client.GetSecret($"Clients--{clientName}--Config").Value
    }).ToArray();
});

app.Run();
