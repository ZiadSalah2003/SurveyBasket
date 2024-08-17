using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.API.Abstractions;
using SurveyBasket.API.Contracts.cs.Polls;
using SurveyBasket.API.Services;

namespace SurveyBasket.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize]
	public class PollsController : ControllerBase
	{
		private readonly IPollService _pollService;

		public PollsController(IPollService pollService)
		{
			_pollService = pollService;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var polls = await _pollService.GetAllAsync(cancellationToken);
			var response = polls.Adapt<IEnumerable<PollResponse>>();
			return Ok(polls);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _pollService.GetAsync(id, cancellationToken);
			return result.IsSuccess
				? Ok(result.Value)
				: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
		}
		[HttpPost("")]
		public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
		{

			var result = await _pollService.AddAsync(request, cancellationToken);
			return result.IsSuccess
				? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
				: Problem(statusCode: StatusCodes.Status400BadRequest, title: result.Error.Code, detail: result.Error.Description);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
		{
			var result = await _pollService.UpdateAsync(id, request, cancellationToken);
			if (result.IsSuccess)
				return NoContent();

			return result.Error.Equals(PollErrors.DuplicatedPollTitle)
			? Problem(statusCode: StatusCodes.Status409Conflict, title: result.Error.Code, detail: result.Error.Description)
			: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _pollService.DeleteAsync(id, cancellationToken);
			return result.IsSuccess
				? NoContent()
				: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
		}
		[HttpPut("{id}/TogglePublish")]
		public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
			return result.IsSuccess
				? NoContent()
				: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
		}
	}
}
