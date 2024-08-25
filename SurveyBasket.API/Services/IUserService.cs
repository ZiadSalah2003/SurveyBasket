using SurveyBasket.API.Contracts.cs.Users;

namespace SurveyBasket.API.Services
{
	public interface IUserService
	{
		Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
		Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
		Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
	}
}
