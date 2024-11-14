using api.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace api.Persistence.Repositories
{
    public class Transaction : ITransaction, IDisposable
    {
        private readonly IDbContextTransaction _dbContextTransaction;

        public Transaction(IDbContextTransaction dbContextTransaction)
        {
            _dbContextTransaction = dbContextTransaction;
        }

        public async Task CommitAsync()
        {
            await _dbContextTransaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _dbContextTransaction.RollbackAsync();
        }
        
        public void Dispose()
        {
            _dbContextTransaction?.Dispose();
        }
    }
}
