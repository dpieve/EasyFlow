using EasyFocus.Api.Sessions;
using EasyFocus.Api.Tags;
using EasyFocus.Api.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyFocus.Api;

public sealed class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tag> Tags { get; set; }
    public DbSet<Session> Sessions { get; set; }
}