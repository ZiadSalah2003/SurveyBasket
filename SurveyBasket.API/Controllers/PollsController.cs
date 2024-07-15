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
		public IActionResult GetAll()
		{
			var polls = _pollService.GetAll();
			return Ok(polls);
		}
		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			var poll = _pollService.Get(id);
			return poll is null ? NotFound() : Ok();
		}
		[HttpPost("")]
		public IActionResult Add(Poll request)
		{
			var newPoll= _pollService.Add(request);
			return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
		}
		[HttpPut("{id}")]
		public IActionResult Update(int id,Poll request)
		{
			var isUpdated = _pollService.Update(id, request);
			if(!isUpdated)
				return NotFound();
			return NoContent();
		}
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var isDeleted = _pollService.Delete(id);
			if (!isDeleted)
				return NotFound();
			return NoContent();
		}

	}
}
