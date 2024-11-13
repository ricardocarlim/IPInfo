namespace api.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
