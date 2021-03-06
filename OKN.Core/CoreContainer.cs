﻿using OKN.Core.Mappings;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using OKN.Core.Repositories;

namespace OKN.Core
{
    public static class CoreContainer
    {
        public static void Init(IServiceCollection services)
        {   
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddSingleton<ObjectsRepository>();
            services.AddSingleton<ObjectsEventRepository>();
        }
    }
}
