using System.Runtime.CompilerServices;
using CSharpVitamins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Accounts.Schemas;
using WebNovel.API.Areas.Models.Novels.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Core.Services;
using WebNovel.API.Core.Services.Schemas;
using WebNovel.API.Databases.Entities;
using WebNovel.API.Databases.Entitites;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Novels
{
    public interface INovelModel
    {
        Task<List<NovelDto>> GetListNovel(SearchCondition searchCondition);
        Task<ResponseInfo> AddNovel(NovelCreateEntity novel);
        Task<ResponseInfo> UpdateNovel(NovelUpdateEntity novel);
        Task<ResponseInfo> RemoveNovel(NovelDeleteEntity novel);
        Task<NovelDto?> GetNovelAsync(string id);
        Task<List<NovelDto>> GetListNovelByGenreId(long genreId);
        Task<List<NovelDto>> GetListRecommendedNovel(string accountId);
        Task<List<NovelDto>> GetListNewestNovel();
        Task<List<NovelDto>> GetListNovelByAccountId(string accountId);
    }

    public class NovelModel : BaseModel, INovelModel
    {
        private readonly ILogger<INovelModel> _logger;
        private readonly IAwsS3Service _awsS3Service;

        private string _className = "";
        public NovelModel(IServiceProvider provider, ILogger<INovelModel> logger, IAwsS3Service awsS3Service) : base(provider)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            _awsS3Service = awsS3Service;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;

        public async Task<ResponseInfo> AddNovel(NovelCreateEntity novel)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                var GuID = (ShortGuid)Guid.NewGuid();

                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo results = new ResponseInfo();
                var fileType = System.IO.Path.GetExtension(novel.File.FileName);
                await _awsS3Service.UploadToS3(novel.File, $"thumbnail{fileType}", GuID.ToString());
                var fileName = $"thumbnail{fileType}";
                var newNovel = new Novel()
                {
                    Id = GuID.ToString(),
                    Name = novel.Name,
                    Title = novel.Title,
                    AccountId = novel.AccountId,
                    Year = DateTime.Now.Year,
                    Views = 0,
                    Rating = 0,
                    Description = novel.Description,
                    Status = true,
                    ApprovalStatus = false,
                    ImageURL = fileName,
                };

                if (novel.GenreIds.Any())
                {
                    foreach (var genreId in novel.GenreIds)
                    {
                        newNovel.Genres.Add(new NovelGenre()
                        {
                            NovelId = newNovel.Id,
                            GenreId = genreId
                        });
                    }
                }

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Novel.AddAsync(newNovel);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
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

            List<NovelDto> listNovel = new List<NovelDto>();

            var novels = await _context.Novel.Where(e => e.DelFlag == false).Include(x => x.Genres).Include(x => x.Account)
            .Where(x => string.IsNullOrEmpty(searchCondition.Key)
                        || x.Title.Contains(searchCondition.Key)
                        || x.Account != null && x.Account.Username.Contains(searchCondition.Key))
                        .ToListAsync();
            var novelDtoTasks = novels.Select(x => new NovelDto()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                AuthorId = x.Account.Id,
                Author = x.Account.Username,
                Year = x.Year,
                Views = x.Views,
                ImagesURL = _awsS3Service.GetFileImg(x.Id.ToString(), $"{x.ImageURL}"),
                Description = x.Description,
                Status = x.Status,
                ApprovalStatus = x.ApprovalStatus,
                CreateAt = x.CreatedAt
            }).ToList();


            foreach (var novel in novelDtoTasks)
            {
                var ratings = await _context.Ratings.Where(x => x.NovelId == novel.Id).Select(x => x.RateScore).ToListAsync();
                var numRating = ratings.Count;
                if (numRating > 0) novel.Rating = (int)(ratings.Sum() / numRating);
                else novel.Rating = 0;
                var genres = await _context.GenreOfNovels.Include(x => x.Genre).Where(x => x.NovelId == novel.Id).ToListAsync();
                novel.GenreName = genres.Select(x => x.Genre.Name).ToList();
                novel.GenreIds = genres.Select(x => x.Genre.Id).ToList();
                novel.NumChapter = (await _context.Chapter.Where(e => e.NovelId == novel.Id).ToListAsync()).Count;
            }

            listNovel = novelDtoTasks;


            return listNovel;
        }

        //below function is used for display the novel that followed by genre id when user hover in browse
        //and click in one genre
        public async Task<List<NovelDto>> GetListNovelByGenreId(long genreId)
        {
            List<NovelDto> listNovel = new List<NovelDto>();
            //List<NovelGenre> listOfNovelGenre = new List<NovelGenre>();


            var novels = await _context.Novel.Where(e => e.DelFlag == false).Include(x => x.Genres).Include(x => x.Account).
                                Where(novel => novel.Genres != null && novel.Genres.Any(ng => ng.GenreId == genreId)).
                                ToListAsync();
            var novelDtoTasks = novels.Select(x => new NovelDto()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                AuthorId = x.Account.Id,
                Author = x.Account.Username,
                Year = x.Year,
                Views = x.Views,
                ImagesURL = _awsS3Service.GetFileImg(x.Id.ToString(), $"{x.ImageURL}"),
                Description = x.Description,
                Status = x.Status,
                ApprovalStatus = x.ApprovalStatus,
            }).ToList();

            foreach (var novel in novelDtoTasks)
            {
                var ratings = await _context.Ratings.Where(x => x.NovelId == novel.Id).Select(x => x.RateScore).ToListAsync();
                var numRating = ratings.Count;
                if (numRating > 0) novel.Rating = (int)(ratings.Sum() / numRating);
                else novel.Rating = 0;
                var genres = await _context.GenreOfNovels.Include(x => x.Genre).Where(x => x.NovelId == novel.Id).ToListAsync();
                novel.GenreName = genres.Select(x => x.Genre.Name).ToList();
                novel.GenreIds = genres.Select(x => x.Genre.Id).ToList();
                novel.NumChapter = (await _context.Chapter.Where(e => e.NovelId == novel.Id).ToListAsync()).Count;
            }

            listNovel = novelDtoTasks;

            return listNovel;

        }

        public async Task<List<NovelDto>> GetListNovelByAccountId(string accountId)
        {
            List<NovelDto> listNovel = new List<NovelDto>();
            var novels = await _context.Novel.Where(e => e.DelFlag == false).Include(x => x.Genres).Include(x => x.Account).
                                Where(e => e.AccountId == accountId).ToListAsync();
            var novelDtoTasks = novels.Select(x => new NovelDto()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                AuthorId = x.Account.Id,
                Author = x.Account.Username,
                Year = x.Year,
                Views = x.Views,
                ImagesURL = _awsS3Service.GetFileImg(x.Id.ToString(), $"{x.ImageURL}"),
                Description = x.Description,
                Status = x.Status,
                ApprovalStatus = x.ApprovalStatus,
            }).ToList();

            foreach (var novel in novelDtoTasks)
            {
                var ratings = await _context.Ratings.Where(x => x.NovelId == novel.Id).Select(x => x.RateScore).ToListAsync();
                var numRating = ratings.Count;
                if (numRating > 0) novel.Rating = (int)(ratings.Sum() / numRating);
                else novel.Rating = 0;
                var genres = await _context.GenreOfNovels.Include(x => x.Genre).Where(x => x.NovelId == novel.Id).ToListAsync();
                novel.GenreName = genres.Select(x => x.Genre.Name).ToList();
                novel.GenreIds = genres.Select(x => x.Genre.Id).ToList();
                novel.NumChapter = (await _context.Chapter.Where(e => e.NovelId == novel.Id).ToListAsync()).Count;
            }

            listNovel = novelDtoTasks;

            return listNovel;
        }

        public async Task<NovelDto?> GetNovelAsync(string id)
        {
            var novel = await _context.Novel.Where(e => e.DelFlag == false).Include(x => x.Genres).Include(x => x.Account).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (novel is null)
            {
                return null;
            }
            var novelDto = new NovelDto
            {
                Id = novel.Id,
                Name = novel.Name,
                Title = novel.Title,
                AuthorId = novel.Account.Id,
                Author = novel.Account.Username,
                Year = novel.Year,
                Views = novel.Views,
                ImagesURL = _awsS3Service.GetFileImg(novel.Id.ToString(), $"{novel.ImageURL}"),
                Description = novel.Description,
                Status = novel.Status,
                ApprovalStatus = novel.ApprovalStatus,
                NumChapter = (await _context.Chapter.Where(e => e.NovelId == novel.Id).ToListAsync()).Count
            };

            var ratings = await _context.Ratings.Where(x => x.NovelId == novel.Id).Select(x => x.RateScore).ToListAsync();
            var numRating = ratings.Count;
            if (numRating > 0) novelDto.Rating = (int)ratings.Sum() / numRating;
            else novelDto.Rating = 0;
            var genres = await _context.GenreOfNovels.Include(x => x.Genre).Where(x => x.NovelId == novel.Id).ToListAsync();
            novelDto.GenreName = genres.Select(x => x.Genre.Name).ToList();
            novelDto.GenreIds = genres.Select(x => x.Genre.Id).ToList();

            return novelDto;
        }

        public async Task<ResponseInfo> UpdateNovel(NovelUpdateEntity novel)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existNovel = _context.Novel.Where(e => e.DelFlag == false).Where(n => n.Id == novel.Id).FirstOrDefault();
                if (existNovel is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                if (novel.File is not null)
                {
                    var fileNames = new List<string>
                    {
                        existNovel.ImageURL
                    };
                    var fileName = novel.File.FileName;
                    var fileType = System.IO.Path.GetExtension(novel.File.FileName);
                    await _awsS3Service.DeleteFromS3(existNovel.Id.ToString(), fileNames);
                    await _awsS3Service.UploadToS3(novel.File, $"thumbnail{fileType}", existNovel.Id.ToString());
                    existNovel.ImageURL = $"thumbnail{fileType}";
                }

                if (novel.Name is not null) existNovel.Name = novel.Name;
                if (novel.Title is not null) existNovel.Title = novel.Title;
                if (novel.Views is not null) existNovel.Views = (int)novel.Views;
                if (novel.Description is not null) existNovel.Description = novel.Description;
                if (novel.Status is not null) existNovel.Status = (bool)novel.Status;
                if (novel.ApprovalStatus is not null) existNovel.ApprovalStatus = (bool)novel.ApprovalStatus;

                if (novel.GenreIds is not null)
                    if (novel.GenreIds.Any())
                    {
                        _context.GenreOfNovels.RemoveRange(_context.GenreOfNovels.Where(x => x.NovelId == existNovel.Id));
                        foreach (var genreId in novel.GenreIds)
                        {
                            existNovel.Genres.Add(new NovelGenre()
                            {
                                NovelId = existNovel.Id,
                                GenreId = genreId
                            });
                        }
                    }


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

        public async Task<ResponseInfo> RemoveNovel(NovelDeleteEntity novel)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existNovel = _context.Novel.Where(e => e.DelFlag == false).Where(n => n.Id == novel.Id).FirstOrDefault();
                if (existNovel is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                _context.Novel.Remove(existNovel);

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

        public async Task<List<NovelDto>> GetListRecommendedNovel(string accountId)
        {
            List<NovelDto> listNovel = new List<NovelDto>();
            var preference = await _context.Preferences.Where(x => x.AccountId == accountId).Select(x => x.NovelId).ToListAsync();
            var novels = await _context.Novel.Where(e => e.DelFlag == false && preference.Contains(e.Id)).Include(x => x.Genres).Include(x => x.Account).ToListAsync();
            var novelDtoTasks = novels.Select(x => new NovelDto()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                AuthorId = x.Account.Id,
                Author = x.Account.Username,
                Year = x.Year,
                Views = x.Views,
                ImagesURL = _awsS3Service.GetFileImg(x.Id.ToString(), $"{x.ImageURL}"),
                Description = x.Description,
                Status = x.Status,
                ApprovalStatus = x.ApprovalStatus,
            }).ToList();

            foreach (var novel in novelDtoTasks)
            {
                var ratings = await _context.Ratings.Where(x => x.NovelId == novel.Id).Select(x => x.RateScore).ToListAsync();
                var numRating = ratings.Count;
                if (numRating > 0) novel.Rating = (int)(ratings.Sum() / numRating);
                else novel.Rating = 0;
                var genres = await _context.GenreOfNovels.Include(x => x.Genre).Where(x => x.NovelId == novel.Id).ToListAsync();
                novel.GenreName = genres.Select(x => x.Genre.Name).ToList();
                novel.GenreIds = genres.Select(x => x.Genre.Id).ToList();
                novel.NumChapter = (await _context.Chapter.Where(e => e.NovelId == novel.Id).ToListAsync()).Count;
            }

            listNovel = novelDtoTasks;

            return listNovel;
        }

        public async Task<List<NovelDto>> GetListNewestNovel()
        {
            DateTimeOffset currentDate = DateTimeOffset.Now;
            // Calculate the date 7 days ago
            DateTimeOffset sevenDaysAgo = currentDate.AddDays(-7);
            List<NovelDto> listNovel = new List<NovelDto>();

            var novels = await _context.Novel.Where(e => e.DelFlag == false && e.CreatedAt > sevenDaysAgo).Include(x => x.Genres).Include(x => x.Account).ToListAsync();
            var novelDtoTasks = novels.Select(x => new NovelDto()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                AuthorId = x.Account.Id,
                Author = x.Account.Username,
                Year = x.Year,
                Views = x.Views,
                ImagesURL = _awsS3Service.GetFileImg(x.Id.ToString(), $"{x.ImageURL}"),
                Description = x.Description,
                Status = x.Status,
                ApprovalStatus = x.ApprovalStatus,
            }).ToList();

            foreach (var novel in novelDtoTasks)
            {
                var ratings = await _context.Ratings.Where(x => x.NovelId == novel.Id).Select(x => x.RateScore).ToListAsync();
                var numRating = ratings.Count;
                if (numRating > 0) novel.Rating = (int)(ratings.Sum() / numRating);
                else novel.Rating = 0;
                var genres = await _context.GenreOfNovels.Include(x => x.Genre).Where(x => x.NovelId == novel.Id).ToListAsync();
                novel.GenreName = genres.Select(x => x.Genre.Name).ToList();
                novel.GenreIds = genres.Select(x => x.Genre.Id).ToList();
                novel.NumChapter = (await _context.Chapter.Where(e => e.NovelId == novel.Id).ToListAsync()).Count;
            }

            listNovel = novelDtoTasks;

            return listNovel;
        }
    }
}