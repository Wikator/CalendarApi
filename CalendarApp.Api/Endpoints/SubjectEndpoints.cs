using AutoMapper;
using CalendarApp.Api.Data.Repository.Contracts;
using CalendarApp.Api.Dtos.Requests;
using CalendarApp.Api.Dtos.Responses;
using CalendarApp.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CalendarApp.Api.Endpoints;

public static class SubjectEndpoints
{
    public static void MapSubjectEndpoints(this WebApplication app)
    {
        var subjectApi = app.MapGroup("api/subjects");

        subjectApi.MapGet("/", GetAll);
        subjectApi.MapGet("{id}", GetById);
        subjectApi.MapPost("/", Create)
            .RequireAuthorization(new AuthorizeAttribute { Policy = "RequireAdminRole" });
        subjectApi.MapPut("{id}", Update)
            .RequireAuthorization(new AuthorizeAttribute { Policy = "RequireAdminRole" });
        subjectApi.MapDelete("{id}", Delete)
            .RequireAuthorization(new AuthorizeAttribute { Policy = "RequireAdminRole" });
    }

    public static async Task<Ok<IEnumerable<SubjectDto>>> GetAll(IUnitOfWork unitOfWork)
    {
        return TypedResults.Ok(await unitOfWork.SubjectRepository.GetAllAsync<SubjectDto>());
    }

    public static async Task<Results<Ok<SubjectDto>, NotFound>> GetById(IUnitOfWork unitOfWork, uint id)
    {
        var subject = await unitOfWork.SubjectRepository.GetByIdAsync<SubjectDto>(id);
        return subject is null ? TypedResults.NotFound() : TypedResults.Ok(subject);
    }

    public static async Task<Results<Created<SubjectDto>, BadRequest<string>>> Create(IUnitOfWork unitOfWork,
        UpsertSubjectDto upsertSubjectDto, IMapper mapper)
    {
        var subject = mapper.Map<Subject>(upsertSubjectDto);
        unitOfWork.SubjectRepository.Add(subject);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to create subject.");

        var subjectDto = mapper.Map<SubjectDto>(subject);
        return TypedResults.Created($"api/subjects/{subjectDto.Id}", subjectDto);
    }

    public static async Task<Results<Ok<SubjectDto>, BadRequest<string>, NotFound>> Update(uint id,
        IUnitOfWork unitOfWork, UpsertSubjectDto upsertSubjectDto, IMapper mapper)
    {
        var subject = await unitOfWork.SubjectRepository.GetByIdAsync(id);

        if (subject is null)
            return TypedResults.NotFound();

        mapper.Map(upsertSubjectDto, subject);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to update subject.");

        var subjectDto = mapper.Map<SubjectDto>(subject);
        return TypedResults.Ok(subjectDto);
    }

    public static async Task<Results<NoContent, BadRequest<string>, NotFound>> Delete(uint id, IUnitOfWork unitOfWork)
    {
        var subject = await unitOfWork.SubjectRepository.GetByIdAsync(id);

        if (subject is null)
            return TypedResults.NotFound();

        unitOfWork.SubjectRepository.Delete(subject);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to delete subject.");

        return TypedResults.NoContent();
    }
}