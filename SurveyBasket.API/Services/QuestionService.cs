using Mapster;
using SurveyBasket.API.Contracts.cs.Answers;
using SurveyBasket.API.Contracts.cs.Questions;
using SurveyBasket.API.Entities;

namespace SurveyBasket.API.Services
{
	public class QuestionService : IQuestionService
	{
		private readonly ApplicationDbContext _context;
		public QuestionService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
		{
			var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken: cancellationToken);
			if (!pollIsExists)
				return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

			var questions = await _context.Questions
				.Where(q => q.PollId == pollId)
				.Include(q => q.Answers)
				//.Select(q=>new QuestionResponse(
				//	q.Id,
				//	q.Content,
				//	q.Answers.Select(a=>new AnswerResponse(a.Id,a.Content))
				//))
				.ProjectToType<QuestionResponse>()
				.AsNoTracking()
				.ToListAsync(cancellationToken);
			return Result.Success<IEnumerable<QuestionResponse>>(questions);
		}
		public async Task<Result<IEnumerable<QuestionResponse>>> GetAvaliableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
		{
			var hasVote=await _context.Votes.AnyAsync(v => v.UserId == userId && v.PollId == pollId, cancellationToken);
			if (hasVote)
				return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

			var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
			if (!pollIsExists)
				return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

			var questions = await _context.Questions
				.Where(q => q.PollId == pollId && q.IsActive)
				.Include(q => q.Answers)
				.Select(q=> new QuestionResponse(
					q.Id,
					q.Content,
					q.Answers.Where(a=>a.IsActive).Select(a=>new AnswerResponse(a.Id,a.Content))
					))
				.AsNoTracking()
				.ToListAsync(cancellationToken);
			return Result.Success<IEnumerable<QuestionResponse>>(questions);
		}
		public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
		{
			var question = await _context.Questions
				.Where(q => q.PollId == pollId && q.Id == id)
				.Include(q => q.Answers)
				.ProjectToType<QuestionResponse>()
				.AsNoTracking()
				.SingleOrDefaultAsync(cancellationToken);
			return question is null ? Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound) : Result.Success(question);
		}
		public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
		{
			var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken: cancellationToken);
			if (!pollIsExists)
				return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

			var questionIsExists = await _context.Questions.AnyAsync(q => q.Content == request.Content && q.PollId == pollId, cancellationToken: cancellationToken);
			if (questionIsExists)
				return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

			var question = request.Adapt<Question>();
			question.PollId = pollId;
			//request.Answer.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));
			await _context.Questions.AddAsync(question, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success(question.Adapt<QuestionResponse>());
		}
		public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
		{
			var questionisExists = await _context.Questions.AnyAsync(x => x.PollId == pollId && x.Id != id && x.Content == request.Content, cancellationToken);
			if (questionisExists)
				return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

			var question = await _context.Questions.Include(x => x.Answers).SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);
			if (question is null)
				return Result.Failure(QuestionErrors.QuestionNotFound);

			question.Content = request.Content;

			var currentAnswer=question.Answers.Select(x => x.Content).ToList();
			var newAnswer = request.Answers.Except(currentAnswer).ToList();
			newAnswer.ForEach(answer =>
			{
				question.Answers.Add(new Answer { Content = answer });
			});

			question.Answers.ToList().ForEach(answer =>
			{
				answer.IsActive = request.Answers.Contains(answer.Content);
			});

			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success();

		}
		public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
		{
			var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);
			if (question is null)
				return Result.Failure(QuestionErrors.QuestionNotFound);
			question.IsActive = !question.IsActive;
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success();
		}
	}
}
