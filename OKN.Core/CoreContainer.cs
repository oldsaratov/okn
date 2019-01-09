using OKN.Core.Mappings;
using AutoMapper;
using EventFlow;
using EventFlow.DependencyInjection.Extensions;
using EventFlow.Extensions;
using Microsoft.Extensions.DependencyInjection;
using OKN.Core.Events;
using OKN.Core.Handlers.Commands;
using OKN.Core.Handlers.Queries;
using OKN.Core.Models;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Queries;

namespace OKN.Core
{
    public static class CoreContainer
    {
        public static void Init(IServiceCollection services)
        {   
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddEventFlow(options =>
                EventFlowOptions.New
                    .UseServiceCollection(services)
                    .AddEvents(typeof(ObjectUpdatedEvent))
                    .AddCommands(typeof(UpdateObjectCommand))
                    .AddCommandHandlers(typeof(UpdateObjectCommandHandler))
                    .AddCommandHandlers(typeof(CreateObjectEventCommandHandler))
                    .AddQueryHandler<ObjectQueryHandler, ObjectQuery, OknObject>()
                    .AddQueryHandler<ObjectEventQueryHandler, ObjectEventQuery, OKNObjectEvent>()
                    .AddQueryHandler<ListObjectsQueryHandler, ListObjectsQuery, PagedList<OknObject>>()
                    .AddQueryHandler<ListObjectVersionsQueryHandler, ListVersionsQuery, PagedList<VersionInfo>>()
                    .AddQueryHandler<ListObjectEventsQueryHandler, ListObjectEventsQuery, PagedList<OKNObjectEvent>>()
                    .CreateServiceProvider(false));
        }
    }
}
