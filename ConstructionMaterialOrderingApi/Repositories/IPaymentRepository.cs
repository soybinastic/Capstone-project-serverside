using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ConstructionMaterialOrderingApi.Models;

namespace ConstructionMaterialOrderingApi.Repositories
{
    public interface IPaymentRepository<T> where T : class
    {
        Task<T> OnPay(Dashboard dashboard, string successUrl = null, string cancelUrl = null);
        Task<List<PaymentDetail>> GetAllPaymentDetails();
        Task<PaymentDetail> GetPaymentDetail(int paymentDetailId);
    }
}