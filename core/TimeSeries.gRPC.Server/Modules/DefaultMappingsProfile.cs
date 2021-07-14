using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using System;
using Api = TimeSeries.Shared.Contracts.Api;
using Entities = TimeSeries.Shared.Contracts.Entities;
using Internal = TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.gRPC.Server.Profiles
{
    public class DefaultMappingsProfile : Profile
    {
        public override string ProfileName => "DefaultMappings";

        public DefaultMappingsProfile()
        {
            // Entities --> Protos

            CreateMap<Entities.AggregatedTimeSeries, AggregatedTimeSeries>()
                .ForPath(dest => dest.Time, input => input.MapFrom(i => Timestamp.FromDateTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Time))))
                .ForPath(dest => dest.Value, input => input.MapFrom(i => i.Value));

            // Entities/Internal Objects --> API objects

            CreateMap<Api.ReadResponse<Api.AggrTimeSeriesData>, Internal.ReadResponse<Entities.AggregatedTimeSeries>>();
        }
    }
}