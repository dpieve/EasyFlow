using EasyFlow.Api.Sessions;
using EasyFlow.Api.Tags;
using EasyFlow.Api.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Api;

public sealed class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tag> Tags { get; set; }
    public DbSet<Session> Sessions { get; set; }
}