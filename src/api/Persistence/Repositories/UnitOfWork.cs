using api.Domain.Repositories;
using api.Persistence.Contexts;

namespace api.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ITransaction _transaction;

        public UnitOfWork(AppDbContext context, ITransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }

        // Inicia uma nova transação
        public async Task BeginTransactionAsync()
        {
            await _transaction.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }
        
    }
}
