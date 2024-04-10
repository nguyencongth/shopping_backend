﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebServiceShopping.Connections.Mono;
using WebServiceShopping.Models;

namespace WebServiceShopping.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MomoController : Controller
{
   [HttpGet("PaymentMomo")]
    public async Task<IActionResult> Payment()
    {
        try
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "tuantv test API Momo";
            string returnUrl = "https://localhost:7249/api/Momo/ConfirmPaymentClient";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment";

            string amount = "1000";
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                             partnerCode + "&accessKey=" +
                             accessKey + "&requestId=" +
                             requestId + "&amount=" +
                             amount + "&orderId=" +
                             orderid + "&orderInfo=" +
                             orderInfo + "&returnUrl=" +
                             returnUrl + "&notifyUrl=" +
                             notifyurl + "&extraData=" +
                             extraData;

            MomoSecurity crypto = new MomoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);
            
            return Ok(jmessage.ToString());
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpGet("ConfirmPaymentClient")]
    public async Task<IActionResult> ConfirmPaymentClient(Momo result)
    {
        //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
        string rMessage = result.message;
        string rOrderId = result.orderId;
        string rErrorCode = result.errorCode; // = 0: thanh toán thành công
        return Ok(rMessage);
    }

    /*[HttpPost]
    public void SavePayment()
    {
        //cập nhật dữ liệu vào db
        String a = "";
    }*/
}