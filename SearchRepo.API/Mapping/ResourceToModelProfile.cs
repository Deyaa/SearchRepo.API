using AutoMapper;
using SearchRepo.API.Domain.Models;
using SearchRepo.API.Resources;

namespace SearchRepo.API.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<BookmarkRepoResource, BookmarkRepo>();
        }
    }
}
