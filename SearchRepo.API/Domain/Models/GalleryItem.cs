using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchRepo.API.Domain.Models
{
    public class GalleryItem
    {
        public string RepositoryName { get; set; }
        public string AvatarOwner { get; set; }
        public string IdentityId { get; set; }
    }
}
