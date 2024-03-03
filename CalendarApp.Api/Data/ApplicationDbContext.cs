using CalendarApp.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
    public DbSet<Subject> Subjects { get; init; }
    public DbSet<ScheduledClass> ScheduledClasses { get; init; }
}