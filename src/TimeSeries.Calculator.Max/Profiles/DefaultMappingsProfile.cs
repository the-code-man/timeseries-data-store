using AutoMapper;
using System;
using System.Collections.Generic;
using ApiContracts = TimeSeries.Shared.Contracts.Api;
using Entities = TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.Calculator.Max.Profiles
{
    public class DefaultMappingsProfile : Profile
    {
        public override string ProfileName => "DefaultMappings";

        public DefaultMappingsProfile()
        {
            CreateMap<Entities.SingleValueTimeSeries, ApiContracts.MultiValueTimeSeries>()
                .ForPath(dest => dest.Time, opt => opt.MapFrom(src => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(src.Time)))
                .ForPath(dest => dest.Values, opt => opt.MapFrom(src => new List<double> { src.Value } ));
        }
    }
}
