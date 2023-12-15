using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Accounts;
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
        Task<ResponseInfo> AddComment(CommentCreateEntity comment);
        Task<List<CommentDto>> GetCommentByAccount(string accountId);
        Task<List<CommentDto>> GetCommentByNovel(string NovelId);
        Task<List<CommentDto>> GetCommentByAccountNovel(string accountId, string novelId);
        Task<CommentDto?> GetComment(long Id);
        Task<ResponseInfo> UpdateComment(CommentUpdateEntity comment);
        Task<ResponseInfo> RemoveComment(CommentDeleteEntity comment);
    }
    public class CommentModel : BaseModel, ICommentModel
    {
        private readonly ILogger<ICommentModel> _logger;
        private readonly IAccountModel _accountModel;
        private string _className = "";
        public CommentModel(IServiceProvider provider, ILogger<ICommentModel> logger, IAccountModel accountModel) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            _accountModel = accountModel;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;

        public async Task<ResponseInfo> AddComment(CommentCreateEntity comment)
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
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Comment.AddAsync(newComment);
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

        public async Task<List<CommentDto>> GetCommentByAccount(string accountId)
        {
            var listComment = await _context.Comment.Where(e => e.DelFlag == false).Where(e => e.AccountId == accountId).Select(x => new CommentDto()
            {
                Id = x.Id,
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                Text = x.Text,
                CreateOn = x.CreateOn,
            }).ToListAsync();

            foreach (var comment in listComment.ToList())
            {
                var Novel = await _context.Novel.Where(e => e.DelFlag == false).Where(e => e.Id == comment.NovelId).FirstOrDefaultAsync();
                if (Novel is null)
                {
                    listComment.Remove(comment);
                    continue;
                }
                var Account = await _accountModel.GetAccount(comment.AccountId);
                if (Account is not null)
                {
                    comment.Username = Account.Username;
                    comment.Email = Account.Email;
                    comment.NickName = Account.NickName;
                    comment.RoleIds = Account.RoleIds;
                    comment.AccountImagesURL = Account.ImagesURL;
                }
            }

            return listComment;
        }

        public async Task<List<CommentDto>> GetCommentByNovel(string novelId)
        {
            var listComment = await _context.Comment.Where(e => e.DelFlag == false).Where(e => e.NovelId == novelId).Select(x => new CommentDto()
            {
                Id = x.Id,
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                Text = x.Text,
                CreateOn = x.CreateOn,
            }).ToListAsync();

            foreach (var comment in listComment.ToList())
            {
                var Account = await _accountModel.GetAccount(comment.AccountId);
                if (Account is null)
                {
                    comment.Username = "[Deleted]";
                    comment.Email = "[Deleted]";
                    comment.NickName = "[Deleted]";
                    comment.RoleIds = null;
                    comment.AccountImagesURL = null;
                    continue;
                }
                comment.Username = Account.Username;
                comment.Email = Account.Email;
                comment.NickName = Account.NickName;
                comment.RoleIds = Account.RoleIds;
                comment.AccountImagesURL = Account.ImagesURL;
            }

            return listComment;
        }

        public async Task<List<CommentDto>> GetCommentByAccountNovel(string accountId, string novelId)
        {
            var listComment = await _context.Comment.Where(e => e.DelFlag == false).Where(e => e.NovelId == novelId && e.AccountId == accountId).Select(x => new CommentDto()
            {
                Id = x.Id,
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                Text = x.Text,
                CreateOn = x.CreateOn,
            }).ToListAsync();

            foreach (var comment in listComment.ToList())
            {
                var Account = await _accountModel.GetAccount(comment.AccountId);
                if (Account is not null)
                {
                    comment.Username = Account.Username;
                    comment.Email = Account.Email;
                    comment.NickName = Account.NickName;
                    comment.RoleIds = Account.RoleIds;
                    comment.AccountImagesURL = Account.ImagesURL;
                }
            }

            return listComment;
        }

        public async Task<CommentDto?> GetComment(long Id)
        {
            var Comment = await _context.Comment.Where(e => e.DelFlag == false).Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (Comment is not null)
            {
                var Novel = await _context.Novel.Where(e => e.DelFlag == false).Where(e => e.Id == Comment.NovelId).FirstOrDefaultAsync();
                if (Novel is null)
                {
                    return null;
                }
                var Account = await _accountModel.GetAccount(Comment.AccountId);
                if (Account is null)
                {
                    return null;
                }
                var CommentDto = new CommentDto()
                {
                    Id = Comment.Id,
                    NovelId = Comment.NovelId,
                    AccountId = Comment.AccountId,
                    Text = Comment.Text,
                    Username = Account.Username,
                    Email = Account.Email,
                    NickName = Account.NickName,
                    RoleIds = Account.RoleIds,
                    CreateOn = Comment.CreateOn,
                    AccountImagesURL = Account.ImagesURL,
                };
                return CommentDto;
            }
            return null;
        }

        public async Task<List<CommentDto>> GetListComment()
        {
            var listComment = await _context.Comment.Where(e => e.DelFlag == false).Select(x => new CommentDto()
            {
                NovelId = x.NovelId,
                AccountId = x.AccountId,
                Text = x.Text,
                CreateOn = x.CreateOn,
            }).ToListAsync();

            return listComment;
        }

        public async Task<ResponseInfo> UpdateComment(CommentUpdateEntity comment)
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

                var existComment = _context.Comment.Where(e => e.DelFlag == false).Where(x => x.Id == comment.Id).FirstOrDefault();
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

        public async Task<ResponseInfo> RemoveComment(CommentDeleteEntity comment)
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

                var existComment = _context.Comment.Where(e => e.DelFlag == false).Where(x => x.Id == comment.Id).FirstOrDefault();
                if (existComment is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                _context.Comment.Remove(existComment);

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