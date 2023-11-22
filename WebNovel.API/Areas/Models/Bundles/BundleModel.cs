using System;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Bundles.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Models.Bundles
{
    public interface IBundleModel
    {
        Task<List<BundleDto>> GetListBundle();
        Task<ResponseInfo> AddBundle(BundleCreateEntity bundle);
        Task<ResponseInfo> UpdateBundle(BundleUpdateEntity bundle);
        Task<ResponseInfo> RemoveBundle(BundleDeleteEntity bundle);
        Task<BundleDto?> GetBundle(long Id);
    }
    public class BundleModel : BaseModel, IBundleModel
    {
        private readonly ILogger<IBundleModel> _logger;
        private string _className = "";
        public BundleModel(IServiceProvider provider, ILogger<IBundleModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddBundle(BundleCreateEntity bundle)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();

                var newBundle = new Bundle()
                {
                    CoinAmount = bundle.CoinAmount,
                    Price = bundle.Price,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Bundles.AddAsync(newBundle);
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

        public async Task<BundleDto?> GetBundle(long Id)
        {
            var bundle = await _context.Bundles.Where(e => e.DelFlag == false).Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (bundle is null)
            {
                return null;
            }
            var BundleDto = new BundleDto()
            {
                Id = bundle.Id,
                CoinAmount = bundle.CoinAmount,
                Price = bundle.Price,
            };
            return BundleDto;
        }

        public async Task<List<BundleDto>> GetListBundle()
        {
            var listBundle = await _context.Bundles.Where(e => e.DelFlag == false).Select(x => new BundleDto()
            {
                Id = x.Id,
                CoinAmount = x.CoinAmount,
                Price = x.Price,
            }).ToListAsync();

            return listBundle;
        }

        public async Task<ResponseInfo> RemoveBundle(BundleDeleteEntity bundle)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();

            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existBundle = _context.Bundles.Where(e => e.DelFlag == false).Where(x => x.Id == bundle.Id).FirstOrDefault();
                if (existBundle is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                _context.Bundles.Remove(existBundle);

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

        public async Task<ResponseInfo> UpdateBundle(BundleUpdateEntity bundle)
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

                var existBundle = _context.Bundles.Where(e => e.DelFlag == false).Where(x => x.Id == bundle.Id).FirstOrDefault();
                if (existBundle is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                if (bundle.CoinAmount is not null) existBundle.CoinAmount = (float)bundle.CoinAmount;
                if (bundle.Price is not null) existBundle.Price = (decimal)bundle.Price;

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
