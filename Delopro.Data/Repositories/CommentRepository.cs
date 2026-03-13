using Delopro.Data.Entities;
using Delopro.Data.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Delopro.Data.Repositories
{
    public class CommentRepository : IRepository<Comment>
    {
        public readonly DeloproDbContext _dbContext;

        public CommentRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Comment?> CreateAsync(Comment? item)
        {
            var createdComment = _dbContext.Comments.Add(item!).Entity;
            await _dbContext.SaveChangesAsync();
            return createdComment;
        }

        public async Task<Comment?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            var comment = await _dbContext.Comments.FindAsync(id_1);

            if (comment is not null)
            {
                var result = _dbContext.Comments.Remove(comment).Entity;
                await _dbContext.SaveChangesAsync();
                return result;
            }            

            return null;
        }

        public Task DeleteRangeAsync(IEnumerable<Comment?> items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Comment? item)
        {
            throw new NotImplementedException();
        }

        public Task<Comment?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Comment?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment?>> GetListAsync(int? id = null)
        {
            var comments = _dbContext.Comments.Where(x => x.ThemeId == id);
            return Task.FromResult<IEnumerable<Comment?>>(comments);
        }

        public Task<Comment?> UpdateAsync(Comment? item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment?>> GetListIncludeAsync(int? id = null)
        {
            var comments = _dbContext.Comments.Include(x => x.User).Where(x => x.ThemeId == id);
            return Task.FromResult<IEnumerable<Comment?>>(comments);
        }
    }
}
