using AutoMapper;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CalendarApp.Api.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this WebApplication app)
    {
        var noteApi = app.MapGroup("api/notes");
        var scheduledClassApi = app.MapGroup("api/scheduled-classes/{id}/notes");

        scheduledClassApi.MapGet("/", GetAll)
            .RequireAuthorization(new AuthorizeAttribute());
        noteApi.MapGet("{id}", GetById);
        scheduledClassApi.MapPost("", Create);
        noteApi.MapPut("{id}", Update);
        noteApi.MapDelete("{id}", Delete);
    }
    
    public static async Task<Ok<IEnumerable<NoteDto>>> GetAll(IUnitOfWork unitOfWork,
        HttpContext httpContext, IClaimsProvider claimsProvider, uint id)
    {
        var userId = claimsProvider.GetUserId(httpContext.User);
        return TypedResults.Ok(await unitOfWork.NoteRepository.GetAllAsync<NoteDto>(userId,
            n => n.ScheduledClassId == id));
    }

    public static async Task<Results<Ok<NoteDto>, NotFound>> GetById(IUnitOfWork unitOfWork,
        HttpContext httpContext, IClaimsProvider claimsProvider, uint id)
    {
        var userId = claimsProvider.GetUserId(httpContext.User);
        var note = await unitOfWork.NoteRepository.GetByIdAsync<NoteDto>(userId, id);
        
        return note is null ? TypedResults.NotFound() : TypedResults.Ok(note);
    }

    public static async Task<Results<Created<NoteDto>, BadRequest<string>>> Create(IUnitOfWork unitOfWork,
        UpsertNoteDto upsertNoteDto, IMapper mapper, HttpContext httpContext, IClaimsProvider claimsProvider,
        uint id)
    {
        var userId = claimsProvider.GetUserId(httpContext.User);
        var note = mapper.Map<Note>(upsertNoteDto);
        note.UserId = userId;
        note.ScheduledClassId = id;
        unitOfWork.NoteRepository.Add(note);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to create note.");

        var scheduledClassDto = mapper.Map<NoteDto>(note);
        return TypedResults.Created($"api/notes/{scheduledClassDto.Id}", scheduledClassDto);
    }

    public static async Task<Results<Ok<NoteDto>, BadRequest<string>, NotFound>> Update(uint id,
        IUnitOfWork unitOfWork, UpsertNoteDto upsertNoteDto, IMapper mapper,
        HttpContext httpContext, IClaimsProvider claimsProvider)
    {
        var note = await unitOfWork.NoteRepository.GetByIdAsync(id,
            claimsProvider.GetUserId(httpContext.User));

        if (note is null)
            return TypedResults.NotFound();

        mapper.Map(upsertNoteDto, note);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to update note.");

        var noteDto = mapper.Map<NoteDto>(note);
        return TypedResults.Ok(noteDto);
    }

    public static async Task<Results<NoContent, BadRequest<string>, NotFound>> Delete(uint id, IUnitOfWork unitOfWork,
        HttpContext httpContext, IClaimsProvider claimsProvider)
    {
        var note = await unitOfWork.NoteRepository.GetByIdAsync(id,
            claimsProvider.GetUserId(httpContext.User));

        if (note is null)
            return TypedResults.NotFound();

        unitOfWork.NoteRepository.Delete(note);

        if (!await unitOfWork.SaveChangesAsync())
            return TypedResults.BadRequest("Failed to delete note.");

        return TypedResults.NoContent();
    }
}