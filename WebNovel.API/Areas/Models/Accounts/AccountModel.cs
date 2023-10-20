using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using Webnovel.API.Databases;
using WebNovel.API.Areas.Models.Accounts.Schemas;
using WebNovel.API.Commons;
using WebNovel.API.Commons.CodeMaster;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using WebNovel.API.Databases.Entitites;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Accounts
{
    public interface IAccountModel
    {
        Task<List<AccountDto>> GetListAccount(SearchCondition searchCondition);
        Task<ResponseInfo> AddAccount(AccountCreateUpdateEntity account);
        Task<ResponseInfo> UpdateAccount(long id, AccountCreateUpdateEntity account);
        AccountDto GetAccount (long id);
    }
    public class AccountModel : BaseModel, IAccountModel
    {
        private readonly ILogger<IAccountModel> _logger;
        private string _className = "";
        public AccountModel(IServiceProvider provider, ILogger<IAccountModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddAccount(AccountCreateUpdateEntity account)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = await ValidateUser(null, account);
                if (result.Code != CodeResponse.OK)
                {
                    return result;
                }

                var newAccount = new Account()
                {
                    Id = account.Id,
                    NickName = account.NickName,
                    Username = account.Username,
                    Password = Security.Sha256(account.Password),
                    Status = A001.NORMAL.CODE,
                    Email = account.Email,
                    Phone = account.Phone
                };
                if (account.RoleIds.Any())
                {
                    foreach (var roleId in account.RoleIds)
                    {
                        newAccount.Roles.Add(new RolesOfUser()
                        {
                            RoleId = roleId,
                            AccountId = account.Id,
                        });
                        newAccount.IsAdmin = account.IsAdmin;
                    }
                }

                await _context.Accounts.AddAsync(newAccount);
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

        public AccountDto GetAccount(long id)
        {
            var account = _context.Accounts.Include(x => x.Roles).ThenInclude(x =>x.Role).Where(x => x.Id == id).FirstOrDefault();
            var accountDto = new AccountDto
            {
                NickName = account.NickName,
                Username = account.Username,
                Status = account.Status,
                Email = account.Email,
                Phone = account.Phone,
                IsAdmin = account.IsAdmin,
                RoleIds = account.Roles.Select(x => x.RoleId).ToList()
            };

            return accountDto;
        }

        public async Task<List<AccountDto>> GetListAccount(SearchCondition searchCondition)
        {
            var listAccount = _context.Accounts.Include(x => x.Roles).ThenInclude(x =>x.Role).Select(x => new AccountDto() 
            {
                Username = x.Username,
                Email = x.Email,
                RoleName = x.Roles.Select(x => x.Role.Name).ToList()
            }).ToList();

            return listAccount;
        }

        public async Task<ResponseInfo> UpdateAccount(long id, AccountCreateUpdateEntity account)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();

            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = await ValidateUser(null, account);
                ResponseInfo response = await ValidateUser(null, account);
                if (result.Code != CodeResponse.OK)
                {
                    return result;
                }

                var existAccount = await _context.Accounts.Include(x => x.Roles).ThenInclude(x => x.Role).Where(x => x.Id == id).FirstOrDefaultAsync();
                if (existAccount is null) 
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existAccount.NickName = account.NickName;
                existAccount.Username = account.Username;
                existAccount.Password = Security.Sha256(account.Password);
                existAccount.Phone = account.Phone;
                existAccount.Email = account.Email;
                existAccount.WalletAmmount = account.WalletAmmount;
                existAccount.IsAdmin = account.IsAdmin;
                _context.RolesOfUsers.RemoveRange(_context.RolesOfUsers.Where(x => x.AccountId == existAccount.Id));
                foreach (var roleId in account.RoleIds)
                {
                    existAccount.Roles.Add(new RolesOfUser()
                    {
                        AccountId = existAccount.Id,
                        RoleId = roleId,
                    });
                }
                existAccount.IsActive = account.IsActive;

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
            catch(Exception e)
            {
                if (transaction != null)
                {
                    await _context.RollbackAsync(transaction);
                }
                _logger.LogInformation($"[{_className}][{method}] Exception: {e.Message}");
                throw;
            }
        }

        private async Task<ResponseInfo> ValidateUser(long? userId, AccountCreateUpdateEntity account)
        {
            ResponseInfo result = new ResponseInfo();

            if (await base.ValidatePhone(userId, account.Phone))
            {
                result.MsgNo = MSG_NO.USERNAME_HAD_USED;
                result.Code = CodeResponse.HAVE_ERROR;
                return result;
            }

            if (!string.IsNullOrEmpty(account.Password) && (account.Password != account.ConfirmPassword))
            {
                result.MsgNo = MSG_NO.CONFIRM_PASSWORD_INVALIDATE;
                result.Code = CodeResponse.HAVE_ERROR;
                return result;
            }
            return result;
        }
    }
}