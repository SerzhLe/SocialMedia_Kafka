using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<BaseEvent> _changes = new();

        public int Version { get; set; } = -1;

        public Guid Id { get; protected set; }

        public IEnumerable<BaseEvent> GetUncommitedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        //apply events that are in event store
        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyChanges(@event, false);
            }
        }

        protected void RaiseNewEvent(BaseEvent @event)
        {
            ApplyChanges(@event, true);
        }

        private void ApplyChanges(BaseEvent @event, bool isNew) //@ is here because "event" is reserved keyword
        {
            var method = GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if (method == null)
            {
                throw new ArgumentNullException(
                    $"The Apply method was not found in the aggregate for {@event.GetType().Name}");
            }

            method.Invoke(this, new object[] { @event });

            if (isNew)
            {
                _changes.Add(@event);
            }
        }
    }
}
