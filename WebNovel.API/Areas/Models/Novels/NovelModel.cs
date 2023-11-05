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
using WebNovel.API.Databases.Entities;
using WebNovel.API.Databases.Entitites;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Novels
{
    public interface INovelModel
    {
        Task<List<NovelDto>> GetListNovel(SearchCondition searchCondition);
        Task<ResponseInfo> AddNovel(IFormFile formFile, NovelCreateUpdateEntity novel);
        Task<ResponseInfo> UpdateNovel(string id, NovelCreateUpdateEntity novel, IFormFile formFile);
        Task<NovelDto> GetNovelAsync(string id);

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

        public async Task<ResponseInfo> AddNovel(IFormFile formFile, NovelCreateUpdateEntity novel)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                var GuID = (ShortGuid)Guid.NewGuid();

                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo results = new ResponseInfo();
                var fileType = System.IO.Path.GetExtension(formFile.FileName);
                await _awsS3Service.UploadToS3(formFile, $"thumbnail{fileType}", GuID.ToString());
                var fileName = $"thumbnail{fileType}";
                var newNovel = new Novel()
                {
                    Id = GuID.ToString(),
                    Name = novel.Name,
                    Title = novel.Title,
                    AccountId = novel.AccountId,
                    Year = novel.Year,
                    Views = novel.Views,
                    Rating = novel.Rating,
                    Description = novel.Description,
                    Status = novel.Status,
                    ApprovalStatus = novel.ApprovalStatus,
                    ImageURL = fileName,
                };

                if (novel.GenreIds.Any())
                {
                    foreach (var genreId in novel.GenreIds)
                    {
                        newNovel.Genres.Add(new NovelGenre()
                        {
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

            List<NovelDto> listNovel = new List<NovelDto>();

            if (searchCondition is null)
            {
                var novels = await _context.Novel.Include(x => x.Genres).Include(x => x.Account).ToListAsync();
                var novelDtoTasks = novels.Select(x => new NovelDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Title = x.Title,
                    Author = x.Account.Username,
                    Year = x.Year,
                    Views = x.Views,
                    ImagesURL = _awsS3Service.GetFileImg(x.Id.ToString(), $"{x.ImageURL}"),
                    Rating = x.Rating,
                    Description = x.Description,
                    Status = x.Status,
                    ApprovalStatus = x.ApprovalStatus,
                }).ToList();


                foreach (var novel in novelDtoTasks)
                {
                    novel.GenreName = await _context.GenreOfNovels.Include(x => x.Genre).Select(x => x.Genre.Name).ToListAsync();
                    novel.NumChapter = (await _context.Chapter.Where(e => e.NovelId == novel.Id).ToListAsync()).Count;
                }

                listNovel = novelDtoTasks;
            }

            return listNovel;
        }

        public async Task<NovelDto> GetNovelAsync(string id)
        {
            var novel = await _context.Novel.Include(x => x.Genres).Include(x => x.Account).Where(x => x.Id == id).FirstOrDefaultAsync();
            var novelDto = new NovelDto
            {
                Id = novel.Id,
                Name = novel.Name,
                Title = novel.Title,
                Author = novel.Account.Username,
                Year = novel.Year,
                Views = novel.Views,
                ImagesURL = _awsS3Service.GetFileImg(novel.Id.ToString(), $"{novel.ImageURL}"),
                Rating = novel.Rating,
                Description = novel.Description,
                Status = novel.Status,
                ApprovalStatus = novel.ApprovalStatus,
                GenreName = await _context.GenreOfNovels.Include(x => x.Genre).Select(x => x.Genre.Name).ToListAsync(),
                NumChapter = (await _context.Chapter.Where(e => e.NovelId == novel.Id).ToListAsync()).Count
            };

            return novelDto;
        }

        public async Task<ResponseInfo> UpdateNovel(string id, NovelCreateUpdateEntity novel, IFormFile formFile)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existNovel = _context.Novel.Where(n => n.Id == id).FirstOrDefault();
                if (existNovel is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                var fileNames = new List<string>
                {
                    existNovel.ImageURL
                };
                var fileName = formFile.FileName;
                await _awsS3Service.DeleteFromS3(id.ToString(), fileNames);
                await _awsS3Service.UploadToS3(formFile, existNovel.ImageURL, id.ToString());

                existNovel.Name = novel.Name;
                existNovel.Title = novel.Title;
                existNovel.Year = novel.Year;
                existNovel.Views = novel.Views;
                existNovel.Rating = novel.Rating;
                existNovel.Description = novel.Description;
                existNovel.Status = novel.Status;
                existNovel.ApprovalStatus = novel.Status;
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