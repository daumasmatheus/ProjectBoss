using AutoMapper;
using ClosedXML.Excel;
using DinkToPdf;
using DinkToPdf.Contracts;
using ProjectBoss.Api.Dtos;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IPersonRepository personRepository;
        private readonly IPersonService personService;
        private readonly IAuthenticationService authenticationService;
        private readonly IMapper mapper;
        private readonly IConverter converter;

        public UserService(IUserRepository _userRepository,
                           IPersonRepository _personRepository,
                           IPersonService _personService,
                           IAuthenticationService _authenticationService,
                           IMapper _mapper,
                           IConverter _converter)
        {
            userRepository = _userRepository;
            personRepository = _personRepository;
            personService = _personService;
            authenticationService = _authenticationService;
            mapper = _mapper;
            converter = _converter;
        }

        public async Task<bool> EditUser(UserViewDto userData)
        {
            bool updatedRole = false;
            if (userData?.Role != null)
                updatedRole = await authenticationService.EditRole(userData.Id, userData.Role.Id);

            var userEntity = await userRepository.GetSingleByCondition(x => x.Id == userData.Id);
            var updated = mapper.Map(mapper.Map<UserDataDto>(userData), userEntity);

            await userRepository.Update(userEntity, updated);
            var updatedUser = await userRepository.SaveChanges();

            bool updatedPerson;
            if (userData?.Person != null)
            {
                var person = mapper.Map<EditPersonDataDto>(userData.Person);
                updatedPerson = await personService.EditPerson(person);
            }
            else
                updatedPerson = true;

            return (updatedUser || updatedRole) && updatedPerson;
        }

        public async Task<UserViewDto> GetUserById(string userId)
        {
            UserViewDto result = null;

            var dbResult = await userRepository.GetUserById(userId);
            if (dbResult == null)
                return result;

            result = new UserViewDto
            {
                Id = dbResult.Id,
                CreatedDate = dbResult.CreatedDate,
                Email = dbResult.Email,
                Provider = dbResult.Provider,
                UserName = dbResult.UserName,
                Person = mapper.Map<PersonFullDto>(personRepository.GetPersonWithChildEntities(userId: dbResult.Id).Result)
            };

            return result;
        }

        public async Task<IEnumerable<UserViewDto>> GetUsers()
        {
            List<UserViewDto> result = null;

            var dbResult = await userRepository.GetUsers();
            if (!dbResult.Any())
                return result;

            result = dbResult.Select(s => new UserViewDto
            {
                Id = s.Id,
                CreatedDate = s.CreatedDate,
                Email = s.Email,
                Provider = s.Provider,
                UserName = s.UserName,
                Role = mapper.Map<UserRoleDto>(userRepository.GetUserRole(s.Id).Result),
                Person = mapper.Map<PersonFullDto>(personRepository.GetPersonWithChildEntities(userId: s.Id).Result)
            }).ToList();

            return result;
        }

        public async Task<bool> ResetUserPassword(string userId)
        {
            return await authenticationService.ResetPassword(userId);
        }

        public async Task<byte[]> DownloadUsersPdf()
        {
            var users = await GetUsers();

            if (users == null)
                return null;

            var settings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = $"Usuários do Sistema-{DateTime.Now:dd/MM/yyyy}"
            };

            var objSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GenerateHtmlTemplate(users.ToList()),
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] e [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Usuários" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = settings,
                Objects = { objSettings }
            };

            return converter.Convert(pdf);
        }

        private string GenerateHtmlTemplate(List<UserViewDto> users)
        {
            string header = $"Usuários do sistema";

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
                            <th>Tipo de Conta</th>
                            <th>Usuário</th>
                            <th>Email</th>
                            <th>Permissionamento</th>
                            <th>Data de Criação</th>                            
                        </tr>");

            foreach (var user in users)
            {
                sb.Append(
                    $@"
                        <tr>
                            <td>{user.Provider}</td>
                            <td>{user.UserName}</td>
                            <td>{user.Email}</td>
                            <td>{user.Role.Name}</td>
                            <td>{user.CreatedDate.Value:dd/MM/yyyy}</td>                            
                        </tr>
                      ");
            }

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString().Replace("[HEADER]", header);
        }

        public async Task<byte[]> DownloadUsersXlsl()
        {
            var users = GetUsers().Result.ToList();

            if (users == null)
                return null;

            var workBook = new XLWorkbook();
            IXLWorksheet worksheet = workBook.Worksheets.Add($"Usuários do sistema");

            worksheet.Cell(1, 1).Value = "Tipo de Conta";
            worksheet.Cell(1, 2).Value = "Usuário";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Permissionamento";
            worksheet.Cell(1, 5).Value = "Data de Criação";

            var headerCells = worksheet.Range("A1:E1");
            headerCells.Style.Font.SetBold()
                             .Fill.SetBackgroundColor(XLColor.BabyBlue)
                             .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            for (int i = 1; i <= users.Count(); i++)
            {
                worksheet.Cell(i + 1, 1).Value = users[i - 1].Provider;
                worksheet.Cell(i + 1, 2).Value = users[i - 1].UserName;
                worksheet.Cell(i + 1, 3).Value = users[i - 1].Email;
                worksheet.Cell(i + 1, 4).Value = users[i - 1].Role.Name;
                worksheet.Cell(i + 1, 5).Value = users[i - 1].CreatedDate.Value.ToString("dd/MM/yyyy");                
            }

            worksheet.Columns().AdjustToContents();

            using MemoryStream stream = new MemoryStream();
            workBook.SaveAs(stream);

            stream.Seek(0, SeekOrigin.Begin);

            return stream.ToArray();
        }

        public async Task<List<RoleDto>> GetRoles()
        {
            var eRoles = await authenticationService.GetRoles();
            eRoles.RemoveAt(eRoles.FindIndex(x => x.Name.ToUpper() == "ADMINISTRATOR"));

            return eRoles.Select(s => mapper.Map<RoleDto>(s)).ToList();
        }
    }
}
