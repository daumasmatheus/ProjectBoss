using AutoMapper;
using ClosedXML.Excel;
using DinkToPdf;
using DinkToPdf.Contracts;
using ProjectBoss.Api.Dtos;
using ProjectBoss.Api.Dtos.Enums;
using ProjectBoss.Api.Services.Helpers;
using ProjectBoss.Api.Services.Interfaces;
using ProjectBoss.Core.Entities;
using ProjectBoss.Data.Repositories.Interfaces;
using ProjectBoss.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IPersonInProjectService personInProjectService;

        private readonly IProjectRepository projectRepository;
        private readonly IConverter converter;
        private readonly IMapper mapper;

        public ProjectService(IProjectRepository _projectRepository,      
                              IPersonInProjectService _personInProjectService,
                              IConverter _converter,
                              IMapper _mapper)
        {
            projectRepository = _projectRepository;
            personInProjectService = _personInProjectService;
            converter = _converter;
            mapper = _mapper;
        }

        public async Task<ProjectDto> CreateNewProject(ProjectDto project)
        {
            var projectEntity = mapper.Map<Project>(project);            
            
            var projectSaved = await projectRepository.InsertAsNoTracking(projectEntity);

            if (!projectSaved)
                return null;

            var attendantsSaved = await personInProjectService.AddProjectAttendant(new PersonInProjectParameterDto
            {
                ProjectId = project.ProjectId,
                AttendantIds = project.AttendantIds.ToArray()
            });

            return (projectSaved && attendantsSaved) ? project : null;
        }

        public async Task<bool> EditProjectData(ProjectDto project)
        {
            var entity = await projectRepository.GetSingleByCondition(x => x.ProjectId == project.ProjectId);

            if (entity == null)
                return false;

            var updated = mapper.Map(project, entity);

            await projectRepository.Update(updated);

            return await projectRepository.SaveChanges();
        }        

        public async Task<ProjectDataDto> GetProjectDataById(Guid projectId)
        {
            var res = await projectRepository.GetProjectDataById(projectId);

            return mapper.Map<ProjectDataDto>(res);
        }

        public async Task<byte[]> ExportProjectAsPdf(Guid projectId)
        {
            var project = await projectRepository.GetProjectById(projectId);

            if (project == null)
                return null;

            var settings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = $"Projeto-{project.Title}-{DateTime.Now:dd-MM-yyyy}"
            };

            var objSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GenerateHtmlReport(project),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Tarefas" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = settings,
                Objects = { objSettings }
            };

            return converter.Convert(pdf);
        }

        private string GenerateHtmlReport(Project project)
        {
            string header = $"{project.Title.ToUpper()}";

            var sb = new StringBuilder();
            sb.Append(
                @"
                <html>
                <head>
                </head>
                <style>
                body{
                      font-family: Arial, Helvetica, sans-serif;
                    }
                    .header {
                        text-align: center;
                        padding-bottom: 35px;
                    }
                    table {
                        width: 80%;
                        border-collapse: collapse;
                        page-break-inside: avoid;
                    }
                    td, th {
                        border: 1px solid gray;
                        padding: 5px;
                        font-size: 16px;
                        text-align: left;
                    }
                    table th {
                        background-color: #0079a1;
                        color: #e7e7e7;
                    }
                </style>
                <body>");

            sb.Append(
                $@"
                    <h1>Projeto {header}</h1>
                    <div>
                      <table align='center'>
                        <tr>
                          <th>Titulo</th>
                          <td>{project.Title}</td>
                          <th>Autor</th>
                          <td>{project.Author.FirstName} {project.Author.LastName}</td>
                          <th>Data de Abertura</th>
                          <td>{project.CreatedDate:dd/MM/yyyy}</td>
                          <th>Data de Conclusão</th>
                          <td>{project.ConclusionDate.Value:dd/MM/yyyy}</td>         
                        </tr>
                        <tr>
                          <th>Participantes</th>
                          <td>{project.PersonInProject.Count}</td>
                          <th>Tarefas</th>
                          <td>{project.Tasks.Count}</td>
                          <th>Concluída</th>
                          <td>{(project.ConcludedDate == null ? "Não" : "Sim")}</td>                          
                          [CONCLUSION_DATE]
                        </tr>
                        <tr>      
                          <th colspan='10'>Descrição</th>                            
                        </tr>    
                        <tr>
                          <td colspan='10'>{project.Description}</td>
                        </tr>    
                      </table>
                      <hr>
                    </div>");

            if (project.ConcludedDate != null)
                sb.Replace("[CONCLUSION_DATE]", $"<th>Concluída em</th><td>{project.ConcludedDate.Value:dd/MM/yyyy}</td>");
            else
                sb.Replace("[CONCLUSION_DATE]", string.Empty);

            sb.Append(
                $@"
                <h1>Tarefas</h1>
                <div>
                    <table align='center'>
                ");

            foreach (var task in project.Tasks)
            {
                sb.Append(
                    $@"
                    <tr>
                        <th>Titulo</th>
                        <td>{task.Title}</td>
                        <th>Data de Abertura</th>
                        <td>{task.CreatedDate}</td>
                        <th>Data de Conclusão</th>
                        <td>{(task.ConclusionDate.HasValue ? task.ConclusionDate.Value.ToString("dd/MM/yyyy") : "Sem data de conclusão")}</td>
                    </tr>
                    <tr>
                        <th>Concluída</th>
                        <td>{(task.ConcludedDate == null ? "Não" : "Sim")}</td>
                        <th>Status</th>
                        <td>{task.Status.Name}</td>
                        <th>Prioridade</th>
                        <td>{task.Priority.Name}</td>
                    </tr>
                    <tr>
                        <th>Responsável</th>
                        <td colspan='10'>{task.Attendant.FirstName} {task.Attendant.LastName}</td>
                    </tr>
                    <tr>
                        <th colspan ='10'>Descrição</th>
                    </tr>
                    <tr>
                        <td colspan='10'>{task.Description}</td>    
                    </tr>");
            }

            sb.Append(
                $@"
                    </table>
                  <hr>
                </div>
                ");

            sb.Append(
                $@"
                <h1>Participantes</h1>
                <div>
                    <table align='center'>");

            foreach (var attendant in project.PersonInProject)
            {
                sb.Append(
                    $@"
                    <tr>
                        <th>Nome</th>
                        <td colspan='10'>{attendant.Person.FirstName} {attendant.Person.LastName}</td>         
                    </tr> 
                    <tr>
                        <th>Tarefas abertas</th>
                        <td>{project.Tasks.Where(x => x.AttendantId == attendant.PersonId && x.ConcludedDate == null).Count()}</td>
                        <th>Tarefas concluidas</th>
                        <td>{project.Tasks.Where(x => x.AttendantId == attendant.PersonId && x.ConcludedDate != null).Count()}</td>
                    </tr>");
            }

            sb.Append(
                $@"
                    </table>
                </div>
                ");

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString();
        }

        public async Task<byte[]> ExportProjectAsXlsl(Guid projectId)
        {
            var project = await projectRepository.GetProjectById(projectId);

            if (project == null)
                return null;

            var workBook = new XLWorkbook();

            #region Dados do Projeto
            IXLWorksheet worksheet = workBook.Worksheets.Add($"Dados do Projeto");

            worksheet.Cell("A1").Value = "Dados do Projeto";
            worksheet.Cell("A1").Style.Font.SetBold()
                                      .Fill.SetBackgroundColor(XLColor.FromTheme(XLThemeColor.Accent1, 0.5))
                                      .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range("A1:H1").Row(1).Merge();

            worksheet.Cell("A2").Value = "Titulo";
            worksheet.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            worksheet.Cell("B2").Value = project.Title;

            worksheet.Cell("C2").Value = "Data de Criação";
            worksheet.Cell("C2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            worksheet.Cell("D2").Value = project.CreatedDate.ToString("dd/MM/yyyy");

            worksheet.Cell("E2").Value = "Data de Conclusão";
            worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            worksheet.Cell("F2").Value = project.ConclusionDate.Value.ToString("dd/MM/yyyy");

            worksheet.Cell("G2").Value = "Participantes";
            worksheet.Cell("G2").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            worksheet.Cell("H2").Value = project.PersonInProject.Count;

            worksheet.Cell("A3").Value = "Autor";
            worksheet.Cell("A3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            worksheet.Cell("B3").Value = $"{project.Author.FirstName} {project.Author.LastName}";

            worksheet.Cell("C3").Value = "Tarefas";
            worksheet.Cell("C3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
            worksheet.Cell("D3").Value = project.Tasks.Count;

            if (project.ConcludedDate != null)
            {
                worksheet.Cell("E3").Value = "Concluída?";
                worksheet.Cell("E3").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                worksheet.Cell("F3").Value = project.ConcludedDate.Value.ToString("dd/MM/yyyy");
            }

            worksheet.Cell("A4").Value = "Descrição";
            worksheet.Cell("A4").Style.Fill.SetBackgroundColor(XLColor.FromTheme(XLThemeColor.Accent1, 0.5))
                                      .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range("A4:H4").Row(1).Merge();

            worksheet.Cell("A5").Value = project.Description;
            worksheet.Range("A5:H5").Row(1).Merge();

            worksheet.Columns().AdjustToContents();

            IXLRange taskDetailsRange = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(5, 8));
            taskDetailsRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            taskDetailsRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            taskDetailsRange.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
            taskDetailsRange.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
            #endregion

            #region Tarefas
            IXLWorksheet tasksWs = workBook.Worksheets.Add($"Tarefas");

            tasksWs.Cell("A1").Value = "Tarefas do Projeto";
            tasksWs.Cell("A1").Style.Font.SetBold()
                                    .Fill.SetBackgroundColor(XLColor.FromTheme(XLThemeColor.Accent1, 0.5))
                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            tasksWs.Range("A1:H1").Row(1).Merge();

            for (int i = 1; i <= project.Tasks.Count; i++)
            {
                tasksWs.Cell($"A{i+1}").Value = "Tarefa";
                tasksWs.Cell($"A{i+1}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                tasksWs.Cell($"B{i+1}").Value = project.Tasks[i-1].Title;

                tasksWs.Cell($"C{i+1}").Value = "Data de Conclusão";
                tasksWs.Cell($"C{i+1}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                tasksWs.Cell($"D{i+1}").Value = project.Tasks[i - 1].ConclusionDate.HasValue ? project.Tasks[i-1].ConclusionDate.Value.ToString("dd/MM/yyyy") : 
                                                                                               "Sem data de conclusão";

                tasksWs.Cell($"E{i + 1}").Value = "Status";
                tasksWs.Cell($"E{i + 1}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                tasksWs.Cell($"F{i + 1}").Value = EnumHelpers.GetStatusString(project.Tasks[i-1].StatusId);

                tasksWs.Cell($"G{i + 1}").Value = "Prioridade";
                tasksWs.Cell($"G{i + 1}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                tasksWs.Cell($"H{i + 1}").Value = EnumHelpers.GetPriorityString(project.Tasks[i-1].PriorityId);

                tasksWs.Cell($"A{i+2}").Value = "Descrição";
                tasksWs.Cell($"A{i+2}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                tasksWs.Cell($"A{i+2}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                tasksWs.Range($"A{i+2}:H{i+2}").Row(1).Merge();

                tasksWs.Cell($"A{i+3}").Value = project.Tasks[i - 1].Description;
                tasksWs.Range($"A{i+3}:H{i+3}").Row(1).Merge();

                tasksWs.Range($"A{i + 4}:H{i + 4}").Row(1).Merge().Style.Fill.BackgroundColor = XLColor.FromHtml("#a6a6a6");
            }

            tasksWs.Columns().AdjustToContents();

            IXLRange tasksRange = tasksWs.Range(tasksWs.Cell(1, 1),
                                                tasksWs.Cell(tasksWs.RowsUsed().Count() + 1, 8));

            tasksRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            tasksRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            tasksRange.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
            tasksRange.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
            #endregion

            #region Participantes
            IXLWorksheet attendantsWs = workBook.Worksheets.Add($"Participantes");

            attendantsWs.Cell("A1").Value = "Participantes do Projeto";
            attendantsWs.Cell("A1").Style.Font.SetBold()
                                         .Fill.SetBackgroundColor(XLColor.FromTheme(XLThemeColor.Accent1, 0.5))
                                         .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            attendantsWs.Range("A1:H1").Row(1).Merge();

            var personInProjectList = project.PersonInProject.ToList();
            for (int i = 1; i <= project.PersonInProject.Count; i++)
            {
                attendantsWs.Cell($"A{i + 1}").Value = "Nome";
                attendantsWs.Cell($"A{i + 1}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                attendantsWs.Cell($"B{i + 1}").Value = $"{personInProjectList[i - 1].Person.FirstName} {personInProjectList[i - 1].Person.LastName}";

                attendantsWs.Cell($"C{i + 1}").Value = "Tarefas (total)";
                attendantsWs.Cell($"C{i + 1}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                attendantsWs.Cell($"D{i + 1}").Value = project.Tasks.Where(x => x.AttendantId == personInProjectList[i-1].PersonId).Count();

                attendantsWs.Cell($"E{i + 1}").Value = "Tarefas Abertas";
                attendantsWs.Cell($"E{i + 1}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                attendantsWs.Cell($"F{i + 1}").Value = project.Tasks.Where(x => x.AttendantId == personInProjectList[i - 1].PersonId &&
                                                                           x.ConcludedDate == null).Count();

                attendantsWs.Cell($"G{i + 1}").Value = "Tarefas Finalizadas";
                attendantsWs.Cell($"G{i + 1}").Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                attendantsWs.Cell($"H{i + 1}").Value = project.Tasks.Where(x => x.AttendantId == personInProjectList[i - 1].PersonId &&
                                                                           x.ConcludedDate != null).Count();

                attendantsWs.Range($"A{i+2}:H{i+2}").Row(1).Merge().Style.Fill.BackgroundColor = XLColor.FromHtml("#a6a6a6");
            }

            attendantsWs.Columns().AdjustToContents();

            IXLRange attendantsWsRange = attendantsWs.Range(attendantsWs.Cell(1, 1),
                                                            attendantsWs.Cell(attendantsWs.RowsUsed().Count() + 1, 8));
            attendantsWsRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
            attendantsWsRange.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);
            attendantsWsRange.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
            attendantsWsRange.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
            #endregion

            using MemoryStream stream = new MemoryStream();
            workBook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream.ToArray();
        }

        public async Task<IEnumerable<ProjectDataForDropdownDto>> GetProjectDataForDropdown()
        {
            IEnumerable<ProjectDataForDropdownDto> result = null;

            var data = await projectRepository.GetAll();

            if (data == null)
                return result;

            return result = data.Select(s => new ProjectDataForDropdownDto { ProjectId = s.ProjectId, ProjectName = s.Title });
        }
    }
}
