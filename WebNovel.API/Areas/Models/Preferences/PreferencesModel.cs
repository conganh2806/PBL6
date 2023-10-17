using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using WebNovel.API.Areas.Models.Preferences.Schemas;
using WebNovel.API.Commons;
using WebNovel.API.Commons.CodeMaster;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Preferences
{
    public interface IPreferencesModel
    {
        Task<List<PreferencesDto>> GetListPreference();
        Task<ResponseInfo> AddPreference(PreferencesCreateUpdateEntity account);
        Task<ResponseInfo> UpdatePreference(long id, PreferencesCreateUpdateEntity account);
        PreferencesDto GetPreference(long id);
    }
    public class PreferencesModel : BaseModel, IPreferencesModel
    {
        private readonly ILogger<IPreferencesModel> _logger;
        private string _className = "";
        public PreferencesModel(IServiceProvider provider, ILogger<IPreferencesModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddPreference(PreferencesCreateUpdateEntity preference)
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

                var newPreference = new Databases.Entities.Preferences()
                {
                    Id = preference.Id,
                    NovelId = preference.NovelId,
                    AccountId = preference.AccountId,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Preferences.AddAsync(newPreference);
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

        public PreferencesDto GetPreference(long id)
        {
            var preference = _context.Preferences.Where(x => x.Id == id).FirstOrDefault();
            var preferenceDto = new PreferencesDto()
            {
                Id = preference.Id,
                NovelId = preference.NovelId,
                AccountId = preference.AccountId,
            };

            return preferenceDto;
        }

        public async Task<List<PreferencesDto>> GetListPreference()
        {
            var listPreference = _context.Preferences.Select(x => new PreferencesDto()
            {
                Id = x.Id,
                NovelId = x.NovelId,
                AccountId = x.AccountId,
            }).ToList();

            return listPreference;
        }

        public async Task<ResponseInfo> UpdatePreference(long id, PreferencesCreateUpdateEntity preference)
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

                var existPreference = _context.Preferences.Where(x => x.Id == id).FirstOrDefault();
                if (existPreference is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existPreference.NovelId = preference.NovelId;
                existPreference.AccountId = preference.AccountId;

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