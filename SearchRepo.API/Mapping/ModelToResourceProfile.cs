using AutoMapper;
using SearchRepo.API.Domain.Models;
using SearchRepo.API.Resources;

namespace SearchRepo.API.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<GalleryItem, GalleryItemsResource>()
                .ForMember(dest => dest.RepositoryName, opt => opt.MapFrom(src => src.RepositoryName))
                .ForMember(dest => dest.avatarOwner, opt => opt.MapFrom(src => src.AvatarOwner));

            CreateMap< BookmarkRepo, BookmarkRepoResource>();
        }
    }
}
