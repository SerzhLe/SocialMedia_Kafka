using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    public class PostLookupController : BaseApiQueryController
    {
        private readonly ILogger<PostLookupController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

        public PostLookupController(IQueryDispatcher<PostEntity> queryDispatcher,
            ILogger<PostLookupController> logger)
        {
            _queryDispatcher = queryDispatcher;
            _logger = logger;
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPostsAsync()
        {
            try
            {
                var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());

                if (posts == null || !posts.Any())
                {
                    return NotFound(new BaseResponse()
                    {
                        Message = "Posts not found."
                    });
                }

                return Ok(new PostLookupResponse()
                {
                    Posts = posts,
                    Message = "Posts are returned successfully"
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing a request to retrieve all posts";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }

        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetPostByIdAsync(Guid id)
        {
            try
            {
                var post = await _queryDispatcher.SendAsync(new FindPostByIdQuery() { Id = id });

                if (post == null || !post.Any())
                {
                    return NotFound(new BaseResponse()
                    {
                        Message = "Post not found."
                    });
                }

                return Ok(new PostLookupResponse()
                {
                    Posts = post,
                    Message = "Post is returned successfully"
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing a request to retrieve the post";
                _logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
