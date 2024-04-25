using AutoMapper;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CalendarApp.Api.Endpoints;

public static class TestEndpoints
{
    public static void MapTestEndpoints(this WebApplication app)
    {
        var testApi = app.MapGroup("api/tests");

        testApi.MapGet("/", GetAll);
        testApi.MapGet("{id:int}", GetById);

        testApi.MapPost("/", Create)
            .RequireAuthorization(new AuthorizeAttribute { Policy = "RequireAdminRole" });
    }

    public static async Task<Ok<IEnumerable<TestDto>>> GetAll(IUnitOfWork unitOfWork) =>
        TypedResults.Ok(await unitOfWork.TestRepository.GetAllAsync<TestDto>());

    public static async Task<Results<Ok<TestDto>, NotFound>> GetById(IUnitOfWork unitOfWork, int id)
    {
        var result = await unitOfWork.TestRepository.GetByIdAsync<TestDto>(id);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static async Task<Results<Created<TestDto>, UnprocessableEntity<ErrorMessage>>> Create(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UpsertTestDto upsertTestDto)
    {
        var subjectDto = await unitOfWork.SubjectRepository.GetByIdAsync<SubjectDto>(upsertTestDto.SubjectId);

        if (subjectDto is null)
            return TypedResults.UnprocessableEntity(new ErrorMessage("Invalid subject id"));

        var test = mapper.Map<Test>(upsertTestDto);
        unitOfWork.TestRepository.Add(test);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.UnprocessableEntity(new ErrorMessage("Failed to save test"));

        var testDto = mapper.Map<TestDto>(test);
        testDto.Subject = subjectDto;
        return TypedResults.Created($"api/tests/{test.Id}", testDto);
    }
}