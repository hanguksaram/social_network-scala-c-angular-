using System.Linq;
using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //convention based maps object fields by fieldnames equality
            //from _ to _ 
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(src => src.CalculateAge());
                });
            CreateMap<User, UserForDetailedDto>()
                 .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(src => src.CalculateAge());
                });
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>()
            //skip null fields during mapping
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(dest => dest.RecipientKnownAs, opt => {
                  opt.MapFrom(src => src.Recipient.KnownAs);
                })
                .ForMember(dest => dest.SenderKnownAs, opt => {
                  opt.MapFrom(src => src.Sender.KnownAs);
                })
                .ForMember(dest => dest.RecipientPhotoUrl, opt => {
                  opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.SenderPhotoUrl, opt => {
                  opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url);
                });
                

        }

    }
}