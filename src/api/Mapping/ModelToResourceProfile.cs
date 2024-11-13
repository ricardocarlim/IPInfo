using api.Domain.Models;
using api.Domain.Models.Queries;
using api.Resources;
using AutoMapper;

namespace api.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {            
            CreateMap<Country, CountryResource>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TwoLetterCode, opt => opt.MapFrom(src => src.TwoLetterCode))
            .ForMember(dest => dest.ThreeLetterCode, opt => opt.MapFrom(src => src.ThreeLetterCode));

            CreateMap<IPAddressQueryResource, IPAddressQuery>();

            // Mapeamento de QueryResult<IPAddress> para QueryResultResource<IPAddressResource>
            CreateMap<QueryResult<IPAddress>, QueryResultResource<IPAddressResource>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(ip => new IPAddressResource
                {
                    IP = ip.IP,
                    Country = ip.Country,
                    CountryId = ip.CountryId,
                    CreatedAt = ip.CreatedAt,
                    UpdatedAt = ip.UpdatedAt
                }).ToList()))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.TotalItems));

            // Mapeamento de IPAddress para IPAddressResource
            CreateMap<IPAddress, IPAddressResource>()
                .ForMember(dest => dest.IP, opt => opt.MapFrom(src => src.IP))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }

}
