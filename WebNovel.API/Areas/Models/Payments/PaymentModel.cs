using System;
using System.Runtime.CompilerServices;
using CSharpVitamins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebNovel.API.Areas.Models.Payments.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Core.Services;
using WebNovel.API.Core.Services.Schemas;
using WebNovel.API.Core.Services.VnPay.Schemas;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Models.Payments
{
    public interface IPaymentModel
    {
        Task<PaymentLinkDto> CreatePayment(CreatePaymentEntity payment);
        Task<BaseResultWithData<(PaymentReturnDto, string)>> ProcessVnpayPaymentReturn(VnpayPayResponse vnpayPayResponse);
        Task<VnpayPayIpnResponse> ProcessVnpayPaymentIpn(VnpayPayResponse vnpayPayResponse);
        Task<List<PaymentHistoryDto>> GetPaymentHistory(string accountId);
        Task<List<PaymentHistoryDto>> GetAllPaymentHistory();
        Task<ResponseInfo> CreateRequestPayout(CreatePayoutDto payout);
        Task<ResponseInfo> DeleteRequestPayout(DeletePayoutDto payout);
        Task<ResponseInfo> Payout(long PayoutId);
        Task<List<PayoutDto>> GetAllPayout();
        Task<List<PayoutDto>> GetAllPayout(string AccountId);
        Task<ResponseInfo> GetRevenue(string AccountId);
        Task<ResponseInfo> GetAdminRevenue();
    }

    public class BaseResultWithData<T>
    {
        public T? Data { get; set; }
        public void Set(T data)
        {
            this.Data = data;
        }
    }

    public class PaymentModel : BaseModel, IPaymentModel
    {
        private readonly ILogger<IPaymentModel> _logger;
        private string _className = "";
        private readonly VnpayConfig vnpayConfig;
        private readonly ICurrentUserService currentUserService;
        private readonly IJobService _jobService;
        private readonly IEmailService _emailService;
        public PaymentModel(IServiceProvider provider,
         ILogger<IPaymentModel> logger,
         IOptions<VnpayConfig> vnpayConfigOptions,
         ICurrentUserService currentUserService,
         IJobService jobService,
         IEmailService emailService) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            vnpayConfig = vnpayConfigOptions.Value;
            this.currentUserService = currentUserService;

            _emailService = emailService;
            _jobService = jobService;
        }

        static string GetActualAsyncMethodName([CallerMemberName] string name = null) => name;

        public async Task<PaymentLinkDto> CreatePayment(CreatePaymentEntity payment)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();
            try
            {
                var GuID = (ShortGuid)Guid.NewGuid();

                _logger.LogInformation($"[{_className}][{method}] Start");

                var newPayment = new Payment()
                {
                    Id = GuID.ToString(),
                    PaymentContent = "THANH TOAN DON HANG " + payment.PaymentRefId,
                    PaymentCurrency = payment.PaymentCurrency,
                    PaymentRefId = payment.PaymentRefId,
                    RequiredAmount = payment.RequiredAmount,
                    PaymentDate = DateTime.Now,
                    ExpireDate = DateTime.Now.AddMinutes(15),
                    PaymentLanguage = payment.PaymentLanguage,
                    MerchantId = payment.MerchantId,
                    PaymentDestinationId = payment.PaymentDestinationId,
                    PaymentStatus = "0",
                };
                var newPaymentSignature = new PaymentSignature()
                {
                    Id = (ShortGuid)Guid.NewGuid().ToString(),
                    SignValue = payment.Signature,
                    SignDate = DateTime.Now,
                    SignOwn = payment.MerchantId,
                    PaymentId = newPayment.Id,
                    IsValid = true,
                };

                await _context.Payments.AddAsync(newPayment);
                await _context.PaymentSignatures.AddAsync(newPaymentSignature);
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

                var paymentUrl = string.Empty;

                switch (payment.PaymentDestinationId)
                {
                    case "VNPAY":
                        var vnpayPayRequest = new VnpayPayRequest(vnpayConfig.Version,
                                vnpayConfig.TmnCode, DateTime.Now, currentUserService.IpAddress ?? string.Empty, payment.RequiredAmount ?? 0, payment.PaymentCurrency ?? string.Empty,
                                "other", newPayment.PaymentContent ?? string.Empty, vnpayConfig.ReturnUrl, newPayment.Id ?? string.Empty);
                        paymentUrl = vnpayPayRequest.GetLink(vnpayConfig.PaymentUrl, vnpayConfig.HashSecret);
                        break;
                    default:
                        break;
                }

                var result = new PaymentLinkDto()
                {
                    PaymentId = newPayment.Id ?? string.Empty,
                    PaymentUrl = paymentUrl,
                };

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

        public async Task<BaseResultWithData<(PaymentReturnDto, string)>> ProcessVnpayPaymentReturn(VnpayPayResponse vnpayPayResponse)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();

            string returnUrl = string.Empty;
            var result = new BaseResultWithData<(PaymentReturnDto, string)>();
            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");

                var resultData = new PaymentReturnDto();
                var isValidSignature = vnpayPayResponse.IsValidSignature(vnpayConfig.HashSecret);

                if (isValidSignature)
                {
                    var payment = await _context.Payments.Where(e => e.Id == vnpayPayResponse.vnp_TxnRef).FirstOrDefaultAsync();

                    if (payment is not null)
                    {
                        var merchant = await _context.Merchants.Where(e => e.Id == payment.MerchantId).FirstOrDefaultAsync();
                        returnUrl = merchant?.MerchantReturnUrl ?? string.Empty;
                        if (payment.RequiredAmount == (vnpayPayResponse.vnp_Amount / 100))
                        {
                            if (vnpayPayResponse.vnp_ResponseCode == "00" && vnpayPayResponse.vnp_TransactionStatus == "00")
                            {
                                payment.PaidAmount = vnpayPayResponse.vnp_Amount / 100;
                                payment.PaymentStatus = "1";

                                resultData.PaymentStatus = "00";
                                resultData.PaymentId = payment.Id;
                                resultData.PaymentRefId = payment.PaymentRefId;
                                resultData.Signature = (await _context.PaymentSignatures.Where(e => e.PaymentId == payment.Id).FirstAsync()).SignValue;

                                //TODO: Add coins to account
                                var order = await _context.Orders.Where(e => e.Id == payment.PaymentRefId).FirstAsync();
                                var bundle = await _context.Bundles.Where(e => e.Id == order.BundleId).FirstAsync();
                                var account = await _context.Accounts.Where(e => e.Id == order.AccountId).FirstAsync();

                                account.WalletAmmount += bundle.CoinAmount;

                                var mailRequest = new EmailRequest()
                                {
                                    Subject = "Payment success",
                                    Body = "",
                                    ToMail = account.Email
                                };
                                _jobService.Enqueue(() => _emailService.SendAsync(mailRequest));
                            }
                            else
                            {
                                payment.PaymentStatus = "2";

                                resultData.PaymentStatus = "10";
                                resultData.PaymentMessage = "Payment process failed";
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

                    }
                    else
                    {
                        resultData.PaymentStatus = "11";
                        resultData.PaymentMessage = "Can't find payment at payment service";
                    }
                }
                else
                {
                    resultData.PaymentStatus = "99";
                    resultData.PaymentMessage = "Invalid signature in response";

                }

                result.Data = (resultData, returnUrl);

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

        public async Task<VnpayPayIpnResponse> ProcessVnpayPaymentIpn(VnpayPayResponse vnpayPayResponse)
        {
            IDbContextTransaction transaction = null;
            string method = GetActualAsyncMethodName();

            var resultData = new VnpayPayIpnResponse();

            try
            {
                _logger.LogInformation($"[{_className}][{method}] Start");

                var isValidSignature = vnpayPayResponse.IsValidSignature(vnpayConfig.HashSecret);

                if (isValidSignature)
                {
                    /// Get payment request
                    var payment = await _context.Payments.Where(e => e.Id == vnpayPayResponse.vnp_TxnRef).FirstOrDefaultAsync();

                    if (payment != null)
                    {
                        if (payment.RequiredAmount == (vnpayPayResponse.vnp_Amount / 100))
                        {
                            if (payment.PaymentStatus != "0")
                            {
                                string message = "";
                                string status = "";

                                if (vnpayPayResponse.vnp_ResponseCode == "00" &&
                                   vnpayPayResponse.vnp_TransactionStatus == "00")
                                {
                                    status = "0";
                                    message = "Tran success";
                                }
                                else
                                {
                                    status = "-1";
                                    message = "Tran error";
                                }

                                /// Update database
                                var newPaymentTransaction = new PaymentTransaction()
                                {
                                    Id = (ShortGuid)Guid.NewGuid().ToString(),
                                    TranMessage = message,
                                    TranPayload = JsonConvert.SerializeObject(vnpayPayResponse),
                                    TranStatus = status,
                                    TranAmount = vnpayPayResponse.vnp_Amount,
                                    TranDate = DateTime.Now,
                                    PaymentId = vnpayPayResponse.vnp_TxnRef,
                                };

                                await _context.PaymentTransactions.AddAsync(newPaymentTransaction);
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

                                resultData.Set("00", "Confirm success");
                            }
                            else
                            {
                                resultData.Set("02", "Order already confirmed");
                            }
                        }
                        else
                        {
                            resultData.Set("04", "Invalid amount");
                        }
                    }
                    else
                    {
                        resultData.Set("01", "Order not found");
                    }
                }
                else
                {
                    resultData.Set("97", "Invalid signature");
                }

                return resultData;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    await _context.RollbackAsync(transaction);
                }
                _logger.LogError($"[{_className}][{method}] Exception: {e.Message}");

                /// TODO: process when exception
                resultData.Set("99", "Input required data");

                throw;
            }
        }

        public async Task<List<PaymentHistoryDto>> GetPaymentHistory(string accountId)
        {
            var paymentsHistory = new List<PaymentHistoryDto>();
            var account = await _context.Accounts.Where(e => e.DelFlag == false).Where(e => e.Id == accountId).FirstOrDefaultAsync();
            if (account is null)
            {
                return paymentsHistory;
            }
            var orders = await _context.Orders.Where(e => e.DelFlag == false).Where(e => e.AccountId == accountId).ToListAsync();
            var payments = await _context.Payments.Where(e => e.DelFlag == false).ToListAsync();
            var bundles = await _context.Bundles.Where(e => e.DelFlag == false).ToListAsync();
            foreach (var order in orders.ToList())
            {
                var payment = payments.Where(e => e.PaymentRefId == order.Id).FirstOrDefault();
                if (payment is null)
                {
                    continue;
                }
                if (payment.PaymentStatus != "1")
                {
                    continue;
                }

                var bundle = bundles.Where(e => e.Id == order.BundleId).FirstOrDefault();
                if (bundle is null)
                {
                    continue;
                }
                var PaymentHistoryDto = new PaymentHistoryDto()
                {
                    Id = payment.Id,
                    OrderId = payment.PaymentRefId,

                    AccountId = account.Id,
                    Username = account.Username,
                    Email = account.Email,

                    BundleId = bundle.Id,
                    CoinAmount = bundle.CoinAmount,
                    Price = bundle.Price,

                    PaymentDate = payment.PaymentDate,
                    PaymentStatus = payment.PaymentStatus,
                };
                paymentsHistory.Add(PaymentHistoryDto);
            }
            return paymentsHistory;
        }

        public async Task<List<PaymentHistoryDto>> GetAllPaymentHistory()
        {
            var paymentsHistory = new List<PaymentHistoryDto>();
            var orders = await _context.Orders.Where(e => e.DelFlag == false).ToListAsync();
            var payments = await _context.Payments.Where(e => e.DelFlag == false).ToListAsync();
            var accounts = await _context.Accounts.Where(e => e.DelFlag == false).ToListAsync();
            var bundles = await _context.Bundles.Where(e => e.DelFlag == false).ToListAsync();
            foreach (var order in orders.ToList())
            {
                var payment = payments.Where(e => e.PaymentRefId == order.Id).FirstOrDefault();
                if (payment is null)
                {
                    continue;
                }
                if (payment.PaymentStatus != "1")
                {
                    continue;
                }

                var account = accounts.Where(e => e.Id == order.AccountId).FirstOrDefault();
                if (account is null)
                {
                    continue;
                }

                var bundle = bundles.Where(e => e.Id == order.BundleId).FirstOrDefault();
                if (bundle is null)
                {
                    continue;
                }

                var PaymentHistoryDto = new PaymentHistoryDto()
                {
                    Id = payment.Id,
                    OrderId = payment.PaymentRefId,

                    AccountId = account.Id,
                    Username = account.Username,
                    Email = account.Email,

                    BundleId = bundle.Id,
                    CoinAmount = bundle.CoinAmount,
                    Price = bundle.Price,

                    PaymentDate = payment.PaymentDate,
                    PaymentStatus = payment.PaymentStatus,
                };
                paymentsHistory.Add(PaymentHistoryDto);
            }
            return paymentsHistory;
        }

        public async Task<ResponseInfo> CreateRequestPayout(CreatePayoutDto request)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            ResponseInfo results = new ResponseInfo();
            try
            {
                var payout = new Payout()
                {
                    AccountId = request.AccountId,
                    PayoutAmount = request.PayoutAmount,
                    Bank = request.Bank,
                    BankName = request.BankName,
                    BankNumber = request.BankNumber
                };

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(
                    async () =>
                    {
                        using (transaction = await _context.Database.BeginTransactionAsync())
                        {
                            await _context.Payouts.AddAsync(payout);
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

        public async Task<ResponseInfo> Payout(long PayoutId)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            ResponseInfo results = new ResponseInfo();
            try
            {
                var requestPayout = await _context.Payouts.Where(e => e.Id == PayoutId)
                                                            .Include(e => e.Account)
                                                            .FirstOrDefaultAsync();
                if (requestPayout is null)
                {
                    results.Code = CodeResponse.HAVE_ERROR;
                    results.MsgNo = MSG_NO.NOT_FOUND;
                    return results;
                }

                if (requestPayout.Account.CreatorWallet < requestPayout.PayoutAmount)
                {
                    results.Code = CodeResponse.HAVE_ERROR;
                    results.MsgNo = "Account doesn't have enough amount";
                    return results;
                }

                requestPayout.PayoutStatus = true;
                requestPayout.Account.CreatorWallet -= requestPayout.PayoutAmount;

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

                var mailRequest = new EmailRequest()
                {
                    Subject = "Request Payout",
                    Body = "Request Payout is accepted. The amount is: " + requestPayout.PayoutAmount + ".",
                    ToMail = requestPayout.Account.Email
                };
                _jobService.Enqueue(() => _emailService.SendAsync(mailRequest));

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

        public async Task<ResponseInfo> DeleteRequestPayout(DeletePayoutDto request)
        {
            IDbContextTransaction? transaction = null;
            string method = GetActualAsyncMethodName();
            ResponseInfo results = new ResponseInfo();
            try
            {
                var requestPayout = await _context.Payouts.Where(e => e.Id == request.PayoutId)
                                                            .Include(e => e.Account)
                                                            .FirstOrDefaultAsync();
                if (requestPayout is null)
                {
                    results.Code = CodeResponse.HAVE_ERROR;
                    results.MsgNo = MSG_NO.NOT_FOUND;
                    return results;
                }

                _context.Payouts.Remove(requestPayout);

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

                var mailRequest = new EmailRequest()
                {
                    Subject = "Request Payout canceled",
                    Body = "Request Payout is canceled. The amount is: " + requestPayout.PayoutAmount + ".",
                    ToMail = requestPayout.Account.Email
                };
                _jobService.Enqueue(() => _emailService.SendAsync(mailRequest));

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

        public async Task<List<PayoutDto>> GetAllPayout()
        {
            var payouts = await _context.Payouts.Where(e => e.DelFlag == false)
            .Include(e => e.Account)
            .OrderBy(e => e.PayoutStatus).ThenBy(e => e.CreatedAt)
            .Select(e => new PayoutDto()
            {
                Id = e.Id,
                AccountId = e.AccountId,
                Username = e.Account.Username,
                Email = e.Account.Email,
                NickName = e.Account.NickName,
                PayoutAmount = e.PayoutAmount,
                PayoutStatus = e.PayoutStatus,
                Bank = e.Bank,
                BankName = e.BankName,
                BankNumber = e.BankNumber,
            }).ToListAsync();

            return payouts;
        }

        public async Task<List<PayoutDto>> GetAllPayout(string AccountId)
        {
            var payouts = await _context.Payouts.Where(e => e.DelFlag == false)
            .Where(e => e.AccountId == AccountId)
            .Include(e => e.Account)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new PayoutDto()
            {
                Id = e.Id,
                AccountId = e.AccountId,
                Username = e.Account.Username,
                Email = e.Account.Email,
                NickName = e.Account.NickName,
                PayoutAmount = e.PayoutAmount,
                PayoutStatus = e.PayoutStatus,
                Bank = e.Bank,
                BankName = e.BankName,
                BankNumber = e.BankNumber,
            }).ToListAsync();

            return payouts;
        }

        public async Task<ResponseInfo> GetRevenue(string AccountId)
        {
            var ChapterOfAccounts = await _context.ChapterOfAccounts.Where(e => e.DelFlag == false)
            .Include(e => e.Novel)
            .Where(e => e.Novel.AccountId == AccountId)
            .Include(e => e.Chapter).ThenInclude(e => e.UpdatedFee)
            .ToListAsync();

            var DayOfWeek = (int)DateTimeOffset.Now.DayOfWeek;
            var currentWeek = DateTimeOffset.Now.AddDays(-DayOfWeek);
            var currentMonth = DateTimeOffset.Now.Month;
            var currentYear = DateTimeOffset.Now.Year;

            var ResponseInfo = new ResponseInfo
            {
                MsgNo = AccountId + " Revenue"
            };

            ResponseInfo.Data.Add("WeeklyRevenue", ChapterOfAccounts.Where(e => e.CreatedAt > currentWeek).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("MonthlyRevenue", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == currentMonth).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("YearlyRevenue", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());

            ResponseInfo.Data.Add("January", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 1).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("February", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 2).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("March", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 3).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("April", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 4).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("May", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 5).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("June", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 6).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("July", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 7).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("August", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 8).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("September", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 9).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("October", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 10).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("November", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 11).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());
            ResponseInfo.Data.Add("December", ChapterOfAccounts.Where(e => e.CreatedAt.Year == currentYear && e.CreatedAt.Month == 12).Select(e => e.Chapter.UpdatedFee.Fee).Sum().ToString());

            return ResponseInfo;
        }

        public async Task<ResponseInfo> GetAdminRevenue()
        {
            var payments = await GetAllPaymentHistory();
            var DayOfWeek = (int)DateTime.Now.DayOfWeek;
            var currentWeek = DateTime.Now.AddDays(-DayOfWeek);
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var ResponseInfo = new ResponseInfo
            {
                MsgNo = "Admin Revenue"
            };

            ResponseInfo.Data.Add("WeeklyRevenue", payments.Where(e => e.PaymentDate > currentWeek).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("MonthlyRevenue", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == currentMonth).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("YearlyRevenue", payments.Where(e => e.PaymentDate?.Year == currentYear).Select(e => e.Price).Sum().ToString());

            ResponseInfo.Data.Add("January", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 1).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("February", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 2).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("March", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 3).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("April", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 4).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("May", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 5).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("June", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 6).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("July", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 7).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("August", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 8).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("September", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 9).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("October", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 10).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("November", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 11).Select(e => e.Price).Sum().ToString());
            ResponseInfo.Data.Add("December", payments.Where(e => e.PaymentDate?.Year == currentYear && e.PaymentDate?.Month == 12).Select(e => e.Price).Sum().ToString());

            return ResponseInfo;
        }
    }
}
