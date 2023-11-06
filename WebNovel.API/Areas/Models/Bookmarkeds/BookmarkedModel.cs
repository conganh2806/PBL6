using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using WebNovel.API.Areas.Models.Bookmarked.Schemas;
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
        Task<ResponseInfo> UpdateBookmarked(string AccountId, string NovelId, BookmarkedCreateUpdateEntity Bookmarked);
        Task<List<BookmarkedDto>> GetBookmarkedByAccount(string AccountId);
        Task<List<BookmarkedDto>> GetBookmarkedByNovel(string NovelId);
        Task<BookmarkedDto> GetBookmarked(string AccountId, string NovelId);
    }
    public class BookmarkedModel : BaseModel, IBookmarkedModel
    {
        private readonly ILogger<IBookmarkedModel> _logger;
        private string _className = "";
        public BookmarkedModel(IServiceProvider provider, ILogger<IBookmarkedModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddBookmarked(BookmarkedCreateUpdateEntity Bookmarked)
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

                var newBookmarked = new Databases.Entities.Bookmarked()
                {
                    NovelId = Bookmarked.NovelId,
                    AccountId = Bookmarked.AccountId,
                    ChapterId = Bookmarked.ChapterId,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.BookMarked.AddAsync(newBookmarked);
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

        public async Task<List<BookmarkedDto>> GetBookmarkedByAccount(string AccountId)
        {
            var listBookmarked = await _context.BookMarked.Where(e => e.AccountId == AccountId).Select(x => new BookmarkedDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                ChapterId = x.ChapterId,
            }).ToListAsync();

            return listBookmarked;
        }

        public async Task<List<BookmarkedDto>> GetBookmarkedByNovel(string NovelId)
        {
            var listBookmarked = await _context.BookMarked.Where(e => e.NovelId == NovelId).Select(x => new BookmarkedDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                ChapterId = x.ChapterId,
            }).ToListAsync();

            return listBookmarked;
        }

        public async Task<BookmarkedDto> GetBookmarked(string AccountId, string NovelId)
        {
            var Bookmarked = await _context.BookMarked.Where(x => x.NovelId == NovelId && x.AccountId == AccountId).FirstOrDefaultAsync();
            var BookmarkedDto = new BookmarkedDto()
            {
                NovelId = Bookmarked.NovelId,
                AccountId = Bookmarked.AccountId,
                ChapterId = Bookmarked.ChapterId,
            };

            return BookmarkedDto;
        }

        public async Task<List<BookmarkedDto>> GetListBookmarked()
        {
            var listBookmarked = await _context.BookMarked.Select(x => new BookmarkedDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                ChapterId = x.ChapterId,
            }).ToListAsync();

            return listBookmarked;
        }

        public async Task<ResponseInfo> UpdateBookmarked(string AccountId, string NovelId, BookmarkedCreateUpdateEntity Bookmarked)
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

                var existBookmarked = _context.BookMarked.Where(x => x.NovelId == NovelId && x.AccountId == AccountId).FirstOrDefault();
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