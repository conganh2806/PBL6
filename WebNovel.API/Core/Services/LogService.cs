using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webnovel.API.Databases;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Core.Services
{
    public interface ILogService
    {
        Task SaveLogException(Exception e);
    }
    public class LogService : ILogService
    {
        private readonly DataContext _context;

        public LogService(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SaveLogException(Exception e)
        {
            _context.ExceptionLogs.Add(new ExceptionLog()
            {
                Project = System.Reflection.Assembly.GetEntryAssembly().GetName().Name,
                Class = e.TargetSite == null || e.TargetSite.ReflectedType == null || e.TargetSite.ReflectedType.DeclaringType == null ? "" : e.TargetSite.ReflectedType.DeclaringType.FullName,
                Method = e.TargetSite == null || e.TargetSite.ReflectedType == null ? "" : e.TargetSite.ReflectedType.Name,
                Message = e.Message,
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(e.Data),
                Source = e.Source,
                StackTrace = e.StackTrace,
                InnerException = e.InnerException == null ? "" : e.InnerException.Message
            });
            await _context.SaveChangesAsync();
        }
    }
}