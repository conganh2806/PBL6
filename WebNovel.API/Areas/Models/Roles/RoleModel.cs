using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using WebNovel.API.Areas.Models.Roles.Schemas;
using WebNovel.API.Commons;
using WebNovel.API.Commons.CodeMaster;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Roles
{
    public interface IRoleModel
    {
        Task<List<RoleDto>> GetListRole();
        Task<ResponseInfo> AddRole(RoleCreateUpdateEntity account);
        Task<ResponseInfo> UpdateRole(string id, RoleCreateUpdateEntity account);
        Task<RoleDto> GetRole(string id);
    }
    public class RoleModel : BaseModel, IRoleModel
    {
        private readonly ILogger<IRoleModel> _logger;
        private string _className = "";
        public RoleModel(IServiceProvider provider, ILogger<IRoleModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddRole(RoleCreateUpdateEntity role)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                if (result.Code != CodeResponse.OK)
                {
                    return result;
                }

                var newRole = new Role()
                {
                    Id = role.Id,
                    Name = role.Name,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Roles.AddAsync(newRole);
                            await _context.SaveChangesAsync();
                            await trn.CommitAsync();
                        }
                    }
                );

                // await _context.Roles.AddAsync(newRole);
                // transaction = await _context.Database.BeginTransactionAsync();
                // await _context.SaveChangesAsync();
                // await transaction.CommitAsync();
                _logger.LogInformation($"[{_className}][{method}] End");
                return result;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    await _context.RollbackAsync(transaction);
                }
                _logger.LogError($"[{_className}][{method}] Exception: {e.Message}");

                throw;
            }
        }

        public async Task<RoleDto> GetRole(string id)
        {
            var role = await _context.Roles.Where(x => x.Id == id).FirstOrDefaultAsync();
            var roleDto = new RoleDto()
            {
                Id = role.Id,
                Name = role.Name,
            };

            return roleDto;
        }

        public async Task<List<RoleDto>> GetListRole()
        {
            var listRole = _context.Roles.Select(x => new RoleDto()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();

            return listRole;
        }

        public async Task<ResponseInfo> UpdateRole(string id, RoleCreateUpdateEntity role)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();

            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();
                if (result.Code != CodeResponse.OK)
                {
                    return result;
                }

                var existRole = _context.Roles.Where(x => x.Id == id).FirstOrDefault();
                if (existRole is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existRole.Name = role.Name;

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.SaveChangesAsync();
                            await trn.CommitAsync();
                        }
                    }
                );

                // transaction = await _context.Database.BeginTransactionAsync();
                // await _context.SaveChangesAsync();
                // await transaction.CommitAsync();

                _logger.LogInformation($"[{_className}][{method}] End");

                return result;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    await _context.RollbackAsync(transaction);
                }
                _logger.LogInformation($"[{_className}][{method}] Exception: {e.Message}");
                throw;
            }
        }


    }
}