using FinanceSystem.Models.Momo;
using FinanceSystem.Models.Order;

namespace FinanceSystem.Services;

public interface IMomoService
{
    Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
    MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
}