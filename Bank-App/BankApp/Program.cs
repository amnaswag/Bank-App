using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BankApp;
using BankApp.Interfaces;
using BankApp.Services;

/// <summary>
/// The main entry point of the Blazor WebAssembly application.
/// </summary>
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IStorageService, StorageService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IAccountService, AccountService>();

/// <summary>
/// Builds and runs the application asynchronously.
/// </summary>
await builder.Build().RunAsync();