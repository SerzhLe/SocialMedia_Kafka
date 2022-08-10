using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    public class EditMessageController : BaseApiCommandController
    {
        private readonly ILogger<EditMessageController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public EditMessageController(ICommandDispatcher commandDispatcher, ILogger<EditMessageController> logger)
        {
            _commandDispatcher = commandDispatcher;
            _logger = logger;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessageAsync(Guid id, EditMessageCommand command)
        {
            try
            {
                command.Id = id;

                await _commandDispatcher.SendAsync(command);

                return Ok(new BaseResponse()
                {
                    Message = "Edit message request completed successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, "Client made a bad request.");
                return BadRequest(new BaseResponse()
                {
                    Message = ex.Message
                });
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, "Post not found, incorrect id was passed.");
                return NotFound(new BaseResponse()
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to edit a post message.";

                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse()
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}
