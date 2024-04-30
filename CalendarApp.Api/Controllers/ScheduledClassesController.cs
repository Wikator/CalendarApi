using AutoMapper;
using CalendarApp.Api.Services.Contracts;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Dtos.Responses.ScheduledClass;
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
        
       return Ok(await unitOfWork.ScheduledClassRepository.GetAllAsync<ScheduledClassDetailsDto>(userId,
           s => s.StartTime >= startTime && s.StartTime <= endTime));
    }
   
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, IClaimsProvider claimsProvider)
    {
       var userId = claimsProvider.GetUserIdOrDefault(User);
       var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync<ScheduledClassDetailsDto>(id, userId);
       return scheduledClass is null ? NotFound() : Ok(scheduledClass);
    }
   
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Create(UpsertScheduledClassDto upsertScheduledClassDto,
        IMapper mapper)
    {
        if (!await unitOfWork.SubjectRepository.ExistsAsync(s => s.Id == upsertScheduledClassDto.SubjectId))
        {
            ModelState.AddModelError("SubjectId", "Subject does not exist");
            return UnprocessableEntity(ModelState);
        }
        
        var scheduledClass = mapper.Map<ScheduledClass>(upsertScheduledClassDto);
        unitOfWork.ScheduledClassRepository.Add(scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
           return UnprocessableEntity("Failed to create scheduled class.");

        var scheduledClassDto = mapper.Map<ScheduledClassDto>(scheduledClass);
        return CreatedAtAction(nameof(GetById), new { id = scheduledClassDto.Id}, scheduledClassDto);
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Update(int id, UpsertScheduledClassDto upsertScheduledClassDto,
        IClaimsProvider claimsProvider, IMapper mapper)
    {
        var scheduledClass = await unitOfWork.ScheduledClassRepository.GetByIdAsync(id,
            claimsProvider.GetUserId(User));

        if (scheduledClass is null)
            return NotFound();

        if (scheduledClass.SubjectId != upsertScheduledClassDto.SubjectId &&
            !await unitOfWork.SubjectRepository.ExistsAsync(s => s.Id == upsertScheduledClassDto.SubjectId))
        {
            ModelState.AddModelError("SubjectId", "Subject does not exist");
            return UnprocessableEntity(ModelState);
        }

        mapper.Map(upsertScheduledClassDto, scheduledClass);
        unitOfWork.ScheduledClassRepository.Update(scheduledClass);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity("Failed to update scheduled class.");

        var scheduledClassDto = mapper.Map<ScheduledClassDto>(scheduledClass);
        
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
