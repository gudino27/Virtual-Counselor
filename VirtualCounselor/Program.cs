using VirtualCounselor;
using VirtualCounselor.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register CentralBackend and related services
builder.Services.AddSingleton<CentralBackend>();

var app = builder.Build();
var webScraper = new WebScraper();

// Retrieve the backend from the service container. Load mock data after backend is initialized
var backend = app.Services.GetRequiredService<CentralBackend>();

// Initialize the system and load mock data
backend.InitializeSystem();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

