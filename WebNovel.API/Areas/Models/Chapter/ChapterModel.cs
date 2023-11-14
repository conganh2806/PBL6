using System.Runtime.CompilerServices;
using CSharpVitamins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Chapter.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Core.Services;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Models.Chapter
{

    public interface IChapterModel
    {
        Task<List<ChapterDto>> GetListChapter();
        Task<ResponseInfo> AddChapter(ChapterCreateEntity chapter);
        Task<ResponseInfo> UpdateChapter(ChapterUpdateEntity chapter);
        Task<ResponseInfo> RemoveChapter(ChapterDeleteEntity chapter);
        Task<ChapterDto?> GetChapterAsync(string id);
        Task<List<ChapterDto>> GetChapterByNovel(string NovelId);
    }

    public class ChapterModel : BaseModel, IChapterModel
    {
        private readonly ILogger<IChapterModel> _logger;
        private readonly IAwsS3Service _awsS3Service;

        private string _className = "";
        public ChapterModel(IServiceProvider provider, ILogger<IChapterModel> logger, IAwsS3Service awsS3Service) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            _awsS3Service = awsS3Service;
        }
        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;
        public async Task<ResponseInfo> AddChapter(ChapterCreateEntity chapter)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {

                var GuID = (ShortGuid)Guid.NewGuid();

                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                var fileType = System.IO.Path.GetExtension(chapter.File.FileName);
                await _awsS3Service.UploadToS3(chapter.File, $"text{fileType}", chapter.NovelId.ToString() + "/" + GuID.ToString());
                var fileName = $"text{fileType}";

                var newChapter = new Databases.Entities.Chapter()
                {
                    Id = GuID.ToString(),
                    Name = chapter.Name,
                    IsLocked = false,
                    PublishDate = DateTime.Now,
                    IsPublished = false,
                    Views = 0,
                    Rating = 0,
                    FileContent = fileName,
                    ApprovalStatus = false,
                    NovelId = chapter.NovelId
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Chapter.AddAsync(newChapter);
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

        public async Task<ChapterDto?> GetChapterAsync(string id)
        {
            var chapter = await _context.Chapter.Where(e => e.DelFlag == false).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (chapter is null)
            {
                return null;
            }
            var chapterDto = new ChapterDto()
            {
                Id = chapter.Id,
                Name = chapter.Name,
                IsLocked = chapter.IsLocked,
                PublishDate = chapter.PublishDate,
                IsPublished = chapter.IsPublished,
                Views = chapter.Views,
                FeeId = chapter.FeeId,
                FileContent = _awsS3Service.GetFileImg(chapter.NovelId.ToString() + "/" + chapter.Id.ToString(), $"{chapter.FileContent}"),
                Discount = chapter.Discount,
                ApprovalStatus = chapter.ApprovalStatus,
                NovelId = chapter.NovelId
            };

            var listChapter = await _context.Chapter.Where(x => x.NovelId == chapter.NovelId).OrderBy(e => e.PublishDate).ToListAsync();
            chapterDto.ChapIndex = listChapter.FindIndex(a => a.Id == chapter.Id) + 1;

            return chapterDto;
        }

        public async Task<List<ChapterDto>> GetChapterByNovel(string NovelId)
        {
            List<ChapterDto> listChapter = new List<ChapterDto>();

            listChapter = await _context.Chapter.Where(e => e.DelFlag == false).Where(x => x.NovelId == NovelId).OrderBy(e => e.PublishDate).Select(x => new ChapterDto()
            {
                Id = x.Id,
                Name = x.Name,
                IsLocked = x.IsLocked,
                PublishDate = x.PublishDate,
                IsPublished = x.IsPublished,
                Views = x.Views,
                FeeId = x.FeeId,
                FileContent = _awsS3Service.GetFileImg(x.NovelId.ToString() + "/" + x.Id.ToString(), $"{x.FileContent}"),
                Discount = x.Discount,
                ApprovalStatus = x.ApprovalStatus,
                NovelId = x.NovelId,
            }).ToListAsync();

            for (int i = 0; i < listChapter.Count; i++)
            {
                listChapter[i].ChapIndex = i + 1;
            }

            return listChapter;
        }

        public async Task<List<ChapterDto>> GetListChapter()
        {
            List<ChapterDto> listChapter = new List<ChapterDto>();

            listChapter = await _context.Chapter.Where(e => e.DelFlag == false).Select(x => new ChapterDto()
            {
                Id = x.Id,
                Name = x.Name,
                IsLocked = x.IsLocked,
                PublishDate = x.PublishDate,
                Views = x.Views,
                FeeId = x.FeeId,
                FileContent = _awsS3Service.GetFileImg(x.NovelId.ToString() + "/" + x.Id.ToString(), $"{x.FileContent}"),
                Discount = x.Discount,
                ApprovalStatus = x.ApprovalStatus,
                NovelId = x.NovelId

            }).ToListAsync();

            return listChapter;
        }

        public async Task<ResponseInfo> RemoveChapter(ChapterDeleteEntity chapter)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();

            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existChapter = _context.Chapter.Where(e => e.DelFlag == false).Where(n => n.Id == chapter.Id).FirstOrDefault();
                if (existChapter is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                _context.Chapter.Remove(existChapter);

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

        public async Task<ResponseInfo> UpdateChapter(ChapterUpdateEntity chapter)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existChapter = _context.Chapter.Where(e => e.DelFlag == false).Where(n => n.Id == chapter.Id).FirstOrDefault();
                if (existChapter is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                // if (existChapter.IsPublished)
                // {
                //     response.Code = CodeResponse.BAD_REQUEST;
                //     response.MsgNo = "Chapter is published";
                //     return response;
                // }

                if (chapter.File is not null)
                {
                    var fileNames = new List<string>
                    {
                        existChapter.FileContent
                    };
                    var fileName = chapter.File.FileName;
                    await _awsS3Service.DeleteFromS3(existChapter.NovelId.ToString() + "/" + existChapter.Id.ToString(), fileNames);
                    await _awsS3Service.UploadToS3(chapter.File, existChapter.FileContent, existChapter.NovelId.ToString() + "/" + existChapter.Id.ToString());
                }

                if (chapter.Name is not null) existChapter.Name = chapter.Name;
                if (chapter.IsLocked is not null) existChapter.IsLocked = (bool)chapter.IsLocked;
                if (chapter.Views is not null) existChapter.Views = (int)chapter.Views;
                if (chapter.FeeId is not null) existChapter.FeeId = chapter.FeeId;
                if (chapter.Discount is not null) existChapter.Discount = chapter.Discount;
                if (chapter.ApprovalStatus is not null) existChapter.ApprovalStatus = (bool)chapter.ApprovalStatus;
                if (chapter.IsPublished is not null) existChapter.IsPublished = (bool)chapter.IsPublished;

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