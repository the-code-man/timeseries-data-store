using AutoMapper;
using System;
using ApiContract = TimeSeries.Shared.Contracts.Api;
using Entities = TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.DataIngestor.Profiles
{
    public class DefaultMappingsProfile : Profile
    {
        public DefaultMappingsProfile()
        {
            //Entities --> Api Contract

            CreateMap<Entities.MultiValueTimeSeries, ApiContract.MultiValueTimeSeries>()
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(src.Time)));
        }

        public override string ProfileName => "DefaultMapping";
    }
}
