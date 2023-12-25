using System;

namespace WebNovel.API.Core.Services.VnPay.Schemas
{
    public class VnpayPayIpnResponse
    {
        public VnpayPayIpnResponse()
        {

        }
        public VnpayPayIpnResponse(string rspCode, string message)
        {
            RspCode = rspCode;
            Message = message;
        }
        public void Set(string rspCode, string message)
        {
            RspCode = rspCode;
            Message = message;
        }
        public string RspCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
