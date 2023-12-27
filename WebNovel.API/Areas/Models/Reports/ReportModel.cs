using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Reports.Schemas;
using WebNovel.API.Areas.Models.Reports.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Models.Reports
{
    public interface IReportModel
    {
        Task<List<ReportDto>> GetListReport();
        Task<ResponseInfo> AddReport(ReportCreateUpdateEntity account);
    }
    public class ReportModel : BaseModel, IReportModel
    {
        private readonly ILogger<IReportModel> _logger;
        private string _className = "";
        public ReportModel(IServiceProvider provider, ILogger<IReportModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;
        public async Task<ResponseInfo> AddReport(ReportCreateUpdateEntity report)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                var novel = await _context.Novel.Where(x => x.Id == report.NovelId && !x.DelFlag).FirstOrDefaultAsync();
                if (novel is null)
                {
                    result.Code = 202;
                    result.MsgNo = "Novel is not exist!";
                    return result;
                }
                var reportExist = await _context.Reports.Where(x => x.NovelId == report.NovelId && x.AccountId == report.AccountId).FirstOrDefaultAsync();
                if (reportExist is not null)
                {
                    result.Code = 202;
                    result.MsgNo = "This novel already is reported by this user!";
                    return result;
                }

                var newReport = new Report()
                {
                    Id = report.Id,
                    AccountId = report.AccountId,
                    NovelId = report.NovelId,
                    Reason = report.Reason
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (var trn = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Reports.AddAsync(newReport);
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

        public async Task<List<ReportDto>> GetListReport()
        {
            var list = _context.Reports.Include(x => x.Novel).Where(x => !x.DelFlag).Select(x => new ReportDto()
            {
                Id = x.Id,
                NameNovel = x.Novel.Name,
                NovelId = x.NovelId,
                Reason = x.Reason
            }).ToList();
            return list;
        }
    }
}