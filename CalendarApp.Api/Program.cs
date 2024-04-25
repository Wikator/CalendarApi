using System.Text;
using CalendarApp.Api.Configuration;
using CalendarApp.Api.Services;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess;
using CalendarApp.DataAccess.Repository;
using CalendarApp.DataAccess.Repository.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState, 422);

            return new ObjectResult(problemDetails)
            {
                StatusCode = 422
            };
        }
    );

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlite("Data Source=calendar.db",
        x => x.MigrationsAssembly("CalendarApp.DataAccess")));

var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token key not found");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IClaimsProvider, ClaimsProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(cors =>
    cors.AllowAnyHeader()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:3000")
    );

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
if ((await db.Database.GetPendingMigrationsAsync()).Any()) await db.Database.MigrateAsync();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();