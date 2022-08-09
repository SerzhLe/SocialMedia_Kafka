using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContextFactory _dataContextFactory;

        public CommentRepository(DataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task CreateAsync(CommentEntity comment)
        {
            using (var context = _dataContextFactory.CreateDataContext())
            {
                context.Comments.Add(comment);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid commentId)
        {
            using (var context = _dataContextFactory.CreateDataContext())
            {
                var comment = context.Comments.SingleOrDefault(c => c.Id == commentId);

                if (comment == null) return;

                context.Comments.Remove(comment);

                await context.SaveChangesAsync();
            }
        }

        public async Task<CommentEntity>? GetByIdAsync(Guid commentId)
        {
            using (var context = _dataContextFactory.CreateDataContext())
            {
#pragma warning disable
                return await context.Comments.SingleOrDefaultAsync(c => c.Id == commentId);
#pragma warning enable
            }
        }

        public async Task UpdateAsync(CommentEntity comment)
        {
            using (var context = _dataContextFactory.CreateDataContext())
            {
                context.Comments.Update(comment);

                await context.SaveChangesAsync();
            }
        }
    }
}
