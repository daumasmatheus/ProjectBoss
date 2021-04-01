using ClosedXML.Excel;
using ProjectBoss.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateNewProject(ProjectDto project);
        Task<bool> EditProjectData(ProjectDto project);
        Task<ProjectDataDto> GetProjectDataById(Guid projectId);
        Task<byte[]> ExportProjectAsXlsl(Guid projectId);
        Task<byte[]> ExportProjectAsPdf(Guid projectId);
        Task<IEnumerable<ProjectDataForDropdownDto>> GetProjectDataForDropdown();
        Task<bool> ToggleProjectStatus(Guid projectId);
    }
}
