using AutoMapper;
using CalendarApp.DataAccess.Repository.Contracts;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses;
using CalendarApp.Models.Dtos.Responses.Test;
using CalendarApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await unitOfWork.TestRepository.GetAllAsync<TestDetailsDto>());
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await unitOfWork.TestRepository.GetByIdAsync<TestDetailsDto>(id);
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Create(CreateTestDto createTestDto, IMapper mapper)
    {
        if (!await unitOfWork.SubjectRepository.ExistsAsync(t => t.Id == createTestDto.SubjectId))
        {
            ModelState.AddModelError("SubjectId", "Subject does not exist");
            return UnprocessableEntity(ModelState);
        }

        var test = mapper.Map<Test>(createTestDto);
        unitOfWork.TestRepository.Add(test);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to save test"));

        var testDto = mapper.Map<TestDto>(test);
        return CreatedAtAction(nameof(GetById), new { id = testDto.Id }, testDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Update(int id, UpdateTestDto updateTestDto, IMapper mapper)
    {
        var test = await unitOfWork.TestRepository.GetByIdAsync(id, t => t.Subject);

        if (test is null)
            return NotFound();

        mapper.Map(updateTestDto, test);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to update test"));

        return Ok(mapper.Map<TestDto>(test));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Delete(int id)
    {
        var test = await unitOfWork.TestRepository.GetByIdAsync(id);

        if (test is null)
            return NotFound();
        
        unitOfWork.TestRepository.Delete(test);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to delete test"));

        return NoContent();
    }
}
