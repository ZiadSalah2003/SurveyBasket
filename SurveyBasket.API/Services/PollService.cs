using Azure.Core;
using Hangfire;
using Mapster;
using SurveyBasket.API.Contracts.cs.Polls;
using SurveyBasket.API.Contracts.cs.Questions;
using SurveyBasket.API.Entities;

namespace SurveyBasket.API.Services
{
	public class PollService : IPollService
	{
		private readonly ApplicationDbContext _context;
		private readonly INotificationService _notificationService;

		public PollService(ApplicationDbContext context, INotificationService notificationService)
		{
			_context = context;
			_notificationService = notificationService;
		}
		public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _context.Polls.AsNoTracking().ProjectToType<PollResponse>().ToListAsync(cancellationToken);
		}
		public async Task<IEnumerable<PollResponse>> GetCurrentAsyncV1(CancellationToken cancellationToken = default)
		{
			return await _context.Polls
				.Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
				.AsNoTracking()
				.ProjectToType<PollResponse>()
				.ToListAsync(cancellationToken);
		}
		public async Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default)
		{
			return await _context.Polls
				.Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
				.AsNoTracking()
				.ProjectToType<PollResponseV2>()
				.ToListAsync(cancellationToken);
		}
		public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			var poll = await _context.Polls.FindAsync(id, cancellationToken);
			return poll is null ? Result.Failure<PollResponse>(PollErrors.PollNotFound) : Result.Success(poll.Adapt<PollResponse>());
		}

		public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
		{
			var isExistingTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken: cancellationToken);
			if (isExistingTitle)
				return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);
			var poll = request.Adapt<Poll>();
			await _context.Polls.AddAsync(poll, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success(poll.Adapt<PollResponse>());
		}

		public async Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default)
		{
			var isExistingTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id, cancellationToken);
			if (isExistingTitle)
				return Result.Failure<PollResponse>(PollErrors.PollNotFound);

			var updated = await _context.Polls.FindAsync(id, cancellationToken);
			if (updated is null)
				return Result.Failure(PollErrors.PollNotFound);
			updated.Title = request.Title;
			updated.Summary = request.Summary;
			updated.StartsAt = request.StartsAt;
			updated.EndsAt = request.EndsAt;
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success();
		}

		public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
		{
			var poll = await _context.Polls.FindAsync(id, cancellationToken);
			if (poll is null)
				return Result.Failure(PollErrors.PollNotFound);
			_context.Polls.Remove(poll);
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success();
		}

		public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
		{
			var poll = await _context.Polls.FindAsync(id, cancellationToken);
			if (poll is null)
				return Result.Failure(PollErrors.PollNotFound);
			poll.IsPublished = !poll.IsPublished;
			await _context.SaveChangesAsync(cancellationToken);

			if (poll.IsPublished && poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
				BackgroundJob.Enqueue(() => _notificationService.SendNewPollsNotification(poll.Id));

			return Result.Success();
		}
	}
}
