using AutoMapper;
using Untz.Database.Models;
using Untz.Endpoints.Dtos;
using UntzApi.Database.Models;

namespace Untz.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventDto, Event>();
            CreateMap<Event, EventDto>();
            CreateMap<EventSubscriptionDto, EventSubscription>();

            CreateMap<TicketDto, Ticket>();
            CreateMap<Ticket, TicketDto>();

            CreateMap<MainEventDto, MainEvent>();
            CreateMap<MainEvent, MainEventDto>();

            CreateMap<Image, ImageDto>();
            CreateMap<ImageDto, Image>();

            CreateMap<UntzUserDto, UntzUser>()
                .ForMember(_ => _.Id, (_) => _.MapFrom(_ => Guid.NewGuid().ToString()));
            CreateMap<UntzUser, UntzUserDto>();
            CreateMap<UntzUser, UntzCurrentLoggedInUserDto>();

            CreateMap<GuestUserDto, GuestUser>();
            CreateMap<GuestUser, GuestUserDto>();

            CreateMap<PaymentMethod, PaymentMethodDto>();

            CreateMap<PaymentAcknowledgement, PaymentStatus>()
                .ForMember(_ => _.MerchantId, (_) => _.MapFrom((_) => _.merchant_id))
                .ForMember(_ => _.OrderId, (_) => _.MapFrom((_) => _.order_id))
                .ForMember(_ => _.PaymentId, (_) => _.MapFrom((_) => _.payment_id))
                .ForMember(_ => _.PayhereAmount, (_) => _.MapFrom((_) => _.payhere_amount))
                .ForMember(_ => _.PayhereCurrency, (_) => _.MapFrom((_) => _.payhere_currency))
                .ForMember(_ => _.StatusCode, (_) => _.MapFrom((_) => _.status_code))
                .ForMember(_ => _.Md5Sig, (_) => _.MapFrom((_) => _.md5sig))
                .ForMember(_ => _.Method, (_) => _.MapFrom((_) => _.method))
                .ForMember(_ => _.StatusMessage, (_) => _.MapFrom((_) => _.status_message));

        }
    }
}
