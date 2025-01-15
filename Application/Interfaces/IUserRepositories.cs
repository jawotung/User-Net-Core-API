using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IUserRepositories
    {
        Task<PaginatedList<User,UserDTO>> GetUserList(int page = 1);
        Task<List<User>> GetUserList();
        Task<User> GetUser(string Username);
        Task<User> GetUser(int ID);
        Task<ReturnStatus> UpdateUser(User user);
        Task<ReturnStatus> AddUser(User user);
        Task<ReturnStatus> DeleteUser(int id);
    }
}
