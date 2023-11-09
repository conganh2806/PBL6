using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Core.Services;
using WebNovel.API.Databases.Entities;
using WebNovel.API.Databases.Entitites;
using WebNovel.API.Commons.CodeMaster;
using WebNovel.API.Commons;

namespace WebNovel.API.Areas.Models.Login
{
    public interface ILoginModel
    {
        Task<TokenResponse> Login(string email, string password);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenQuery request);
        Task<ResponseInfo> LoginWithGoogle(string email);
    }
    public class LoginModel : BaseModel, ILoginModel
    {
        private readonly IAccountModel _accountModel;
        private readonly ITokenService _tokenService;

        private readonly ILogger<ILoginModel> _logger;
        private string _className = "";
        public LoginModel(IServiceProvider provider, ILogger<ILoginModel> logger, IAccountModel accountModel, ITokenService tokenService) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            _accountModel = accountModel;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> Login(string email, string password)
        {
            var userId = await _accountModel.LoginWithPasswordAsync(email, password);

            return string.IsNullOrEmpty(userId)
            ? throw new UnauthorizedAccessException("Authentication Failed.")
            : await _tokenService.GetTokenAsync(userId);
        }

        public async Task<ResponseInfo> LoginWithGoogle(string email)
        {
            IDbContextTransaction transaction = null;
            try
            {
                var result = new ResponseInfo();
                var userDB = await _context.Accounts.Where(x => x.Email == email).FirstOrDefaultAsync();
                transaction = await _context.Database.BeginTransactionAsync();
                if (userDB == null)
                {
                    userDB = new Account()
                    {
                        Username = email,
                        Email = email,
                        Password = "",
                        IsActive = true,
                        IsVerifyEmail = true,
                    };
                    userDB.Roles.Add(new RolesOfUser()
                    {
                        RoleId = R001.READER.CODE
                    });
                    _context.Accounts.Add(userDB);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                else
                {
                    if (!userDB.IsVerifyEmail)
                    {
                        userDB.IsVerifyEmail = true;
                    }
                }
                var tokeninfo = await _tokenService.GetTokenAsync(email);
                SaveLoginResultData(result, tokeninfo.Token, tokeninfo.RefreshToken);

                return result;
            }
            catch (Exception e)
            {
                await _context.RollbackAsync(transaction);
                throw;
            }
        }

        private ResponseInfo SaveLoginResultData(ResponseInfo result, string token, string refreshToken)
        {
            result.Data.Add("token", token);
            result.Data.Add("refreshToken", refreshToken);
            result.Data.Add("expires", Constants.API_EXPIRES_MINUTE.ToString());
            return result;
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenQuery request) => await _tokenService.RefreshTokenAsync(request);
    }
}