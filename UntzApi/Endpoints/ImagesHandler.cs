using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Untz.Database;
using UntzCommon.Models.Dtos;

namespace Untz.Endpoints
{
    public class ImagesHandler
    {
        public static async Task<IResult> GetImagesForEventAsync(long eventId, UntzDbContext dbContext, IMapper mapper)
        {

            var selectedEvent = await dbContext.Events.FirstOrDefaultAsync(_ => _.Id.Equals(eventId));

            if (selectedEvent is null)
                return Results.NotFound("Event not found");

            var images = dbContext.Images.Where(_ => _.Event.Id.Equals(eventId));

            if (images.Any())
                return Results.Ok(mapper.Map<List<ImageDto>>(images));

            return Results.NoContent();
        }
    }
}
