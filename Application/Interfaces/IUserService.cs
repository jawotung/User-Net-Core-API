using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedList<UserDTO>> GetUserList(int page = 1);
        Task<List<UserDTO>> GetUserList();
        Task<ReturnStatus> SaveUser(UserDTO user);
        Task<ReturnStatus> DeleteUser(int id);
    }
}
