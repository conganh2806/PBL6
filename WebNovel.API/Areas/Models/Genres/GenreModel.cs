using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using WebNovel.API.Areas.Models.Genres.Schemas;
using WebNovel.API.Commons;
using WebNovel.API.Commons.CodeMaster;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Models.Genres
{
    public interface IGenreModel
    {
        Task<List<GenreDto>> GetListGenre();
        Task<ResponseInfo> AddGenre(GenreCreateUpdateEntity genre);
        Task<ResponseInfo> UpdateGenre(long id, GenreCreateUpdateEntity genre);
        GenreDto GetGenre(long id);
    }

    public class GenreModel : BaseModel, IGenreModel
    {
        private readonly ILogger<IGenreModel> _logger;
        private string _className = "";
        public GenreModel(IServiceProvider provider, ILogger<IGenreModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddGenre(GenreCreateUpdateEntity genre)
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

                var newGenre = new Genre()
                {
                    Id = genre.Id,
                    Name = genre.Name,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Genre.AddAsync(newGenre);
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

        public GenreDto GetGenre(long id)
        {
            var genre = _context.Genre.Where(x => x.Id == id).FirstOrDefault();
            var genreDto = new GenreDto()
            {
                Id = genre.Id,
                Name = genre.Name,
            };

            return genreDto;
        }

        public async Task<List<GenreDto>> GetListGenre()
        {
            var listGenre = _context.Genre.Select(x => new GenreDto()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();

            return listGenre;
        }

        public async Task<ResponseInfo> UpdateGenre(long id, GenreCreateUpdateEntity genre)
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

                var existGenre = _context.Genre.Where(x => x.Id == id).FirstOrDefault();
                if (existGenre is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                existGenre.Name = genre.Name;

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