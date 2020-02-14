using AutoMapper;
using MongoDB.Driver;
using OKN.Core.Exceptions;
using OKN.Core.Models;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Entities;
using OKN.Core.Models.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uploadcare;

namespace OKN.Core.Repositories
{
    public class ObjectsEventRepository : BaseVersionRepository
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ObjectsEventRepository(IMapper mapper, DbContext context) : base(context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OknObjectEvent> GetEvent(ObjectEventQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == query.ObjectId);

            var entity = await _context.Objects
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            var evnt = entity?.Events?.FirstOrDefault(x => x.EventId == query.EventId);

            return evnt != null ? _mapper.Map<ObjectEventEntity, OknObjectEvent>(evnt) : null;
        }

        public async Task<PagedList<OknObjectEvent>> GetObjectEvents(ListObjectEventsQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == query.ObjectId);

            var objectEntity = await _context.Objects.Find(filter).SingleOrDefaultAsync(cancellationToken);
            if (objectEntity?.Events == null) return null;

            var count = objectEntity.Events.Count;
            var items = objectEntity.Events.AsQueryable()
                .OrderByDescending(x => x.OccuredAt)
                .Skip((query.Page - 1) * query.PerPage)
                .Take(query.PerPage).ToList();

            var model = _mapper.Map<List<ObjectEventEntity>, List<OknObjectEvent>>(items);

            var paged = new PagedList<OknObjectEvent>
            {
                Data = model,
                Page = query.Page,
                PerPage = query.PerPage,
                Total = count
            };

            return paged;
        }

        private FileEntity ProcessFileInfo(FileInfo fileInfo)
        {
            var fileEntity = new FileEntity
            {
                FileId = fileInfo.FileId,
                Description = fileInfo.Description,
            };

            if (string.IsNullOrEmpty(fileEntity.Url) && !string.IsNullOrEmpty(fileInfo.FileId))
            {
                fileEntity.Url = CdnPathBuilder.Build(fileInfo.FileId).ToString();
            }
            else
            {
                fileEntity.Url = fileInfo.Url;
            }

            return fileEntity;
        }

        public async Task<OknObject> CreateObjectEvent(CreateObjectEventCommand command, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == command.ObjectId);
            var originalEntity = await _context.Objects
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            if (originalEntity == null)
            {
                throw new ObjectEventNotExistException("Object event with this id doesn't exist");
            }

            SetObjectVersionIfNotExist(command, originalEntity);

            var eventEntity = new ObjectEventEntity
            {
                EventId = command.EventId,
                Name = command.Name,
                Description = command.Description,
                OccuredAt = command.OccuredAt,
                Author = new UserInfoEntity(command.UserId, command.UserName, command.Email)
            };

            if (command.Photos != null && command.Photos.Any())
            {
                eventEntity.Photos = command.Photos.Select(x => ProcessFileInfo(x)).ToList();
            }

            if (command.Files != null && command.Files.Any())
            {
                eventEntity.Files = command.Files.Select(x => ProcessFileInfo(x)).ToList();
            }

            originalEntity.AddEvent(eventEntity);

            await IncObjectVersion(command, originalEntity, originalEntity, cancellationToken);
            var result = await _context.Objects.ReplaceOneAsync(filter, originalEntity, cancellationToken: cancellationToken);

            return null;
        }

        public async Task<OknObject> UpdateObjectEvent(UpdateObjectEventCommand command, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == command.ObjectId);
            var originalEntity = await _context.Objects
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            if (originalEntity == null)
            {
                throw new ObjectEventNotExistException("Object with this id doesn't exist");
            }

            var objectEvent = originalEntity.Events.FirstOrDefault(x => x.EventId == command.EventId);
            if (objectEvent == null)
            {
                throw new ObjectEventNotExistException("Object event with this id doesn't exist");
            }

            SetObjectVersionIfNotExist(command, originalEntity);

            objectEvent.Name = command.Name;
            objectEvent.Description = command.Description;
            objectEvent.OccuredAt = command.OccuredAt;

            if (command.Photos != null)
            {
                objectEvent.Photos = command.Photos.Select(x => ProcessFileInfo(x)).ToList();
            }

            if (command.Files != null)
            {
                objectEvent.Files = command.Files.Select(x => ProcessFileInfo(x)).ToList();
            }

            await IncObjectVersion(command, originalEntity, originalEntity, cancellationToken);
            var result = await _context.Objects.ReplaceOneAsync(filter, originalEntity, cancellationToken: cancellationToken);

            return null;
        }

        public async Task<OknObject> DeleteObjectEvent(DeleteObjectEventCommand command, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == command.ObjectId);
            var originalEntity = await _context.Objects
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            if (originalEntity == null)
            {
                throw new ObjectEventNotExistException("Object with this id doesn't exist");
            }

            var objectEvent = originalEntity.Events.FirstOrDefault(x => x.EventId == command.EventId);
            if (objectEvent == null)
            {
                throw new ObjectEventNotExistException("Object event with this id doesn't exist");
            }

            originalEntity.Events = originalEntity.Events.Where(x => x.EventId != command.EventId).ToList();

            RecalculateEventsCount(originalEntity);
            SetObjectVersionIfNotExist(command, originalEntity);

            await IncObjectVersion(command, originalEntity, originalEntity, cancellationToken);
            var result = await _context.Objects.ReplaceOneAsync(filter, originalEntity, cancellationToken: cancellationToken);

            return null;
        }

        private static void RecalculateEventsCount(ObjectEntity originalEntity)
        {
            originalEntity.EventsCount = originalEntity.Events.Count;
        }
    }
}