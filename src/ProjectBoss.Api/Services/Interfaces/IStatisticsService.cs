using ProjectBoss.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<IEnumerable<GroupedChartDataDto>> GetOpenAndOnGoingTasksByPersonInProject(Guid projectId);
        Task<IEnumerable<ChartDataDto>> GetTasksStatusByProject(Guid projectId);        
        Task<IEnumerable<GroupedChartDataDto>> GetNewAndClosedTasksByDateByProject(Guid projectId);
        Task<PersonOverviewStatsDto> GetPersonOverviewStatistics(Guid personId);

        //Admin Overview Statistics
        Task<GroupedChartDataDto> GetCreatedUsers();
        Task<GroupedChartDataDto> GetTotalCreatedTasksByDate();
        Task<GroupedChartDataDto> GetTotalConcludedTasksByDate();
        Task<GroupedChartDataDto> GetTotalCreatedProjectsByDate();
        Task<GroupedChartDataDto> GetTotalConcludedProjectsByDate();
    }
}
