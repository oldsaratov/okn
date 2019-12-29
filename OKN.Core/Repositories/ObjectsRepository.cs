using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using OKN.Core.Exceptions;
using OKN.Core.Models;
using OKN.Core.Models.Commands;
using OKN.Core.Models.Entities;
using OKN.Core.Models.Queries;
using Uploadcare;

namespace OKN.Core.Repositories
{
    public class ObjectsRepository
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ObjectsRepository(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OknObject> GetObject(ObjectQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == query.ObjectId);

            ObjectEntity entity = null;

            if (query.Version.HasValue)
            {
                var versionFilter = Builders<ObjectEntity>.Filter
                    .Where(x => x.Version.VersionId == query.Version);

                var emptyVersionFilter = Builders<ObjectEntity>.Filter
                    .Where(x => x.Version == null);

                filter = Builders<ObjectEntity>.Filter
                    .And(filter, query.Version == 0 ? emptyVersionFilter : versionFilter);

                entity = await _context.ObjectVersions
                    .Find(filter)
                    .SortByDescending(x => x.Version.VersionId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            //if not found in versions table - try to get last version
            if (entity == null)
            {
                entity = await _context.Objects
                    .Find(filter)
                    .FirstOrDefaultAsync(cancellationToken);

                //check if requested version is last version
                if (entity != null && query.Version.HasValue && query.Version.Value != entity.Version?.VersionId)
                {
                    //current version has different version, so requested version not found
                    return null;
                }
            }

            if (entity != null)
            {
                var model = _mapper.Map<ObjectEntity, OknObject>(entity);
                return model;
            }

            return null;
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

        public async Task<PagedList<VersionInfo>> GetVersions(ListVersionsQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.Version != null);

            var cursor = _context.ObjectVersions.Find(filter);
            var count = cursor.CountDocuments(cancellationToken);
            var items = await cursor
                .Skip((query.Page - 1) * query.PerPage)
                .Limit(query.PerPage)
                .Project(x => x.Version)
                .ToListAsync(cancellationToken);

            var model = _mapper.Map<List<VersionInfoEntity>, List<VersionInfo>>(items);

            var paged = new PagedList<VersionInfo>
            {
                Data = model,
                Page = query.Page,
                PerPage = query.PerPage,
                Total = count
            };

            return paged;
        }

        public async Task<PagedList<OknObject>> GetObjects(ListObjectsQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Empty;
            if (query.Types != null)
            {
                filter = Builders<ObjectEntity>.Filter.In(x => x.Type, query.Types);
            }

            if (!string.IsNullOrEmpty(query.NameToken))
            {
                var nameTokenFilter = Builders<ObjectEntity>.Filter.Regex("name", new BsonRegularExpression(query.NameToken, "i"));

                filter = Builders<ObjectEntity>.Filter.And(filter, nameTokenFilter);
            }

            var cursor = _context.Objects.Find(filter);
            var count = cursor.CountDocuments(cancellationToken);
            var items = await cursor
                .SortByDescending(x => x.Version.VersionId)
                .Limit(query.PerPage)
                .Skip((query.Page - 1) * query.PerPage)
                .ToListAsync(cancellationToken);

            var model = _mapper.Map<List<ObjectEntity>, List<OknObject>>(items);

            var paged = new PagedList<OknObject>
            {
                Data = model,
                Page = query.Page,
                PerPage = query.PerPage,
                Total = count
            };

            return paged;
        }

        public async Task<PagedList<OknObjectEvent>> GetObjectEvents(ListObjectEventsQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == query.ObjectId);

            var objectEntity = await _context.Objects.Find(filter).SingleOrDefaultAsync(cancellationToken);
            if (objectEntity?.Events == null) return null;

            var count = objectEntity.Events.Count;
            var items = objectEntity.Events.AsQueryable()
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

        public async Task UpdateObject(UpdateObjectCommand command, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == command.ObjectId);
            var originalEntity = await _context.Objects
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);

            if (originalEntity == null)
            {
                throw new ObjectNotExistException("Object with this id doesn't exist");
            }

            SetObjectVersionIfNotExist(command, originalEntity);

            var newEntity = new ObjectEntity
            {
                ObjectId = originalEntity.ObjectId,
                Name = command.Name,
                Description = command.Description,
                Longitude = command.Longitude,
                Latitude = command.Latitude,
                Type = command.Type,
                Events = originalEntity.Events
            };

            newEntity.MainPhoto = command.MainPhoto != null
                ? ProcessFileInfo(command.MainPhoto)
                : null;

            if (command.Photos != null)
            {
                newEntity.Photos = command.Photos.Select(x => ProcessFileInfo(x)).ToList();
            }

            await IncObjectVersion(command, originalEntity, newEntity, cancellationToken);
            await _context.Objects.ReplaceOneAsync(filter, newEntity, cancellationToken: cancellationToken);
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

        private async Task IncObjectVersion(BaseCommandWithInitiator command, ObjectEntity originalEntity, ObjectEntity newEntity, CancellationToken cancellationToken)
        {
            await _context.ObjectVersions.InsertOneAsync(originalEntity, cancellationToken: cancellationToken);

            newEntity.Version = new VersionInfoEntity(originalEntity.Version.VersionId + 1, new UserInfoEntity(command.UserId, command.UserName, command.Email));
        }

        private static void SetObjectVersionIfNotExist(BaseCommandWithInitiator command, ObjectEntity originalEntity)
        {
            if (originalEntity.Version == null)
            {
                originalEntity.Version = new VersionInfoEntity(1, new UserInfoEntity(command.UserId, command.UserName, command.Email));
            }
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

            var entity = new ObjectEventEntity
            {
                EventId = command.EventId,
                Name = command.Name,
                Description = command.Description,
                OccuredAt = command.OccuredAt,
                Author = new UserInfoEntity(command.UserId, command.UserName, command.Email)
            };

            if (command.Photos != null && command.Photos.Any())
            {
                entity.Photos = command.Photos.Select(x => ProcessFileInfo(x)).ToList();
            }

            if (command.Files != null && command.Files.Any())
            {
                entity.Files = command.Files.Select(x => ProcessFileInfo(x)).ToList();
            }

            if (originalEntity.Events == null)
                originalEntity.Events = new List<ObjectEventEntity>();

            originalEntity.Events.Add(entity);

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
    }
}