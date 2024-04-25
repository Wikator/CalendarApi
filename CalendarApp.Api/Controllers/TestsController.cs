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
public class TestsController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await unitOfWork.TestRepository.GetAllAsync<TestDto>());
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await unitOfWork.TestRepository.GetByIdAsync<TestDto>(id);
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Create(UpsertTestDto upsertTestDto, IMapper mapper)
    {
        var subjectDto = await unitOfWork.SubjectRepository.GetByIdAsync<SubjectDto>(upsertTestDto.SubjectId!.Value);

        if (subjectDto is null)
            return UnprocessableEntity(new ErrorMessage("Invalid subject id"));

        var test = mapper.Map<Test>(upsertTestDto);
        unitOfWork.TestRepository.Add(test);

        if (!await unitOfWork.SaveChangesAsync())
            return UnprocessableEntity(new ErrorMessage("Failed to save test"));

        var testDto = mapper.Map<TestDto>(test);
        testDto.Subject = subjectDto;
        return CreatedAtAction(nameof(GetById), new { id = testDto.Id }, testDto);
    }
}
