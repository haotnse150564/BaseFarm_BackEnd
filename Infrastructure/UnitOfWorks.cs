using Application;

namespace Infrastructure
{
    public class UnitOfWorks : IUnitOfWorks
    {
        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
