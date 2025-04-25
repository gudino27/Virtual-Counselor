using VirtualCounselor.Components;
using BlazorApp1.Services;
using VirtualCounselor;
using Microsoft.Win32;
using VirtualCounselor.Backend;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<CourseDataBackgroundService>();
builder.Services.AddSingleton<CourseService>();

// Degree‐scrape services
builder.Services.AddHostedService<DegreeDataBackgroundService>();
builder.Services.AddSingleton<DegreeService>();

// Register the DegreeService so it’s available for smart‐search
builder.Services.AddSingleton<DegreeService>();

builder.Services.AddSingleton<CourseManager>();
builder.Services.AddSingleton<SmartSearch>();

// ─── 1) Seed CourseManager with a few mock courses ─────────────
builder.Services.AddSingleton<CourseManager>(sp =>
{
    var mgr = new CourseManager();
    mgr.AddCourse(new Course { CourseCode = "CPT S 101", Title = "Intro to CS", Credits = 1 });
    mgr.AddCourse(new Course { CourseCode = "CPT S 121", Title = "Program Design", Credits = 4 });
    mgr.AddCourse(new Course { CourseCode = "ENGLISH 101", Title = "College Composition", Credits = 3 });
    mgr.AddCourse(new Course { CourseCode = "MATH 171", Title = "Calculus I", Credits = 4 });
    return mgr;
});

// ─── 2) Register SmartSearch so it wraps that manager ────────────
builder.Services.AddSingleton<SmartSearch>();

// ─── 3) You can leave your real background service in place too ──
builder.Services.AddHostedService<CourseDataBackgroundService>();
builder.Services.AddSingleton<CourseService>();

// ─── 4) No DegreeService for now; we’re using SmartSearch only ──
// builder.Services.AddSingleton<DegreeService>();

builder.Services.AddHttpClient();

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

// SCRAPE AND CACHE THE REAL DEGREES
// 1) Trigger your static scraper to fill its in-memory list
DegreeScrape.scrapeall();

// 2) Grab the IMemoryCache and store the scraped list under "DegreeData"
using (var scope = app.Services.CreateScope())
{
    var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
    cache.Set(
        key: "DegreeData",
        value: DegreeScrape.degreeList   // the static List<Degree> your scraper populated
    );
}

// TEMPORARY FOR DEGREE MANAGER TESTING
// this is probably where the calls to boot will be to centralbackend in the future
void initApp()
{
    DegreeManager.AddDegree("test degree 1");
    DegreeManager.AddDegree("test degree 2");

    //// SEED TEST DEGREES
    var cs = DegreeManager.AddDegree("Computer Science");
    cs.AddRequiredCourse(new Course("CPT S 121", 0, "Program Design and Development", string.Empty, 4, null));
    cs.AddRequiredCourse(new Course("CPT S 122", 0, "Data Structures", string.Empty, 4, null));

    var se = DegreeManager.AddDegree("Software Engineering");
    se.AddRequiredCourse(new Course("CPT S 321", 0, "Software Design", string.Empty, 3, null));
    se.AddRequiredCourse(new Course("CPT S 322", 0, "Software Engineering Principles", string.Empty, 3, null));
}
