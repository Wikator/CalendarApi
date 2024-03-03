using AutoMapper;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CalendarApp.Api.Endpoints;

public static class ScheduledClassEndpoints
{
    public static void MapScheduledClassEndpoints(this WebApplication app)
    {
        var scheduledClassApi = app.MapGroup("api/scheduled-classes");

        scheduledClassApi.MapGet("/", GetAll);
        scheduledClassApi.MapGet("{id}", GetById);
        scheduledClassApi.MapPost("/", Create)
            .RequireAuthorization(new AuthorizeAttribute { Policy = "RequireAdminRole" });
        scheduledClassApi.MapPut("{id}", Update)
            .RequireAuthorization(new AuthorizeAttribute { Policy = "RequireAdminRole" });
        scheduledClassApi.MapDelete("{id}", Delete)
            .RequireAuthorization(new AuthorizeAttribute { Policy = "RequireAdminRole" });
    }

    public static async Task<Ok<IEnumerable<ScheduledClassDto>>> GetAll(IUnitOfWork unitOfWork)
    {
        return TypedResults.Ok(await unitOfWork.ScheduledClassRepository.GetAllAsync<ScheduledClassDto>());
    }

    public static async Task<Results<Ok<ScheduledClassDto>, NotFound>> GetById(IUnitOfWork unitOfWork, uint id)
    {
        var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync<ScheduledClassDto>(id);
        return scheduledClass is null ? TypedResults.NotFound() : TypedResults.Ok(scheduledClass);
    }

    public static async Task<Results<Created<ScheduledClassDto>, BadRequest<string>>> Create(IUnitOfWork unitOfWork,
        UpsertScheduledClassDto upsertScheduledClassDto, IMapper mapper)
    {
        var scheduledClass = mapper.Map<ScheduledClass>(upsertScheduledClassDto);
        unitOfWork.ScheduledClassRepository.Add(scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to create scheduled class.");

        var scheduledClassDto = mapper.Map<ScheduledClassDto>(scheduledClass);
        return TypedResults.Created($"api/scheduled-classes/{scheduledClassDto.Id}", scheduledClassDto);
    }

    public static async Task<Results<Ok<ScheduledClassDto>, BadRequest<string>, NotFound>> Update(uint id,
        IUnitOfWork unitOfWork, UpsertScheduledClassDto upsertScheduledClassDto, IMapper mapper)
    {
        var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync(id);

        if (scheduledClass is null)
            return TypedResults.NotFound();

        mapper.Map(upsertScheduledClassDto, scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to update scheduled class.");

        var scheduledClassDto = mapper.Map<ScheduledClassDto>(scheduledClass);
        return TypedResults.Ok(scheduledClassDto);
    }

    public static async Task<Results<NoContent, BadRequest<string>, NotFound>> Delete(uint id, IUnitOfWork unitOfWork)
    {
        var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync(id);

        if (scheduledClass is null)
            return TypedResults.NotFound();

        unitOfWork.ScheduledClassRepository.Delete(scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to delete scheduled class.");

        return TypedResults.NoContent();
    }
}