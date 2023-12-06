using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using WebNovel.API.Areas.Models.Bookmarked.Schemas;
using WebNovel.API.Areas.Models.Novels;
using WebNovel.API.Commons;
using WebNovel.API.Commons.CodeMaster;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Bookmarked
{
    public interface IBookmarkedModel
    {
        Task<List<BookmarkedDto>> GetListBookmarked();
        Task<ResponseInfo> AddBookmarked(BookmarkedCreateUpdateEntity Bookmarked);
        Task<ResponseInfo> UpdateBookmarked(BookmarkedCreateUpdateEntity Bookmarked);
        Task<ResponseInfo> RemoveBookmarked(BookmarkedDeleteEntity Bookmarked);
        Task<List<BookmarkedDto>> GetBookmarkedByAccount(string AccountId);
        Task<List<BookmarkedDto>> GetBookmarkedByNovel(string NovelId);
        Task<BookmarkedDto?> GetBookmarked(string AccountId, string NovelId);
    }
    public class BookmarkedModel : BaseModel, IBookmarkedModel
    {
        private readonly ILogger<IBookmarkedModel> _logger;
        private readonly INovelModel _novelModel;
        private string _className = "";
        public BookmarkedModel(IServiceProvider provider, ILogger<IBookmarkedModel> logger, INovelModel novelModel) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            _novelModel = novelModel;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddBookmarked(BookmarkedCreateUpdateEntity Bookmarked)
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

                var newBookmarked = new Databases.Entities.Bookmarked()
                {
                    NovelId = Bookmarked.NovelId,
                    AccountId = Bookmarked.AccountId,
                    ChapterId = Bookmarked.ChapterId,
                };

                var novel = await _context.Novel.Where(e => e.DelFlag == false).Where(e => e.Id == newBookmarked.NovelId).FirstAsync();
                novel.Views += 1;

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.BookMarked.AddAsync(newBookmarked);
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

        public async Task<List<BookmarkedDto>> GetBookmarkedByAccount(string AccountId)
        {
            var listBookmarked = await _context.BookMarked.Where(e => e.DelFlag == false).Where(e => e.AccountId == AccountId).Select(x => new BookmarkedDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                ChapterId = x.ChapterId,
            }).ToListAsync();

            foreach (var bookmarked in listBookmarked.ToList())
            {
                var Novel = await _novelModel.GetNovelAsync(bookmarked.NovelId);
                if (Novel is null)
                {
                    listBookmarked.Remove(bookmarked);
                    continue;
                }
                bookmarked.Name = Novel.Name;
                bookmarked.Title = Novel.Title;
                bookmarked.Author = Novel.Author;
                bookmarked.Year = Novel.Year;
                bookmarked.Views = Novel.Views;
                bookmarked.Rating = Novel.Rating;
                bookmarked.ImagesURL = Novel.ImagesURL;
                bookmarked.GenreName = Novel.GenreName;
                bookmarked.GenreIds = Novel.GenreIds;
                bookmarked.Description = Novel.Description;
                bookmarked.Status = Novel.Status;
                bookmarked.ApprovalStatus = Novel.ApprovalStatus;
                bookmarked.NumChapter = Novel.NumChapter;
            }

            return listBookmarked;
        }

        public async Task<List<BookmarkedDto>> GetBookmarkedByNovel(string NovelId)
        {
            var listBookmarked = await _context.BookMarked.Where(e => e.DelFlag == false).Where(e => e.NovelId == NovelId).Select(x => new BookmarkedDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                ChapterId = x.ChapterId,
            }).ToListAsync();

            return listBookmarked;
        }

        public async Task<BookmarkedDto?> GetBookmarked(string AccountId, string NovelId)
        {
            var Bookmarked = await _context.BookMarked.Where(e => e.DelFlag == false).Where(x => x.NovelId == NovelId && x.AccountId == AccountId).FirstOrDefaultAsync();
            if (Bookmarked is null)
            {
                return null;
            }
            var Novel = await _novelModel.GetNovelAsync(Bookmarked.NovelId);
            if (Novel is null)
            {
                return null;
            }
            var BookmarkedDto = new BookmarkedDto()
            {
                NovelId = Bookmarked.NovelId,
                AccountId = Bookmarked.AccountId,
                ChapterId = Bookmarked.ChapterId,
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

            return BookmarkedDto;
        }

        public async Task<List<BookmarkedDto>> GetListBookmarked()
        {
            var listBookmarked = await _context.BookMarked.Where(e => e.DelFlag == false).Select(x => new BookmarkedDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                ChapterId = x.ChapterId,
            }).ToListAsync();

            foreach (var bookmarked in listBookmarked.ToList())
            {
                var Novel = await _novelModel.GetNovelAsync(bookmarked.NovelId);
                if (Novel is null)
                {
                    listBookmarked.Remove(bookmarked);
                    continue;
                }
                bookmarked.Name = Novel.Name;
                bookmarked.Title = Novel.Title;
                bookmarked.Author = Novel.Author;
                bookmarked.Year = Novel.Year;
                bookmarked.Views = Novel.Views;
                bookmarked.Rating = Novel.Rating;
                bookmarked.ImagesURL = Novel.ImagesURL;
                bookmarked.GenreName = Novel.GenreName;
                bookmarked.GenreIds = Novel.GenreIds;
                bookmarked.Description = Novel.Description;
                bookmarked.Status = Novel.Status;
                bookmarked.ApprovalStatus = Novel.ApprovalStatus;
                bookmarked.NumChapter = Novel.NumChapter;
            }

            return listBookmarked;
        }

        public async Task<ResponseInfo> UpdateBookmarked(BookmarkedCreateUpdateEntity Bookmarked)
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

                var existBookmarked = _context.BookMarked.Where(e => e.DelFlag == false).Where(x => x.NovelId == Bookmarked.NovelId && x.AccountId == Bookmarked.AccountId).FirstOrDefault();
                if (existBookmarked is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existBookmarked.ChapterId = Bookmarked.ChapterId;

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

        public async Task<ResponseInfo> RemoveBookmarked(BookmarkedDeleteEntity Bookmarked)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();

            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existBookmarked = _context.BookMarked.Where(e => e.DelFlag == false).Where(x => x.NovelId == Bookmarked.NovelId && x.AccountId == Bookmarked.AccountId).FirstOrDefault();
                if (existBookmarked is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                _context.BookMarked.Remove(existBookmarked);

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