using System;
using System.Runtime.CompilerServices;
using CSharpVitamins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebNovel.API.Areas.Models.Orders.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;


namespace WebNovel.API.Areas.Models.Orders
{
    public interface IOrderModel
    {
        Task<List<OrderDto>> GetListOrder();
        Task<List<OrderDto>> GetListOrderByAccount(string AccountId);
        Task<List<OrderDto>> GetListOrderByBundle(long BundleId);
        Task<ResponseInfo> AddOrder(OrderCreateEntity order);
        Task<ResponseInfo> UpdateOrder(OrderUpdateEntity order);
        Task<ResponseInfo> RemoveOrder(OrderDeleteEntity order);
        Task<OrderDto?> GetOrder(string Id);
    }
    public class OrderModel : BaseModel, IOrderModel
    {
        private readonly ILogger<IOrderModel> _logger;
        private string _className = "";
        public OrderModel(IServiceProvider provider, ILogger<IOrderModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<ResponseInfo> AddOrder(OrderCreateEntity order)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();

                var newOrder = new Order()
                {
                    Id = ((ShortGuid)Guid.NewGuid()).ToString(),
                    AccountId = order.AccountId,
                    BundleId = order.BundleId,
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Orders.AddAsync(newOrder);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                    }
                );

                result.Data.Add("OrderId(paymentRefId)", newOrder.Id);
                var bundle = await _context.Bundles.Where(e => e.Id == order.BundleId).FirstAsync();
                result.Data.Add("Price(requiredAmount)", bundle.Price.ToString());

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

        public async Task<List<OrderDto>> GetListOrder()
        {
            var listOrder = await _context.Orders.Where(e => e.DelFlag == false).Select(x => new OrderDto()
            {
                Id = x.Id,
                AccountId = x.AccountId,
                BundleId = x.BundleId,
            }).ToListAsync();

            foreach (var order in listOrder)
            {
                var account = await _context.Accounts.Where(e => e.Id == order.AccountId).FirstAsync();
                order.Username = account.Username;
                order.Email = account.Email;

                var bundle = await _context.Bundles.Where(e => e.Id == order.BundleId).FirstAsync();
                order.CoinAmount = bundle.CoinAmount;
                order.Price = bundle.Price;
            }

            return listOrder;
        }

        public async Task<List<OrderDto>> GetListOrderByAccount(string AccountId)
        {
            var listOrder = await _context.Orders.Where(e => e.DelFlag == false).Where(e => e.AccountId == AccountId).Select(x => new OrderDto()
            {
                Id = x.Id,
                AccountId = x.AccountId,
                BundleId = x.BundleId,
            }).ToListAsync();

            foreach (var order in listOrder)
            {
                var account = await _context.Accounts.Where(e => e.Id == order.AccountId).FirstAsync();
                order.Username = account.Username;
                order.Email = account.Email;

                var bundle = await _context.Bundles.Where(e => e.Id == order.BundleId).FirstAsync();
                order.CoinAmount = bundle.CoinAmount;
                order.Price = bundle.Price;
            }

            return listOrder;
        }

        public async Task<List<OrderDto>> GetListOrderByBundle(long BundleId)
        {
            var listOrder = await _context.Orders.Where(e => e.DelFlag == false).Where(e => e.BundleId == BundleId).Select(x => new OrderDto()
            {
                Id = x.Id,
                AccountId = x.AccountId,
                BundleId = x.BundleId,
            }).ToListAsync();

            foreach (var order in listOrder)
            {
                var account = await _context.Accounts.Where(e => e.Id == order.AccountId).FirstAsync();
                order.Username = account.Username;
                order.Email = account.Email;

                var bundle = await _context.Bundles.Where(e => e.Id == order.BundleId).FirstAsync();
                order.CoinAmount = bundle.CoinAmount;
                order.Price = bundle.Price;
            }

            return listOrder;
        }

        public async Task<OrderDto?> GetOrder(string Id)
        {
            var order = await _context.Orders.Where(e => e.DelFlag == false).Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (order is null)
            {
                return null;
            }
            var orderDto = new OrderDto()
            {
                Id = order.Id,
                AccountId = order.AccountId,
                BundleId = order.BundleId,
            };

            var account = await _context.Accounts.Where(e => e.Id == order.AccountId).FirstAsync();
            orderDto.Username = account.Username;
            orderDto.Email = account.Email;

            var bundle = await _context.Bundles.Where(e => e.Id == order.BundleId).FirstAsync();
            orderDto.CoinAmount = bundle.CoinAmount;
            orderDto.Price = bundle.Price;

            return orderDto;
        }

        public async Task<ResponseInfo> RemoveOrder(OrderDeleteEntity order)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();

            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");
                ResponseInfo result = new ResponseInfo();
                ResponseInfo response = new ResponseInfo();

                var existOrder = _context.Orders.Where(e => e.DelFlag == false).Where(x => x.Id == order.Id).FirstOrDefault();
                if (existOrder is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                _context.Orders.Remove(existOrder);

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

        public async Task<ResponseInfo> UpdateOrder(OrderUpdateEntity order)
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

                var existOrder = _context.Orders.Where(e => e.DelFlag == false).Where(x => x.Id == order.Id).FirstOrDefault();
                if (existOrder is null)
                {
                    response.Code = CodeResponse.HAVE_ERROR;
                    response.MsgNo = MSG_NO.NOT_FOUND;
                    return response;
                }

                if (order.AccountId is not null) existOrder.AccountId = order.AccountId;
                if (order.BundleId is not null) existOrder.BundleId = (long)order.BundleId;

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
