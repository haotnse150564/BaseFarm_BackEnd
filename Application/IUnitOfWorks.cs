using Application;

namespace Application
{
    public interface IUnitOfWorks
    {
        public Task<int> SaveChangesAsync();
    }
}
