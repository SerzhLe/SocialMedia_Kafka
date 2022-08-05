using CQRS.Core.Events;

namespace Post.Cmd.Api.Events
{
    public class PostDeletedEvent : BaseEvent
    {
        public PostDeletedEvent() : base(nameof(PostDeletedEvent))
        {
        }
    }
}
