using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance;
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
        Task<ResponseInfo> AddReport(ReportCreateEntity account);
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
        public async Task<ResponseInfo> AddReport(ReportCreateEntity report)
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
                var reportExist = await _context.Report.Where(x => x.NovelId == report.NovelId && x.AccountId == report.AccountId).FirstOrDefaultAsync();
                if (reportExist is not null)
                {
                    result.Code = 202;
                    result.MsgNo = "This novel already is reported by this user!";
                    return result;
                }

                var newReport = new Report()
                {
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
                            await _context.Report.AddAsync(newReport);
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
            var list = _context.Report.Include(x => x.Novel).ThenInclude(e => e.Account)
            .Include(e => e.Account)
            .Where(x => !x.DelFlag)
            .Where(e => !e.Novel.Account.DelFlag && e.Novel.Account.IsActive == true)
            .Where(e => !e.Account.DelFlag && e.Account.IsActive == true)
            .Where(e => !e.Novel.DelFlag)
            .OrderBy(e => e.NovelId).ThenByDescending(e => e.CreatedAt)
            .Select(x => new ReportDto()
            {
                Id = x.Id,
                AccountId = x.AccountId,
                AccountReport = x.Account.Username,
                NovelId = x.NovelId,
                NovelTitle = x.Novel.Title,
                AccountIdOfNovelId = x.Novel.Account.Id,
                AccountOfNovel = x.Novel.Account.Username,
                Reason = x.Reason
            }).ToList();
            return list;
        }
    }
}