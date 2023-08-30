using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Untz.Database;
using Untz.Endpoints.Dtos;

namespace Untz.Endpoints
{
    public class PaymentMethodsHandler
    {
        public static async Task<IResult> GetPaymentMethodsAsync(UntzDbContext dbContext, IMapper mapper)
        {
            var paymentMethods = await dbContext.PaymentMethods.ToListAsync();

            if (paymentMethods.Any())
                return Results.Ok(mapper.Map<List<PaymentMethodDto>>(paymentMethods));

            return Results.NoContent();
        }
    }
}
