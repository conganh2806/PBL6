using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CSharpVitamins;
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
        Task<ResponseInfo> AddAccount(AccountCreateEntity account);
        Task<ResponseInfo> UpdateAccount(AccountUpdateEntity account);
        Task<AccountDto?> GetAccount(string id);
        Task<AccountDto> FindByEmailAsync(string email);
        Task<ResponseInfo> UpdateToken(AccountCreateUpdateEntity account);
        Task<string> LoginWithPasswordAsync(string email, string password);
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

        public async Task<ResponseInfo> AddAccount(AccountCreateEntity account)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                var GuID = (ShortGuid)Guid.NewGuid();

                _logger.LogInformation($"[{_className}][{method}] Start");
                var accountValidate = new AccountCreateUpdateEntity()
                {
                    Password = account.Password,
                    ConfirmPassword = account.ConfirmPassword,
                    Email = account.Email,
                };
                ResponseInfo result = await ValidateUser(null, accountValidate);
                if (result.Code != CodeResponse.OK)
                {
                    return result;
                }

                var newAccount = new Account()
                {
                    Id = GuID.ToString(),
                    NickName = account.NickName,
                    Username = account.Username,
                    Password = Security.Sha256(account.Password),
                    Status = A001.NORMAL.CODE,
                    Email = account.Email,
                    IsAdmin = account.IsAdmin,
                    IsActive = true,
                };
                if (account.RoleIds.Any())
                {
                    foreach (var roleId in account.RoleIds)
                    {
                        newAccount.Roles.Add(new RolesOfUser()
                        {
                            RoleId = roleId,
                            AccountId = newAccount.Id,
                        });
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

        public async Task<AccountDto> FindByEmailAsync(string email)
        {
            var account = await _context.Accounts.Include(x => x.Roles).ThenInclude(x => x.Role).Where(x => x.Email == email).FirstOrDefaultAsync();
            if (account == null)
            {
                return null;
            }
            var accountDto = new AccountDto
            {
                Id = account.Id,
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

        public async Task<AccountDto?> GetAccount(string id)
        {
            var account = await _context.Accounts.Include(x => x.Roles).ThenInclude(x => x.Role).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (account == null)
            {
                return null;
            }
            var accountDto = new AccountDto
            {
                Id = account.Id,
                NickName = account.NickName,
                Username = account.Username,
                Status = account.Status,
                Email = account.Email,
                Phone = account.Phone,
                IsAdmin = account.IsAdmin,
                RoleIds = account.Roles.Select(x => x.RoleId).ToList(),
                RefreshToken = account.RefreshToken,
                RefreshTokenExpiryTime = account.RefreshTokenExpiryTime,
            };

            return accountDto;
        }

        public async Task<List<AccountDto>> GetListAccount(SearchCondition searchCondition)
        {
            var listAccount = await _context.Accounts.Include(x => x.Roles).ThenInclude(x => x.Role).Select(x => new AccountDto()
            {
                Id = x.Id,
                NickName = x.NickName,
                Username = x.Username,
                Status = x.Status,
                Email = x.Email,
                Phone = x.Phone,
                IsAdmin = x.IsAdmin,
                RoleIds = x.Roles.Select(x => x.RoleId).ToList(),
                RefreshToken = x.RefreshToken,
                RefreshTokenExpiryTime = x.RefreshTokenExpiryTime,
            }).ToListAsync();

            return listAccount;
        }

        public async Task<ResponseInfo> UpdateAccount(AccountUpdateEntity account)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();

            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();
                bool IsChangeEmailPassword = false;

                if (account.Email is not null
                && account.Password is not null
                && account.ConfirmPassword is not null)
                {
                    IsChangeEmailPassword = true;
                    var accountValidate = new AccountCreateUpdateEntity()
                    {
                        Id = account.Id,
                        Password = account.Password,
                        ConfirmPassword = account.ConfirmPassword,
                        Email = account.Email,
                    };
                    result = await ValidateUser(accountValidate.Id, accountValidate);

                    if (result.Code != CodeResponse.OK)
                    {
                        return result;
                    }
                }

                var existAccount = await _context.Accounts.Include(x => x.Roles).ThenInclude(x => x.Role).Where(x => x.Id == account.Id).FirstOrDefaultAsync();
                if (existAccount is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                if (account.NickName is not null) existAccount.NickName = account.NickName;
                if (account.Username is not null) existAccount.Username = account.Username;

                if (IsChangeEmailPassword
                && account.Email is not null
                && account.Password is not null)
                {
                    existAccount.Password = Security.Sha256(account.Password);
                    existAccount.Email = account.Email;
                }

                existAccount.Phone = account.Phone;

                if (account.WalletAmmount is not null) existAccount.WalletAmmount = (float)account.WalletAmmount;
                if (account.IsAdmin is not null) existAccount.IsAdmin = (bool)account.IsAdmin;
                if (account.IsActive is not null) existAccount.IsActive = (bool)account.IsActive;

                if (account.RoleIds is not null)
                    if (account.RoleIds.Any())
                    {
                        _context.RolesOfUsers.RemoveRange(_context.RolesOfUsers.Where(x => x.AccountId == existAccount.Id));
                        foreach (var roleId in account.RoleIds)
                        {
                            existAccount.Roles.Add(new RolesOfUser()
                            {
                                AccountId = existAccount.Id,
                                RoleId = roleId,
                            });
                        }
                    }

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

        public async Task<ResponseInfo> UpdateToken(AccountCreateUpdateEntity account)
        {
            ResponseInfo result = new();
            string method = GetActualAsyncMethodName();
            try
            {
                var item = await _context.Accounts.Where(x => x.Id == account.Id).FirstOrDefaultAsync();

                item.RefreshToken = account.RefreshToken;
                item.RefreshTokenExpiryTime = account.RefreshTokenExpiryTime;

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
            }
            catch (Exception e)
            {
                _logger.LogInformation($"[{_className}][{method}] Exception: {e.Message}");
                throw;
            }
            return result;
        }

        private async Task<ResponseInfo> ValidateUser(string userId, AccountCreateUpdateEntity account)
        {
            ResponseInfo result = new ResponseInfo();

            if (await base.ValidateEmail(userId, account.Email))
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

        public async Task<string> LoginWithPasswordAsync(string email, string password)
        {
            var user = await _context.Accounts.Where(x => x.Email == email && x.Password == Security.Sha256(password)).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new UnauthorizedAccessException("Authentication Failed.");
            }

            // if (!user.IsVerifyEmail)
            // {
            //     throw new UnauthorizedAccessException("Email's not confirmed.");
            // }

            return user.Id;
        }
    }
}