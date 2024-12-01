using Microsoft.EntityFrameworkCore;
using MParchin.Authority.Model;

namespace MParchin.Authority.Service;

public interface IAuthorityDb<TDbUser>
    where TDbUser : User, IDbUser
{
    public DbSet<TDbUser> Users { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}