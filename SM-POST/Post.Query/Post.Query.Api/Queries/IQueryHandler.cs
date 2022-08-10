using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries
{
    public interface IQueryHandler
    {
        Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query);
        Task<List<PostEntity>> HandleAsync(FindAllPostsWithCommentsQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsWithCertainLikesQuery query);
    }
}
