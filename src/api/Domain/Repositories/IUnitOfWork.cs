using StackExchange.Redis;

namespace api.Domain.Repositories
{
    public interface IUnitOfWork
    {        
        Task<ITransaction> BeginTransactionAsync();        
        Task CompleteAsync();        
        Task RollbackAsync();
    }
}
