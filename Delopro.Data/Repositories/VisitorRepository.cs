
using Delopro.Data.Interfaces;
using Delopro.Data.Entities;

namespace Delopro.Data.Repositories
{
    public class VisitorRepository : IRepository<Visitor>
    {
        private readonly DeloproDbContext _dbContext;

        public VisitorRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Visitor?> CreateAsync(Visitor? item)
        {
            if (item == null)
            {
                return null;
            }

            var createdVisitor = _dbContext.Visitors.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            return createdVisitor;
        }

        public Task<Visitor?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<Visitor> items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Visitor? item)
        {
            throw new NotImplementedException();
        }

        public async Task<Visitor?> FindByAsync(object? parameter)
        {
            if (parameter is not null and string)
            {
                return _dbContext.Visitors.FirstOrDefault(x => x.IpAddress == (string)parameter);
            }
            else
            {
                return null;
            }
        }

        public Task<Visitor?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Visitor?>> GetListAsync(int? id = null)
        {
            throw new NotImplementedException();
        }

        public Task<Visitor?> UpdateAsync(Visitor? item)
        {
            throw new NotImplementedException();
        }
    }
}
