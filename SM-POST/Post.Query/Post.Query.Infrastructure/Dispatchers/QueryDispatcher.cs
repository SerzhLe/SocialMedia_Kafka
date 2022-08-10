using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;

namespace Post.Query.Infrastructure.Dispatchers
{
    public class QueryDispatcher<TEntity> : IQueryDispatcher<TEntity>
    {
        private readonly Dictionary<Type, Func<BaseQuery, Task<List<TEntity>>>> _handlers = new();
        public void RegisterHandler<TQuery>(Func<TQuery, Task<List<TEntity>>> handler) where TQuery : BaseQuery
        {
            if (_handlers.ContainsKey(typeof(TQuery)))
            {
                throw new InvalidOperationException($"You cannot register the query handler {nameof(TQuery)} twice.");
            }

            _handlers.Add(typeof(TQuery), q => handler((TQuery)q));
        }

        public async Task<List<TEntity>> SendAsync(BaseQuery query)
        {
            if (!_handlers.TryGetValue(query.GetType(), out var handler))
            {
                throw new ArgumentNullException("No registered query handler was found.");
            }

            return await handler(query);
        }
    }
}
