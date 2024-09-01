using SurveyBasket.API.Contracts.cs.Common;
using SurveyBasket.API.Contracts.cs.Questions;

namespace SurveyBasket.API.Services
{
	public interface IQuestionService
	{
		Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters filters, CancellationToken cancellationToken = default);
		Task<Result<IEnumerable<QuestionResponse>>> GetAvaliableAsync(int pollId, string userId, CancellationToken cancellationToken = default);
		Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default);
		Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest question, CancellationToken cancellationToken = default);
		Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default);
		Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);

	}
}
