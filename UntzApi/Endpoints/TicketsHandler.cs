using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Untz.Database;

namespace Untz.Endpoints
{
    public class TicketsHandler
    {
        public static async Task<IResult> GetTicketsForEventAsync(long eventId, UntzDbContext dbContext, IMapper mapper)
        {
            var tickets = await dbContext.Tickets.Where(_ => _.Event.Id.Equals(eventId)).ToListAsync();

            if (tickets.Any())
                return Results.Ok(tickets);

            return Results.NoContent();
        }
    }
}
