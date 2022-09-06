using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConstructionMaterialOrderingApi.Context;
using ConstructionMaterialOrderingApi.Models;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public class PaymentRepository : IPaymentRepository<Session>
    {
        private readonly ApplicationDbContext _db;
        public PaymentRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            string apiSecretKey = configuration["StripeSecretKey"] as string;
            StripeConfiguration.ApiKey = apiSecretKey;
        }
        public Task<List<PaymentDetail>> GetAllPaymentDetails()
        {
            throw new NotImplementedException();
        }

        public Task<PaymentDetail> GetPaymentDetail(int paymentDetailId)
        {
            throw new NotImplementedException();
        }

        public async Task<Session> OnPay(Dashboard dashboard, string successUrl = null, string cancelUrl = null)
        {
            // string totalAmountString = (dashboard.Total + dashboard.PlatformFee).ToString();
            // string[]total = totalAmountString.Split('.');
            // totalAmountString = total[0] + total[1];
            // Console.WriteLine(totalAmountString);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions 
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)Math.Round((dashboard.Total + dashboard.PlatformFee) * 100),
                            Currency = "PHP",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Fastline Software",
                                Images = new List<string> { "https://www.pngitem.com/pimgs/m/527-5274636_software-installation-symbol-hd-png-download.png" }
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                CancelUrl = cancelUrl,
                SuccessUrl = successUrl
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            Console.WriteLine(session.PaymentStatus);
            return session;
        }
    }
}