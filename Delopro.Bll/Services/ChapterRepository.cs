using Delopro.Bll.Interfaces;
using Delopro.Data;
using Delopro.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace Delopro.Bll.Services
{
    public class ChapterRepository : IRepository<Chapter>
    {
        private readonly DeloproDbContext _dbContext;

        public ChapterRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Chapter?> CreateAsync(Chapter? item)
        {
            if (item == null)
            {
                return null;
            }

            var createdChapter = _dbContext.Chapters.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            return createdChapter;
        }

        public async Task<Chapter?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            var chapter = await GetAsync(id_1);

            if (chapter == null)
            {
                return null;
            }

            var deletedTheme = _dbContext.Chapters.Remove(chapter).Entity;
            await _dbContext.SaveChangesAsync();

            return deletedTheme;
        }

        public Task DeleteRangeAsync(IEnumerable<Chapter> items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Chapter? item)
        {
            throw new NotImplementedException();
        }

        public Task<Chapter?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Chapter?> GetAsync(int? id)
        {
            return Task.FromResult(_dbContext.Chapters .Select(c => 
                new Chapter 
                {
                    ChapterId = c.ChapterId,
                    UserId = c.UserId,
                    ChapterTitle = c.ChapterTitle,
                    DateCreated = c.DateCreated,
                    DateDeleted = c.DateDeleted,
                    ImagePath = c.ImagePath,
                    Themes = c.Themes!.Select(t => 
                        new Theme 
                        {
                            ThemeId = t.ThemeId,
                            UserId = t.UserId,
                            ChapterId = t.ChapterId,
                            ThemeTitle = t.ThemeTitle,
                            Content = null,
                            DateCreated = t.DateCreated,
                            DateDeleted = t.DateDeleted
                        }).ToList()
                }).FirstOrDefault(x => x.ChapterId == id));
        }

        public Task<IEnumerable<Chapter?>> GetListAsync(int? id = null)
        {
            return Task.FromResult<IEnumerable<Chapter?>>(_dbContext.Chapters
                .Select(c => 
                new Chapter 
                {
                    ChapterId = c.ChapterId,
                    UserId = c.UserId,
                    ChapterTitle = c.ChapterTitle,
                    DateCreated = c.DateCreated,
                    DateDeleted = c.DateDeleted,
                    ImagePath = c.ImagePath,
                    Themes = c.Themes!.Select(t => 
                        new Theme 
                        {
                            ThemeId = t.ThemeId,
                            UserId = t.UserId,
                            ChapterId = t.ChapterId,
                            ThemeTitle = t.ThemeTitle,
                            Content = null,
                            DateCreated = t.DateCreated,
                            DateDeleted = t.DateDeleted
                        }).ToList()
                }).AsEnumerable());
        }

        public async Task<Chapter?> UpdateAsync(Chapter? item)
        {
            if (item == null)
            {
                return null;
            }

            var updatedChapter = _dbContext.Chapters.Update(item).Entity;
            await _dbContext.SaveChangesAsync();

            return updatedChapter;
        }
    }
}
