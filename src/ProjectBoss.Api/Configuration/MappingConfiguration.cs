using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Core.Entities;
using ProjectBoss.Domain.Entities;
using ProjectBoss.Domain.Extensions;

namespace ProjectBoss.Api.Configuration
{
    public static class MappingConfiguration
    {
        public static IServiceCollection AddMapperConfiguration(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);

            return services;
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Person Mappings
            CreateMap<CreatePersonDto, Person>();

            CreateMap<EditPersonDataDto, Person>()
                .ForMember(dest => dest.PersonCode, opt => opt.Ignore());

            CreateMap<Person, PersonBasicDto>();

            CreateMap<Person, PersonDataDto>();

            CreateMap<Person, PersonFullDto>()
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));
            CreateMap<PersonFullDto, Person>();

            CreateMap<PersonFullDto, PersonBasicDto>();

            CreateMap<PersonFullDto, EditPersonDataDto>();

            CreateMap<CreatePersonDto, PersonBasicDto>();

            CreateMap<PersonBasicDto, Person>();
            #endregion

            #region User
            CreateMap<UserRegisterDto, CreatePersonDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<ExternalUserDto, CreatePersonDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<IdentityRole, UserRoleDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<UserViewDto, UserDataDto>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserDataDto, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion

            #region Task Mappings
            CreateMap<Task, TaskDto>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status.StatusId))
                .ForMember(dest => dest.PriorityId, opt => opt.MapFrom(src => src.Priority.PriorityId));                

            CreateMap<Task, TaskSimpleViewDto>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status.StatusId))
                .ForMember(dest => dest.PriorityId, opt => opt.MapFrom(src => src.Priority.PriorityId));

            CreateMap<CreateTaskDto, Task>()
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.Attendant, opt => opt.Ignore());

            CreateMap<NewTaskMinDto, CreateTaskDto>()
                .ForPath(dest => dest.Author.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<TaskDto, EditTaskDto>();

            CreateMap<Task, EditTaskDto>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status.StatusId))
                .ForMember(dest => dest.PriorityId, opt => opt.MapFrom(src => src.Priority.PriorityId));

            CreateMap<EditTaskDto, Task>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateProjectTaskDto, Task>()
                .ForMember(dest => dest.AttendantId, opt => opt.MapFrom(src => src.Attendant.PersonId))
                .ForMember(dest => dest.Attendant, opt => opt.Ignore());
            #endregion

            #region Project Mappings
            CreateMap<Project, ProjectDto>();
            CreateMap<Project, ProjectSimpleViewDto>();
            CreateMap<ProjectDto, Project>();

            CreateMap<ProjectFullDto, Project>();
            CreateMap<Project, ProjectFullDto>();

            CreateMap<Project, ProjectDataDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));

            CreateMap<ProjectDto, Project>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion

            #region PersonInProject Mappings
            CreateMap<PersonInProjectDto, PersonInProject>();
            CreateMap<PersonInProject, PersonInProjectDto>();

            CreateMap<PersonInProject, PersonInProjectSimpleDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Title));
            #endregion

            #region Comments Mappings
            CreateMap<Comment, CommentSimpleDto>();

            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();

            CreateMap<NewCommentDto, Comment>();
            #endregion

            #region Role
            CreateMap<IdentityRole, RoleDto>();
            #endregion
        }
    }
}
