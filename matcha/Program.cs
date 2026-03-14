using matcha.Components;
using Microsoft.Data.SqlClient;
using Radzen;
using Matcha.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();
builder.Services.AddRadzenComponents();

builder.Services.AddScoped(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("SqlConnection") 
                           ?? throw new InvalidOperationException("Connection string 'SqlConnection' not found.");
    return new SqlConnection(connectionString);
});

builder.Services.AddScoped<Matcha.Controllers.ICitasService, Matcha.Controllers.CitasController>();
builder.Services.AddScoped<UsuariosController>();
builder.Services.AddScoped<EmpleadosController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
