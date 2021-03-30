using ClosedXML.Excel;
using ProjectBoss.Api.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBoss.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewDto>> GetUsers();
        Task<UserViewDto> GetUserById(string userId);
        Task<bool> EditUser(UserViewDto userData);
        Task<bool> ResetUserPassword(string userId);
        Task<byte[]> DownloadUsersXlsl();
        Task<byte[]> DownloadUsersPdf();
    }
}
