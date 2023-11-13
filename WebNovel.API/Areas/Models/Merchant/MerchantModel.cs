using System;
using System.Runtime.CompilerServices;
using CSharpVitamins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Areas.Models.Merchant.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Models.Merchant
{
    public interface IMerchantModel
    {
        Task<ResponseInfo> AddMerchant(MerchantCreateUpdateEntity account);
        Task<ResponseInfo> UpdateMerchant(string id, MerchantCreateUpdateEntity account);
        Task<MerchantDto?> GetMerchant(string id);
        Task<List<MerchantDto>> GetListMerchant();
    }

    public class MerchantModel : BaseModel, IMerchantModel
    {

        private readonly ILogger<IMerchantModel> _logger;
        private string _className = "";
        public MerchantModel(IServiceProvider provider, ILogger<IMerchantModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddMerchant(MerchantCreateUpdateEntity merchant)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                var GuID = (ShortGuid)Guid.NewGuid();

                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                if (result.Code != CodeResponse.OK)
                {
                    return result;
                }

                var newMerchant = new Databases.Entities.Merchant()
                {
                    Id = GuID.ToString(),
                    MerchantName = merchant.MerchantName,
                    MerchantWebLink = merchant.MerchantWebLink,
                    MerchantIpnUrl = merchant.MerchantIpnUrl,
                    MerchantReturnUrl = merchant.MerchantReturnUrl,
                    IsActive = true,
                };


                await _context.Merchants.AddAsync(newMerchant);
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
                _logger.LogError($"[{_className}][{method}] Exception: {e.Message}");

                throw;
            }
        }

        public async Task<List<MerchantDto>> GetListMerchant()
        {
            var listMerchant = await _context.Merchants.Select(x => new MerchantDto
            {
                Id = x.Id,
                MerchantName = x.MerchantName,
                MerchantWebLink = x.MerchantWebLink,
                MerchantIpnUrl = x.MerchantIpnUrl,
                MerchantReturnUrl = x.MerchantReturnUrl,
                IsActive = x.IsActive,
            }).ToListAsync();

            return listMerchant;
        }

        public async Task<MerchantDto?> GetMerchant(string id)
        {
            var merchant = await _context.Merchants.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (merchant == null)
            {
                return null;
            }
            var merchantDto = new MerchantDto
            {
                Id = merchant.Id,
                MerchantName = merchant.MerchantName,
                MerchantWebLink = merchant.MerchantWebLink,
                MerchantIpnUrl = merchant.MerchantIpnUrl,
                MerchantReturnUrl = merchant.MerchantReturnUrl,
                IsActive = merchant.IsActive,
            };

            return merchantDto;
        }

        public async Task<ResponseInfo> UpdateMerchant(string id, MerchantCreateUpdateEntity merchant)
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

                var existMerchant = await _context.Merchants.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (existMerchant is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existMerchant.MerchantName = merchant.MerchantName;
                existMerchant.MerchantWebLink = merchant.MerchantWebLink;
                existMerchant.MerchantIpnUrl = merchant.MerchantIpnUrl;
                existMerchant.MerchantReturnUrl = merchant.MerchantReturnUrl;

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
