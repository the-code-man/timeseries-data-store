using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using ApiContract = TimeSeries.Shared.Contracts.Api;
using Entities = TimeSeries.Shared.Contracts.Entities;
using Internal = TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Api.Profiles
{
    public class DefaultMappingsProfile : Profile
    {
        public override string ProfileName => "DefaultMappings";

        public DefaultMappingsProfile()
        {
            // Model <--> DTO

            CreateMap<ApiContract.RawTimeSeriesData, Entities.RawTimeSeries>()
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds))
                .ReverseMap()
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(src.Time)));

            // Model --> DTO

            CreateMap<Internal.ReadResponse<Entities.RawTimeSeries>, ApiContract.ReadResponse<ApiContract.RawTimeSeriesData>>()
                .ForPath(dest => dest.Data.Time, input => input.MapFrom(i => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Data.Time)))
                .ForPath(dest => dest.Data.Values, input => input.MapFrom(i => i.Data.Values));

            CreateMap<Internal.ReadResponse<List<Entities.RawTimeSeries>>, ApiContract.ReadResponse<List<ApiContract.RawTimeSeriesData>>>()
                .ForPath(dest => dest.Data, opt => opt.MapFrom(src => src.Data.Select(d => new ApiContract.RawTimeSeriesData
                {
                    Values = d.Values,
                    Time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(d.Time)
                })));

            CreateMap<Internal.WriteResponse, ApiContract.WriteResponse>();
            CreateMap<Internal.ReadResponse<List<string>>, ApiContract.ReadResponse<List<string>>>();
            CreateMap<Internal.ReadResponse<List<ApiContract.AggrTimeSeriesData>>, ApiContract.ReadResponse<List<ApiContract.AggrTimeSeriesData>>>();

            // Protos --> DTO

            CreateMap<gRPC.Server.AggregatedTimeSeries, ApiContract.AggrTimeSeriesData>()
                .ForPath(dest => dest.Time, input => input.MapFrom(i => i.Time.ToDateTime()))
                .ForPath(dest => dest.Value, input => input.MapFrom(i => i.Value));

            CreateMap<gRPC.Server.GetHistoricResponse, ApiContract.ReadResponse<List<ApiContract.AggrTimeSeriesData>>>()
                .ForPath(dest => dest.Data, input => input.MapFrom(i => i.TimeSeriesData.ToList()))
                .ForPath(dest => dest.ErrorMessage, input => input.MapFrom(i => i.ErrorMessage))
                .ForPath(dest => dest.IsSuccess, input => input.MapFrom(i => i.Success));

            CreateMap<gRPC.Server.GetLatestResponse, ApiContract.ReadResponse<ApiContract.AggrTimeSeriesData>>()
                .ForPath(dest => dest.Data, input => input.MapFrom(i => i.TimeSeriesData))
                .ForPath(dest => dest.ErrorMessage, input => input.MapFrom(i => i.ErrorMessage))
                .ForPath(dest => dest.IsSuccess, input => input.MapFrom(i => i.Success));
        }
    }
}