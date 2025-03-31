using EasyFocus.Api;
using EasyFocus.Api.Data;
using EasyFocus.Api.Users;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();
bld.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=EasyFocus.db"));
bld.Services.AddIdentityApiEndpoints<AppUser>(opt =>
{
    opt.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

bld.Services.AddAuthorization();

var app = bld.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("api").MapIdentityApi<AppUser>();

app.UseFastEndpoints(c => c.Endpoints.RoutePrefix = "api");

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context, userManager);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();