using CalendarApp.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Tests.RepositoryTests.Base;

public abstract class RepositoryTestsBase
{
    protected ApplicationDbContext Context { get; }

    protected RepositoryTestsBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }
}
