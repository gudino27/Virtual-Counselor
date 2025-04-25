using BlazorApp1.Components;
using BlazorApp1.Components.Pages;
using BlazorApp1.Services;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
var builder = WebApplication.CreateBuilder(args);

var keysFolder = "/app/keys";
Directory.CreateDirectory(keysFolder);

builder.Services.AddDataProtection()
       .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
       .SetApplicationName("BlazorApp1");

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<CourseDataBackgroundService>();
builder.Services.AddSingleton<CourseService>();
builder.Services.AddSingleton<DegreeService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddScoped<Transcript.StudentRecord>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

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
