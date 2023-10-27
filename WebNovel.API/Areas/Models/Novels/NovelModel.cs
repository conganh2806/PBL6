using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Accounts.Schemas;
using WebNovel.API.Areas.Models.Novels.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using WebNovel.API.Databases.Entitites;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Novels
{
    public interface INovelModel
    {
        Task<List<NovelDto>> GetListNovel(SearchCondition searchCondition);
        Task<ResponseInfo> AddNovel(NovelCreateUpdateEntity novel);
        Task<ResponseInfo> UpdateNovel(long id, NovelCreateUpdateEntity novel);
        NovelDto GetNovel(long id);

    }

    public class NovelModel : BaseModel, INovelModel
    {
        private readonly ILogger<INovelModel> _logger;

        private string _className = "";
        public NovelModel(IServiceProvider provider, ILogger<INovelModel> logger) : base(provider)
        {

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
                if (results.Code != CodeResponse.OK)
                {
                    return results;
                }

                var newNovel = new Novel()
                {
                    Name = novel.Name,
                    Title = novel.Title,
                    AccountId = novel.AccountId,
                    Year = novel.Year,
                    Views = novel.Views,
                    Rating = novel.Rating,
                    Description = novel.Description,
                    Status = novel.Status,
                    ApprovalStatus = novel.ApprovalStatus,
                    ImageURL = novel.ImagesURL
                };

                if(novel.GenreIds.Any()) {
                    foreach(var genreId in novel.GenreIds) {
                        newNovel.Genres.Add (new NovelGenre(){
                            NovelId = novel.Id,
                            GenreId = genreId
                        });
                    }
                }

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

        public async Task<List<NovelDto>> GetListNovel(SearchCondition searchCondition)
        {
            
            List<NovelDto> listNovel =  new List<NovelDto>();
            
            if (searchCondition is null)
            {
                var novels = await _context.Novel.ToListAsync();
                var novelDtoTasks  =  novels.Select(async x => new NovelDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Title = x.Title,
                    Author = (await _context.Accounts.FirstOrDefaultAsync(e => e.Id == x.AccountId))?.NickName,
                    Year = x.Year,
                    Views = x.Views,
                    ImagesURL = x.ImageURL,
                    Rating = x.Rating,
                    Description = x.Description,
                    Status = x.Status,
                    ApprovalStatus = x.ApprovalStatus,

                    GenreName = await _context.GenreOfNovels
                                        .Where(gn => gn.NovelId == x.Id)
                                        .Select(gn => gn.Genre.Name)
                                        .ToListAsync()




                });


                var novelDtoList = await Task.WhenAll(novelDtoTasks);
                listNovel = novelDtoList.ToList();
                
            }

            return listNovel;

        }

        public NovelDto GetNovel(long id)
        {
            var novel = _context.Novel.Include(x => x.Genres).ThenInclude(x => x.Genre).Where(x => x.Id == id).FirstOrDefault();
            var genres = _context.Genre.Where(t => novel.Genres.Select(x => x.GenreId).ToList().Contains(t.Id)).ToList();
            

            var novelDto = new NovelDto()
            {
                Id = novel.Id,
                Name = novel.Name,
                Title = novel.Title,
                Author = _context.Accounts.Where(n => n.Id == novel.AccountId).FirstOrDefault().NickName,
                Year = novel.Year,
                Views = novel.Views,
                ImagesURL = novel.ImageURL,
                Rating = novel.Rating,
                Description = novel.Description,
                Status = novel.Status,
                ApprovalStatus = novel.ApprovalStatus,
                GenreIds = genres.Select(x => x.Id).ToList(),
                GenreName = genres.Select(x => x.Name).ToList()
            };

            return novelDto;
        }

        public async Task<ResponseInfo> UpdateNovel(long id, NovelCreateUpdateEntity novel)
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

                var existNovel = _context.Novel.Where(n => n.Id == id).FirstOrDefault();
                if (existNovel is null)
                {
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
                existNovel.ImageURL = novel.ImagesURL;
                _context.GenreOfNovels.RemoveRange(_context.GenreOfNovels.Where(x => x.NovelId == existNovel.Id));
                foreach (var genreId in novel.GenreIds)
                {
                    existNovel.Genres.Add(new NovelGenre()
                    {
                        NovelId = existNovel.Id,
                        GenreId = genreId
                    });
                }


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