using AutoMapper;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Dtos.Responses.Subject;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(IClaimsProvider claimsProvider)
    {
        var group = await GetCurrentUserGroupAsync(claimsProvider);
        var subjects = group switch
        {
            null => await unitOfWork.SubjectRepository.GetAllAsync<SubjectDetailsDto>(),
            _ => await unitOfWork.SubjectRepository.GetAllAsync<SubjectDetailsDto>(group.Value)
        };
        
        return Ok(subjects);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, IClaimsProvider claimsProvider)
    {
        var group = await GetCurrentUserGroupAsync(claimsProvider);
        var subject = group switch
        {
            null => await unitOfWork.SubjectRepository.GetByIdAsync<SubjectDetailsDto>(id),
            _ => await unitOfWork.SubjectRepository.GetByIdAsync<SubjectDetailsDto>(id, group.Value)
        };
        
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
        return CreatedAtAction(nameof(GetById), new { id = subjectDto.Id }, subjectDto);
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
    
    private async Task<int?> GetCurrentUserGroupAsync(IClaimsProvider claimsProvider)
    {
        var userId = claimsProvider.GetUserIdOrDefault(User);
        return userId is null ? null : await unitOfWork.UserRepository.GetGroupByUserIdAsync(userId.Value);
    }
}
