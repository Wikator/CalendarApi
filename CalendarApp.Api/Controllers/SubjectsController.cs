using AutoMapper;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await unitOfWork.SubjectRepository.GetAllAsync<SubjectDto>());
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var subject = await unitOfWork.SubjectRepository.GetByIdAsync<SubjectDto>(id);
        return subject is null ? NotFound() : Ok(subject);
    }
    
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Create(UpsertSubjectDto upsertSubjectDto, IMapper mapper)
    {
        var subject = mapper.Map<Subject>(upsertSubjectDto);
        unitOfWork.SubjectRepository.Add(subject);

        if (!await unitOfWork.SaveChangesAsync())
            return BadRequest(new ErrorMessage("Failed to create subject."));

        var subjectDto = mapper.Map<SubjectDto>(subject);
        return CreatedAtAction(nameof(GetById), new { id = subjectDto.Id}, subjectDto);
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Update(int id, UpsertSubjectDto upsertSubjectDto, IMapper mapper)
    {
        var subject = await unitOfWork.SubjectRepository.GetByIdAsync(id);

        if (subject is null)
            return NotFound();

        mapper.Map(upsertSubjectDto, subject);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to update subject."));

        var subjectDto = mapper.Map<SubjectDto>(subject);
        return Ok(subjectDto);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Delete(int id)
    {
        var subject = await unitOfWork.SubjectRepository.GetByIdAsync(id);

        if (subject is null)
            return NotFound();

        unitOfWork.SubjectRepository.Delete(subject);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity("Failed to delete subject.");

        return NoContent();
    }
}
