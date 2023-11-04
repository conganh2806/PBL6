using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
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
        RatingDto GetRatingByAccount(string AccountId);
        RatingDto GetRatingByNovel(long NovelId);
        RatingDto GetRating(string AccountId, long NovelId);
        Task<ResponseInfo> UpdateRating(string AccountId, long NovelId, RatingCreateUpdateEntity rating);
    }
    public class RatingModel : BaseModel, IRatingModel
    {
        private readonly ILogger<IRatingModel> _logger;
        private string _className = "";
        public RatingModel(IServiceProvider provider, ILogger<IRatingModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddRating(RatingCreateUpdateEntity Rating)
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
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Ratings.AddAsync(newRating);
                            await _context.SaveChangesAsync();
                            await trn.CommitAsync();
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

        public RatingDto GetRatingByAccount(string AccountId)
        {
            var Rating = _context.Ratings.Where(x => x.AccountId == AccountId).FirstOrDefault();
            var RatingDto = new RatingDto()
            {
                NovelId = Rating.NovelId,
                AccountId = Rating.AccountId,
                RateScore = Rating.RateScore
            };

            return RatingDto;
        }

        public RatingDto GetRatingByNovel(long NovelId)
        {
            var Rating = _context.Ratings.Where(x => x.NovelId == NovelId).FirstOrDefault();
            var RatingDto = new RatingDto()
            {
                NovelId = Rating.NovelId,
                AccountId = Rating.AccountId,
                RateScore = Rating.RateScore
            };

            return RatingDto;
        }

        public RatingDto GetRating(string AccountId, long NovelId)
        {
            var Rating = _context.Ratings.Where(x => x.NovelId == NovelId && x.AccountId == AccountId).FirstOrDefault();
            var RatingDto = new RatingDto()
            {
                NovelId = Rating.NovelId,
                AccountId = Rating.AccountId,
                RateScore = Rating.RateScore
            };

            return RatingDto;
        }

        public async Task<List<RatingDto>> GetListRating()
        {
            var listRating = _context.Ratings.Select(x => new RatingDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                RateScore = x.RateScore
            }).ToList();

            return listRating;
        }

        public async Task<ResponseInfo> UpdateRating(string AccountId, long NovelId, RatingCreateUpdateEntity rating)
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

                var existRating = _context.Ratings.Where(x => x.NovelId == NovelId && x.AccountId == AccountId).FirstOrDefault();
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
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.SaveChangesAsync();
                            await trn.CommitAsync();
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