using Application.Interfaces;
using Application.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepositories _userRepositories;
        private readonly IMapper _mapper;
        public UserService(IUserRepositories userRepositories, IMapper mapper)
        {
            _userRepositories = userRepositories;
            _mapper = mapper;
        }
        public async Task<PaginatedList<UserDTO>> GetUserList(int Page = 1)
        {
            PaginatedList<User, UserDTO> data = await _userRepositories.GetUserList(Page);
            return new PaginatedList<UserDTO>(data.Data, data.PageIndex, data.TotalPages, data.CountData);
        }
        public async Task<List<UserDTO>> GetUserList()
        {
            List<User> data = await _userRepositories.GetUserList();
            return _mapper.Map<List<UserDTO>>(data);
        }
        public async Task<ReturnStatus> SaveUser(UserDTO user)
        {
            ReturnStatus result = new() { Status = 1 };
            try
            {
                User data = _mapper.Map<User>(user);
                if (await UserExists(data.Id, data.Username))
                {
                    result.Message = "Username already existing";
                    return result;
                }
                if(user.Id == 0)
                    result = await _userRepositories.AddUser(data);
                else
                {
                    var existinguser = await _userRepositories.GetUser(user.Id);
                    if (existinguser.Id == 0)
                    {
                        result.Message = "No user found";
                        return result;
                    }
                    result = await _userRepositories.UpdateUser(data);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        public async Task<ReturnStatus> DeleteUser(int id)
        {
            ReturnStatus result = new() { Status = 1 };
            try
            {
                var user = await _userRepositories.GetUser(id);
                if (user.Id == 0)
                {
                    result.Message = "No user found";
                    return result;
                }
                result = await _userRepositories.DeleteUser(id);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        private async Task<bool> UserExists(int id, string Username)
        {
            var data = await _userRepositories.GetUser(Username);
            return data != null ? (id != 0 ? (data.Id != id && data.Username == Username) : data.Username == Username) : false;
        }
    }
}
