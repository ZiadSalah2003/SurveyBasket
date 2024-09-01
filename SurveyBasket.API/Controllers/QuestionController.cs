using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.API.Abstractions;
using SurveyBasket.API.Contracts.cs.Common;
using SurveyBasket.API.Contracts.cs.Questions;
using SurveyBasket.API.Services;

namespace SurveyBasket.API.Controllers
{
	[Route("api/polls/{pollId}/[controller]")]
	[ApiController]
	public class QuestionController : ControllerBase
	{
		private readonly IQuestionService _questionService;

		public QuestionController(IQuestionService questionService)
		{
			_questionService = questionService;
		}

		[HttpGet]
		[HasPermission(Permissions.GetQuestions)]
		public async Task<IActionResult> GetAll([FromRoute] int pollId, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
		{
			var result = await _questionService.GetAllAsync(pollId, filters, cancellationToken);
			return result.IsSuccess
				? Ok(result.Value)
				: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
		}

		[HttpGet("{id}")]
		[HasPermission(Permissions.GetQuestions)]
		public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _questionService.GetAsync(pollId, id, cancellationToken);
			return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
		}

		[HttpPost("")]
		[HasPermission(Permissions.AddQuestions)]
		public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken = default)
		{
			var result = await _questionService.AddAsync(pollId, request, cancellationToken: cancellationToken);

			if (result.IsSuccess)
				CreatedAtAction(nameof(Get), new { pollId, result.Value.Id }, result.Value);

			return result.Error.Equals(QuestionErrors.DuplicatedQuestionContent)
			? Problem(statusCode: StatusCodes.Status409Conflict, title: result.Error.Code, detail: result.Error.Description)
			: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
		}

		[HttpPut("{id}")]
		[HasPermission(Permissions.UpdateQuestions)]
		public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
		{
			var result = await _questionService.UpdateAsync(pollId, id, request, cancellationToken);

			return result.IsSuccess ? NoContent() : result.ToProblem();
		}

		[HttpPut("{id}/ToggleStatus")]
		[HasPermission(Permissions.UpdateQuestions)]
		public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
		{
			var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);
			return result.IsSuccess
				? NoContent()
				: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
		}
	}
}
