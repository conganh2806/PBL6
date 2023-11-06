using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Comment.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Models.Comment
{
    public interface ICommentModel
    {
        Task<List<CommentDto>> GetListComment();
        Task<ResponseInfo> AddComment(CommentCreateUpdateEntity comment);
        Task<List<CommentDto>> GetCommentByAccount(string accountId);
        Task<List<CommentDto>> GetCommentByNovel(string NovelId);
        Task<List<CommentDto>> GetCommentByAccountNovel(string accountId, string novelId);
        Task<CommentDto> GetComment(long Id);
        Task<ResponseInfo> UpdateComment(long Id, CommentCreateUpdateEntity comment);
    }
    public class CommentModel : BaseModel, ICommentModel
    {
        private readonly ILogger<ICommentModel> _logger;
        private string _className = "";
        public CommentModel(IServiceProvider provider, ILogger<ICommentModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;

        public async Task<ResponseInfo> AddComment(CommentCreateUpdateEntity comment)
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

                var newComment = new Databases.Entities.Comment()
                {
                    NovelId = comment.NovelId,
                    AccountId = comment.AccountId,
                    Text = comment.Text,
                    CreateOn = DateTime.Now,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Comment.AddAsync(newComment);
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

        public async Task<List<CommentDto>> GetCommentByAccount(string accountId)
        {
            var listComment = await _context.Comment.Where(e => e.AccountId == accountId).Select(x => new CommentDto()
            {
                Id = x.Id,
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                Text = x.Text,
                CreateOn = x.CreateOn,
            }).ToListAsync();

            return listComment;
        }

        public async Task<List<CommentDto>> GetCommentByNovel(string novelId)
        {
            var listComment = await _context.Comment.Where(e => e.NovelId == novelId).Select(x => new CommentDto()
            {
                Id = x.Id,
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                Text = x.Text,
                CreateOn = x.CreateOn,
            }).ToListAsync();

            return listComment;
        }

        public async Task<List<CommentDto>> GetCommentByAccountNovel(string accountId, string novelId)
        {
            var listComment = await _context.Comment.Where(e => e.NovelId == novelId && e.AccountId == accountId).Select(x => new CommentDto()
            {
                Id = x.Id,
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                Text = x.Text,
                CreateOn = x.CreateOn,
            }).ToListAsync();

            return listComment;
        }

        public async Task<CommentDto> GetComment(long Id)
        {
            var Comment = await _context.Comment.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (Comment is not null)
            {
                var CommentDto = new CommentDto()
                {
                    NovelId = Comment.NovelId,
                    AccountId = Comment.AccountId,
                    Text = Comment.Text,
                    CreateOn = Comment.CreateOn,
                };
                return CommentDto;
            }
            return new CommentDto();
        }

        public async Task<List<CommentDto>> GetListComment()
        {
            var listComment = await _context.Comment.Select(x => new CommentDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                Text = x.Text,
                CreateOn = x.CreateOn,
            }).ToListAsync();

            return listComment;
        }

        public async Task<ResponseInfo> UpdateComment(long Id, CommentCreateUpdateEntity comment)
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

                var existComment = _context.Comment.Where(x => x.Id == Id).FirstOrDefault();
                if (existComment is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existComment.Text = comment.Text;

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