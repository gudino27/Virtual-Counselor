using VirtualCounselor.Components;
using VirtualCounselor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

initApp();

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

// TEMPORARY FOR DEGREE MANAGER TESTING
// this is probably where the calls to boot will be to centralbackend in the future
void initApp()
{
    DegreeManager.AddDegree("test degree 1");
    DegreeManager.AddDegree("test degree 2");
}
