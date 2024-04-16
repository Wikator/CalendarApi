using AutoMapper;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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

    public static async Task<Ok<IEnumerable<ScheduledClassDto>>> GetAll([FromQuery] DateTime? startTime,
        [FromQuery] DateTime? endTime, IUnitOfWork unitOfWork, HttpContext httpContext, IClaimsProvider claimsProvider)
    {
        startTime ??= DateTime.MinValue;
        endTime ??= DateTime.MaxValue;
        
        return TypedResults.Ok(await unitOfWork.ScheduledClassRepository.GetAllAsync<ScheduledClassDto>(
            claimsProvider.GetUserIdOrDefault(httpContext.User), s => s.StartTime >= startTime && s.StartTime <= endTime));
    }

    public static async Task<Results<Ok<ScheduledClassDto>, NotFound>> GetById(IUnitOfWork unitOfWork,
        HttpContext httpContext, IClaimsProvider claimsProvider, uint id)
    {
        var userId = claimsProvider.GetUserIdOrDefault(httpContext.User);
        var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync<ScheduledClassDto>(id, userId);
        return scheduledClass is null ? TypedResults.NotFound() : TypedResults.Ok(scheduledClass);
    }

    public static async Task<Results<Created<ScheduledClassDto>, BadRequest<string>>> Create(IUnitOfWork unitOfWork,
        UpsertScheduledClassDto upsertScheduledClassDto, IMapper mapper)
    {
        var scheduledClass = mapper.Map<ScheduledClass>(upsertScheduledClassDto);

        var subjectDto = await unitOfWork.SubjectRepository.GetByIdAsync<SubjectDto>(upsertScheduledClassDto.SubjectId);

        if (subjectDto is null)
            return TypedResults.BadRequest("Invalid subject id");
        
        unitOfWork.ScheduledClassRepository.Add(scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to create scheduled class.");
        
        var scheduledClassDto = mapper.Map<ScheduledClassDto>(scheduledClass);
        scheduledClassDto.Subject = subjectDto;
        return TypedResults.Created($"api/scheduled-classes/{scheduledClassDto.Id}", scheduledClassDto);
    }

    public static async Task<Results<Ok<ScheduledClassDto>, BadRequest<string>, NotFound>> Update(uint id,
        IUnitOfWork unitOfWork, UpsertScheduledClassDto upsertScheduledClassDto, IMapper mapper,
        HttpContext httpContext, IClaimsProvider claimsProvider)
    {
        var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync(id,
            claimsProvider.GetUserId(httpContext.User));

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