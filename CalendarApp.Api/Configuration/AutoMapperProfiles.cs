using AutoMapper;
using CalendarApp.Api.Dtos.Requests;
using CalendarApp.Api.Dtos.Responses;
using CalendarApp.Api.Entities;

namespace CalendarApp.Api.Configuration;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<User, UserDto>();
        CreateMap<UpsertSubjectDto, Subject>();
        CreateMap<Subject, SubjectDto>();
        CreateMap<ScheduledClass, ScheduledClassDto>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EndTime - src.StartTime));

        CreateMap<UpsertScheduledClassDto, ScheduledClass>();
    }
}