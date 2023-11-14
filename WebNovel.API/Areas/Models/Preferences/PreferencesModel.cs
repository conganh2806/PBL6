using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using WebNovel.API.Areas.Models.Novels;
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
        Task<ResponseInfo> RemovePreference(PreferencesDeleteEntity account);
        Task<List<PreferencesDto>> GetPreferenceByAccount(string AccountId);
        Task<List<PreferencesDto>> GetPreferenceByNovel(string NovelId);
        Task<PreferencesDto?> GetPreference(string AccountId, string NovelId);
    }
    public class PreferencesModel : BaseModel, IPreferencesModel
    {
        private readonly ILogger<IPreferencesModel> _logger;
        private readonly INovelModel _novelModel;
        private string _className = "";
        public PreferencesModel(IServiceProvider provider, ILogger<IPreferencesModel> logger, INovelModel novelModel) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            _novelModel = novelModel;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddPreference(PreferencesCreateUpdateEntity preference)
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

                var newPreference = new Databases.Entities.Preferences()
                {
                    NovelId = preference.NovelId,
                    AccountId = preference.AccountId,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Preferences.AddAsync(newPreference);
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

        public async Task<List<PreferencesDto>> GetPreferenceByAccount(string AccountId)
        {
            var listPreference = await _context.Preferences.Where(e => e.DelFlag == false).Where(e => e.AccountId == AccountId).Select(x => new PreferencesDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
            }).ToListAsync();

            foreach (var preference in listPreference)
            {
                var Novel = await _novelModel.GetNovelAsync(preference.NovelId);
                preference.Name = Novel.Name;
                preference.Title = Novel.Title;
                preference.Author = Novel.Author;
                preference.Year = Novel.Year;
                preference.Views = Novel.Views;
                preference.Rating = Novel.Rating;
                preference.ImagesURL = Novel.ImagesURL;
                preference.GenreName = Novel.GenreName;
                preference.GenreIds = Novel.GenreIds;
                preference.Description = Novel.Description;
                preference.Status = Novel.Status;
                preference.ApprovalStatus = Novel.ApprovalStatus;
                preference.NumChapter = Novel.NumChapter;
            }

            return listPreference;
        }

        public async Task<List<PreferencesDto>> GetPreferenceByNovel(string NovelId)
        {
            var listPreference = await _context.Preferences.Where(e => e.DelFlag == false).Where(e => e.NovelId == NovelId).Select(x => new PreferencesDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
            }).ToListAsync();

            foreach (var preference in listPreference)
            {
                var Novel = await _novelModel.GetNovelAsync(preference.NovelId);
                preference.Name = Novel.Name;
                preference.Title = Novel.Title;
                preference.Author = Novel.Author;
                preference.Year = Novel.Year;
                preference.Views = Novel.Views;
                preference.Rating = Novel.Rating;
                preference.ImagesURL = Novel.ImagesURL;
                preference.GenreName = Novel.GenreName;
                preference.GenreIds = Novel.GenreIds;
                preference.Description = Novel.Description;
                preference.Status = Novel.Status;
                preference.ApprovalStatus = Novel.ApprovalStatus;
                preference.NumChapter = Novel.NumChapter;
            }

            return listPreference;
        }

        public async Task<PreferencesDto?> GetPreference(string AccountId, string NovelId)
        {
            var preference = await _context.Preferences.Where(e => e.DelFlag == false).Where(x => x.NovelId == NovelId && x.AccountId == AccountId).FirstOrDefaultAsync();
            if (preference is null)
            {
                return null;
            }
            var Novel = await _novelModel.GetNovelAsync(preference.NovelId);
            var preferenceDto = new PreferencesDto
            {
                NovelId = preference.NovelId,
                AccountId = preference.AccountId,
                Name = Novel.Name,
                Title = Novel.Title,
                Author = Novel.Author,
                Year = Novel.Year,
                Views = Novel.Views,
                Rating = Novel.Rating,
                ImagesURL = Novel.ImagesURL,
                GenreName = Novel.GenreName,
                GenreIds = Novel.GenreIds,
                Description = Novel.Description,
                Status = Novel.Status,
                ApprovalStatus = Novel.ApprovalStatus,
                NumChapter = Novel.NumChapter,
            };

            return preferenceDto;
        }

        public async Task<List<PreferencesDto>> GetListPreference()
        {
            var listPreference = _context.Preferences.Where(e => e.DelFlag == false).Select(x => new PreferencesDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
            }).ToList();

            foreach (var preference in listPreference)
            {
                var Novel = await _novelModel.GetNovelAsync(preference.NovelId);
                preference.Name = Novel.Name;
                preference.Title = Novel.Title;
                preference.Author = Novel.Author;
                preference.Year = Novel.Year;
                preference.Views = Novel.Views;
                preference.Rating = Novel.Rating;
                preference.ImagesURL = Novel.ImagesURL;
                preference.GenreName = Novel.GenreName;
                preference.GenreIds = Novel.GenreIds;
                preference.Description = Novel.Description;
                preference.Status = Novel.Status;
                preference.ApprovalStatus = Novel.ApprovalStatus;
                preference.NumChapter = Novel.NumChapter;
            }

            return listPreference;
        }

        public async Task<ResponseInfo> RemovePreference(PreferencesDeleteEntity preference)
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

                var existPreference = _context.Preferences.Where(e => e.DelFlag == false).Where(x => x.NovelId == preference.NovelId && x.AccountId == preference.AccountId).FirstOrDefault();
                if (existPreference is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                _context.Preferences.Remove(existPreference);

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