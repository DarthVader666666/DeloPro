
using Delopro.Data.Interfaces;
using Delopro.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace Delopro.Data.Repositories
{
    public class VisitRepository: IRepository<Visit>
    {
        private readonly DeloproDbContext _dbContext;

        public VisitRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Visit?> CreateAsync(Visit? item)
        {
            if (item == null)
            {
                return null;
            }

            var createdVisit = _dbContext.Visits.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            return createdVisit;
        }

        public async Task<Visit?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            var visit = await GetAsync(id_1);

            if (visit == null)
            {
                return null;
            }

            var deletedVisit = _dbContext.Visits.Remove(visit).Entity;
            await _dbContext.SaveChangesAsync();

            return deletedVisit;
        }

        public Task DeleteRangeAsync(IEnumerable<Visit?> items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Visit? item)
        {
            throw new NotImplementedException();
        }

        public Task<Visit?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Visit?> GetAsync(int? id)
        {
            return Task.FromResult(_dbContext.Visits.FirstOrDefault(v => v.VisitId == id));
        }

        public Task<IEnumerable<Visit?>> GetListAsync(int? id = null)
        {
            return Task.FromResult<IEnumerable<Visit?>>(_dbContext.Visits
                .Where(v => v.VisitDate.Year == DateTime.Now.Year && v.VisitDate.Month == DateTime.Now.Month)
                .OrderBy(v => v.VisitDate));
        }

        public Task<Visit?> UpdateAsync(Visit? item)
        {
            throw new NotImplementedException();
        }
    }
}
