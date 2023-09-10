using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Untz.Database;
using UntzCommon.Database.Models;
using UntzCommon.Models.Dtos;

namespace Untz.Endpoints
{
    public class EventsHandler
    {

        public async static Task<IResult> AddEventAsync(EventDto eventDto, UntzDbContext dbContext, IMapper mapper)
        {
            if (eventDto.MainEvent is not null && eventDto.MainEvent.IsActive)
            {
                var currentMainEvent = await dbContext.MainEvents.FirstOrDefaultAsync(_ => _.IsActive);
                if (currentMainEvent is not null)
                {
                    currentMainEvent.IsActive = false;
                }
            }

            if (eventDto.Images is not null && eventDto.Images.Count() > 0)
            {
                eventDto.Images.ToList().ForEach(_ =>
                {
                    var fileType = _.Name.Split(".").Last();
                    _.Name = $"{Guid.NewGuid()}.{fileType}";
                    var imageContent = Convert.FromBase64String(_.Base64Content!);

                    using (var file = File.Open($"{Environment.GetEnvironmentVariable("image_upload_path")}/{_.Name}", FileMode.OpenOrCreate))
                    {
                        file.Write(imageContent, 0, imageContent.Length);
                        file.Flush();
                    }
                });
            }

            var _event = mapper.Map<Event>(eventDto);
            _event.IsActive = true;

            var addedEvent = dbContext.Events.Add(_event).Entity;

            await dbContext.SaveChangesAsync();

            return Results.Created(eventDto.Id.ToString(), mapper.Map<EventDto>(addedEvent));
        }

        public async static Task<IResult> UpadateEventAsync(EventDto eventDto, UntzDbContext dbContext, IMapper mapper)
        {
            if (eventDto.MainEvent is not null && eventDto.MainEvent.IsActive)
            {
                var currentMainEvent = await dbContext.MainEvents.Include(_ => _.Event).FirstOrDefaultAsync(_ => _.IsActive);
                if (currentMainEvent is not null && !currentMainEvent.Event.Id.Equals(eventDto.Id))
                {
                    currentMainEvent.IsActive = false;
                }
            }

            var selectedEvent = await dbContext.Events
                .Include(_ => _.Images)
                .Include(_ => _.Tickets)
                .Include(_ => _.MainEvent)
                .FirstOrDefaultAsync(_ => _.Id.Equals(eventDto.Id));

            if (selectedEvent is null)
                return Results.NotFound("Event not found");

            selectedEvent.Name = eventDto.Name;
            selectedEvent.Description = eventDto.Description;
            selectedEvent.PreSaleStartDate = eventDto.PreSaleStartDate;
            selectedEvent.IsActive = eventDto.IsActive;
            selectedEvent.EventStartTime = eventDto.EventStartTime;
            selectedEvent.Location = eventDto.Location;
            selectedEvent.Entrance = eventDto.Entrance;
            if (selectedEvent.MainEvent is not null)
                selectedEvent.MainEvent.IsActive = Convert.ToBoolean(eventDto.MainEvent?.IsActive);

            //Manage tickets - Start
            var incomingTickets = eventDto.Tickets;
            var existingTickets = selectedEvent.Tickets;

            //Update already existing list
            var eventsToBeUpdated = incomingTickets.Select(_ => _.Id).Intersect(existingTickets.Select(_ => _.Id));
            foreach (var item in existingTickets.Where(_ => eventsToBeUpdated.Contains(_.Id)))
            {
                var updatableTicket = incomingTickets.FirstOrDefault(_ => _.Id.Equals(item.Id))!;
                item.Name = updatableTicket.Name;
                item.Price = updatableTicket.Price;
            }

            //Add new list
            foreach (var item in incomingTickets.Where(_ => _.Id.Equals(default)))
            {
                selectedEvent.Tickets.Add(mapper.Map<Ticket>(item));
            }

            //Delete other list
            var toBeDeletedIdList = existingTickets.Select(_ => _.Id).Except(incomingTickets.Select(_ => _.Id));
            existingTickets.Where(_ => toBeDeletedIdList.Contains(_.Id)).ToList().ForEach(_ =>
            {
                selectedEvent.Tickets.Remove(_);
            });
            //Manage tickets - End

            //Remove all images and reinsert
            selectedEvent.Images.ToList().ForEach(_ =>
            {
                var actualFilePath = $"{Environment.GetEnvironmentVariable("image_upload_path")}/{_.Name}";
                if (File.Exists(actualFilePath)) { File.Delete(actualFilePath); }
                selectedEvent.Images.Remove(_);
            });

            if (eventDto.Images is not null && eventDto.Images.Count() > 0)
            {
                eventDto.Images.ToList().ForEach(_ =>
                {
                    var fileType = _.Name.Split(".").Last();
                    _.Name = $"{Guid.NewGuid()}.{fileType}";
                    var imageContent = Convert.FromBase64String(_.Base64Content!);

                    using (var file = File.Open($"{Environment.GetEnvironmentVariable("image_upload_path")}/{_.Name}", FileMode.OpenOrCreate))
                    {
                        file.Write(imageContent, 0, imageContent.Length);
                        file.Flush();
                    }
                });

                selectedEvent.Images.AddRange(mapper.Map<List<Image>>(eventDto.Images));
            }

            selectedEvent.IsActive = true;

            await dbContext.SaveChangesAsync();

            return Results.Ok(eventDto);
        }

