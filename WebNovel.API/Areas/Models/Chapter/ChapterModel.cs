using System.Runtime.CompilerServices;
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
        Task<ResponseInfo> AddChapter(IFormFile formFile, ChapterCreateUpdateEntity chapter);
        Task<ResponseInfo> UpdateChapter(long id, ChapterCreateUpdateEntity chapter, IFormFile formFile);
        Task<ChapterDto> GetChapterAsync(long id);
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
        public async Task<ResponseInfo> AddChapter(IFormFile formFile, ChapterCreateUpdateEntity chapter)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                var fileType = System.IO.Path.GetExtension(formFile.FileName);
                await _awsS3Service.UploadToS3(formFile, $"text{fileType}", chapter.NovelId.ToString() + "/" + chapter.Id.ToString());
                var fileName = $"text{fileType}";

                var newChapter = new Databases.Entities.Chapter()
                {
                    Name = chapter.Name,
                    IsLocked = chapter.IsLocked,
                    PublishDate = chapter.PublishDate,
                    Views = chapter.Views,
                    Rating = chapter.Rating,
                    FeeId = chapter.FeeId,
                    FileContent = fileName,
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

        public async Task<ChapterDto> GetChapterAsync(long id)
        {
            var chapter = _context.Chapter.Where(x => x.Id == id).FirstOrDefault();
            var novelDto = new ChapterDto()
            {
                Id = chapter.Id,
                Name = chapter.Name,
                IsLocked = chapter.IsLocked,
                PublishDate = chapter.PublishDate,
                Views = chapter.Views,
                Rating = chapter.Rating,
                FeeId = chapter.FeeId,
                FileContent = _awsS3Service.GetFileImg(chapter.NovelId.ToString() + "/" + chapter.Id.ToString(), $"{chapter.FileContent}"),
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
                FileContent = _awsS3Service.GetFileImg(x.NovelId.ToString() + "/" + x.Id.ToString(), $"{x.FileContent}"),
                Discount = x.Discount,
                ApprovalStatus = x.ApprovalStatus,
                NovelId = x.NovelId

            }).ToList();

            return listChapter;
        }

        public async Task<ResponseInfo> UpdateChapter(long id, ChapterCreateUpdateEntity chapter, IFormFile formFile)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existChapter = _context.Chapter.Where(n => n.Id == id).FirstOrDefault();
                if (existChapter is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                var fileNames = new List<string>
                {
                    existChapter.FileContent
                };
                var fileName = formFile.FileName;
                await _awsS3Service.DeleteFromS3(existChapter.NovelId.ToString() + "/" + existChapter.Id.ToString(), fileNames);
                await _awsS3Service.UploadToS3(formFile, existChapter.FileContent, existChapter.NovelId.ToString() + "/" + existChapter.Id.ToString());

                existChapter.Name = chapter.Name;
                existChapter.IsLocked = chapter.IsLocked;
                existChapter.PublishDate = chapter.PublishDate;
                existChapter.Views = chapter.Views;
                existChapter.Rating = chapter.Rating;
                existChapter.FeeId = chapter.FeeId;
                existChapter.Discount = chapter.Discount;
                existChapter.ApprovalStatus = chapter.ApprovalStatus;

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