using MtslErp.Common.Infrastructure.Persistence.Repositories;
using PrintFactoryManagement.Domain.Entities;
using PrintFactoryManagement.Domain.Repositories;

namespace PrintFactoryManagement.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(PrintFactoryDbContext context) : base(context)
    {
    }
}
