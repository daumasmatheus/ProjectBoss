using System.Collections.Generic;

namespace ProjectBoss.Api.Dtos
{
    public class PersonOverviewStatsDto
    {
        public PersonOverviewStatsDto()
        {
            ConcludedTasks = new List<TaskSimpleViewDto>();
            TasksDueSoon = new List<TaskSimpleViewDto>();
            RecentProjects = new List<ProjectSimpleViewDto>();
        }

        public List<TaskSimpleViewDto> ConcludedTasks { get; set; }
        public List<TaskSimpleViewDto> TasksDueSoon { get; set; }
        public List<ProjectSimpleViewDto> RecentProjects { get; set; }
    }    
}
