using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private string _authorName;

        //Tuple: 1) comment; 2) username
        //Tuple as a class - values are immutable
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool IsActive { get; private set; }

        public PostAggregate()
        {
        }

        public PostAggregate(Guid id, string authorName, string message)
        {
            //we should always raise create event in constructor
            RaiseNewEvent(new PostCreatedEvent()
            {
                Id = id,
                Author = authorName,
                Message =  message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            IsActive = true;

            Id = @event.Id;

            _authorName = @event.Author;
        }

        public void EditMessage(string message)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException($"You cannot edit {nameof(message)} of inactive post");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"Your {nameof(message)} cannot be null or empty.");
            }

            RaiseNewEvent(new MessageUpdatedEvent()
            {
                Id = this.Id,
                Message = message
            });
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            this.Id = @event.Id;
        }

        public void LikePost()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("You cannot like inactive post.");
            }

            RaiseNewEvent(new PostLikedEvent()
            {
                Id = this.Id
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            this.Id = @event.Id;
        }

        public void AddComment(string comment, string username)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException($"You cannot add {nameof(comment)} to inactive post.");
            }

            if (username == null)
            {
                throw new InvalidOperationException($"Username is null.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"Your {nameof(comment)} cannot be null or empty.");
            }

            RaiseNewEvent(new CommentAddedEvent()
            {
                Id = this.Id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                Username = username,
                CommentDate = DateTime.Now
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            this.Id = @event.Id;

            _comments.Add(@event.CommentId, Tuple.Create(@event.Comment, @event.Username));
        }

        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException($"You cannot edit {nameof(comment)} in inactive post.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"Your {nameof(comment)} cannot be null or empty.");
            }

            if (!_comments.ContainsKey(commentId))
            {
                throw new InvalidOperationException($"No {nameof(comment)} found to edit.");
            }

            if (!_comments[commentId].Item2.Equals(username))
            {
                throw new InvalidOperationException($"You cannot edit {comment} that was made by another user.");
            }

            RaiseNewEvent(new CommentUpdatedEvent()
            {
                Id = this.Id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            this.Id = @event.Id;

            _comments[@event.CommentId] = Tuple.Create(@event.Comment, @event.Username);
        }

        public void RemoveComment(Guid commentId, string username)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("You cannot remove comment in inactive post.");
            }

            if (!_comments.ContainsKey(commentId))
            {
                throw new InvalidOperationException("No comment found to remove.");
            }

            if (!_comments[commentId].Item2.Equals(username))
            {
                throw new InvalidOperationException("You cannot remove comment that was made by another user.");
            }

            RaiseNewEvent(new CommentRemovedEvent()
            {
                Id = this.Id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            this.Id = @event.Id;

            _comments.Remove(@event.CommentId);
        }

        public void DeletePost(Guid postId, string username)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("The post has already been removed.");
            }

            if (!_authorName.Equals(username))
            {
                throw new InvalidOperationException("You cannot delete post that was not made by you.");
            }

            RaiseNewEvent(new PostDeletedEvent()
            {
                Id = this.Id
            });
        }

        public void Apply(PostDeletedEvent @event)
        {
            this.Id = @event.Id;

            IsActive = false;
        }
    }
}
