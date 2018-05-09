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
            CreateMap<ObjectEntity, OKNObject>();
            CreateMap<ObjectId, string>().ConvertUsing(o => o.ToString());
            CreateMap<string, ObjectId>().ConvertUsing(ObjectId.Parse);
            CreateMap<BsonDateTime, DateTime>().ConvertUsing(o => (DateTime) o);
        }
    }
}