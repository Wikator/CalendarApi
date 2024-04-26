using AutoMapper;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class NotesController(
    IUnitOfWork unitOfWork,
    IClaimsProvider claimsProvider) : ControllerBase
{
    [HttpGet("scheduled-classes/{id:int}/notes")]
    public async Task<IActionResult> GetAll(int id)
    {
        var userId = claimsProvider.GetUserId(User);
        return Ok(await unitOfWork.NoteRepository.GetAllAsync<NoteDto>(userId,
            n => n.ScheduledClassId == id));
    }
    
    [HttpGet("notes")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = claimsProvider.GetUserId(User);
        var note = await unitOfWork.NoteRepository.GetByIdAsync<NoteDto>(userId, id);
        
        return note is null ? NotFound() : Ok(note);
    }
    
    [HttpPost("scheduled-classes/{id:int}/notes")]
    public async Task<IActionResult> Create(int id, UpsertNoteDto upsertNoteDto, IMapper mapper)
    {
        if (!await unitOfWork.ScheduledClassRepository.ExistsAsync(s => s.Id == id))
            return NotFound();

        var userId = claimsProvider.GetUserId(User);
        var note = mapper.Map<Note>(upsertNoteDto);
        note.ScheduledClassId = id;
        unitOfWork.NoteRepository.Add(note, userId);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to create note."));

        var noteDto = mapper.Map<NoteDto>(note);
        return CreatedAtAction(nameof(GetById), new { id = noteDto.Id }, noteDto);
    }
    
    [HttpPut("notes/{id:int}")]
    public async Task<IActionResult> Update(int id, UpsertNoteDto upsertNoteDto, IMapper mapper)
    {
        var note = await unitOfWork.NoteRepository.GetByIdAsync(id,
            claimsProvider.GetUserId(User));

        if (note is null)
            return NotFound();

        mapper.Map(upsertNoteDto, note);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to update note."));

        var noteDto = mapper.Map<NoteDto>(note);
        return Ok(noteDto);
    }
    
    [HttpDelete("notes/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await unitOfWork.NoteRepository.DeleteByIdAsync(id, claimsProvider.GetUserId(User)))
            return NotFound();
            
        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to delete note."));

        return NoContent();
    }
}
