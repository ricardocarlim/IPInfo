using api.Domain.Repositories;
using api.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace api.Persistence.Repositories
{
    public class Transaction : ITransaction
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        public Transaction(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }
    }
}
