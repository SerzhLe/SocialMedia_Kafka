using CQRS.Core.Queries;

namespace Post.Query.Api.Queries
{
    public class FindPostsWithCertainLikesQuery : BaseQuery
    {
        public int Likes { get; set; }
    }
}
