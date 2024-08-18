﻿using SurveyBasket.API.Contracts.cs.Polls;

namespace SurveyBasket.API.Services
{
	public interface IPollService
	{
		Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default);
		Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default);
		Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
		Task<Result<PollResponse>> AddAsync(PollRequest poll, CancellationToken cancellationToken = default);
		Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default);
		Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
		Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);
	}
}
