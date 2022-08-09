using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContextFactory _contextFactory;

        public PostRepository(DataContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task CreateAsync(PostEntity post)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                context.Posts.Add(post);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid postId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var post = await context.Posts.FindAsync(postId);

                if (post == null) return;

                context.Posts.Remove(post!);

                await context.SaveChangesAsync();
            }
        }

        public async Task<PostEntity>? GetByIdAsync(Guid postId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
#pragma warning disable
                return await context.Posts
                    .Include(p => p.Comments)
                    .SingleOrDefaultAsync(p => p.Id == postId);
#pragma warning enable
            }
        }

        public async Task<List<PostEntity>> ListAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                return await context.Posts.AsNoTracking()
                    .Include(p => p.Comments).AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<List<PostEntity>> ListByAuthorAsync(string author)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                return await context.Posts.AsNoTracking()
                    .Include(p => p.Comments).AsNoTracking()
                    .Where(p => p.Author.Equals(author))
                    .ToListAsync();
            }
        }

        public async Task<List<PostEntity>> ListWithCommentsAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                return await context.Posts.AsNoTracking()
                    .Include(p => p.Comments).AsNoTracking()
                    .Where(p => p.Comments != null && p.Comments.Any())
                    .ToListAsync();
            }
        }

        public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                return await context.Posts.AsNoTracking()
                    .Include(p => p.Comments).AsNoTracking()
                    .Where(p => p.Likes >= numberOfLikes)
                    .ToListAsync();
            }
        }

        public async Task UpdateAsync(PostEntity post)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                context.Posts.Update(post);

                await context.SaveChangesAsync();
            }
        }
    }
}
