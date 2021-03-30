using AutoMapper;
using ClosedXML.Excel;
using DinkToPdf;
using DinkToPdf.Contracts;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Dtos.Enums;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;
        private readonly IMapper mapper;
        private readonly IConverter converter;

        public TaskService(ITaskRepository _taskRepository,                           
                           IMapper _mapper,
                           IConverter _converter)
        {
            taskRepository = _taskRepository;
            mapper = _mapper;
            converter = _converter;
        }

        public async Task<CreateTaskDto> CreateNewTask(CreateTaskDto task)
        {
            var eTask = mapper.Map<Core.Entities.Task>(task);

            await taskRepository.Create(eTask);

            return await taskRepository.SaveChanges() ? task : null;
        }

        public async Task<bool> EditTask(EditTaskDto task)
        {
            var entity = await taskRepository.GetSingleByCondition(x => x.TaskId == task.TaskId);

            if (entity == null)
                return false;

            var updated = mapper.Map(task, entity);

            await taskRepository.Update(updated);

            return await taskRepository.SaveChanges();
        }

        public async Task<TaskDto> GetTaskByTaskId(Guid taskId)
        {
            var result = await taskRepository.GetSingleByCondition(x => x.TaskId == taskId);
            return mapper.Map<TaskDto>(result);
        }

        public async Task<List<TaskDto>> GetTasksByAttendant(Guid attendantId)
        {
            var result = await taskRepository.GetTasksByAttendant(attendantId);
            if (!result.Any())
                return new List<TaskDto>();

            return result.Select(s => mapper.Map<TaskDto>(s)).ToList();
        }

        public async Task<List<TaskDto>> GetAllActiveTasks()
        {
            var result = await taskRepository.GetAllActiveTasksWithChildEntities();
            return result.Select(s => mapper.Map<TaskDto>(s)).ToList();
        }

        public async Task<List<TaskDto>> GetTasksByAuthorId(Guid authorId)
        {
            var result = await taskRepository.GetTasksByAuthor(authorId);            

            return result.Select(x => mapper.Map<TaskDto>(x))
                         .ToList();
        }

        public async Task<List<TaskDto>> GetTasksByProjectId(Guid projectId)
        {
            var res = await taskRepository.GetTasksByProjectIdWithChildEntities(projectId);
            return res.Select(s => mapper.Map<TaskDto>(s)).ToList();
        }

        public async Task<bool> RemoveTask(Guid taskId)
        {
            var task = await taskRepository.GetSingleByCondition(x => x.TaskId == taskId);

            if (task == null)
                return false;

            task.Removed = true;
            task.RemovedDate = DateTime.Now;

            await taskRepository.Update(task);

            return await taskRepository.SaveChanges();
        }

        public async Task<bool> SetTaskCompleted(Guid taskId)
        {
            var task = await taskRepository.GetSingleByCondition(x => x.TaskId == taskId);

            if (task == null)
                return false;

            task.ConcludedDate = DateTime.Now;
            task.StatusId = (int)EStatus.Finished;

            await taskRepository.Update(task);

            return await taskRepository.SaveChanges();
        }

        public async Task<bool> ToggleTaskStatus(Guid taskId, int statusId)
        {
            var task = await taskRepository.GetTaskByTaskIdWithChildEntities(taskId);

            if (task == null || !Enum.IsDefined(typeof(EStatus), statusId))
                return false;

            task.StatusId = statusId;
            if (statusId == 4)
                task.ConcludedDate = DateTime.Now.Date;

            await taskRepository.Update(task);

            return await taskRepository.SaveChanges();
        }

        public async Task<byte[]> ExportTasksAsXlsl(Guid userId, bool restrictData)
        {
            List<Core.Entities.Task> tasks = null;
            if (!restrictData)
                tasks = await taskRepository.GetAllTasksWithChildEntities();
            else 
                tasks = await taskRepository.GetAllTasksByUserIdWithChildEntities(userId.ToString());

            if (tasks == null)
                return null;

            var tasksDto = tasks.Select(x => mapper.Map<TaskDto>(x))
                                .Where(x => !x.Removed)
                                .ToList();

            var workBook = new XLWorkbook();
            IXLWorksheet worksheet = workBook.Worksheets.Add($"Tarefas - {tasks.First().Attendant.FirstName}");

            worksheet.Cell(1, 1).Value = "Tarefa";
            worksheet.Cell(1, 2).Value = "Descrição";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Prioridade";
            worksheet.Cell(1, 5).Value = "Data de Criação";
            worksheet.Cell(1, 6).Value = "Data de Conclusão";
            worksheet.Cell(1, 7).Value = "Concluída em";

            var headerCells = worksheet.Range("A1:G1");
            headerCells.Style.Font.SetBold()
                             .Fill.SetBackgroundColor(XLColor.BabyBlue)
                             .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            for (int i = 1; i <= tasksDto.Count(); i++)
            {
                worksheet.Cell(i + 1, 1).Value = tasksDto[i - 1].Title;
                worksheet.Cell(i + 1, 2).Value = tasksDto[i - 1].Description;
                worksheet.Cell(i + 1, 3).Value = tasksDto[i - 1].Status;
                worksheet.Cell(i + 1, 4).Value = tasksDto[i - 1].Priority;
                worksheet.Cell(i + 1, 5).Value = tasksDto[i - 1].CreatedDate;
                worksheet.Cell(i + 1, 6).Value = tasksDto[i - 1].ConclusionDate.HasValue ? tasksDto[i - 1].ConclusionDate.Value.ToString("dd/MM/yyyy") : "Sem data de conclusão";
                worksheet.Cell(i + 1, 7).Value = tasksDto[i - 1].ConcludedDate.HasValue ? tasksDto[i - 1].ConcludedDate.Value.ToString("dd/MM/yyyy") : "Não concluída";
            }

            worksheet.Columns().AdjustToContents();

            using MemoryStream stream = new MemoryStream();
            workBook.SaveAs(stream);

            stream.Seek(0, SeekOrigin.Begin);

            return stream.ToArray();
        }

        public async Task<byte[]> ExportTasksAsPdf(Guid userId, bool restrictData)
        {
            List<Core.Entities.Task> tasks = null;
            if (!restrictData)
                tasks = await taskRepository.GetAllTasksWithChildEntities();
            else
                tasks = await taskRepository.GetAllTasksByUserIdWithChildEntities(userId.ToString());

            if (tasks == null)
                return null;

            var tasksDto = tasks.Select(s => mapper.Map<TaskDto>(s))
                                .Where(x => !x.Removed)
                                .ToList();

            var settings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = $"Tarefas-{DateTime.Now:dd-MM-yyyy}"
            };

            var objSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GenerateHtmlReport(tasksDto),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Tarefas" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = settings,
                Objects = { objSettings }
            };

            var aa = converter.Convert(pdf);
            return aa;
        }

        private string GenerateHtmlReport(List<TaskDto> tasks)
        {
            string header = $"Tarefas {tasks.First().Author.FirstName} - {DateTime.Now:dd-MM-yyyy}";

            var sb = new StringBuilder();
            sb.Append(
                @"
                <html>
                <head>
                </head>
                <style>
                .header {
                    text-align: center;
                    padding-bottom: 35px;
                }
                table {
                    width: 80%;
                    border-collapse: collapse;
                }
                td, th {
                    border: 1px solid gray;
                    padding: 15px;
                    font-size: 22px;
                    text-align: center;
                }
                table th {
                    background-color: #29B6F6;
                    color: white;
                }
                </style>
                <body>
                    <div class='header'><h1>[HEADER]</h1></div>
                    <table align='center'>
                        <tr>
                            <th>Tarefa</th>
                            <th>Descrição</th>
                            <th>Status</th>
                            <th>Prioridade</th>
                            <th>Data de Criação</th>
                            <th>Data de Conclusão</th>
                            <th>Concluída em</th>
                        </tr>");

            foreach (var task in tasks)
            {
                sb.Append(
                    $@"
                        <tr>
                            <td>{task.Title}</td>
                            <td>{task.Description}</td>
                            <td>{task.Status}</td>
                            <td>{task.Priority}</td>
                            <td>{task.CreatedDate}</td>
                            <td>{(task.ConclusionDate.HasValue ? task.ConclusionDate.Value.ToString("dd/MM/yyyy") : "Sem data de conclusão")}</td>
                            <td>{(task.ConcludedDate.HasValue ? task.ConcludedDate.Value.ToString("dd/MM/yyyy") : "Não concluída")}</td>
                        </tr>
                      ");
            }

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString().Replace("[HEADER]", header);
        }
    }
}
