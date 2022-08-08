using CQRS.Core.Events;

namespace Post.Common.Events
{
    public class PostDeletedEvent : BaseEvent
    {
        public PostDeletedEvent() : base(nameof(PostDeletedEvent))
        {
        }
    }
}
