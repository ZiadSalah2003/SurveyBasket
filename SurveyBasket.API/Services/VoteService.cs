using Mapster;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.API.Contracts.cs.Questions;
using SurveyBasket.API.Contracts.cs.Votes;

namespace SurveyBasket.API.Services
{
	public class VoteService(ApplicationDbContext context) : IVoteService
	{
		private readonly ApplicationDbContext _context = context;
		public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
		{
			var hasVote = await _context.Votes.AnyAsync(v => v.UserId == userId && v.PollId == pollId, cancellationToken);
			if (hasVote)
				return Result.Failure(VoteErrors.DuplicatedVote);

			var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
			if (!pollIsExists)
				return Result.Failure(PollErrors.PollNotFound);

			var avaliableQuestions = await _context.Questions
				.Where(q => q.PollId == pollId && q.IsActive)
				.Select(x => x.Id)
				.ToListAsync(cancellationToken);
			if (!request.Answers.Select(x => x.QuestionId).SequenceEqual(avaliableQuestions))
				return Result.Failure(VoteErrors.InvalidQuestion);
			var vote = new Vote
			{
				PollId = pollId,
				UserId = userId,
				VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
			};
			await _context.Votes.AddAsync(vote, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success();
			}
		}
	}
}
