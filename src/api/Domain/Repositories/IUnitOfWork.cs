using StackExchange.Redis;

namespace api.Domain.Repositories
{
    public interface IUnitOfWork
    {        
        Task BeginTransactionAsync();        
        Task CompleteAsync();        
        Task RollbackAsync();
        Task CommitAsync();
    }
}
