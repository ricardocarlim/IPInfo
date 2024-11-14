namespace api.Domain.Repositories
{
    public interface ITransaction
    {
        Task CommitAsync();
        Task RollbackAsync();
        Task BeginTransactionAsync();
    }
}
