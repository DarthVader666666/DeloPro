using Delopro.Data.Interfaces;
using Delopro.Data.Entities;

namespace Delopro.Data.Repositories
{
    public class ThemeRepository: IRepository<Theme>
    {
        private readonly DeloproDbContext _dbContext;

        public ThemeRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Theme?> CreateAsync(Theme? item)
        {
            if (item == null)
            {
                return null;
            }

            var createdTheme = _dbContext.Themes.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            return createdTheme;
        }

        public async Task<Theme?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            var theme = await GetAsync(id_1);

            if (theme == null)
            {
                return null;
            }

            var deletedTheme = _dbContext.Themes.Remove(theme).Entity;
            await _dbContext.SaveChangesAsync();

            return deletedTheme;
        }

        public Task DeleteRangeAsync(IEnumerable<Theme> items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Theme? item)
        {
            throw new NotImplementedException();
        }

        public Task<Theme?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Theme?> GetAsync(int? id)
        {
            return Task.FromResult(_dbContext.Themes.FirstOrDefault(x => x.ThemeId == id));
        }

        public Task<IEnumerable<Theme?>> GetListAsync(int? id = null)
        {
            var themes = id == null
                ? _dbContext.Themes.Select(t =>
                    new Theme
                    {
                        ThemeId = t.ThemeId,
                        UserId = t.UserId,
                        ChapterId = t.ChapterId,
                        ThemeTitle = t.ThemeTitle,
                        Content = null,
                        DateCreated = t.DateCreated,
                        DateDeleted = t.DateDeleted
                    })
                : _dbContext.Chapters
                .SelectMany<Chapter, Theme, Theme?>(c => c.Themes!, (c, t) =>
                    new Theme
                    {
                        ThemeId = t.ThemeId,
                        UserId = t.UserId,
                        ChapterId = t.ChapterId,
                        ThemeTitle = t.ThemeTitle,
                        Content = null,
                        DateCreated = t.DateCreated,
                        DateDeleted = t.DateDeleted
                    }).AsEnumerable();

            return Task.FromResult(themes ?? []);
        }

        public async Task<Theme?> UpdateAsync(Theme? item)
        {
            if (item == null)
            {
                return null;
            }

            var updatedTheme = _dbContext.Themes.Update(item).Entity;
            await _dbContext.SaveChangesAsync();

            return updatedTheme;
        }

        public Task<IEnumerable<Theme?>> GetListIncludeAsync(int? id = null)
        {
            return Task.FromResult<IEnumerable<Theme?>>(_dbContext.Themes.AsEnumerable());
        }
    }
}
