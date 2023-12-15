using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using WebNovel.API.Areas.Models.UpdatedFees.Schemas;
using WebNovel.API.Commons;
using WebNovel.API.Commons.CodeMaster;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.UpdatedFees
{
    public interface IUpdatedFeeModel
    {
        Task<List<UpdatedFeeDto>> GetListUpdatedFee();
        Task<ResponseInfo> AddUpdatedFee(UpdatedFeeCreateUpdateEntity account);
        Task<ResponseInfo> UpdateUpdatedFee(long id, UpdatedFeeCreateUpdateEntity account);
        UpdatedFeeDto GetUpdatedFee(long id);
        Task<UpdatedFeeDto?> GetActiveFee();
    }
    public class UpdatedFeeModel : BaseModel, IUpdatedFeeModel
    {
        private readonly ILogger<IUpdatedFeeModel> _logger;
        private string _className = "";
        public UpdatedFeeModel(IServiceProvider provider, ILogger<IUpdatedFeeModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddUpdatedFee(UpdatedFeeCreateUpdateEntity updatedFee)
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

                var newUpdatedFee = new UpdatedFee()
                {
                    Fee = updatedFee.Fee,
                    DateUpdated = DateTime.Now,
                    Year = DateTime.Now.Year,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.UpdatedFee.AddAsync(newUpdatedFee);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                    }
                );

                result = await UpdateFeeAllChapter();

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

        public async Task<ResponseInfo> UpdateFeeAllChapter()
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

                var Fee = await GetActiveFee();
                if (Fee is null)
                {
                    result.Code = CodeResponse.HAVE_ERROR;
                    result.MsgNo = "Update Chapter Failed";
                    return result;
                }

                foreach (var chapter in await _context.Chapter.Where(e => e.DelFlag == false && e.IsLocked == true)
                                                                .ToListAsync())
                {
                    chapter.FeeId = Fee.Id;
                }

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
                _logger.LogError($"[{_className}][{method}] Exception: {e.Message}");

                throw;
            }
        }

        public UpdatedFeeDto GetUpdatedFee(long id)
        {
            var updatedFee = _context.UpdatedFee.Where(x => x.Id == id).FirstOrDefault();
            var updatedFeeDto = new UpdatedFeeDto()
            {
                Id = updatedFee.Id,
                Fee = updatedFee.Fee,
                DateUpdated = updatedFee.DateUpdated,
                Year = updatedFee.Year,
            };

            return updatedFeeDto;
        }

        public async Task<List<UpdatedFeeDto>> GetListUpdatedFee()
        {
            var listUpdatedFee = await _context.UpdatedFee.Select(x => new UpdatedFeeDto()
            {
                Id = x.Id,
                Fee = x.Fee,
                DateUpdated = x.DateUpdated,
                Year = x.Year,
            }).OrderByDescending(x => x.DateUpdated).ToListAsync();

            return listUpdatedFee;
        }

        public async Task<ResponseInfo> UpdateUpdatedFee(long id, UpdatedFeeCreateUpdateEntity updatedFee)
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

                var existUpdatedFee = _context.UpdatedFee.Where(x => x.Id == id).FirstOrDefault();
                if (existUpdatedFee is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existUpdatedFee.Fee = updatedFee.Fee;
                existUpdatedFee.DateUpdated = DateTime.Now;

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

        public async Task<UpdatedFeeDto?> GetActiveFee()
        {
            var activeFee = await _context.UpdatedFee.OrderByDescending(e => e.CreatedAt).FirstOrDefaultAsync();
            if (activeFee is null)
            {
                return null;
            }

            var updatedFeeDto = new UpdatedFeeDto()
            {
                Id = activeFee.Id,
                Fee = activeFee.Fee,
                DateUpdated = activeFee.DateUpdated,
                Year = activeFee.Year,
            };

            return updatedFeeDto;
        }
    }
}