        public static async Task<IResult> GetMainEventAsync(UntzDbContext dbContext, IMapper mapper)
        {
            var mainEvent = await dbContext.Events.Include(_ => _.Images).FirstOrDefaultAsync(_ => _.MainEvent != null && _.MainEvent.IsActive && _.IsActive);

            if (mainEvent is null)
                return Results.NoContent();

            return Results.Ok(mapper.Map<EventDto>(mainEvent));
        }

        public static async Task<IResult> GetAllEventsDetailedAsync(UntzDbContext dbContext, IMapper mapper)
        {

            var selectedEvent = await dbContext.Events
                .Include(_ => _.Images)
                .Include(_ => _.Tickets)
                .Include(_ => _.MainEvent)
                .Where(_ => _.IsActive)
                .ToListAsync();

            if (selectedEvent is null)
                return Results.NoContent();

            return Results.Ok(mapper.Map<List<EventDto>>(selectedEvent));
        }

        public static async Task<IResult> GetAllEventsAsync(UntzDbContext dbContext, IMapper mapper)
        {

            var selectedEvent = await dbContext.Events.Where(_ => _.IsActive).ToListAsync();

            if (selectedEvent is null)
                return Results.NoContent();

            return Results.Ok(mapper.Map<List<EventDto>>(selectedEvent));
        }

        public static async Task<IResult> GetDetailedEventAsync(long eventId, UntzDbContext dbContext, IMapper mapper)
        {

            var selectedEvent = await dbContext.Events
                .Include(_ => _.Images)
                .Include(_ => _.Tickets)
                .Include(_ => _.MainEvent)
                .FirstOrDefaultAsync(_ => _.Id.Equals(eventId) && _.IsActive);

            if (selectedEvent is null)
                return Results.NoContent();

            return Results.Ok(mapper.Map<EventDto>(selectedEvent));
        }

        public static async Task<IResult> DeleteEventAsync(long id, UntzDbContext dbContext, IMapper mapper)
        {

            var selectedEvent = await dbContext.Events.FirstOrDefaultAsync(_ => _.Id.Equals(id));

            if (selectedEvent is null)
                return Results.Problem();

            selectedEvent.IsActive = false;
            await dbContext.SaveChangesAsync();

            return Results.Ok(true);
        }

        public static async Task<IResult> SubscribeForEventAsync(EventSubscriptionDto eventSubscriptionDto, UntzDbContext dbContext, IMapper mapper)
        {
            var modelToBeAdded = mapper.Map<EventSubscription>(eventSubscriptionDto);
            await dbContext.EventSubscriptions.AddAsync(modelToBeAdded);
            await dbContext.SaveChangesAsync();
            return Results.Ok(true);
        }
    }
}
