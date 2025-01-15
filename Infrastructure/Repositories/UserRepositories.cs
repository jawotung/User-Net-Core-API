using Application.Interfaces;
using Application.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class UserRepositories : IUserRepositories
    {
        private readonly POSDBContext _dbContext;
        private readonly IMapper _mapper;

        public UserRepositories(POSDBContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        public async Task<PaginatedList<User, UserDTO>> GetUserList(int page = 1)
        {
            IQueryable<User> list = _dbContext.Users.OrderBy(x => x.Username);
            return await PaginatedList<User, UserDTO>.CreateAsync(list, _mapper, page);
        }
        public async Task<List<User>> GetUserList()
        {
            return await _dbContext.Users.ToListAsync();
        }
        public async Task<User> GetUser(string Username)
        {
            return await _dbContext.Users.Where(x => x.Username == Username).FirstOrDefaultAsync() ?? new();
        }
        public async Task<User> GetUser(int ID)
        {
            return await _dbContext.Users.FindAsync(ID) ?? new();
        }
        public async Task<ReturnStatus> UpdateUser(User user)
        {
            ReturnStatus result = new() { Status = 1 };
            try
            {
                var existingUser = await _dbContext.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    result.Message = "No user found";
                    return result;
                }

                _mapper.Map(user, existingUser);
                existingUser.UpdateDate = DateTime.Now;
                _dbContext.Entry(existingUser).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                result.Status = 0;
                result.Message = "Successfull! User was successfully updated";
            }
            catch (DbUpdateConcurrencyException)
            {
                result.Message = "No user found";
            }

            return result;
        }
        public async Task<ReturnStatus> AddUser(User user)
        {
            ReturnStatus result = new() { Status = 1 };
            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                result.Status = 0;
                result.Message = "Successfull! User was successfully added";
            }
            catch (DbUpdateConcurrencyException)
            {
                result.Message = "No user found";
            }
            return result;
        }
        public async Task<ReturnStatus> DeleteUser(int id)
        {
            ReturnStatus result = new() { Status = 1, Message = "No user found" };
            try
            {
                var user = await _dbContext.Users.FindAsync(id);
                if(user != null)
                {
                    _dbContext.Users.Remove(user);
                    await _dbContext.SaveChangesAsync();
                    result.Status = 0;
                    result.Message = "Successfull! User was successfully deleted";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                result.Message = "No user found";
            }
            return result;
        }

    }
}
