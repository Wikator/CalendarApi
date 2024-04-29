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
[Route("api/Scheduled-Classes")]
public class ScheduledClassesController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? startTime,
       [FromQuery] DateTime? endTime, IClaimsProvider claimsProvider)
    {
       startTime ??= DateTime.MinValue;
       endTime ??= DateTime.MaxValue;

       var userId = claimsProvider.GetUserIdOrDefault(User);
        
       return Ok(await unitOfWork.ScheduledClassRepository.GetAllAsync<ScheduledClassDto>(userId,
           s => s.StartTime >= startTime && s.StartTime <= endTime));
    }
   
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, IClaimsProvider claimsProvider)
    {
       var userId = claimsProvider.GetUserIdOrDefault(User);
       var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync<ScheduledClassDto>(id, userId);
       return scheduledClass is null ? NotFound() : Ok(scheduledClass);
    }
   
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Create(UpsertScheduledClassDto upsertScheduledClassDto,
        IMapper mapper)
    {
        var scheduledClass = mapper.Map<ScheduledClass>(upsertScheduledClassDto);

        var subjectDto = await unitOfWork.SubjectRepository.GetByIdAsync<SubjectDto>(
           upsertScheduledClassDto.SubjectId!.Value);

        if (subjectDto is null)
           return UnprocessableEntity(new ErrorMessage("Invalid subject id"));

        unitOfWork.ScheduledClassRepository.Add(scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
           return UnprocessableEntity("Failed to create scheduled class.");

        var scheduledClassDto = mapper.Map<ScheduledClassDto>(scheduledClass);
        scheduledClassDto.Subject = subjectDto;
        return CreatedAtAction(nameof(GetById), new { id = scheduledClassDto.Id}, scheduledClassDto);
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequiredAdminRole")]
    public async Task<IActionResult> Update(int id, UpsertScheduledClassDto upsertScheduledClassDto,
        IClaimsProvider claimsProvider, IMapper mapper)
    {
        var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync(id,
            claimsProvider.GetUserId(User));

        if (scheduledClass is null)
            return NotFound();

        SubjectDto? subjectDto = null;
        if (scheduledClass.SubjectId != upsertScheduledClassDto.SubjectId!.Value)
        {
            subjectDto = await unitOfWork.SubjectRepository
                .GetByIdAsync<SubjectDto>(upsertScheduledClassDto.SubjectId!.Value);

            if (subjectDto is null)
            {
                return UnprocessableEntity("Invalid subject id");
            }
        }

        mapper.Map(upsertScheduledClassDto, scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity("Failed to update scheduled class.");

        var scheduledClassDto = mapper.Map<ScheduledClassDto>(scheduledClass);

        if (subjectDto is not null)
            scheduledClassDto.Subject = subjectDto;
        
        return Ok(scheduledClassDto);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Delete(int id)
    {
        var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync(id);

        if (scheduledClass is null)
            return NotFound();

        unitOfWork.ScheduledClassRepository.Delete(scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity("Failed to delete scheduled class.");

        return NoContent();
    }
}
