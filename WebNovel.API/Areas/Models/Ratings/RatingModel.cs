using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Areas.Models.Rating.Schemas;
using WebNovel.API.Commons;
using WebNovel.API.Commons.CodeMaster;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using WebNovel.API.Databases.Entitites;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Rating
{
    public interface IRatingModel
    {
        Task<List<RatingDto>> GetListRating();
        Task<ResponseInfo> AddRating(RatingCreateUpdateEntity rating);
        Task<List<RatingDto>> GetRatingByAccount(string AccountId);
        Task<List<RatingDto>> GetRatingByNovel(string NovelId);
        Task<RatingDto?> GetRating(string AccountId, string NovelId);
        Task<ResponseInfo> UpdateRating(RatingCreateUpdateEntity rating);
        Task<ResponseInfo> RemoveRating(RatingDeleteEntity rating);
    }
    public class RatingModel : BaseModel, IRatingModel
    {
        private readonly ILogger<IRatingModel> _logger;
        private readonly IAccountModel _accountModel;
        private string _className = "";
        public RatingModel(IServiceProvider provider, ILogger<IRatingModel> logger, IAccountModel accountModel) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            _accountModel = accountModel;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddRating(RatingCreateUpdateEntity Rating)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                if (result.Code != CodeResponse.OK)
                {
                    return result;
                }

                var newRating = new Databases.Entitites.Rating()
                {
                    NovelId = Rating.NovelId,
                    AccountId = Rating.AccountId,
                    RateScore = Rating.RateScore
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Ratings.AddAsync(newRating);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                    }
                );

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

        public async Task<List<RatingDto>> GetRatingByAccount(string AccountId)
        {
            var listRating = await _context.Ratings.Where(e => e.DelFlag == false).Where(e => e.AccountId == AccountId).Select(x => new RatingDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                RateScore = x.RateScore
            }).ToListAsync();

            foreach (var rating in listRating)
            {
                var Account = await _accountModel.GetAccount(rating.AccountId);
                rating.Username = Account.Username;
                rating.Email = Account.Email;
                rating.NickName = Account.NickName;
                rating.RoleIds = Account.RoleIds;
            }

            return listRating;
        }

        public async Task<List<RatingDto>> GetRatingByNovel(string NovelId)
        {
            var listRating = await _context.Ratings.Where(e => e.DelFlag == false).Where(e => e.NovelId == NovelId).Select(x => new RatingDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                RateScore = x.RateScore
            }).ToListAsync();

            foreach (var rating in listRating)
            {
                var Account = await _accountModel.GetAccount(rating.AccountId);
                rating.Username = Account.Username;
                rating.Email = Account.Email;
                rating.NickName = Account.NickName;
                rating.RoleIds = Account.RoleIds;
            }

            return listRating;
        }

        public async Task<RatingDto?> GetRating(string AccountId, string NovelId)
        {
            var Rating = await _context.Ratings.Where(e => e.DelFlag == false).Where(x => x.NovelId == NovelId && x.AccountId == AccountId).FirstOrDefaultAsync();
            if (Rating is null)
            {
                return null;
            }
            var Account = await _accountModel.GetAccount(Rating.AccountId);
            var RatingDto = new RatingDto()
            {
                NovelId = Rating.NovelId,
                AccountId = Rating.AccountId,
                RateScore = Rating.RateScore,
                Username = Account.Username,
                Email = Account.Email,
                NickName = Account.NickName,
                RoleIds = Account.RoleIds,
            };

            return RatingDto;
        }

        public async Task<List<RatingDto>> GetListRating()
        {
            var listRating = await _context.Ratings.Where(e => e.DelFlag == false).Select(x => new RatingDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                RateScore = x.RateScore
            }).ToListAsync();

            return listRating;
        }

        public async Task<ResponseInfo> UpdateRating(RatingCreateUpdateEntity rating)
        {
            IDbContextTransaction? transaction = null;
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

                var existRating = _context.Ratings.Where(e => e.DelFlag == false).Where(x => x.NovelId == rating.NovelId && x.AccountId == rating.AccountId).FirstOrDefault();
                if (existRating is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existRating.RateScore = rating.RateScore;

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                    }
                );

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

        public async Task<ResponseInfo> RemoveRating(RatingDeleteEntity rating)
        {
            IDbContextTransaction? transaction = null;
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

                var existRating = _context.Ratings.Where(e => e.DelFlag == false).Where(x => x.NovelId == rating.NovelId && x.AccountId == rating.AccountId).FirstOrDefault();
                if (existRating is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                _context.Ratings.Remove(existRating);

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                    }
                );

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