using AutoMapper;

namespace PetsStoreClient
{
    /// <summary>
    /// represents automapper mapping profile 
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PetsStoreClient.Nswag.Pet, Pet>();          
        }
    }
}
