using API.Models;
using API.ViewModels;
using AutoMapper;

namespace API.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mapping
            CreateMap<User, UserVM>();
            CreateMap<User, UserListVM>();
            CreateMap<User, ProfileVM>();
            CreateMap<User, UpdateProfileModels>();
            CreateMap<User, UserLoginVM>();

            // Topic mapping
            CreateMap<Topic, TopicVM>();
            CreateMap<Topic, CreateUpdateTopicVM>();
        }
    }
}
