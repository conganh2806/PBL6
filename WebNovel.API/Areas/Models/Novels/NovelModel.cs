using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Accounts.Schemas;
using WebNovel.API.Areas.Models.Novels.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entitites;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Novels
{
    public interface INovelModel 
    {
        Task<List<NovelDto>> GetListNovel(SearchCondition searchCondition);
        Task<ResponseInfo> AddNovel(NovelCreateUpdateEntity novel);
        Task<ResponseInfo> UpdateNovel(long id, NovelCreateUpdateEntity novel);
        NovelDto GetNovel (long id);

    }

    public class NovelModel : BaseModel, INovelModel
    {
        private readonly ILogger<INovelModel> _logger;
        
        private string _className = "";
        public NovelModel(IServiceProvider provider, ILogger<INovelModel> logger) : base(provider) {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;

        public async Task<ResponseInfo> AddNovel(NovelCreateUpdateEntity novel)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try 
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo results = new ResponseInfo();
                if(results.Code != CodeResponse.OK) {
                    return results;
                }

                var newNovel = new Novel() {
                    Name = novel.Name,
                    Title = novel.Title,
                    AccountId = novel.AccountId,
                    Year = novel.Year,
                    Views = novel.Views,
                    Rating = novel.Rating,
                    Description = novel.Description,
                    Status = novel.Status,
                    ApprovalStatus = novel.ApprovalStatus
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Novel.AddAsync(newNovel);
                            await _context.SaveChangesAsync();
                            await trn.CommitAsync();
                        }
                    }

                );


                _logger.LogInformation($"[{_className}][{method}] End");
                return results;

                

            }
            catch(Exception e) {
                if (transaction != null)
                {
                    await _context.RollbackAsync(transaction);
                }
                _logger.LogError($"[{_className}][{method}] Exception: {e.Message}");
                throw;
            }
        }

        public async Task<List<NovelDto>> GetListNovel(SearchCondition searchCondition)
        {
            var listNovel = _context.Novel.Select(x => new NovelDto()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                Author = _context.Accounts.Where(e => e.Id == x.AccountId).FirstOrDefault().NickName,
                Year = x.Year,
                Views = x.Views,
                Rating = x.Rating,
                Description = x.Description,
                Status = x.Status,
                ApprovalStatus = x.ApprovalStatus
                


            }).ToList();

            return listNovel;

        }

        public NovelDto GetNovel(long id)
        {
            var novel = _context.Novel.Where(x => x.Id == id).FirstOrDefault();
            var novelDto = new NovelDto() 
            {
                Id = novel.Id,
                Name = novel.Name,
                Title = novel.Title,
                Author = _context.Accounts.Where(n => n.Id == novel.AccountId).FirstOrDefault().NickName,
                Year = novel.Year,
                Views = novel.Views,
                Rating = novel.Rating,
                Description = novel.Description,
                Status = novel.Status,
                ApprovalStatus = novel.ApprovalStatus

            };

            return novelDto;
        }

        public async Task<ResponseInfo> UpdateNovel(long id, NovelCreateUpdateEntity novel)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                if (result.Code != CodeResponse.OK)
                {
                    return result;
                }

                var existNovel = _context.Novel.Where(n => n.Id == id).FirstOrDefault();
                if(existNovel is null) {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existNovel.Name = novel.Name;
                existNovel.Title = novel.Title;
                existNovel.Year = novel.Year;
                existNovel.Views = novel.Views;
                existNovel.Rating = novel.Rating;
                existNovel.Description = novel.Description;
                existNovel.Status = novel.Status;
                existNovel.ApprovalStatus = novel.Status;


                transaction = await _context.Database.BeginTransactionAsync();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                _logger.LogInformation($"[{_className}][{method}] End");
                return result;

            }
            catch(Exception e) {
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