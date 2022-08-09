using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers
{
    //class that takes events from kafka, produces their data to business entities and pass to repositories
    public class EventHandler : IEventHandler
    {
        private readonly IPostRepository _postRepository;

        private readonly ICommentRepository _commentRepository;

        public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        public async Task On(PostCreatedEvent @event)
        {
            var post = new PostEntity()
            {
                Id = @event.Id,
                Author = @event.Author,
                DatePosted = @event.DatePosted,
                Message = @event.Message
            };

            await _postRepository.CreateAsync(post);
        }

        public async Task On(PostDeletedEvent @event)
        {
            await _postRepository.DeleteAsync(@event.Id);
        }

        public async Task On(PostLikedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id)!;

            if (post == null) return;

            post.Likes++;

            await _postRepository.UpdateAsync(post);
        }

        public async Task On(MessageUpdatedEvent @event)
        {
            var post = await _postRepository.GetByIdAsync(@event.Id)!;

            if (post == null) return;

            post.Message = @event.Message;

            await _postRepository.UpdateAsync(post);
        }

        public async Task On(CommentAddedEvent @event)
        {
            var comment = new CommentEntity()
            {
                Id = @event.CommentId,
                Content = @event.Comment,
                DateCreated = @event.CommentDate,
                Username = @event.Username,
                PostId = @event.Id
            };

            await _commentRepository.CreateAsync(comment);
        }

        public async Task On(CommentUpdatedEvent @event)
        {
            var comment = await _commentRepository.GetByIdAsync(@event.Id)!;

            if (comment == null) return;

            comment.Content = @event.Comment;

            comment.Edited = true;

            comment.DateCreated = @event.EditDate;

            await _commentRepository.UpdateAsync(comment);
        }

        public async Task On(CommentRemovedEvent @event)
        {
            await _commentRepository.DeleteAsync(@event.Id);
        }
    }
}
