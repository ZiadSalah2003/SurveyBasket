using Microsoft.EntityFrameworkCore;
using SurveyBasket.API.Entities;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services
{
	public class PollService : IPollService
	{
		private readonly ApplicationDbContext _context;

		public PollService(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
		}
		public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _context.Polls.FindAsync(id, cancellationToken);
		}

		public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken=default)
		{
			await _context.Polls.AddAsync(poll, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return poll;
		}

		public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
		{
			var updated = await GetAsync(id, cancellationToken);
			if (updated is null)
				return false;
			updated.Title = poll.Title;
			updated.Summary = poll.Summary;
			updated.StartsAt = poll.StartsAt;
			updated.EndsAt = poll.EndsAt;
			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}

		public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken=default)
		{
			var poll = await GetAsync(id, cancellationToken);
			if (poll is null)
				return false;
			_context.Polls.Remove(poll);
			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}

		public async Task<bool> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
		{
			var poll = await GetAsync(id, cancellationToken);
			if (poll is null)
				return false;
			poll.IsPublished = !poll.IsPublished;
			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}
	}
}
