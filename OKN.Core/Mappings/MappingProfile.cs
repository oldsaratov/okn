using System;
using OKN.Core.Models;
using OKN.Core.Models.Entities;
using AutoMapper;
using MongoDB.Bson;

namespace OKN.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ObjectEntity, OknObject>();
            CreateMap<ObjectEventEntity, OKNObjectEvent>();
            CreateMap<LinkEntity, OKNObjectEventLink>();
            CreateMap<ImageLinkEntity, OKNObjectEventImage>();
            CreateMap<VersionInfoEntity, VersionInfo>();
            CreateMap<UserInfoEntity, UserInfo>();
            CreateMap<ObjectId, string>().ConvertUsing(o => o.ToString());
            CreateMap<string, ObjectId>().ConvertUsing(o => ObjectId.Parse(o));
            CreateMap<BsonDateTime, DateTime>().ConvertUsing(o => (DateTime)o);
        }
    }
}