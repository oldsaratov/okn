using OKN.Core.Models.Commands;
using OKN.Core.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace OKN.Core.Repositories
{
    public class BaseVersionRepository
    {
        private readonly DbContext _context;

        protected BaseVersionRepository(DbContext context)
        {
            _context = context;
        }

        protected async Task IncObjectVersion(BaseCommandWithInitiator command, ObjectEntity originalEntity, ObjectEntity newEntity, CancellationToken cancellationToken)
        {
            await _context.ObjectVersions.InsertOneAsync(originalEntity, cancellationToken: cancellationToken);

            newEntity.Version = new VersionInfoEntity(originalEntity.Version.VersionId + 1, new UserInfoEntity(command.UserId, command.UserName, command.Email));
        }

        protected static void SetObjectVersionIfNotExist(BaseCommandWithInitiator command, ObjectEntity originalEntity)
        {
            if (originalEntity.Version == null)
            {
                originalEntity.Version = new VersionInfoEntity(1, new UserInfoEntity(command.UserId, command.UserName, command.Email));
            }
        }
    }
}