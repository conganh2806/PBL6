using System;
using System.Runtime.CompilerServices;
using CSharpVitamins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebNovel.API.Areas.Models.Payments.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Core.Services;
using WebNovel.API.Core.Services.Schemas;
using WebNovel.API.Core.Services.VnPay.Schemas;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Areas.Models.Payments
{
    public interface IPaymentModel
    {
        Task<PaymentLinkDto> CreatePayment(CreatePaymentEntity payment);
        Task<BaseResultWithData<(PaymentReturnDto, string)>> ProcessVnpayPaymentReturn(VnpayPayResponse vnpayPayResponse);
        Task<VnpayPayIpnResponse> ProcessVnpayPaymentIpn(VnpayPayResponse vnpayPayResponse);
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
         ICurrentUserService currentUserService) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
            vnpayConfig = vnpayConfigOptions.Value;
            this.currentUserService = currentUserService;
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

                // var mailRequest = new EmailRequest()
                // {
                //     Subject = "Confirm Registration",
                //     Body = "",
                //     ToMail = ""
                // };
                // _jobService.Enqueue(() => _emailService.SendAsync(mailRequest));

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
    }
}
