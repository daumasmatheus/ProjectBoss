using ClosedXML.Excel;
using ProjectBoss.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetTasksByAuthorId(Guid authorId);
        Task<List<TaskDto>> GetTasksByProjectId(Guid projectId);
        Task<List<TaskDto>> GetAllActiveTasks();
        Task<List<TaskDto>> GetTasksByAttendant(Guid attendantId);
        Task<CreateTaskDto> CreateNewTask(CreateTaskDto task);
        Task<bool> EditTask(EditTaskDto task);
        Task<bool> SetTaskCompleted(Guid taskId);
        Task<TaskDto> GetTaskByTaskId(Guid taskId);
        Task<bool> ToggleTaskStatus(Guid taskId, int statusId);
        Task<bool> RemoveTask(Guid taskId);
        Task<byte[]> ExportTasksAsXlsl(Guid userId, bool restrictData);
        Task<byte[]> ExportTasksAsPdf(Guid userId, bool restrictData);
    }
}
