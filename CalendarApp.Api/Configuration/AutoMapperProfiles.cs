using AutoMapper;
using CalendarApp.Models.Dtos.Requests;
using CalendarApp.Models.Dtos.Responses.Note;
using CalendarApp.Models.Dtos.Responses.ScheduledClass;
using CalendarApp.Models.Dtos.Responses.Subject;
using CalendarApp.Models.Dtos.Responses.Test;
using CalendarApp.Models.Dtos.Responses.User;
using CalendarApp.Models.Entities;

namespace CalendarApp.Api.Configuration;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<User, UserDto>();
        CreateMap<UpsertSubjectDto, Subject>();
        
        CreateMap<Subject, SubjectDto>();
        CreateMap<Subject, SubjectDetailsDto>();
        
        CreateMap<ScheduledClass, ScheduledClassDto>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EndTime - src.StartTime));
        CreateMap<ScheduledClass, ScheduledClassDetailsDto>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EndTime - src.StartTime));

        CreateMap<UpsertScheduledClassDto, ScheduledClass>();
        
        CreateMap<UpsertNoteDto, Note>()
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => DateTime.Now));
        CreateMap<Note, NoteDto>();

        CreateMap<Test, TestDetailsDto>();
        CreateMap<Test, TestWithoutSubjectDto>();
        CreateMap<Test, TestDto>();
        CreateMap<CreateTestDto, Test>();
        CreateMap<UpdateTestDto, Test>();
    }
}