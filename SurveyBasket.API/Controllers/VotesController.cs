using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.API.Abstractions.Consts;
using SurveyBasket.API.Contracts.cs.Votes;
using SurveyBasket.API.Extensions;
using SurveyBasket.API.Services;
using System.Security.Claims;

namespace SurveyBasket.API.Controllers
{
	[Route("api/polls/{pollId}/vote")]
	[ApiController]
	[Authorize(Roles =DefaultRoles.Member)]
	public class VotesController(IQuestionService questionService, IVoteService voteService) : ControllerBase
	{
		private readonly IQuestionService _questionService = questionService;
		private readonly IVoteService _voteService = voteService;
		[HttpGet]
		public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
		{
			var userId = User.GetUserId();
			var result = await _questionService.GetAvaliableAsync(pollId, userId!, cancellationToken);

			return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
		}

		[HttpPost("")]
		public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
		{
			var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);
			return result.IsSuccess ? Created() : result.ToProblem();
		}
	}
}
