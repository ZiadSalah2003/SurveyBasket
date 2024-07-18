﻿using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.API.Contracts.cs.Polls;
using SurveyBasket.API.Services;

namespace SurveyBasket.API.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
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
			var poll = await _pollService.GetAsync(id, cancellationToken);
			if(poll is null)
				return NotFound();
			var response = poll.Adapt<PollResponse>();
			return Ok(response);
		}
		[HttpPost("")]
		public async Task<IActionResult> Add([FromBody] PollRequest request,CancellationToken cancellationToken)
		{
			
			var newPoll = await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);
			return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
		}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
		{
			var isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);
			if (!isUpdated)
				return NotFound();
			return NoContent();
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
		{
			var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
			if (!isDeleted)
				return NotFound();
			return NoContent();
		}
		[HttpPut("{id}/TogglePublish")]
		public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
		{
			var isUpdated = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
			if (!isUpdated)
				return NotFound();
			return NoContent();
		}
	}
}
