using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Webnovel.API.Databases;
using WebNovel.API.Core.Services;

namespace WebNovel.API.Core.Models
{
    public class BaseModel
    {
        protected readonly DataContext _context;
        protected readonly ILogService _logService;
        public BaseModel()
        {
        }

        public BaseModel(
            IServiceProvider provider
        )
        {
            ILogService logService = provider.GetService<ILogService>();
            DataContext context = provider.GetService<DataContext>();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }
        protected async Task<bool> ValidatePhone(string userId, string phone)
        {
            return await _context.Accounts.Where(e => e.DelFlag == false)
                .AnyAsync(x =>
                    x.Phone == phone &&
                    (x.Id != userId));
        }

        protected async Task<bool> ValidateEmail(string? userId, string email)
        {
            return await _context.Accounts.Where(e => e.DelFlag == false)
                .AnyAsync(x =>
                    x.Email == email &&
                    (x.Id != userId));
        }
    }
}