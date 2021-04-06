using AutoMapper;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Dtos.Enums;
using ProjectBoss.Api.Services.Helpers;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IProjectRepository projectRepository;
        private readonly IUserRepository userRepository;
        private readonly ITaskRepository taskRepository;
        private readonly IPersonInProjectRepository personInProjectRepository;
        private readonly IMapper mapper;

        public StatisticsService(IProjectRepository _projectRepository,
                                 IUserRepository _userRepository,
                                 ITaskRepository _taskRepository,
                                 IPersonInProjectRepository _personInProjectRepository,
                                 IMapper _mapper)
        {
            projectRepository = _projectRepository;
            userRepository = _userRepository;
            taskRepository = _taskRepository;
            personInProjectRepository = _personInProjectRepository;
            mapper = _mapper;
        }
        public async Task<PersonOverviewStatsDto> GetPersonOverviewStatistics(Guid personId)
        {
            PersonOverviewStatsDto result = new PersonOverviewStatsDto();

            var tasks = await taskRepository.GetManyByCondition(x => x.AuthorId == personId ||
                                                                     x.AttendantId == personId);
            var project = await personInProjectRepository.GetOpenPersonProjectsWithChildEntities(personId);

            if ((tasks == null || !tasks.Any()) && (project == null || !project.Any()))
                return result;

            result.ConcludedTasks = tasks.Where(x => x.StatusId == (int)EStatus.Finished)
                                         .OrderByDescending(x => x.ConcludedDate.Value)
                                         .Select(x => mapper.Map<TaskSimpleViewDto>(x))
                                         .Take(3)
                                         .ToList();

            result.TasksDueSoon = tasks.Where(x => x.StatusId != (int)EStatus.Finished &&
                                                   x.ConclusionDate != null && 
                                                   (x.ConclusionDate.Value >= DateTime.Now.AddDays(-5) && x.ConclusionDate.Value <= DateTime.Now))
                                       .Select(s => mapper.Map<TaskSimpleViewDto>(s))
                                       .Take(3)
                                       .ToList();

            result.RecentProjects = project.Select(s => mapper.Map<ProjectSimpleViewDto>(s.Project))
                                           .Take(3)
                                           .ToList();

            return result;
        }
        
        public async Task<IEnumerable<GroupedChartDataDto>> GetNewAndClosedTasksByDateByProject(Guid projectId)
        {
            List<GroupedChartDataDto> result = new List<GroupedChartDataDto>();

            var project = await projectRepository.GetProjectById(projectId);
            if (project == null)
                return result;

            var closedTasks = new GroupedChartDataDto() { Name = "Tarefas Fechadas" };
            closedTasks.Series = new List<ChartDataDto>();
            foreach (var item in project.Tasks.Where(x => x.ConcludedDate.HasValue).GroupBy(g => g.ConcludedDate.Value.Date))
            {
                closedTasks.Series.Add(new ChartDataDto { Name = item.Key.ToString("dd/MM/yyyy"), Value = item.Count() });                
            }
            result.Add(closedTasks);

            var newTasks = new GroupedChartDataDto() { Name = "Novas Tarefas" };
            newTasks.Series = new List<ChartDataDto>();
            foreach (var item in project.Tasks.GroupBy(g => g.CreatedDate.Date))
            {
                newTasks.Series.Add(new ChartDataDto { Name = item.Key.ToString("dd/MM/yyyy"), Value = item.Count() });                
            }
            result.Add(newTasks);

            return result;
        }

        public async Task<IEnumerable<GroupedChartDataDto>> GetOpenAndOnGoingTasksByPersonInProject(Guid projectId)
        {
            List<GroupedChartDataDto> result = new List<GroupedChartDataDto>();

            var project = await projectRepository.GetProjectById(projectId);
            if (project == null)
                return result;

            foreach (var item in project.Tasks.GroupBy(g => g.AttendantId))
            {
                var data = new GroupedChartDataDto
                {
                    Name = $"{item.First().Attendant.FirstName} {item.First().Attendant.LastName}"
                };

                data.Series = new List<ChartDataDto>
                {
                    new ChartDataDto { Name = "Abertas", Value = item.Where(x => x.StatusId != 4).Count() },
                    new ChartDataDto { Name = "Concluídas", Value = item.Where(x => x.StatusId == 4).Count() }
                };

                result.Add(data);
            }

            return result;
        }

        public async Task<IEnumerable<ChartDataDto>> GetTasksStatusByProject(Guid projectId)
        {
            List<ChartDataDto> result = new List<ChartDataDto>();

            var project = await projectRepository.GetProjectById(projectId);
            if (project == null)
                return result;

            foreach (var item in project.Tasks.GroupBy(g => g.StatusId))
            {
                var data = new ChartDataDto
                {
                    Name = EnumHelpers.GetStatusString(item.Key),
                    Value = item.Count()
                };
                result.Add(data);
            }

            return result;
        }

        #region Admin Statistics
        public async Task<GroupedChartDataDto> GetCreatedUsers()
        {
            GroupedChartDataDto result = new GroupedChartDataDto();

            var users = await userRepository.GetAll();

            if (users.Any())
            {
                result.Name = "Usuários Criados Por Data";
                result.Series = new List<ChartDataDto>();
                foreach (var item in users.OrderBy(x => x.CreatedDate.Value.Date)
                                          .GroupBy(x => x.CreatedDate.Value.Date))
                {
                    result.Series.Add(new ChartDataDto
                    {
                        Name = item.Key.ToString("dd/MM/yyyy"),
                        Value = item.Count()
                    });
                }
            }

            return result;
        }

        public async Task<GroupedChartDataDto> GetTotalCreatedTasksByDate()
        {
            GroupedChartDataDto result = null;

            var tasks = await taskRepository.GetAll();
            if (tasks == null || !tasks.Any())
                return result;

            result = new GroupedChartDataDto();
            if (tasks.Any())
            {
                result.Name = "Total de Tarefas Criadas Por Data";
                result.Series = new List<ChartDataDto>();
                foreach (var item in tasks.OrderByDescending(x => x.CreatedDate.Date)
                                          .GroupBy(g => g.CreatedDate.Date))
                {
                    result.Series.Add(new ChartDataDto
                    {
                        Name = item.Key.ToString("dd/MM/yyyy"),
                        Value = item.Count()
                    });
                }
            }

            return result;
        }

        public async Task<GroupedChartDataDto> GetTotalConcludedTasksByDate()
        {
            GroupedChartDataDto result = null;

            var tasks = await taskRepository.GetAll();
            if (tasks == null || !tasks.Any())
                return result;

            result = new GroupedChartDataDto();
            if (tasks.Any())
            {
                result.Name = "Total de Tarefas Concluída Por Data";
                result.Series = new List<ChartDataDto>();
                foreach (var item in tasks.Where(x => x.ConcludedDate.HasValue)
                                          .OrderByDescending(x => x.CreatedDate.Date)
                                          .GroupBy(g => g.ConcludedDate.Value.Date))
                {
                    result.Series.Add(new ChartDataDto
                    {
                        Name = item.Key.ToString("dd/MM/yyyy"),
                        Value = item.Count()
                    });
                }
            }

            return result;
        }

        public async Task<GroupedChartDataDto> GetTotalCreatedProjectsByDate()
        {
            GroupedChartDataDto result = new GroupedChartDataDto();

            var projects = await projectRepository.GetAll();

            if (projects != null && projects.Any())
            {
                result.Name = "Total de Projetos Criados Por Data";
                result.Series = new List<ChartDataDto>();
                foreach (var item in projects.OrderByDescending(x => x.CreatedDate.Date)
                                             .GroupBy(g => g.CreatedDate.Date))
                {
                    result.Series.Add(new ChartDataDto
                    {
                        Name = item.Key.ToString("dd/MM/yyyy"),
                        Value = item.Count()
                    });
                }
            }

            return result;
        }

        public async Task<GroupedChartDataDto> GetTotalConcludedProjectsByDate()
        {
            GroupedChartDataDto result = new GroupedChartDataDto();

            var projects = await projectRepository.GetAll();

            if (projects != null && projects.Any())
            {
                result.Name = "Total de Projetos Concluídos Por Data";
                result.Series = new List<ChartDataDto>();
                foreach (var item in projects.Where(x => x.ConcludedDate.HasValue)
                                             .OrderByDescending(x => x.CreatedDate.Date)
                                             .GroupBy(g => g.CreatedDate.Date))
                {
                    result.Series.Add(new ChartDataDto
                    {
                        Name = item.Key.ToString("dd/MM/yyyy"),
                        Value = item.Count()
                    });
                }
            }

            return result;
        }
        #endregion
    }
}
