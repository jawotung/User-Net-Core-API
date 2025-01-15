using Application.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class RoleRepositories
    {
        private readonly POSDBContext _dbContext;
        private readonly IMapper _mapper;
        public RoleRepositories(POSDBContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        public async Task<PaginatedList<Role, RoleDTO>> GetRoleList(int page = 1)
        {
            IQueryable<Role> list = _dbContext.Roles.OrderBy(x => x.RoleCode);
            return await PaginatedList<Role, RoleDTO>.CreateAsync(list, _mapper, page);
        }
        public async Task<RoleModule> GetRoleModule(int id)
        {
            return await _dbContext.RoleModules.FirstOrDefaultAsync(rm => rm.Id == id) ?? new();
        }
        public async Task<Role> GetRoleInculdeModule(int id)
        {
            return await _dbContext.Roles.Include(r => r.RoleModules)
                                              .FirstOrDefaultAsync(r => r.Id == id) ?? new();
        }
        public async Task<ReturnStatus> SaveRoleModule(Role role, List<RoleModule> module)
        {
            ReturnStatus result = new() { Status = 1 };
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Save or update the role
                var roleResult = await SaveRole(role);
                if (roleResult.Status != 0)
                {
                    result.Message = roleResult.Message;
                    return result;
                }

                // Save or update the modules
                var moduleResult = await SaveModules(role, module);
                if (moduleResult.Status != 0)
                {
                    result.Message = moduleResult.Message;
                    return result;
                }

                // Commit the transaction
                await transaction.CommitAsync();

                result.Status = 0;
                result.Message = "Successful! Role and modules were successfully updated.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Message = ex.Message;
            }
            return result;
        }
        private async Task<ReturnStatus> SaveRole(Role role)
        {
            ReturnStatus result = new() { Status = 1 };

            try
            {
                // Check if the role with the same RoleCode already exists
                var existingRole = await _dbContext.Roles
                    .FirstOrDefaultAsync(r => r.RoleCode == role.RoleCode);

                if (existingRole != null)
                {
                    // If the role exists, update its details
                    existingRole.RoleName = role.RoleName;
                    existingRole.UpdateId = role.UpdateId;  // Assuming UpdateId is passed for updating
                    existingRole.UpdateDate = DateTime.Now;

                    _dbContext.Entry(existingRole).State = EntityState.Modified;
                }
                else
                {
                    // If the role doesn't exist, add a new one
                    role.CreateDate = DateTime.Now; // Set creation date for new role
                    _dbContext.Roles.Add(role);
                }

                await _dbContext.SaveChangesAsync(); // Save role changes (whether new or updated)
                result.Status = 0;
                result.Message = "Role was successfully saved or updated.";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        private async Task<ReturnStatus> SaveModules(Role role, List<RoleModule> module)
        {
            ReturnStatus result = new() { Status = 1 };

            try
            {
                // Assign the role Id to each module and check for existing ones
                foreach (var x in module)
                {
                    x.RolesId = role.Id;

                    // Check if the RoleModule already exists
                    var existingModule = await _dbContext.RoleModules
                        .FirstOrDefaultAsync(rm => rm.RolesId == x.RolesId && rm.ModuleId == x.ModuleId);

                    if (existingModule == null)
                    {
                        // Add new module if it doesn't exist
                        _dbContext.RoleModules.Add(x);
                    }
                    else
                    {
                        // Update existing module with current date
                        existingModule.UpdateDate = DateTime.Now;
                        _dbContext.Entry(existingModule).State = EntityState.Modified;
                    }
                }

                // Remove any modules that are not in the provided list (i.e., they are removed)
                var modulesToRemove = await _dbContext.RoleModules
                    .Where(rm => rm.RolesId == role.Id && !module.Any(m => m.ModuleId == rm.ModuleId))
                    .ToListAsync();

                if (modulesToRemove.Any())
                {
                    _dbContext.RoleModules.RemoveRange(modulesToRemove);
                }

                await _dbContext.SaveChangesAsync(); // Save changes for RoleModules
                result.Status = 0;
                result.Message = "Modules were successfully saved or updated.";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
        public async Task<ReturnStatus> DeleteRole(int roleId)
        {
            ReturnStatus result = new() { Status = 1 };
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Fetch the role and its associated modules
                var role = await _dbContext.Roles.Include(r => r.RoleModules)
                                                  .FirstOrDefaultAsync(r => r.Id == roleId);
                if (role == null)
                {
                    result.Message = "Role not found.";
                    return result;
                }

                // Remove associated RoleModules
                var modulesToRemove = role.RoleModules.ToList();
                if (modulesToRemove.Any())
                {
                    _dbContext.RoleModules.RemoveRange(modulesToRemove);
                }

                // Remove the Role itself
                _dbContext.Roles.Remove(role);

                // Save changes and commit the transaction
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Status = 0;
                result.Message = "Role and its modules were successfully deleted.";
            }
            catch (Exception ex)
            {
                // Rollback in case of an error
                await transaction.RollbackAsync();
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<ReturnStatus> DeleteRoleModule(int moduleId)
        {
            ReturnStatus result = new() { Status = 1 };
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Fetch the RoleModule to delete
                var moduleToDelete = await _dbContext.RoleModules
                    .FirstOrDefaultAsync(rm => rm.Id == moduleId);

                if (moduleToDelete == null)
                {
                    result.Message = "Module not found.";
                    return result;
                }

                // Remove the module
                _dbContext.RoleModules.Remove(moduleToDelete);

                // Save changes and commit the transaction
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Status = 0;
                result.Message = "Module successfully deleted.";
            }
            catch (Exception ex)
            {
                // Rollback in case of an error
                await transaction.RollbackAsync();
                result.Messsage = ex.Message;
            }
            return result;
        }
    }
}
