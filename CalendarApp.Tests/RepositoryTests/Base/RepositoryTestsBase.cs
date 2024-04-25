using AutoMapper;
using CalendarApp.Api.Configuration;
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
    
    protected static IMapper InitializeMapper()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<AutoMapperProfiles>());

        return mapperConfig.CreateMapper();
    }
}
