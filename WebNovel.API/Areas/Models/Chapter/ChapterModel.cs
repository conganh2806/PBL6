using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Chapter.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Models.Chapter
{

    public interface IChapterModel  
    {
        Task<List<ChapterDto>> GetListChapter();
        Task<ResponseInfo> AddChapter(ChapterCreateUpdateEntity chapter);
        Task<ResponseInfo> UpdateChapter(long id, ChapterCreateUpdateEntity chapter);
        ChapterDto GetChapter(long id);
    }

    public class ChapterModel : BaseModel, IChapterModel
    {
        private readonly ILogger<IChapterModel> _logger;

        private string _className = "";
        public ChapterModel(IServiceProvider provider, ILogger<IChapterModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }
        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;
        public async Task<ResponseInfo> AddChapter(ChapterCreateUpdateEntity chapter)
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

                var newChapter = new Databases.Entities.Chapter()
                {
                    Name = chapter.Name,
                    IsLocked = chapter.IsLocked,
                    PublishDate = chapter.PublishDate,
                    Views = chapter.Views,
                    Rating = chapter.Rating,
                    FeeId = chapter.FeeId,
                    FileContent = chapter.FileContent,
                    Discount = chapter.Discount,
                    ApprovalStatus = chapter.ApprovalStatus,
                    NovelId = chapter.NovelId
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () => 
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Chapter.AddAsync(newChapter);
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

        public ChapterDto GetChapter(long id)
        {
            var chapter = _context.Chapter.Where(x => x.Id == id).FirstOrDefault();
            var novelDto = new ChapterDto()
            {
                Name = chapter.Name,
                IsLocked = chapter.IsLocked,
                PublishDate = chapter.PublishDate,
                Views = chapter.Views,
                Rating = chapter.Rating,
                FeeId = chapter.FeeId,
                FileContent = chapter.FileContent,
                Discount = chapter.Discount,
                ApprovalStatus = chapter.ApprovalStatus,
                NovelId = chapter.NovelId

            };

            return novelDto;
        }


        public async Task<List<ChapterDto>> GetListChapter()
        {
            List<ChapterDto> listChapter = new List<ChapterDto>();
            
            listChapter = _context.Chapter.Select(x => new ChapterDto()
            {
                Id = x.Id,
                Name = x.Name,
                IsLocked = x.IsLocked,
                PublishDate = x.PublishDate,
                Views = x.Views,
                Rating = x.Rating,
                FeeId = x.FeeId,
                FileContent = x.FileContent,
                Discount = x.Discount,
                ApprovalStatus = x.ApprovalStatus,
                NovelId = x.NovelId

            }).ToList();

            return listChapter;
        }

        public async Task<ResponseInfo> UpdateChapter(long id, ChapterCreateUpdateEntity chapter)
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

                var existChapter = _context.Chapter.Where(n => n.Id == id).FirstOrDefault();
                if (existChapter is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existChapter.Name = chapter.Name;
                existChapter.IsLocked = chapter.IsLocked;
                existChapter.PublishDate = chapter.PublishDate;
                existChapter.Views = chapter.Views;
                existChapter.Rating = chapter.Rating;
                existChapter.FeeId = chapter.FeeId;
                existChapter.FileContent = chapter.FileContent;
                existChapter.Discount = chapter.Discount;
                existChapter.ApprovalStatus = chapter.ApprovalStatus;
                existChapter.NovelId = chapter.NovelId;


                transaction = await _context.Database.BeginTransactionAsync();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


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