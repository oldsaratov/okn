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
            CreateMap<ObjectEventEntity, OknObjectEvent>();
            CreateMap<VersionInfoEntity, VersionInfo>();
            CreateMap<UserInfoEntity, UserInfo>();
            CreateMap<FileEntity, FileInfo>();
            CreateMap<ObjectId, string>().ConvertUsing(o => o.ToString());
            CreateMap<string, ObjectId>().ConvertUsing(o => ObjectId.Parse(o));
            CreateMap<BsonDateTime, DateTime>().ConvertUsing(o => (DateTime)o);
        }
    }
}