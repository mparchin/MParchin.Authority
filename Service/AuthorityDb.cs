using Microsoft.EntityFrameworkCore;
using MParchin.Authority.Model;

namespace MParchin.Authority.Service;

public interface IAuthorityDb
{
    public DbSet<DbUser> Users { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}