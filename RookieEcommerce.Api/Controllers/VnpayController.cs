using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using VNPAY.NET;
using RookieEcommerce.SharedViewModels.PaymentDtos;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/vnpay")]
    [ApiController]
    public class VnpayController : ControllerBase
    {
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;

        public VnpayController(IVnpay vnPayservice, IConfiguration configuration)
        {
            _vnpay = vnPayservice;
            _configuration = configuration;

            _vnpay.Initialize(_configuration["Vnpay:TmnCode"]!, _configuration["Vnpay:HashSecret"]!, _configuration["Vnpay:BaseUrl"]!, _configuration["Vnpay:CallbackUrl"]!);
        }

        [HttpPost("payment-url")]
        public ActionResult<string> CreatePaymentUrl(CreatePaymentDto dto)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = (double)dto.TotalAmount,
                    Description = dto.Description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("callback")]
        public ActionResult<PaymentResult> Callback()
        {
            string callbackUrlBase = _configuration["Vnpay:CallbackUrlBase"]! ;
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);

                    if (paymentResult.IsSuccess)
                    {
                        return Redirect($"{callbackUrlBase}?orderId={paymentResult.Description}&transactionId={paymentResult.VnpayTransactionId}");
                    }

                    return BadRequest(paymentResult);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound();
        }
    }
}
