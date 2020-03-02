using System;
using AutoMapper;
using MongoDB.Bson;
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
    public class ObjectsRepository : BaseVersionRepository
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public ObjectsRepository(IMapper mapper, DbContext context) : base(context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<OknObject> GetObject(ObjectQuery query, CancellationToken cancellationToken)
        {
            var filter = Builders<ObjectEntity>.Filter.Where(x => x.ObjectId == query.ObjectId);

            ObjectEntity entity = null;

            var excludeFields = Builders<ObjectEntity>.Projection.Exclude(d => d.Events);

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
                    .Project<ObjectEntity>(excludeFields)
                    .SortByDescending(x => x.Version.VersionId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            //if not found in versions table - try to get last version
            if (entity == null)
            {
                entity = await _context.Objects
                    .Find(filter)
                    .Project<ObjectEntity>(excludeFields)
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

                model.Events = model.Events.OrderByDescending(x => x.OccuredAt).ToList();

                return model;
            }

            return null;
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

            var excludeFields = Builders<ObjectEntity>.Projection
                .Exclude(d => d.Events)
                .Exclude(d => d.Federal)
                .Exclude(d => d.Version)
                .Exclude(d => d.Photos);

            var cursor = _context.Objects.Find(filter);
            var count = cursor.CountDocuments(cancellationToken);
            var items = await cursor
                .Project<ObjectEntity>(excludeFields)
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

        public async Task<OknObject> CreateObject(CreateObjectCommand command, CancellationToken cancellationToken)
        {
            var entity = new ObjectEntity
            {
                ObjectId = Guid.NewGuid().ToString().ToLower().Replace("_", ""),
                Name = command.Name,
                Description = command.Description,
                Longitude = command.Longitude,
                Latitude = command.Latitude,
                Type = command.Type,
                EventsCount = 0,
                Events = null,
                Version = new VersionInfoEntity(1, new UserInfoEntity(command.UserId, command.UserName, command.Email)),
                MainPhoto = command.MainPhoto != null
                    ? ProcessFileInfo(command.MainPhoto)
                    : null
            };

            if (command.Photos != null)
            {
                entity.Photos = command.Photos.Select(x => ProcessFileInfo(x)).ToList();
            }

            await _context.Objects.InsertOneAsync(entity, cancellationToken: cancellationToken);

            return _mapper.Map<ObjectEntity, OknObject>(entity);
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
                Federal = originalEntity.Federal,
                Name = command.Name ?? originalEntity.Name,
                Description = command.Description ?? originalEntity.Description,
                Longitude = command.Longitude ?? originalEntity.Longitude,
                Latitude = command.Latitude ?? originalEntity.Latitude,
                Type = command.Type != default ? command.Type : originalEntity.Type,
                TypeHistory = originalEntity.TypeHistory,
                EventsCount = originalEntity.EventsCount,
                Events = originalEntity.Events,
                MainPhoto = command.MainPhoto != null
                    ? ProcessFileInfo(command.MainPhoto)
                    : null
            };

            if (command.TypeHistory != null)
            {
                if (newEntity.TypeHistory == null)
                {
                    newEntity.TypeHistory = new List<ObjectTypeHistoryEntity>();
                }

                newEntity.TypeHistory.Add(new ObjectTypeHistoryEntity
                {
                    OccuredAt = command.TypeHistory.OccuredAt,
                    Reason = command.TypeHistory.Reason,
                    Type = command.TypeHistory.Type
                });
            }

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
    }
}